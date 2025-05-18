using nebulae.dotPDFium.Native;
using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Forms;

public sealed class PdfFormContext : IDisposable
{
    private nint _formHandle;
    private readonly PdfDocument _document;
    private bool _disposed;
    public PdfFormTextEditor TextEditor { get; }

    internal nint Handle => _formHandle;

    /// <summary>
    /// Represents the context for managing and interacting with PDF form fields within a PDF document.
    /// </summary>
    /// <param name="document">The <see cref="PdfDocument"/> instance representing the PDF document to be used for form operations. Cannot be
    /// <see langword="null"/>.</param>
    /// <param name="info">The <see cref="PdfFormFillInfo"/> structure containing configuration and callback information for form filling.
    /// This must be properly initialized before being passed.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="document"/> is <see langword="null"/>.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the form fill environment fails to initialize.</exception>
    public PdfFormContext(PdfDocument document, PdfFormFillInfo info)
    {
        _document = document ?? throw new ArgumentNullException(nameof(document));
        _formHandle = PdfFormFillNative.FPDFDOC_InitFormFillEnvironment(document.Handle, ref info);

        if (_formHandle == nint.Zero)
            throw new dotPDFiumException("Failed to initialize form fill environment.");

        TextEditor = new PdfFormTextEditor(this);
    }

    /// <summary>
    /// Retrieves a read-only list of all form fields present in the PDF document.
    /// </summary>
    /// <remarks>This method scans all pages of the PDF document and collects form fields of type "widget."
    /// The returned list includes fields such as text inputs, checkboxes, and other interactive elements.</remarks>
    /// <returns>A read-only list of <see cref="PdfFormField"/> objects representing the form fields in the document. The list
    /// will be empty if no form fields are found.</returns>
    public IReadOnlyList<PdfFormField> GetFields()
    {
        var fields = new List<PdfFormField>();

        for (int pageIndex = 0; pageIndex < _document.PageCount; pageIndex++)
        {
            using var page = _document.LoadPage(pageIndex);

            int annotCount = PdfAnnotNative.FPDFPage_GetAnnotCount(page.Handle);
            for (int i = 0; i < annotCount; i++)
            {
                IntPtr annot = PdfAnnotNative.FPDFPage_GetAnnot(page.Handle, i);
                if (annot == IntPtr.Zero) continue;

                int subtype = PdfAnnotNative.FPDFAnnot_GetSubtype(annot);
                if (subtype != 19) continue; // PDF_ANNOT_WIDGET

                fields.Add(new PdfFormField(this, annot, pageIndex));
            }
        }

        return fields;
    }

    /// <summary>
    /// Gets the number of focusable subtypes associated with the current form handle.
    /// </summary>
    /// <remarks>A focusable subtype represents an annotation type that can receive focus within the PDF form.
    /// This method retrieves the count of such subtypes for the current form handle.</remarks>
    /// <returns>The total count of focusable subtypes. Returns 0 if no focusable subtypes are available.</returns>
    public int GetFocusableSubtypeCount()
    {
        return PdfAnnotNative.FPDFAnnot_GetFocusableSubtypesCount(_formHandle);
    }

    /// <summary>
    /// Retrieves a list of annotation subtypes that can receive focus within the current PDF form.
    /// </summary>
    /// <remarks>This method queries the underlying PDF form to determine which annotation subtypes are
    /// focusable. The returned list is immutable and reflects the current state of the PDF form at the time of the
    /// call.</remarks>
    /// <returns>A read-only list of <see cref="PdfAnnotationSubtype"/> values representing the annotation subtypes that can
    /// receive focus. Returns an empty list if no focusable annotation subtypes are available.</returns>
    public IReadOnlyList<PdfAnnotationSubtype> GetFocusableAnnotationSubtypes()
    {
        // First call to get count
        if (!PdfAnnotNative.FPDFAnnot_GetFocusableSubtypes(_formHandle, IntPtr.Zero, UIntPtr.Zero))
            return Array.Empty<PdfAnnotationSubtype>();

        // PDFium fills count into the first 4 bytes of subtypes buffer
        int count;
        unsafe
        {
            PdfAnnotNative.FPDFAnnot_GetFocusableSubtypes(_formHandle, (IntPtr)(&count), (UIntPtr)1);
        }

        if (count == 0)
            return Array.Empty<PdfAnnotationSubtype>();

        // Allocate buffer for subtypes
        int[] buffer = new int[count];
        unsafe
        {
            fixed (int* ptr = buffer)
            {
                if (!PdfAnnotNative.FPDFAnnot_GetFocusableSubtypes(_formHandle, (IntPtr)ptr, (UIntPtr)count))
                    return Array.Empty<PdfAnnotationSubtype>();
            }
        }

        return buffer.Select(x => (PdfAnnotationSubtype)x).ToArray();
    }

    /// <summary>
    /// Sets the annotation subtypes that can receive focus within the PDF form.
    /// </summary>
    /// <remarks>This method allows you to specify which annotation subtypes within the PDF form can receive
    /// focus.  If the provided array is null or empty, the method will return <see langword="false"/> without making
    /// any changes.</remarks>
    /// <param name="subtypes">An array of <see cref="PdfAnnotationSubtype"/> values representing the annotation subtypes  that should be
    /// focusable. Must not be null or empty.</param>
    /// <returns><see langword="true"/> if the focusable annotation subtypes were successfully set;  otherwise, <see
    /// langword="false"/>.</returns>
    public bool SetFocusableAnnotationSubtypes(params PdfAnnotationSubtype[] subtypes)
    {
        if (subtypes == null || subtypes.Length == 0)
            return false;

        int[] raw = subtypes.Select(x => (int)x).ToArray();
        int size = raw.Length * sizeof(int);
        IntPtr buffer = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.Copy(raw, 0, buffer, raw.Length);
            return PdfAnnotNative.FPDFAnnot_SetFocusableSubtypes(_formHandle, buffer, (UIntPtr)raw.Length);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// Retrieves the handle of a form field annotation located at the specified point on a PDF page.
    /// </summary>
    /// <remarks>The coordinates <paramref name="x"/> and <paramref name="y"/> should be specified in the
    /// coordinate system of the PDF page. This method can be used to identify interactive form fields, such as text
    /// boxes or checkboxes, at a given location.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> object representing the page to search for the annotation.</param>
    /// <param name="x">The x-coordinate of the point, in page space.</param>
    /// <param name="y">The y-coordinate of the point, in page space.</param>
    /// <returns>A nullable <see cref="IntPtr"/> representing the handle of the form field annotation at the specified point,  or
    /// <see langword="null"/> if no form field annotation is found.</returns>
    public IntPtr? GetFormFieldAnnotationAtPoint(PdfPage page, float x, float y)
    {
        var point = new FsPointF { X = x, Y = y };
        IntPtr handle = PdfAnnotNative.FPDFAnnot_GetFormFieldAtPoint(_formHandle, page.Handle, ref point);

        return handle != IntPtr.Zero ? handle : null;
    }

    /// <summary>
    /// Executes a specific additional action on the given PDF page.
    /// </summary>
    /// <remarks>This method triggers an additional action, such as entering or leaving a page, as defined by
    /// the <paramref name="actionType"/>. The behavior of the action depends on the PDF document's configuration and
    /// the specified action type.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> on which the action will be performed. Cannot be <c>null</c>.</param>
    /// <param name="actionType">The type of additional action to execute, specified as a <see cref="PdfPageAActionType"/> value.</param>
    public void DoPageAAction(PdfPage page, PdfPageAActionType actionType)
    {
        PdfFormFillNative.FORM_DoPageAAction(page.Handle, _formHandle, (int)actionType);
    }

    /// <summary>
    /// Executes the JavaScript action associated with the document.
    /// </summary>
    /// <remarks>This method triggers any JavaScript actions embedded in the PDF document  that are intended
    /// to run at the document level. Use this method to ensure  that such actions are executed as part of the document
    /// processing workflow.</remarks>
    public void DoDocumentJavaScriptAction()
    {
        PdfFormFillNative.FORM_DoDocumentJSAction(_formHandle);
    }

    /// <summary>
    /// Executes the document's "open action" as defined in the PDF file.
    /// </summary>
    /// <remarks>This method triggers any actions specified in the PDF's document-level "open action,"  such
    /// as scripts or navigation commands, when the document is opened. Ensure that the  document and its associated
    /// form handle are properly initialized before calling this method.</remarks>
    public void DoDocumentOpenAction()
    {
        PdfFormFillNative.FORM_DoDocumentOpenAction(_formHandle);
    }

    /// <summary>
    /// Determines the type of form field located at the specified point on a PDF page.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> to inspect for a form field.</param>
    /// <param name="x">The x-coordinate of the point, in page coordinates.</param>
    /// <param name="y">The y-coordinate of the point, in page coordinates.</param>
    /// <returns>A <see cref="PdfFormFieldType"/> value representing the type of form field at the specified point. Returns <see
    /// cref="PdfFormFieldType.None"/> if no form field is present at the given location.</returns>
    public PdfFormFieldType GetFormFieldTypeAtPoint(PdfPage page, double x, double y)
    {
        int result = PdfFormFillNative.FPDFPage_HasFormFieldAtPoint(_formHandle, page.Handle, x, y);
        return (PdfFormFieldType)result;
    }

    /// <summary>
    /// Removes the highlight effect from all form fields in the current PDF form.
    /// </summary>
    /// <remarks>This method clears any visual highlighting applied to form fields, restoring their default
    /// appearance. It is typically used to reset the visual state of form fields after user interaction or
    /// processing.</remarks>
    public void RemoveFormFieldHighlight()
    {
        PdfFormFillNative.FPDF_RemoveFormFieldHighlight(_formHandle);
    }

    /// <summary>
    /// Sets the highlight color for a specific type of form field in the PDF form.
    /// </summary>
    /// <remarks>The highlight color is applied to all form fields of the specified type within the PDF
    /// document.</remarks>
    /// <param name="fieldType">The type of form field to apply the highlight color to.</param>
    /// <param name="color">The color to use for highlighting, specified as an <see cref="RgbaColor"/>.</param>
    public void SetFormFieldHighlightColor(PdfFormFieldHighlightType fieldType, RgbaColor color)
    {
        uint rgb = ((uint)color.R << 16) | ((uint)color.G << 8) | color.B;
        PdfFormFillNative.FPDF_SetFormFieldHighlightColor(_formHandle, (int)fieldType, rgb);
    }

    /// <summary>
    /// Sets the transparency level for highlighting form fields in the PDF document.
    /// </summary>
    /// <remarks>This method adjusts the visual appearance of form field highlights by setting their
    /// transparency level. Use this to customize the visibility of highlighted fields in the PDF viewer.</remarks>
    /// <param name="alpha">The alpha value for the highlight, ranging from 0 (completely transparent) to 255 (completely opaque).</param>
    public void SetFormFieldHighlightAlpha(byte alpha)
    {
        PdfFormFillNative.FPDF_SetFormFieldHighlightAlpha(_formHandle, alpha);
    }

    /// <summary>
    /// Gets the z-order of a form field at the specified point on a PDF page.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> on which to locate the form field.</param>
    /// <param name="x">The x-coordinate of the point, in page coordinates.</param>
    /// <param name="y">The y-coordinate of the point, in page coordinates.</param>
    /// <returns>The z-order of the form field at the specified point, where a lower value indicates a field closer to the bottom
    /// of the z-order. Returns -1 if no form field is found at the specified point.</returns>
    public int GetFormFieldZOrderAtPoint(PdfPage page, double x, double y)
    {
        return PdfFormFillNative.FPDFPage_FormFieldZOrderAtPoint(_formHandle, page.Handle, x, y);
    }

    /// <summary>
    /// Performs post-processing tasks after a PDF page is loaded.
    /// </summary>
    /// <remarks>This method is typically used to apply form-related operations or updates to the specified
    /// page after it has been loaded. Ensure that the <paramref name="page"/> is valid and properly initialized before
    /// calling this method.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> object representing the loaded PDF page. Cannot be <see langword="null"/>.</param>
    public void OnAfterLoadPage(PdfPage page)
    {
        PdfFormFillNative.FORM_OnAfterLoadPage(page.Handle, _formHandle);
    }

    /// <summary>
    /// Performs necessary operations before closing a PDF page.
    /// </summary>
    /// <remarks>This method ensures that any required cleanup or finalization tasks are performed for the
    /// specified PDF page before it is closed. Call this method before disposing of or releasing the page to avoid
    /// potential issues.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page to be closed. Cannot be null.</param>
    public void OnBeforeClosePage(PdfPage page)
    {
        PdfFormFillNative.FORM_OnBeforeClosePage(page.Handle, _formHandle);
    }

    /// <summary>
    /// Renders the form fields of a PDF page onto a specified bitmap.
    /// </summary>
    /// <remarks>This method renders only the form fields of the specified PDF page. It does not render the
    /// page's content. Ensure that the <paramref name="bitmap"/> has sufficient dimensions to accommodate the specified
    /// rendering area.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> containing the form fields to render.</param>
    /// <param name="bitmap">The <see cref="PdfBitmap"/> onto which the form fields will be rendered.</param>
    /// <param name="startX">The horizontal offset, in pixels, from the left edge of the bitmap where rendering begins. Defaults to 0.</param>
    /// <param name="startY">The vertical offset, in pixels, from the top edge of the bitmap where rendering begins. Defaults to 0.</param>
    /// <param name="width">The width, in pixels, of the area to render. If set to 0, the width of the <paramref name="bitmap"/> is used.</param>
    /// <param name="height">The height, in pixels, of the area to render. If set to 0, the height of the <paramref name="bitmap"/> is used.</param>
    /// <param name="rotation">The rotation angle, in degrees, to apply to the rendered content. Valid values are 0, 90, 180, and 270. Defaults
    /// to 0.</param>
    /// <param name="renderFlags">Flags that control the rendering behavior. These flags are typically defined by the PDF rendering library being
    /// used. Defaults to 0.</param>
    public void RenderFormFields(
        PdfPage page,
        PdfBitmap bitmap,
        int startX = 0,
        int startY = 0,
        int width = 0,
        int height = 0,
        int rotation = 0,
        int renderFlags = 0)
    {
        width = (width == 0) ? bitmap.Width : width;
        height = (height == 0) ? bitmap.Height : height;

        PdfFormFillNative.FPDF_FFLDraw(
            _formHandle,
            bitmap.Handle,
            page.Handle,
            startX,
            startY,
            width,
            height,
            rotation,
            renderFlags);
    }

    /// <summary>
    /// Determines the type of form contained in the PDF document.
    /// </summary>
    /// <remarks>This method retrieves the form type of the PDF document, such as AcroForm or XFA. The
    /// returned value can be used to determine how to process the form.</remarks>
    /// <returns>A <see cref="PdfFormType"/> value indicating the type of form in the PDF document.</returns>
    public PdfFormType GetFormType()
    {
        int type = PdfFormFillNative.FPDF_GetFormType(_document.Handle);
        return (PdfFormType)type;
    }

    /// <summary>
    /// Removes an annotation field from the specified PDF page at the given index.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> object representing the page from which the annotation field will be removed. Cannot
    /// be <see langword="null"/>.</param>
    /// <param name="annotIndex">The zero-based index of the annotation field to remove. Must be within the valid range of annotations on the
    /// page.</param>
    /// <returns><see langword="true"/> if the annotation field was successfully removed; otherwise, <see langword="false"/>.</returns>
    public bool RemoveFieldAt(PdfPage page, int annotIndex)
    {
        return PdfAnnotNative.FPDFPage_RemoveAnnot(page.Handle, annotIndex);
    }

    /// <summary>
    /// Releases all resources used by the current instance of the class.
    /// </summary>
    /// <remarks>This method should be called when the instance is no longer needed to free unmanaged
    /// resources. Once disposed, the instance cannot be used again.</remarks>
    public void Dispose()
    {
        if (_disposed) return;

        PdfFormFillNative.FPDFDOC_ExitFormFillEnvironment(_formHandle);
        _formHandle = nint.Zero;
        _disposed = true;
    }
}



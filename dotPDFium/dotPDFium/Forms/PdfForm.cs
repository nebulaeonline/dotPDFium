using nebulae.dotPDFium.Native;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace nebulae.dotPDFium.Forms;

public sealed class PdfForm : PdfObject
{
    public PdfDocument Document { get; }
    public PdfFormFillInfo FormFillInfo { get; }
    private readonly List<PdfFormElement> _ownedElements = new();

    internal PdfForm(PdfDocument doc, PdfFormFillInfo info)
    : base(PdfFormFillNative.FPDFDOC_InitFormFillEnvironment(doc.Handle, ref info), PdfObjectType.Form)
    {
        if (Handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to initialize form environment: {PdfObject.GetPDFiumError()}");

        Document = doc;
        FormFillInfo = info;
    }

    public PdfPushButton CreatePushButton(PdfPage page, string name, FsRectF bounds)
    {
        if (page == null)
            throw new ArgumentNullException(nameof(page));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Field name cannot be null or empty.", nameof(name));

        // Create widget annotation
        IntPtr annot = PdfAnnotNative.FPDFPage_CreateAnnot(page.Handle, (int)PdfAnnotationSubtype.Widget);
        if (annot == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to create widget annotation: {PdfObject.GetPDFiumError()}");

        // Set field type: /FT /Btn
        if (!PdfAnnotNative.FPDFAnnot_SetStringValue(annot, "FT", "Btn"))
            throw new dotPDFiumException($"Failed to set field type to PushButton: {PdfObject.GetPDFiumError()}");

        // Set field name: /T
        if (!PdfAnnotNative.FPDFAnnot_SetStringValue(annot, "T", name))
            throw new dotPDFiumException($"Failed to set field name: {PdfObject.GetPDFiumError()}");

        // Set annotation rectangle
        if (!PdfAnnotNative.FPDFAnnot_SetRect(annot, ref bounds))
            throw new dotPDFiumException($"Failed to set annotation bounds: {PdfObject.GetPDFiumError()}");

        // Set annotation flags (printable)
        PdfAnnotNative.FPDFAnnot_SetFlags(annot, (int)PdfAnnotationFlags.Print);

        // Determine annotation index
        int index = PdfAnnotNative.FPDFPage_GetAnnotCount(page.Handle) - 1;

        var field = new PdfPushButton(annot, index, this, page);
        _ownedElements.Add(field); // Track the element for cleanup
        return field;
    }

    public PdfCheckBox CreateCheckBox(PdfPage page, string name, FsRectF bounds)
    {
        if (page == null)
            throw new ArgumentNullException(nameof(page));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Field name cannot be null or empty.", nameof(name));

        // Create widget annotation
        IntPtr annot = PdfAnnotNative.FPDFPage_CreateAnnot(page.Handle, (int)PdfAnnotationSubtype.Widget);
        if (annot == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to create widget annotation: {PdfObject.GetPDFiumError()}");

        // Set field type: /FT /Btn
        if (!PdfAnnotNative.FPDFAnnot_SetStringValue(annot, "FT", "Btn"))
            throw new dotPDFiumException($"Failed to set field type to CheckBox: {PdfObject.GetPDFiumError()}");

        // Set field name: /T
        if (!PdfAnnotNative.FPDFAnnot_SetStringValue(annot, "T", name))
            throw new dotPDFiumException($"Failed to set field name: {PdfObject.GetPDFiumError()}");

        // Set annotation rectangle
        if (!PdfAnnotNative.FPDFAnnot_SetRect(annot, ref bounds))
            throw new dotPDFiumException($"Failed to set annotation bounds: {PdfObject.GetPDFiumError()}");

        // Optional: set check box-specific flags (bit 16 = checkbox, bit 15 = radio)
        // We clear the radio bit, and ensure checkbox behavior
        PdfAnnotNative.FPDFAnnot_SetFlags(annot, (int)PdfAnnotationFlags.Print);

        // Determine annotation index
        int index = PdfAnnotNative.FPDFPage_GetAnnotCount(page.Handle) - 1;

        var field = new PdfCheckBox(annot, index, this, page);
        _ownedElements.Add(field); // Track the element for cleanup
        return field;
    }

    public PdfRadioButton CreateRadioButton(PdfPage page, string name, FsRectF bounds)
    {
        if (page == null)
            throw new ArgumentNullException(nameof(page));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Field name cannot be null or empty.", nameof(name));

        // Create widget annotation
        IntPtr annot = PdfAnnotNative.FPDFPage_CreateAnnot(page.Handle, (int)PdfAnnotationSubtype.Widget);
        if (annot == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to create widget annotation: {PdfObject.GetPDFiumError()}");

        // Set field type: /FT /Btn
        if (!PdfAnnotNative.FPDFAnnot_SetStringValue(annot, "FT", "Btn"))
            throw new dotPDFiumException($"Failed to set field type to RadioButton: {PdfObject.GetPDFiumError()}");

        // Set field name: /T
        if (!PdfAnnotNative.FPDFAnnot_SetStringValue(annot, "T", name))
            throw new dotPDFiumException($"Failed to set field name: {PdfObject.GetPDFiumError()}");

        // Set annotation rectangle
        if (!PdfAnnotNative.FPDFAnnot_SetRect(annot, ref bounds))
            throw new dotPDFiumException($"Failed to set annotation bounds: {PdfObject.GetPDFiumError()}");

        // Mark as printable
        PdfAnnotNative.FPDFAnnot_SetFlags(annot, (int)PdfAnnotationFlags.Print);

        // Determine annotation index
        int index = PdfAnnotNative.FPDFPage_GetAnnotCount(page.Handle) - 1;

        var field = new PdfRadioButton(annot, index, this, page);
        _ownedElements.Add(field); // Track the element for cleanup
        return field;
    }

    public List<PdfRadioButton> CreateRadioGroup(PdfPage page, string name, FsRectF[] bounds, string[] exportValues)
    {
        if (page == null)
            throw new ArgumentNullException(nameof(page));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Field name cannot be null or empty.", nameof(name));
        if (bounds == null || exportValues == null)
            throw new ArgumentNullException("Bounds and export values cannot be null.");
        if (bounds.Length != exportValues.Length)
            throw new ArgumentException("The number of bounds must match the number of export values.");

        var buttons = new List<PdfRadioButton>(bounds.Length);

        for (int i = 0; i < bounds.Length; i++)
        {
            IntPtr annot = PdfAnnotNative.FPDFPage_CreateAnnot(page.Handle, (int)PdfAnnotationSubtype.Widget);
            if (annot == IntPtr.Zero)
                throw new dotPDFiumException($"Failed to create widget annotation: {PdfObject.GetPDFiumError()}");

            // Set /FT = Btn
            if (!PdfAnnotNative.FPDFAnnot_SetStringValue(annot, "FT", "Btn"))
                throw new dotPDFiumException($"Failed to set field type to RadioButton: {PdfObject.GetPDFiumError()}");

            // Set shared group name: /T
            if (!PdfAnnotNative.FPDFAnnot_SetStringValue(annot, "T", name))
                throw new dotPDFiumException($"Failed to set group name: {PdfObject.GetPDFiumError()}");

            // Set rectangle
            if (!PdfAnnotNative.FPDFAnnot_SetRect(annot, ref bounds[i]))
                throw new dotPDFiumException($"Failed to set bounds for radio button {i}: {PdfObject.GetPDFiumError()}");

            // Set as printable
            PdfAnnotNative.FPDFAnnot_SetFlags(annot, (int)PdfAnnotationFlags.Print);

            // Set export value
            if (!PdfAnnotNative.FPDFAnnot_SetStringValue(annot, "V", exportValues[i]))
                throw new dotPDFiumException($"Failed to set export value for button {i}: {PdfObject.GetPDFiumError()}");

            int index = PdfAnnotNative.FPDFPage_GetAnnotCount(page.Handle) - 1;
            buttons.Add(new PdfRadioButton(annot, index, this, page));
        }

        var field = buttons;
        _ownedElements.AddRange(buttons); // Track the elements for cleanup
        return field;
    }

    // Add option entries for combo boxes and list boxes — not currently supported via PDFium public API
    // TODO: support /Opt population if/when PDFium adds a setter

    public PdfTextField CreateTextField(PdfPage page, string name, FsRectF bounds)
    {
        if (page == null)
            throw new ArgumentNullException(nameof(page));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Field name cannot be null or empty.", nameof(name));

        // Create widget annotation
        IntPtr annot = PdfAnnotNative.FPDFPage_CreateAnnot(page.Handle, (int)PdfAnnotationSubtype.Widget);

        if (annot == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to create widget annotation: {PdfObject.GetPDFiumError()}");

        // Set required field properties
        if (!PdfAnnotNative.FPDFAnnot_SetStringValue(annot, "FT", "Tx"))
            throw new dotPDFiumException($"Failed to set field type to TextField: {PdfObject.GetPDFiumError()}");

        if (!PdfAnnotNative.FPDFAnnot_SetStringValue(annot, "T", name))
            throw new dotPDFiumException($"Failed to set field name: {PdfObject.GetPDFiumError()}");

        if (!PdfAnnotNative.FPDFAnnot_SetRect(annot, ref bounds))
            throw new dotPDFiumException($"Failed to set annotation bounds: {PdfObject.GetPDFiumError()}");

        // Optional: mark as printable
        PdfAnnotNative.FPDFAnnot_SetFlags(annot, (int)PdfAnnotationFlags.Print);

        // Determine annotation index
        int index = PdfAnnotNative.FPDFPage_GetAnnotCount(page.Handle) - 1;

        var field = new PdfTextField(annot, index, this, page);
        _ownedElements.Add(field); // Track the element for cleanup
        return field;
    }

    public IReadOnlyList<PdfFormElement> GetElements(PdfPage page)
    {
        if (page == null)
            throw new ArgumentNullException(nameof(page));

        var elements = new List<PdfFormElement>();
        int count = PdfAnnotNative.FPDFPage_GetAnnotCount(page.Handle);

        for (int i = 0; i < count; i++)
        {
            IntPtr annot = PdfAnnotNative.FPDFPage_GetAnnot(page.Handle, i);
            if (annot == IntPtr.Zero)
                continue;

            int subtype = PdfAnnotNative.FPDFAnnot_GetSubtype(annot);
            if (subtype != (int)PdfAnnotationSubtype.Widget)
                continue;

            var type = (PdfFormElementType)PdfAnnotNative.FPDFAnnot_GetFormFieldType(this.Handle, annot);

            try
            {
                var element = PdfFormElementFactory.CreateElement(type, annot, i, this, page);
                elements.Add(element);
            }
            catch (NotSupportedException)
            {
                // Skip unrecognized field types
            }
        }

        return elements;
    }

    /// <summary>
    /// Retrieves a list of annotation subtypes that are considered focusable within this form context.
    /// </summary>
    /// <returns>A list of supported focusable annotation subtypes.</returns>
    public List<PdfAnnotationSubtype> GetFocusableSubtypes()
    {
        int count = PdfAnnotNative.FPDFAnnot_GetFocusableSubtypesCount(this.Handle);
        if (count <= 0)
            return new List<PdfAnnotationSubtype>();

        int size = sizeof(int) * count;
        IntPtr buffer = Marshal.AllocHGlobal(size);

        try
        {
            if (!PdfAnnotNative.FPDFAnnot_GetFocusableSubtypes(this.Handle, buffer, (UIntPtr)count))
                throw new dotPDFiumException("Failed to retrieve focusable annotation subtypes.");

            var result = new List<PdfAnnotationSubtype>(count);
            for (int i = 0; i < count; i++)
            {
                int value = Marshal.ReadInt32(buffer, i * sizeof(int));
                if (Enum.IsDefined(typeof(PdfAnnotationSubtype), value))
                    result.Add((PdfAnnotationSubtype)value);
            }

            return result;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }


    /// <summary>
    /// Gets the number of annotation subtypes considered focusable by the current form environment.
    /// </summary>
    /// <returns>The number of focusable annotation subtypes.</returns>
    public int GetFocusableSubtypeCount()
    {
        return PdfAnnotNative.FPDFAnnot_GetFocusableSubtypesCount(this.Handle);
    }

    /// <summary>
    /// Retrieves the alternate name of a form field associated with the specified PDF annotation.
    /// </summary>
    /// <remarks>The alternate name of a form field is an optional, user-friendly name that may be used in
    /// place of the field's technical name.</remarks>
    /// <param name="annotation">The PDF annotation for which to retrieve the form field's alternate name. Cannot be <see langword="null"/>.</param>
    /// <returns>The alternate name of the form field as a string, or <see langword="null"/> if the form field does not have an
    /// alternate name.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="annotation"/> is <see langword="null"/>.</exception>
    public string? GetFormFieldAlternateName(PdfAnnotation annotation)
    {
        if (annotation == null)
            throw new ArgumentNullException(nameof(annotation));

        uint len = PdfAnnotNative.FPDFAnnot_GetFormFieldAlternateName(this.Handle, annotation.Handle, Array.Empty<char>(), 0);
        if (len == 0)
            return null;

        var buffer = new char[len];
        uint written = PdfAnnotNative.FPDFAnnot_GetFormFieldAlternateName(this.Handle, annotation.Handle, buffer, len);
        if (written == 0)
            return null;

        return new string(buffer, 0, (int)(written - 1)); // remove null terminator
    }

    /// <summary>
    /// Retrieves the form field name associated with the specified PDF annotation.
    /// </summary>
    /// <param name="annot">The <see cref="PdfAnnotation"/> object representing the annotation to retrieve the form field name for. Must not
    /// be <see langword="null"/>.</param>
    /// <returns>The name of the form field as a string, or <see langword="null"/> if the annotation is not a widget annotation
    /// or if the form field name cannot be determined.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="annot"/> is <see langword="null"/>.</exception>
    public string? GetFormFieldName(PdfAnnotation annot)
    {
        if (annot == null)
            throw new ArgumentNullException(nameof(annot));

        // Must be a widget annotation
        if (annot.Subtype != PdfAnnotationSubtype.Widget)
            return null;

        uint len = PdfAnnotNative.FPDFAnnot_GetFormFieldName(_handle, annot.Handle, null!, 0);
        if (len == 0)
            return null;

        var buffer = new char[len];
        uint written = PdfAnnotNative.FPDFAnnot_GetFormFieldName(_handle, annot.Handle, buffer, len);
        if (written == 0)
            return null;

        return new string(buffer, 0, (int)(written - 1)); // strip null terminator
    }

    public PdfAnnotation? GetFormFieldAtPoint(PdfPage page, float x, float y)
    {
        var point = new FsPointF(x, y);
        var handle = PdfAnnotNative.FPDFAnnot_GetFormFieldAtPoint(this.Handle, page.Handle, ref point);
        return handle != IntPtr.Zero ? new PdfAnnotation(handle, page) : null;
    }

    /// <summary>
    /// Sets the focusable annotation subtypes for the current PDF annotation.
    /// </summary>
    /// <remarks>This method allocates unmanaged memory to pass the subtypes to the underlying PDF library. 
    /// Ensure that the provided subtypes are valid and meaningful for the current context.</remarks>
    /// <param name="subtypes">A collection of <see cref="PdfAnnotationSubtype"/> values representing the annotation subtypes  that should be
    /// focusable. If the collection is null or empty, no subtypes will be set.</param>
    /// <returns><see langword="true"/> if the focusable subtypes were successfully set; otherwise, <see langword="false"/>.</returns>
    public bool SetFocusableSubtypes(IEnumerable<PdfAnnotationSubtype> subtypes)
    {
        var subtypeList = subtypes?.ToArray() ?? Array.Empty<PdfAnnotationSubtype>();
        if (subtypeList.Length == 0)
            return false;

        int size = subtypeList.Length * sizeof(int);
        IntPtr buffer = Marshal.AllocHGlobal(size);

        try
        {
            for (int i = 0; i < subtypeList.Length; i++)
            {
                Marshal.WriteInt32(buffer, i * sizeof(int), (int)subtypeList[i]);
            }

            return PdfAnnotNative.FPDFAnnot_SetFocusableSubtypes(
                this.Handle,
                buffer,
                (UIntPtr)subtypeList.Length);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    protected override void Dispose(bool disposing)
    {
        foreach (var field in _ownedElements)
        {
            field.Dispose(); // Dispose each owned element
        }

        _ownedElements.Clear(); // Clear the list of owned elements

        PdfFormFillNative.FPDFDOC_ExitFormFillEnvironment(_handle);

        // No additional cleanup required for PdfForm at this time.
        base.Dispose(disposing);
    }
}

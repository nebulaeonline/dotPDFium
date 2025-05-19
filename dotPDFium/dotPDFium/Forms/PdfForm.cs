using System;
using nebulae.dotPDFium.Native;

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

        // Set /Ff (Form Field Flags) to indicate this is a PushButton
        // PDF spec: Ff bit 17 = no toggle-to-off (ignored for push button), bit 16 = checkbox, bit 15 = radio
        // For push button, all those should be unset
        if (!PdfAnnotNative.FPDFAnnot_SetNumberValue(annot, "Ff", 0))
            throw new dotPDFiumException($"Failed to clear field behavior flags: {PdfObject.GetPDFiumError()}");

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
        int rawFlags = 0; // just leave as-is unless you want to set required bits
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

        // Set form field flags: /Ff bit 15 (radio button)
        const int RadioButtonFlag = 1 << 15;
        if (!PdfAnnotNative.FPDFAnnot_SetNumberValue(annot, "Ff", (float)RadioButtonFlag))
            throw new dotPDFiumException($"Failed to set radio button flag: {PdfObject.GetPDFiumError()}");

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

            // Set /Ff bit 15 = radio button
            const int RadioButtonFlag = 1 << 15;
            if (!PdfAnnotNative.FPDFAnnot_SetNumberValue(annot, "Ff", (float)RadioButtonFlag))
                throw new dotPDFiumException($"Failed to set radio flag for button {i}: {PdfObject.GetPDFiumError()}");

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

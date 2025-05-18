using nebulae.dotPDFium.Native;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;

namespace nebulae.dotPDFium.Forms;

public sealed class PdfFormField
{
    public string Name { get; }
    public string? Value { get; private set; }
    public PdfFormFieldType FieldType { get; }
    public int PageIndex { get; }
    internal IntPtr AnnotHandle { get; }
    public uint Flags { get; private set; }
    private readonly PdfFormContext _context;

    /// <summary>
    /// Constructor for creating a PdfFormField object. This constructor is used internally.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="annot"></param>
    /// <param name="pageIndex"></param>
    /// <exception cref="ArgumentNullException"></exception>
    internal PdfFormField(PdfFormContext context, IntPtr annot, int pageIndex)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        AnnotHandle = annot;
        PageIndex = pageIndex;

        Name = TryReadString(annot, "T") ?? "<unnamed>";
        Value = TryReadString(annot, "V");

        var ft = TryReadString(annot, "FT");
        Flags = TryReadFlags(annot);

        FieldType = ft?.ToLowerInvariant() switch
        {
            "btn" when (Flags & (1 << 15)) != 0 => PdfFormFieldType.RadioButton,
            "btn" => PdfFormFieldType.CheckBox,
            "tx" => PdfFormFieldType.Text,
            "ch" => PdfFormFieldType.ComboBox,
            "sig" => PdfFormFieldType.Signature,
            _ => PdfFormFieldType.Unknown
        };
    }

    /// <summary>
    /// Retrieves the alternate name of the form field associated with the annotation.
    /// </summary>
    /// <remarks>The alternate name is an optional, user-friendly name for the form field, which may be used
    /// in place of the field's technical name in certain contexts.</remarks>
    /// <returns>A <see cref="string"/> containing the alternate name of the form field, or <see langword="null"/> if no
    /// alternate name is defined.</returns>
    public string? GetAlternateName()
    {
        char[] buffer = new char[256];
        uint len = PdfAnnotNative.FPDFAnnot_GetFormFieldAlternateName(_context.Handle, AnnotHandle, buffer, (uint)buffer.Length);
        return len == 0 ? null : new string(buffer, 0, (int)len - 1); // Drop null terminator
    }

    /// <summary>
    /// Gets the total number of form controls associated with the annotation.
    /// </summary>
    /// <returns>The number of form controls linked to the annotation. Returns 0 if no form controls are associated.</returns>
    public int GetControlCount()
    {
        return PdfAnnotNative.FPDFAnnot_GetFormControlCount(_context.Handle, AnnotHandle);
    }

    /// <summary>
    /// Retrieves the index of the form control associated with the annotation.
    /// </summary>
    /// <remarks>This method is typically used to identify the position of a form control within a collection
    /// of controls for a given annotation. The returned index can be used to differentiate between multiple controls
    /// associated with the same annotation.</remarks>
    /// <returns>The zero-based index of the form control within the annotation, or -1 if the annotation does not have an
    /// associated form control.</returns>
    public int GetControlIndex()
    {
        return PdfAnnotNative.FPDFAnnot_GetFormControlIndex(_context.Handle, AnnotHandle);
    }

    /// <summary>
    /// Retrieves the font color of the annotation, if available.
    /// </summary>
    /// <remarks>This method queries the font color of the annotation using the underlying PDF library.  If
    /// the font color is not defined or cannot be retrieved, the method returns <see langword="null"/>.</remarks>
    /// <returns>An <see cref="RgbaColor"/> representing the font color of the annotation,  with an alpha value of 255 (fully
    /// opaque), or <see langword="null"/> if the font color is not available.</returns>
    public RgbaColor? GetFontColor()
    {
        if (PdfAnnotNative.FPDFAnnot_GetFontColor(_context.Handle, AnnotHandle, out uint r, out uint g, out uint b))
            return new RgbaColor((byte)r, (byte)g, (byte)b, 255); // Alpha = 255 (opaque)

        return null;
    }

    /// <summary>
    /// Retrieves the font size of the annotation, if available.
    /// </summary>
    /// <remarks>This method attempts to retrieve the font size associated with the annotation.  If the font
    /// size is not defined or cannot be determined, the method returns <see langword="null"/>.</remarks>
    /// <returns>The font size of the annotation as a <see cref="float"/> if the font size is defined;  otherwise, <see
    /// langword="null"/>.</returns>
    public float? GetFontSize()
    {
        if (PdfAnnotNative.FPDFAnnot_GetFontSize(_context.Handle, AnnotHandle, out float size))
            return size;

        return null;
    }

    /// <summary>
    /// Retrieves the form field flags associated with the current annotation.
    /// </summary>
    /// <remarks>The returned flags indicate properties such as whether the field is read-only, required, or
    /// has other specific attributes. Use these flags to determine how the form field should be handled or
    /// displayed.</remarks>
    /// <returns>A <see cref="PdfFormFieldFlags"/> enumeration value representing the flags that define the behavior and
    /// characteristics of the form field.</returns>
    public PdfFormFieldFlags GetFieldFlags()
    {
        int raw = PdfAnnotNative.FPDFAnnot_GetFormFieldFlags(_context.Handle, AnnotHandle);
        return (PdfFormFieldFlags)raw;
    }

    /// <summary>
    /// Retrieves the name of the form field associated with the annotation.
    /// </summary>
    /// <remarks>The returned string represents the name of the form field linked to the annotation.  If the
    /// annotation does not have an associated form field or the field name cannot be retrieved, the method returns <see
    /// langword="null"/>.</remarks>
    /// <returns>The name of the form field as a <see cref="string"/>, or <see langword="null"/> if the field name is not
    /// available.</returns>
    public string? GetFieldName()
    {
        char[] buffer = new char[256];
        uint written = PdfAnnotNative.FPDFAnnot_GetFormFieldName(_context.Handle, AnnotHandle, buffer, (uint)buffer.Length);
        return written == 0 ? null : new string(buffer, 0, (int)written - 1); // Trim null terminator
    }

    /// <summary>
    /// Gets the type of the form field associated with the current annotation.
    /// </summary>
    /// <remarks>This method retrieves the form field type by querying the underlying PDF annotation. The
    /// returned value corresponds to the <see cref="PdfFormFieldType"/> enumeration, which represents various types of
    /// form fields, such as text fields, checkboxes, or radio buttons.</remarks>
    /// <returns>A <see cref="PdfFormFieldType"/> value representing the type of the form field.</returns>
    public PdfFormFieldType GetFieldType()
    {
        int type = PdfAnnotNative.FPDFAnnot_GetFormFieldType(_context.Handle, AnnotHandle);
        return (PdfFormFieldType)type;
    }

    /// <summary>
    /// Retrieves the value of the form field associated with the annotation.
    /// </summary>
    /// <remarks>The returned string represents the current value of the form field. If the field is empty or
    /// the operation fails,  the method returns <see langword="null"/>.</remarks>
    /// <returns>A <see cref="string"/> containing the value of the form field, or <see langword="null"/> if the field has no
    /// value.</returns>
    public string? GetFieldValue()
    {
        char[] buffer = new char[256];
        uint written = PdfAnnotNative.FPDFAnnot_GetFormFieldValue(_context.Handle, AnnotHandle, buffer, (uint)buffer.Length);
        return written == 0 ? null : new string(buffer, 0, (int)written - 1); // Drop null terminator
    }

    /// <summary>
    /// Attempts to read a string value associated with the specified key from the given annotation.
    /// </summary>
    /// <param name="annot">A pointer to the annotation from which the string value is to be retrieved.</param>
    /// <param name="key">The key identifying the string value to retrieve.</param>
    /// <returns>The string value associated with the specified key, or <see langword="null"/> if the key does not exist or the
    /// value cannot be retrieved.</returns>
    private static string? TryReadString(IntPtr annot, string key)
    {
        byte[] buffer = new byte[512];
        if (!PdfAnnotNative.FPDFAnnot_GetStringValue(annot, key, buffer, (uint)buffer.Length))
            return null;

        int len = Array.IndexOf<byte>(buffer, 0);
        return Encoding.ASCII.GetString(buffer, 0, len >= 0 ? len : buffer.Length);
    }

    /// <summary>
    /// Sets the value of the form field, if the field type supports it.
    /// </summary>
    /// <param name="value">The value to set for the form field.</param>
    /// <returns><see langword="true"/> if the value was successfully set; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the field type does not support setting a value. Supported field types are <see
    /// cref="PdfFormFieldType.Text"/>, <see cref="PdfFormFieldType.ComboBox"/>, <see cref="PdfFormFieldType.ListBox"/>,
    /// <see cref="PdfFormFieldType.CheckBox"/>, and <see cref="PdfFormFieldType.RadioButton"/>.</exception>
    public bool SetValue(string value)
    {
        if (FieldType != PdfFormFieldType.Text &&
            FieldType != PdfFormFieldType.ComboBox &&
            FieldType != PdfFormFieldType.ListBox &&
            FieldType != PdfFormFieldType.CheckBox &&
            FieldType != PdfFormFieldType.RadioButton)
            throw new InvalidOperationException("SetValue is not supported for this field type.");

        bool ok = PdfAnnotNative.FPDFAnnot_SetStringValue(AnnotHandle, "V", value);

        if (ok)
            Value = value;

        return ok;
    }

    /// <summary>
    /// Sets the checked state of the form field if it is a checkbox or radio button.
    /// </summary>
    /// <remarks>This method updates the internal value of the form field to "Yes" if <paramref
    /// name="isChecked"/>  is <see langword="true"/> or "Off" if <paramref name="isChecked"/> is <see
    /// langword="false"/>.</remarks>
    /// <param name="isChecked">A value indicating whether the field should be checked.  <see langword="true"/> to set the field as checked;
    /// otherwise, <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the operation succeeds; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the form field is not of type <see cref="PdfFormFieldType.CheckBox"/> or  <see
    /// cref="PdfFormFieldType.RadioButton"/>.</exception>
    public bool SetChecked(bool isChecked)
    {
        if (FieldType != PdfFormFieldType.CheckBox && FieldType != PdfFormFieldType.RadioButton)
            throw new InvalidOperationException("SetChecked only applies to checkbox or radio fields.");

        string value = isChecked ? "Yes" : "Off";
        bool ok = PdfAnnotNative.FPDFAnnot_SetStringValue(AnnotHandle, "V", value);

        if (ok)
            Value = value;

        return ok;
    }

    /// <summary>
    /// Retrieves the color associated with the annotation for the specified color type.
    /// </summary>
    /// <remarks>The method attempts to retrieve the color information for the specified type from the
    /// annotation.  If the operation is successful, the color is returned as an <see cref="RgbaColor"/> object. 
    /// Otherwise, the method returns <see langword="null"/>.</remarks>
    /// <param name="type">The type of color to retrieve. This value determines which color property of the annotation is returned.</param>
    /// <returns>An <see cref="RgbaColor"/> object representing the color, or <see langword="null"/> if the color could not be
    /// retrieved.</returns>
    public RgbaColor? GetColor(int type)
    {
        if (PdfAnnotNative.FPDFAnnot_GetColor(AnnotHandle, type, out uint r, out uint g, out uint b, out uint a))
            return new RgbaColor((byte)r, (byte)g, (byte)b, (byte)a);

        return null;
    }

    /// <summary>
    /// Sets the color of the annotation for the specified color type.
    /// </summary>
    /// <remarks>The behavior of this method depends on the annotation type and the specified color type.
    /// Ensure that the  <paramref name="type"/> parameter corresponds to a valid color type for the
    /// annotation.</remarks>
    /// <param name="type">The type of color to set, such as stroke or fill. Valid values depend on the annotation type.</param>
    /// <param name="color">The <see cref="RgbaColor"/> structure representing the color to apply, including red, green, blue, and alpha
    /// components.</param>
    /// <returns><see langword="true"/> if the color was successfully set; otherwise, <see langword="false"/>.</returns>
    public bool SetColor(PdfFormFieldColorType type, RgbaColor color)
    {
        return PdfAnnotNative.FPDFAnnot_SetColor(
            AnnotHandle,
            (int)type,
            color.R,
            color.G,
            color.B,
            color.A);
    }

    /// <summary>
    /// Sets the fill color for the form field.
    /// </summary>
    /// <param name="color">The <see cref="RgbaColor"/> to use as the fill color. Cannot be null.</param>
    /// <returns><see langword="true"/> if the fill color was successfully set; otherwise, <see langword="false"/>.</returns>
    public bool SetFillColor(RgbaColor color) => SetColor(PdfFormFieldColorType.Fill, color);
    
    /// <summary>
    /// Sets the border color of the PDF form field.
    /// </summary>
    /// <param name="color">The <see cref="RgbaColor"/> to set as the border color. Cannot be null.</param>
    /// <returns><see langword="true"/> if the border color was successfully set; otherwise, <see langword="false"/>.</returns>
    public bool SetBorderColor(RgbaColor color) => SetColor(PdfFormFieldColorType.Border, color);
    
    /// <summary>
    /// Sets the text color for the form field.
    /// </summary>
    /// <param name="color">The <see cref="RgbaColor"/> representing the desired text color. Cannot be null.</param>
    /// <returns><see langword="true"/> if the text color was successfully set; otherwise, <see langword="false"/>.</returns>
    public bool SetTextColor(RgbaColor color) => SetColor(PdfFormFieldColorType.Text, color);

    /// <summary>
    /// Retrieves the export value of the form field associated with the annotation.
    /// </summary>
    /// <remarks>The export value is typically used to represent the value of a form field in a PDF document
    /// for data export purposes.</remarks>
    /// <returns>A <see cref="string"/> containing the export value of the form field, or <see langword="null"/> if no export
    /// value is available.</returns>
    public string? GetExportValue()
    {
        char[] buffer = new char[256];
        uint written = PdfAnnotNative.FPDFAnnot_GetFormFieldExportValue(_context.Handle, AnnotHandle, buffer, (uint)buffer.Length);
        if (written == 0)
            return null;

        return new string(buffer, 0, (int)written - 1); // drop null terminator
    }

    /// <summary>
    /// Retrieves the annotation flags associated with the current PDF annotation.
    /// </summary>
    /// <remarks>The returned flags are determined by the underlying PDF annotation's configuration.  Use the
    /// <see cref="PdfAnnotationFlags"/> enumeration to interpret the specific flags.</remarks>
    /// <returns>A <see cref="PdfAnnotationFlags"/> value representing the flags set on the annotation.  The flags indicate
    /// properties such as visibility, printability, and other annotation behaviors.</returns>
    public PdfAnnotationFlags GetAnnotationFlags()
    {
        int raw = PdfAnnotNative.FPDFAnnot_GetFlags(AnnotHandle);
        return (PdfAnnotationFlags)raw;
    }

    /// <summary>
    /// Sets the annotation flags for the current PDF annotation.
    /// </summary>
    /// <remarks>Annotation flags control the behavior and visibility of a PDF annotation. For example, flags
    /// can specify whether the annotation is hidden, printable, or locked. Refer to the <see
    /// cref="PdfAnnotationFlags"/>  enumeration for the available flag options.</remarks>
    /// <param name="flags">The <see cref="PdfAnnotationFlags"/> value specifying the flags to apply to the annotation.</param>
    /// <returns><see langword="true"/> if the flags were successfully set; otherwise, <see langword="false"/>.</returns>
    public bool SetAnnotationFlags(PdfAnnotationFlags flags)
    {
        return PdfAnnotNative.FPDFAnnot_SetFlags(AnnotHandle, (int)flags);
    }

    /// <summary>
    /// Gets the number of options available in the form field.
    /// </summary>
    /// <remarks>This method is only valid for form fields of type <see cref="PdfFormFieldType.ComboBox"/> or
    /// <see cref="PdfFormFieldType.ListBox"/>. Calling this method on other field types will result in an
    /// exception.</remarks>
    /// <returns>The total number of options available in the form field.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the form field is not of type <see cref="PdfFormFieldType.ComboBox"/> or <see
    /// cref="PdfFormFieldType.ListBox"/>.</exception>
    public int GetOptionCount()
    {
        if (FieldType != PdfFormFieldType.ComboBox && FieldType != PdfFormFieldType.ListBox)
            throw new InvalidOperationException("GetOptionCount is only valid for combo or list boxes.");

        return PdfAnnotNative.FPDFAnnot_GetOptionCount(_context.Handle, AnnotHandle);
    }

    /// <summary>
    /// Retrieves the label of an option at the specified index for a combo box or list box form field.
    /// </summary>
    /// <remarks>This method is only applicable to form fields of type <see cref="PdfFormFieldType.ComboBox"/>
    /// or <see cref="PdfFormFieldType.ListBox"/>. Attempting to call this method on other field types will result in an
    /// exception.</remarks>
    /// <param name="index">The zero-based index of the option whose label is to be retrieved.</param>
    /// <returns>The label of the option as a string, or <see langword="null"/> if the index is invalid or the label cannot be
    /// retrieved.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the form field is not a combo box or list box.</exception>
    public string? GetOptionLabel(int index)
    {
        if (FieldType != PdfFormFieldType.ComboBox && FieldType != PdfFormFieldType.ListBox)
            throw new InvalidOperationException("GetOptionLabel is only valid for combo or list boxes.");

        char[] buffer = new char[256];
        uint written = PdfAnnotNative.FPDFAnnot_GetOptionLabel(_context.Handle, AnnotHandle, index, buffer, (uint)buffer.Length);

        return written == 0 ? null : new string(buffer, 0, (int)written - 1);
    }

    /// <summary>
    /// Retrieves the rectangle that defines the annotation's bounds.
    /// </summary>
    /// <returns>A <see cref="FsRectF"/> structure representing the annotation's bounding rectangle,  or <see langword="null"/>
    /// if the rectangle could not be retrieved.</returns>
    public FsRectF? GetRect()
    {
        if (PdfAnnotNative.FPDFAnnot_GetRect(AnnotHandle, out FsRectF rect))
            return rect;

        return null;
    }

    /// <summary>
    /// Determines whether the current form field is checked.
    /// </summary>
    /// <remarks>This method is only valid for form fields of type <see cref="PdfFormFieldType.CheckBox"/> or
    /// <see cref="PdfFormFieldType.RadioButton"/>. Calling this method on other field types will result in an
    /// exception.</remarks>
    /// <returns><see langword="true"/> if the form field is checked; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the form field is not of type <see cref="PdfFormFieldType.CheckBox"/> or <see
    /// cref="PdfFormFieldType.RadioButton"/>.</exception>
    public bool IsChecked()
    {
        if (FieldType != PdfFormFieldType.CheckBox && FieldType != PdfFormFieldType.RadioButton)
            throw new InvalidOperationException("IsChecked is only valid for checkbox or radio button fields.");

        return PdfAnnotNative.FPDFAnnot_IsChecked(_context.Handle, AnnotHandle);
    }

    /// <summary>
    /// Determines whether the specified option in a combo box or list box field is selected.
    /// </summary>
    /// <param name="index">The zero-based index of the option to check.</param>
    /// <returns><see langword="true"/> if the option at the specified index is selected; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the field is not a combo box or list box.</exception>
    public bool IsOptionSelected(int index)
    {
        if (FieldType != PdfFormFieldType.ComboBox && FieldType != PdfFormFieldType.ListBox)
            throw new InvalidOperationException("IsOptionSelected is only valid for combo or list boxes.");

        return PdfAnnotNative.FPDFAnnot_IsOptionSelected(_context.Handle, AnnotHandle, index);
    }

    /// <summary>
    /// Returns a string representation of the field, including its type, name, value, and page index.
    /// </summary>
    /// <returns>A string in the format: "{FieldType} Field \"{Name}\" = \"{Value}\" (Page {PageIndex})".</returns>
    public override string ToString() => $"{FieldType} Field \"{Name}\" = \"{Value}\" (Page {PageIndex})";

    /// <summary>
    /// Attempts to read the flags associated with the specified annotation.
    /// </summary>
    /// <remarks>This method retrieves the value of the "Ff" property from the annotation.  If the property is
    /// not present or cannot be read, the method returns 0.</remarks>
    /// <param name="annot">A pointer to the annotation from which to read the flags.</param>
    /// <returns>The flags as an unsigned 32-bit integer if successfully read; otherwise, 0.</returns>
    private static uint TryReadFlags(IntPtr annot)
    {
        return PdfAnnotNative.FPDFAnnot_GetNumberValue(annot, "Ff", out float value)
            ? (uint)value
            : 0;
    }
}

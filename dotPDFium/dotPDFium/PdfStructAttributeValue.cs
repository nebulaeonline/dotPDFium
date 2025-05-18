using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public sealed class PdfStructAttributeValue : PdfTaggedObject
{
    internal PdfStructAttributeValue(IntPtr handle) : base(handle) { }

    public bool? TryGetBoolean()
    {
        if (PdfStructTreeNative.FPDF_StructElement_Attr_GetBooleanValue(_handle, out var result))
            return result;

        return null;
    }

    /// <summary>
    /// Attempts to retrieve the attribute's value as a float.
    /// </summary>
    /// <returns>The numeric value, or <c>null</c> if the value is not a number.</returns>
    public float? TryGetNumber()
    {
        return PdfStructTreeNative.FPDF_StructElement_Attr_GetNumberValue(_handle, out float result)
            ? result
            : (float?)null;
    }

    /// <summary>
    /// Attempts to retrieve the attribute's value as a UTF-8 string.
    /// </summary>
    /// <returns>The string value, or <c>null</c> if the value is not a string or missing.</returns>
    public string? TryGetString()
    {
        if (!PdfStructTreeNative.FPDF_StructElement_Attr_GetStringValue(_handle, IntPtr.Zero, 0, out uint size) || size == 0)
            return null;

        var buffer = new byte[size];
        unsafe
        {
            fixed (byte* ptr = buffer)
            {
                if (!PdfStructTreeNative.FPDF_StructElement_Attr_GetStringValue(_handle, (IntPtr)ptr, size, out var written) || written == 0)
                    return null;
            }
        }

        return System.Text.Encoding.UTF8.GetString(buffer, 0, (int)size - 1); // Strip null terminator
    }

    /// <summary>
    /// Gets the underlying type of this attribute value.
    /// </summary>
    /// <returns>The <see cref="PdfObjectType"/> enum value representing the attribute's type.</returns>
    public PdfObjectType GetValueType()
    {
        int raw = PdfStructTreeNative.FPDF_StructElement_Attr_GetType(_handle);
        return Enum.IsDefined(typeof(PdfObjectType), raw)
            ? (PdfObjectType)raw
            : PdfObjectType.Unknown;
    }

}


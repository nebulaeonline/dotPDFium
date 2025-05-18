using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public sealed class PdfStructAttributeSet : PdfTaggedObject
{
    internal PdfStructAttributeSet(IntPtr handle) : base(handle) { }

    /// <summary>
    /// Returns the number of attributes (key-value pairs) in the structure element's attribute dictionary.
    /// </summary>
    public int CountChildren()
    {
        return PdfStructTreeNative.FPDF_StructElement_Attr_CountChildren(_handle);
    }

    /// <summary>
    /// Returns the number of attributes (key-value pairs) in the attribute dictionary.
    /// </summary>
    public int Count => PdfStructTreeNative.FPDF_StructElement_Attr_GetCount(_handle);

    /// <summary>
    /// Attempts to get the attribute's value as a raw binary blob.
    /// </summary>
    /// <returns>
    /// A <see cref="byte[]"/> containing the blob value, or <c>null</c> if the attribute is not a blob or does not exist.
    /// </returns>
    public byte[]? TryGetBlobValue()
    {
        // Query for size
        if (!PdfStructTreeNative.FPDF_StructElement_Attr_GetBlobValue(_handle, IntPtr.Zero, 0, out uint actualSize) || actualSize == 0)
            return null;

        var buffer = new byte[actualSize];
        unsafe
        {
            fixed (byte* ptr = buffer)
            {
                bool success = PdfStructTreeNative.FPDF_StructElement_Attr_GetBlobValue(_handle, (IntPtr)ptr, actualSize, out var written);
                if (!success || written == 0)
                    return null;
            }
        }

        return buffer;
    }

    /// <summary>
    /// Gets the key name for the attribute at the given index.
    /// </summary>
    /// <param name="index">Zero-based index into the attribute set.</param>
    /// <returns>The key name as a string, or null if unavailable.</returns>
    public string? GetKeyAt(int index)
    {
        uint size;
        if (!PdfStructTreeNative.FPDF_StructElement_Attr_GetName(_handle, index, IntPtr.Zero, 0, out size) || size == 0)
            return null;

        var buffer = new byte[size];
        unsafe
        {
            fixed (byte* ptr = buffer)
            {
                if (!PdfStructTreeNative.FPDF_StructElement_Attr_GetName(_handle, index, (IntPtr)ptr, size, out var written) || written == 0)
                    return null;
            }
        }

        return System.Text.Encoding.UTF8.GetString(buffer, 0, (int)size - 1); // Strip null terminator
    }

    /// <summary>
    /// Gets the attribute value object at the given index.
    /// </summary>
    /// <param name="index">Zero-based index of the attribute child.</param>
    /// <returns>A <see cref="PdfStructAttributeValue"/> for the child, or null if not found.</returns>
    public PdfStructAttributeValue? GetValueAt(int index)
    {
        int count = CountChildren();
        if (index < 0 || index >= count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var handle = PdfStructTreeNative.FPDF_StructElement_Attr_GetChildAtIndex(_handle, index);
        return handle == IntPtr.Zero ? null : new PdfStructAttributeValue(handle);
    }

    /// <summary>
    /// Attempts to get a named attribute from the dictionary.
    /// </summary>
    /// <param name="name">The attribute key to retrieve.</param>
    /// <returns>A <see cref="PdfStructAttributeValue"/> if found; otherwise <c>null</c>.</returns>
    public PdfStructAttributeValue? GetValue(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        var handle = PdfStructTreeNative.FPDF_StructElement_Attr_GetValue(_handle, name);
        return handle == IntPtr.Zero ? null : new PdfStructAttributeValue(handle);
    }

}

using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public sealed class PdfStructTree : PdfTaggedObject
{
    internal PdfStructTree(IntPtr handle) : base(handle) { }

    /// <summary>
    /// Returns the number of top-level structure elements in the structure tree.
    /// </summary>
    public int CountChildren()
    {
        return PdfStructTreeNative.FPDF_StructTree_CountChildren(_handle);
    }

    /// <summary>
    /// Returns the structure element at the specified index.
    /// </summary>
    /// <param name="index">Zero-based index of the child element.</param>
    /// <returns>The <see cref="PdfStructElement"/> at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the index is invalid.</exception>
    public PdfStructElement GetChildAt(int index)
    {
        int count = CountChildren();
        if (index < 0 || index >= count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var elementHandle = PdfStructTreeNative.FPDF_StructTree_GetChildAtIndex(_handle, index);

        if (elementHandle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to retrieve structure element at index {index}.");

        return new PdfStructElement(elementHandle);
    }

    protected override void Dispose(bool disposing)
    {
        if (_handle != IntPtr.Zero)
        {
            PdfStructTreeNative.FPDF_StructTree_Close(_handle);
            _handle = IntPtr.Zero;
        }

        base.Dispose(disposing);
    }

}


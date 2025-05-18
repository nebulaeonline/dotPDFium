using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;
/// <summary>
/// Represents the parsed print page range defined in /ViewerPreferences.
/// </summary>
public sealed class PdfPrintPageRange
{
    private readonly IntPtr _rangeHandle;

    internal PdfPrintPageRange(IntPtr handle)
    {
        _rangeHandle = handle;
    }

    /// <summary>
    /// Gets the number of subranges in this print range.
    /// </summary>
    public int Count
    {
        get
        {
            var count = PdfViewNative.FPDF_VIEWERREF_GetPrintPageRangeCount(_rangeHandle);
            return (int)count;
        }
    }

    /// <summary>
    /// Gets the zero-based starting page number of the subrange at the given index.
    /// </summary>
    /// <param name="index">The index of the subrange (0-based).</param>
    /// <returns>The page number, or -1 if invalid.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If index is out of bounds.</exception>
    public int GetPageNumberAt(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        return PdfViewNative.FPDF_VIEWERREF_GetPrintPageRangeElement(_rangeHandle, (UIntPtr)index);
    }

}
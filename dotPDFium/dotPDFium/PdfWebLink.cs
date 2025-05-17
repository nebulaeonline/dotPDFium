using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

/// <summary>
/// Represents a web link within a PDF document, providing access to its associated text range, URL, and bounding
/// rectangles.
/// </summary>
/// <remarks>This class allows users to retrieve information about a web link in a PDF, such as its text range,
/// URL, and the rectangular regions that define its clickable area. Instances of this class are typically obtained
/// through a parent <see cref="PdfText"/> object.</remarks>
public sealed class PdfWebLink
{
    private readonly IntPtr _handle;
    private readonly int _index;
    private readonly PdfText _parent;

    /// <summary>
    /// Represents a hyperlink within a PDF document, associated with a specific text element.
    /// </summary>
    /// <remarks>This class is used internally to manage hyperlinks in PDF documents. It is associated with a
    /// parent <see cref="PdfText"/> object and provides access to the underlying native handle and index of the
    /// link.</remarks>
    /// <param name="parent"></param>
    /// <param name="handle"></param>
    /// <param name="index"></param>
    internal PdfWebLink(PdfText parent, IntPtr handle, int index)
    {
        _parent = parent;
        _handle = handle;
        _index = index;
    }

    /// <summary>
    /// Retrieves the start position and length of the text range associated with the current web link.
    /// </summary>
    /// <returns>A tuple containing the start position and length of the text range: <list type="bullet"> <item>
    /// <description><c>Start</c>: The zero-based starting position of the text range.</description> </item> <item>
    /// <description><c>Length</c>: The number of characters in the text range.</description> </item> </list></returns>
    /// <exception cref="dotPDFiumException">Thrown if the text range cannot be retrieved due to an internal error.</exception>
    public (int Start, int Length) GetTextRange()
    {
        if (!PdfTextNative.FPDFLink_GetTextRange(_handle, _index, out int start, out int length))
            throw new dotPDFiumException("Failed to get web link text range.");
        return (start, length);
    }

    /// <summary>
    /// Retrieves the URL associated with the current link object.
    /// </summary>
    /// <remarks>The returned URL is trimmed of any trailing null characters. This method allocates unmanaged 
    /// memory to retrieve the URL and ensures proper cleanup of resources.</remarks>
    /// <returns>A string containing the URL of the link. Returns an empty string if no URL is available.</returns>
    public string GetUrl()
    {
        int length = PdfTextNative.FPDFLink_GetURL(_handle, _index, IntPtr.Zero, 0);
        if (length <= 0)
            return string.Empty;

        IntPtr buffer = Marshal.AllocHGlobal(length * sizeof(char));
        try
        {
            int written = PdfTextNative.FPDFLink_GetURL(_handle, _index, buffer, length);
            return Marshal.PtrToStringUni(buffer, written)?.TrimEnd('\0') ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// Gets the number of rectangular areas associated with the current link annotation.
    /// </summary>
    /// <remarks>This method retrieves the count of rectangles that represent the clickable regions of a link
    /// annotation in a PDF document. Each rectangle corresponds to a distinct clickable area.</remarks>
    /// <returns>The total number of rectangular areas that define the clickable regions of the link annotation. Returns 0 if no
    /// rectangles are associated with the link.</returns>
    public int GetRectCount()
    {
        return PdfTextNative.FPDFLink_CountRects(_handle, _index);
    }

    /// <summary>
    /// Retrieves the rectangle at the specified index for the current link.
    /// </summary>
    /// <param name="rectIndex">The zero-based index of the rectangle to retrieve.</param>
    /// <returns>An <see cref="FsRect"/> representing the rectangle's boundaries, defined by its left, top, right, and bottom
    /// coordinates.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the rectangle cannot be retrieved for the specified <paramref name="rectIndex"/>.</exception>
    public FsRect GetRect(int rectIndex)
    {
        if (!PdfTextNative.FPDFLink_GetRect(_handle, _index, rectIndex,
            out double left, out double top, out double right, out double bottom))
        {
            throw new dotPDFiumException($"Failed to get rectangle for link at index {_index}, rect {rectIndex}.");
        }

        return new FsRect(left, top, right, bottom);
    }

    /// <summary>
    /// Retrieves a collection of rectangles currently managed by the system.
    /// </summary>
    /// <remarks>Each rectangle in the collection is retrieved lazily as the enumeration progresses. This
    /// method is suitable for scenarios where the number of rectangles is large or unknown, as it avoids loading all
    /// rectangles into memory at once.</remarks>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="FsRect"/> objects representing the rectangles. The collection will
    /// be empty if no rectangles are available.</returns>
    public IEnumerable<FsRect> GetRects()
    {
        int count = GetRectCount();
        for (int i = 0; i < count; i++)
            yield return GetRect(i);
    }
}


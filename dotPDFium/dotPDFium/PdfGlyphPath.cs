using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

/// <summary>
/// Represents a glyph path in a PDF document, providing access to its segments.
/// </summary>
/// <remarks>A glyph path is a series of segments that define the shape of a character or symbol in a PDF. This
/// class allows you to retrieve individual segments or enumerate all segments in the glyph path.</remarks>
public sealed class PdfGlyphPath
{
    private readonly IntPtr _handle;

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfGlyphPath"/> class with the specified native handle.
    /// </summary>
    /// <remarks>This constructor is intended for internal use only and assumes ownership of the provided
    /// handle. Ensure that the handle is properly managed to avoid resource leaks or invalid memory access.</remarks>
    /// <param name="handle">A pointer to the native handle representing the glyph path. Must be a valid, non-null pointer.</param>
    internal PdfGlyphPath(IntPtr handle)
    {
        _handle = handle;
    }

    /// <summary>
    /// Gets the number of segments in the glyph path associated with the current PDF element.
    /// </summary>
    public int SegmentCount => PdfEditNative.FPDFGlyphPath_CountGlyphSegments(_handle);

    /// <summary>
    /// Retrieves the glyph path segment at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the segment to retrieve. Must be greater than or equal to 0 and less than <see
    /// cref="SegmentCount"/>.</param>
    /// <returns>A <see cref="PdfGlyphPathSegment"/> representing the glyph path segment at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than or equal to <see cref="SegmentCount"/>.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the segment cannot be retrieved due to an internal error.</exception>
    public PdfGlyphPathSegment GetSegment(int index)
    {
        if (index < 0 || index >= SegmentCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        var segmentHandle = PdfEditNative.FPDFGlyphPath_GetGlyphPathSegment(_handle, index);
        if (segmentHandle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to retrieve segment at index {index}.");

        return new PdfGlyphPathSegment(segmentHandle);
    }

    /// <summary>
    /// Retrieves all glyph path segments in the current collection.
    /// </summary>
    /// <remarks>This method lazily enumerates the glyph path segments, yielding each segment one at a time. 
    /// It is suitable for scenarios where processing segments incrementally is preferred.</remarks>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="PdfGlyphPathSegment"/> objects representing the glyph path
    /// segments.</returns>
    public IEnumerable<PdfGlyphPathSegment> GetSegments()
    {
        for (int i = 0; i < SegmentCount; i++)
            yield return GetSegment(i);
    }

    /// <summary>
    /// Gets the handle representing a native resource.
    /// </summary>
    internal IntPtr Handle => _handle;
}


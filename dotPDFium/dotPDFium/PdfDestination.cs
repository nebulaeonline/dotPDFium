using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

/// <summary>
/// Represents a destination within a PDF document, typically used for navigation or linking purposes.
/// </summary>
/// <remarks>A destination specifies a particular location within a PDF document, such as a specific page and 
/// optionally a position on that page. This class provides access to the page index and location  information for the
/// destination.</remarks>
public sealed class PdfDestination
{
    private readonly IntPtr _handle;
    private readonly PdfDocument _doc;

    /// <summary>
    /// Represents a destination within a PDF document, such as a specific page or location.
    /// </summary>
    /// <remarks>This class is used to define a target location within a PDF document, which can be used for
    /// navigation or linking purposes. Instances of this class are typically created internally and associated with a
    /// specific PDF document.</remarks>
    /// <param name="handle"></param>
    /// <param name="doc"></param>
    internal PdfDestination(IntPtr handle, PdfDocument doc)
    {
        _handle = handle;
        _doc = doc;
    }

    /// <summary>
    /// Gets the zero-based index of the page associated with the destination.
    /// </summary>
    /// <remarks>This property retrieves the page index from the underlying PDF document. The index is
    /// zero-based, meaning the first page is represented by 0.</remarks>
    public int PageIndex => PdfDocNative.FPDFDest_GetPageIndex(_doc.Handle, _handle);

    /// <summary>
    /// Retrieves the location of the destination within the page.
    /// </summary>
    /// <remarks>This method queries the destination's position within the page and returns it as a point.  If
    /// the destination does not have a defined X or Y coordinate, the corresponding value in  the returned <see
    /// cref="FsPointF"/> will default to 0.</remarks>
    /// <returns>A <see cref="FsPointF"/> representing the location of the destination within the page,  or <see
    /// langword="null"/> if the location cannot be determined.  The X and Y coordinates will be set to 0 if they are
    /// not explicitly defined.</returns>
    public FsPointF? GetLocation()
    {
        if (!PdfDocNative.FPDFDest_GetLocationInPage(_handle, out bool hasX, out bool hasY, out bool hasZoom, out float x, out float y, out float zoom))
            return null;

        return new FsPointF(hasX ? x : 0, hasY ? y : 0); // You can expose zoom later if needed
    }

    /// <summary>
    /// Gets the handle associated with the current instance.
    /// </summary>
    internal IntPtr Handle => _handle;
}



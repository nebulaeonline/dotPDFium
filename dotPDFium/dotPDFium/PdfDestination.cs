using nebulae.dotPDFium.Native;
using System;

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
    /// Creates a new PdfDestination wrapper from a PDFium destination handle and document.
    /// </summary>
    /// <param name="handle">The native destination handle.</param>
    /// <param name="doc">The document this destination belongs to.</param>
    internal PdfDestination(IntPtr handle, PdfDocument doc)
    {
        _handle = handle;
        _doc = doc;
    }

    /// <summary>
    /// Gets the zero-based index of the page associated with the destination.
    /// </summary>
    public int PageIndex => PdfDocNative.FPDFDest_GetPageIndex(_doc.Handle, _handle);

    /// <summary>
    /// Retrieves the location of the destination within the page.
    /// </summary>
    /// <returns>
    /// A <see cref="FsPointF"/> representing the location of the destination within the page,
    /// or <see langword="null"/> if the location cannot be determined.
    /// Coordinates will be zero if not explicitly defined.
    /// </returns>
    public FsPointF? GetLocation()
    {
        if (!PdfDocNative.FPDFDest_GetLocationInPage(
                _handle, out bool hasX, out bool hasY, out bool hasZoom,
                out float x, out float y, out float zoom))
        {
            return null;
        }

        return new FsPointF(hasX ? x : 0, hasY ? y : 0);
    }

    /// <summary>
    /// Retrieves the zoom level of the destination, if explicitly defined.
    /// </summary>
    /// <returns>The zoom factor if defined, otherwise <see langword="null"/>.</returns>
    public float? GetZoom()
    {
        if (!PdfDocNative.FPDFDest_GetLocationInPage(
                _handle, out _, out _, out bool hasZoom,
                out _, out _, out float zoom))
        {
            return null;
        }

        return hasZoom ? zoom : null;
    }

    /// <summary>
    /// Retrieves the zero-based index of the destination page directly associated with this object.
    /// </summary>
    /// <remarks>This method returns <see langword="null"/> if no valid destination page is associated with
    /// the object.</remarks>
    /// <returns>The zero-based index of the destination page if one is associated; otherwise, <see langword="null"/>.</returns>
    public int? GetDirectDestinationPage()
    {
        int index = PdfDocNative.FPDFDest_GetDestPageIndex(_doc.Handle, _handle);
        return index >= 0 ? index : null;
    }


    /// <summary>
    /// Attempts to retrieve the destination page index associated with the current destination object.
    /// </summary>
    /// <param name="pageIndex">When this method returns, contains the zero-based index of the destination page if the operation succeeds; 
    /// otherwise, contains a negative value.</param>
    /// <returns><see langword="true"/> if the destination page index was successfully retrieved; otherwise, <see
    /// langword="false"/>.</returns>
    public bool TryGetDirectDestinationPage(out int pageIndex)
    {
        pageIndex = PdfDocNative.FPDFDest_GetDestPageIndex(_doc.Handle, _handle);
        return pageIndex >= 0;
    }

    /// <summary>
    /// Retrieves the view mode and parameters associated with the destination.
    /// </summary>
    /// <returns>
    /// A tuple containing the <see cref="PdfDestViewMode"/> and associated parameters,
    /// or <see langword="null"/> if the view information is not available.
    /// </returns>
    public (PdfDestViewMode ViewMode, float[] Parameters)? GetView()
    {
        uint paramCount;
        var mode = (PdfDestViewMode)PdfDocNative.FPDFDest_GetView(_handle, out paramCount, Array.Empty<float>());

        if (mode == PdfDestViewMode.Unknown || paramCount == 0)
            return null;

        var buffer = new float[paramCount];
        PdfDocNative.FPDFDest_GetView(_handle, out _, buffer);

        return (mode, buffer);
    }

    /// <summary>
    /// Gets the underlying PDFium destination handle.
    /// </summary>
    internal IntPtr Handle => _handle;
}

using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

public class PdfPathObject : PdfPageObject
{
    internal PdfPathObject(IntPtr handle)
        : base(handle, PdfPageObjectType.Path)
    {
    }

    /// <summary>
    /// Moves the current path to the specified coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate of the new position.</param>
    /// <param name="y">The y-coordinate of the new position.</param>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails to move the path to the specified coordinates.</exception>
    public void MoveTo(float x, float y)
    {
        if (!PdfEditNative.FPDFPath_MoveTo(_handle, x, y))
            throw new dotPDFiumException($"Failed to move path to ({x},{y}): {PdfObject.GetPDFiumError()}");
    }

    /// <summary>
    /// Adds a straight line segment from the current point to the specified coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate of the endpoint of the line segment.</param>
    /// <param name="y">The y-coordinate of the endpoint of the line segment.</param>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails due to an error in the underlying PDF library.</exception>
    public void LineTo(float x, float y)
    {
        if (!PdfEditNative.FPDFPath_LineTo(_handle, x, y))
            throw new dotPDFiumException($"Failed to draw line to ({x},{y}): {PdfObject.GetPDFiumError()}");
    }

    /// <summary>
    /// Closes the current path in the PDF document.
    /// </summary>
    /// <remarks>This method finalizes the current path by connecting the last point to the first point, 
    /// forming a closed shape. It is typically used when creating vector graphics or shapes  in a PDF document. If the
    /// operation fails, an exception is thrown.</remarks>
    /// <exception cref="dotPDFiumException">Thrown if the path cannot be closed due to an error in the underlying PDF library.</exception>
    public void ClosePath()
    {
        if (!PdfEditNative.FPDFPath_Close(_handle))
            throw new dotPDFiumException("Failed to close path: " + PdfObject.GetPDFiumError());
    }

    /// <summary>
    /// Retrieves the drawing mode and stroke information for the current PDF path.
    /// </summary>
    /// <param name="mode">When this method returns, contains the drawing mode of the PDF path, represented as a <see
    /// cref="PdfPathDrawMode"/> enumeration value.</param>
    /// <param name="stroke">When this method returns, contains a value indicating whether the path is stroked. <see langword="true"/> if the
    /// path is stroked; otherwise, <see langword="false"/>.</param>
    /// <exception cref="dotPDFiumException">Thrown if the drawing mode cannot be retrieved due to an error in the underlying PDFium library.</exception>
    public void GetDrawMode(out PdfPathDrawMode mode, out bool stroke)
    {
        if (!PdfEditNative.FPDFPath_GetDrawMode(_handle, out int _mode, out bool _stroke))
            throw new dotPDFiumException("Failed to get draw mode: " + PdfObject.GetPDFiumError());

        mode = (PdfPathDrawMode)_mode;
        stroke = _stroke;
    }

    /// <summary>
    /// Sets the drawing mode for the current PDF path.
    /// </summary>
    /// <param name="mode">The drawing mode to apply to the path. This determines how the path is rendered, such as fill, stroke, or a
    /// combination of both.</param>
    /// <param name="stroke">A value indicating whether the path should be stroked.  <see langword="true"/> to stroke the path; otherwise,
    /// <see langword="false"/>.</param>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails to set the draw mode.</exception>
    public void SetDrawMode(PdfPathDrawMode mode, bool stroke)
    {
        if (!PdfEditNative.FPDFPath_SetDrawMode(_handle, (int)mode, stroke))
            throw new dotPDFiumException($"Failed to set draw mode: {PdfObject.GetPDFiumError()}");
    }
}
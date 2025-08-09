using nebulae.dotPDFium.Native;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium;

public class PdfPathObject : PdfPageObject
{
    internal PdfPathObject(IntPtr handle)
        : base(handle, PdfPageObjectType.Path)
    {
    }

    /// <summary>
    /// Appends a cubic Bézier curve to the current path.
    /// </summary>
    /// <param name="x1">X-coordinate of the first control point.</param>
    /// <param name="y1">Y-coordinate of the first control point.</param>
    /// <param name="x2">X-coordinate of the second control point.</param>
    /// <param name="y2">Y-coordinate of the second control point.</param>
    /// <param name="x3">X-coordinate of the end point.</param>
    /// <param name="y3">Y-coordinate of the end point.</param>
    /// <exception cref="dotPDFiumException">Thrown if the curve could not be added.</exception>
    public void BezierTo(float x1, float y1, float x2, float y2, float x3, float y3)
    {
        if (!PdfEditNative.FPDFPath_BezierTo(_handle, x1, y1, x2, y2, x3, y3))
            throw new dotPDFiumException($"Failed to add Bézier curve to path: {PdfObject.GetPDFiumError()}");
    }

    /// <summary>
    /// Returns the number of segments in this path object.
    /// </summary>
    public int GetSegmentCount()
    {
        int count = PdfEditNative.FPDFPath_CountSegments(_handle);
        return count < 0 ? 0 : count;
    }

    /// <summary>
    /// Returns true if this segment closes the current subpath.
    /// </summary>
    public bool IsCloseSegment()
    {
        return PdfEditNative.FPDFPathSegment_GetClose(this.Handle);
    }

    /// <summary>
    /// Gets the (x, y) coordinates of this path segment.
    /// </summary>
    public (double X, double Y) GetPoint()
    {
        if (!PdfEditNative.FPDFPathSegment_GetPoint(Handle, out double x, out double y))
            throw new dotPDFiumException("Failed to retrieve path segment coordinates.");

        return (x, y);
    }

    /// <summary>
    /// Gets the type of this path segment.
    /// </summary>
    public PdfPathSegmentType GetSegmentType()
    {
        return (PdfPathSegmentType)PdfEditNative.FPDFPathSegment_GetType(Handle);
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

    /// <summary>
    /// Retrieves the dash pattern used for stroking paths in the current page object.
    /// </summary>
    /// <returns>An array of <see langword="float"/> values representing the dash pattern. Each value specifies      the length
    /// of a dash or gap in the pattern. Returns an empty array if no dash pattern is defined.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the dash pattern cannot be retrieved due to an error in the underlying PDF library.</exception>
    /// <summary>
    /// Returns the number of elements in the stroke dash pattern (e.g. [3 2] => 2).
    /// </summary>
    public int GetDashPatternCount()
    {
        int count = PdfEditNative.FPDFPageObj_GetDashCount(_handle);
        return count < 0 ? 0 : count;
    }

    /// <summary>
    /// Retrieves the full stroke dash pattern as an array of floats. If none is defined, returns an empty array.
    /// </summary>
    public float[] GetDashPattern()
    {
        int count = GetDashPatternCount();
        if (count == 0)
            return Array.Empty<float>();

        var dashArray = new float[count];
        bool ok = PdfEditNative.FPDFPageObj_GetDashArray(_handle, dashArray, (UIntPtr)count);
        if (!ok)
            throw new dotPDFiumException("Failed to retrieve dash array.");

        return dashArray;
    }

    /// <summary>
    /// Sets the dash pattern and phase used when stroking the path.
    /// </summary>
    /// <param name="pattern">An array of floats representing the dash/gap lengths (e.g. [3,2] for 3pt line, 2pt gap).</param>
    /// <param name="phase">Offset into the dash pattern at which stroking begins.</param>
    /// <exception cref="ArgumentException">Thrown if pattern is null or empty.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the dash pattern could not be set.</exception>
    public void SetDashPattern(float[] pattern, float phase = 0f)
    {
        if (pattern == null || pattern.Length == 0)
            throw new ArgumentException("Dash pattern cannot be null or empty.", nameof(pattern));

        if (!PdfEditNative.FPDFPageObj_SetDashArray(_handle, pattern, (UIntPtr)pattern.Length, phase))
            throw new dotPDFiumException("Failed to set dash pattern.");
    }

    /// <summary>
    /// Retrieves the dash phase, which specifies the offset into the dash pattern at which stroking begins.
    /// </summary>
    /// <returns>The dash phase as a float.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the phase could not be retrieved.</exception>
    public float GetDashPhase()
    {
        if (!PdfEditNative.FPDFPageObj_GetDashPhase(_handle, out float phase))
            throw new dotPDFiumException("Failed to retrieve dash phase.");

        return phase;
    }

    /// <summary>
    /// Sets the dash phase, which specifies the offset into the dash pattern at which stroking begins.
    /// </summary>
    /// <param name="phase">The phase offset (in points).</param>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails.</exception>
    public void SetDashPhase(float phase)
    {
        if (!PdfEditNative.FPDFPageObj_SetDashPhase(_handle, phase))
            throw new dotPDFiumException("Failed to set dash phase.");
    }

    /// <summary>
    /// Retrieves the line cap style used when stroking the path.
    /// </summary>
    /// <returns>A <see cref="PdfLineCapStyle"/> value indicating the cap type.</returns>
    public PdfLineCapStyle GetLineCap()
    {
        int raw = PdfEditNative.FPDFPageObj_GetLineCap(_handle);

        return raw switch
        {
            0 => PdfLineCapStyle.Butt,
            1 => PdfLineCapStyle.Round,
            2 => PdfLineCapStyle.Square,
            _ => throw new dotPDFiumException($"Unknown line cap style: {raw}")
        };
    }

    /// <summary>
    /// Sets the line cap style used at the ends of stroked open subpaths.
    /// </summary>
    /// <param name="capStyle">The desired <see cref="PdfLineCapStyle"/>.</param>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails.</exception>
    public void SetLineCap(PdfLineCapStyle capStyle)
    {
        bool ok = PdfEditNative.FPDFPageObj_SetLineCap(_handle, (int)capStyle);
        if (!ok)
            throw new dotPDFiumException("Failed to set line cap style.");
    }

    /// <summary>
    /// Retrieves the line join style used when stroking connected segments in the path.
    /// </summary>
    /// <returns>A <see cref="PdfLineJoinStyle"/> indicating how corners are rendered.</returns>
    public PdfLineJoinStyle GetLineJoin()
    {
        int raw = PdfEditNative.FPDFPageObj_GetLineJoin(_handle);

        return raw switch
        {
            0 => PdfLineJoinStyle.Miter,
            1 => PdfLineJoinStyle.Round,
            2 => PdfLineJoinStyle.Bevel,
            _ => throw new dotPDFiumException($"Unknown line join style: {raw}")
        };
    }

    /// <summary>
    /// Sets the line join style used for path corners when stroking.
    /// </summary>
    /// <param name="joinStyle">The desired <see cref="PdfLineJoinStyle"/> to apply.</param>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails.</exception>
    public void SetLineJoin(PdfLineJoinStyle joinStyle)
    {
        bool ok = PdfEditNative.FPDFPageObj_SetLineJoin(_handle, (int)joinStyle);
        if (!ok)
            throw new dotPDFiumException("Failed to set line join style.");
    }    
}
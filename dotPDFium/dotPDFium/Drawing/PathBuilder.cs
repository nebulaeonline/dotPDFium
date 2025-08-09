using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium.Drawing;

/// <summary>
/// Provides a fluent interface for building complex paths using PDFium's path operations.
/// </summary>
public class PathBuilder
{
    private readonly DrawingOptions _options;
    private readonly IntPtr _handle;
    private bool _hasMoved;

    /// <summary>
    /// Creates a new PathBuilder instance with the specified drawing options.
    /// </summary>
    /// <param name="options"></param>
    /// <exception cref="dotPDFiumException"></exception>
    public PathBuilder(DrawingOptions? options = null)
    {
        _options = options ?? DrawingOptions.Default;
        _handle = PdfEditNative.FPDFPageObj_CreateNewPath(0, 0);
        if (_handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to create path: {PdfObject.GetPDFiumError()}");
    }

    /// <summary>
    /// Moves the current point to the specified coordinates.
    /// </summary>
    /// <param name="x">X coordinate to move to</param>
    /// <param name="y">Y coordinate to move to</param>
    /// <returns>PathBuilder</returns>
    /// <exception cref="dotPDFiumException"></exception>
    public PathBuilder MoveTo(float x, float y)
    {
        if (!PdfEditNative.FPDFPath_MoveTo(_handle, x, y))
            throw new dotPDFiumException($"MoveTo failed: {PdfObject.GetPDFiumError()}");
        _hasMoved = true;
        return this;
    }

    /// <summary>
    /// Draws a line from the current point to the specified coordinates.
    /// </summary>
    /// <param name="x">X coordinate to draw to</param>
    /// <param name="y">Y coordinate to draw to</param>
    /// <returns>PathBuilder</returns>
    /// <exception cref="InvalidOperationException">Thrown when there is no place to draw a line to</exception>
    /// <exception cref="dotPDFiumException">Thrown on PDFium library error</exception>
    public PathBuilder LineTo(float x, float y)
    {
        if (!_hasMoved)
            throw new InvalidOperationException("Must call MoveTo() before LineTo().");

        if (!PdfEditNative.FPDFPath_LineTo(_handle, x, y))
            throw new dotPDFiumException($"LineTo failed: {PdfObject.GetPDFiumError()}");
        return this;
    }

    /// <summary>
    /// Draws a cubic Bézier curve from the current point to the specified endpoint.
    /// </summary>
    /// <param name="x1">X coordinate of the first control point</param>
    /// <param name="y1">Y coordinate of the first control point</param>
    /// <param name="x2">X coordinate of the second control point</param>
    /// <param name="y2">Y coordinate of the second control point</param>
    /// <param name="x3">X coordinate of the endpoint</param>
    /// <param name="y3">Y coordinate of the endpoint</param>
    /// <returns>PathBuilder</returns>
    /// <exception cref="InvalidOperationException">Thrown when there is no place to draw a curve to</exception>
    /// <exception cref="dotPDFiumException">Thrown on PDFium library error</exception>
    public PathBuilder BezierTo(float x1, float y1, float x2, float y2, float x3, float y3)
    {
        if (!_hasMoved)
            throw new InvalidOperationException("Must call MoveTo() before BezierTo().");

        if (!PdfEditNative.FPDFPath_BezierTo(_handle, x1, y1, x2, y2, x3, y3))
            throw new dotPDFiumException($"BezierTo failed: {PdfObject.GetPDFiumError()}");
        return this;
    }

    /// <summary>
    /// Draws a rounded rectangle at the specified coordinates with the given radius.
    /// </summary>
    /// <param name="x">X coordinate of the upper left of the rectangle</param>
    /// <param name="y">Y coordinate of the upper left of the rectangle</param>
    /// <param name="width">Width of the rectangle</param>
    /// <param name="height">Height of the rectangle</param>
    /// <param name="radius">Radius of the corners</param>
    /// <returns>PathBuilder</returns>
    public PathBuilder RoundRect(float x, float y, float width, float height, float radius)
    {
        // Clamp radius so it doesn't exceed half the shortest side
        radius = Math.Min(radius, Math.Min(width, height) / 2);

        // Approximation constant for a quarter circle
        const float C = 0.552284749831f; // (4/3)*tan(pi/8)

        float rC = radius * C;

        MoveTo(x + radius, y);
        LineTo(x + width - radius, y);
        BezierTo(x + width - radius + rC, y, x + width, y + radius - rC, x + width, y + radius);
        LineTo(x + width, y + height - radius);
        BezierTo(x + width, y + height - radius + rC, x + width - radius + rC, y + height, x + width - radius, y + height);
        LineTo(x + radius, y + height);
        BezierTo(x + radius - rC, y + height, x, y + height - radius + rC, x, y + height - radius);
        LineTo(x, y + radius);
        BezierTo(x, y + radius - rC, x + radius - rC, y, x + radius, y);
        Close();

        return this;
    }

    /// <summary>
    /// Draws a rounded rectangle based on a rectangle object with the specified radius.
    /// </summary>
    /// <param name="rect">The rectangle object</param>
    /// <param name="radius">The radius of the corners</param>
    /// <returns>PathBuilder</returns>
    public PathBuilder RoundRect(FsRectF rect, float radius)
    {
        float x = rect.left;
        float y = rect.top;
        float width = rect.right - rect.left;
        float height = rect.bottom - rect.top;

        return RoundRect(x, y, width, height, radius);
    }

    /// <summary>
    /// Draws an ellipse at the specified coordinates with the given width and height.
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="width">Width of the ellipse</param>
    /// <param name="height">Height of the ellipse</param>
    /// <returns>PathBuilder</returns>
    public PathBuilder Ellipse(float x, float y, float width, float height)
    {
        // Center + radii
        float rx = width / 2f;
        float ry = height / 2f;
        float cx = x + rx;
        float cy = y + ry;

        // Cubic Bézier kappa for a quarter circle
        const float K = 0.552284749831f;
        float cxr = rx * K;
        float cyr = ry * K;

        // Start at top
        MoveTo(cx, cy - ry);

        // Top-right
        BezierTo(cx + cxr, cy - ry, cx + rx, cy - cyr, cx + rx, cy);
        // Bottom-right
        BezierTo(cx + rx, cy + cyr, cx + cxr, cy + ry, cx, cy + ry);
        // Bottom-left
        BezierTo(cx - cxr, cy + ry, cx - rx, cy + cyr, cx - rx, cy);
        // Top-left
        BezierTo(cx - rx, cy - cyr, cx - cxr, cy - ry, cx, cy - ry);

        Close();
        return this;
    }

    /// <summary>
    /// Draws an ellipse based on a rectangle object.
    /// </summary>
    /// <param name="rect"></param>
    /// <returns>PathBuilder</returns>
    public PathBuilder Ellipse(FsRectF rect) =>
        Ellipse(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);

    /// <summary>
    /// Draws a circle at the specified center with the given radius.
    /// </summary>
    /// <param name="cx">X coordinate of the center</param>
    /// <param name="cy">Y coordinate of the center</param>
    /// <param name="radius">Radius of the circle</param>
    /// <returns>PathBuilder</returns>
    public PathBuilder Circle(float cx, float cy, float radius)
    {
        float r = Math.Max(0f, radius);
        // Same construction as ellipse with rx=ry=r
        const float K = 0.552284749831f;
        float c = r * K;

        MoveTo(cx, cy - r);
        BezierTo(cx + c, cy - r, cx + r, cy - c, cx + r, cy);
        BezierTo(cx + r, cy + c, cx + c, cy + r, cx, cy + r);
        BezierTo(cx - c, cy + r, cx - r, cy + c, cx - r, cy);
        BezierTo(cx - r, cy - c, cx - c, cy - r, cx, cy - r);
        Close();

        return this;
    }

    public PathBuilder Transform(float a, float b, float c, float d, float e, float f)
    {
        FsMatrixF matrix = new FsMatrixF(a, b, c, d, e, f);
        PdfEditNative.FPDFPageObj_TransformF(_handle, ref matrix);
        return this;
    }

    public PathBuilder Transform(FsMatrixF matrix)
    {
        PdfEditNative.FPDFPageObj_TransformF(_handle, ref matrix);
        return this;
    }

    public PathBuilder Translate(float dx, float dy)
        => Transform(1, 0, 0, 1, dx, dy);

    public PathBuilder Scale(float sx, float sy)
        => Transform(sx, 0, 0, sy, 0, 0);

    public PathBuilder Scale(float sx, float sy, float cx, float cy)
    {
        Translate(-cx, -cy);
        Scale(sx, sy);
        Translate(cx, cy);
        return this;
    }

    public PathBuilder Rotate(float degrees)
    {
        double r = Math.PI * degrees / 180.0;
        float cos = (float)Math.Cos(r);
        float sin = (float)Math.Sin(r);
        return Transform(cos, sin, -sin, cos, 0, 0);
    }

    public PathBuilder Rotate(float degrees, float cx, float cy)
    {
        Translate(-cx, -cy);
        Rotate(degrees);
        Translate(cx, cy);
        return this;
    }

    public PathBuilder Shear(float shxDegrees, float shyDegrees)
    {
        float shx = (float)Math.Tan(Math.PI * shxDegrees / 180.0);
        float shy = (float)Math.Tan(Math.PI * shyDegrees / 180.0);
        return Transform(1, shy, shx, 1, 0, 0);
    }

    /// <summary>
    /// Closes the current sub-path.
    /// </summary>
    public PathBuilder Close()
    {
        if (!PdfEditNative.FPDFPath_Close(_handle))
            throw new dotPDFiumException($"ClosePath failed: {PdfObject.GetPDFiumError()}");
        return this;
    }

    /// <summary>
    /// Finalizes the path and applies the drawing options.
    /// </summary>
    /// <returns>PdfPathObject</returns>
    public PdfPathObject Build()
    {
        Drawing.ApplyDrawingOptions(_handle, _options);
        return new PdfPathObject(_handle);
    }
}


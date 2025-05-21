using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium.Drawing;

public class PathBuilder
{
    private readonly DrawingOptions _options;
    private readonly IntPtr _handle;
    private bool _hasMoved;

    public PathBuilder(DrawingOptions? options = null)
    {
        _options = options ?? DrawingOptions.Default;
        _handle = PdfEditNative.FPDFPageObj_CreateNewPath(0, 0);
        if (_handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to create path: {PdfObject.GetPDFiumError()}");
    }

    public PathBuilder MoveTo(float x, float y)
    {
        if (!PdfEditNative.FPDFPath_MoveTo(_handle, x, y))
            throw new dotPDFiumException($"MoveTo failed: {PdfObject.GetPDFiumError()}");
        _hasMoved = true;
        return this;
    }

    public PathBuilder LineTo(float x, float y)
    {
        if (!_hasMoved)
            throw new InvalidOperationException("Must call MoveTo() before LineTo().");

        if (!PdfEditNative.FPDFPath_LineTo(_handle, x, y))
            throw new dotPDFiumException($"LineTo failed: {PdfObject.GetPDFiumError()}");
        return this;
    }

    public PathBuilder BezierTo(float x1, float y1, float x2, float y2, float x3, float y3)
    {
        if (!_hasMoved)
            throw new InvalidOperationException("Must call MoveTo() before BezierTo().");

        if (!PdfEditNative.FPDFPath_BezierTo(_handle, x1, y1, x2, y2, x3, y3))
            throw new dotPDFiumException($"BezierTo failed: {PdfObject.GetPDFiumError()}");
        return this;
    }

    public PathBuilder Close()
    {
        if (!PdfEditNative.FPDFPath_Close(_handle))
            throw new dotPDFiumException($"ClosePath failed: {PdfObject.GetPDFiumError()}");
        return this;
    }

    public PdfPathObject Build()
    {
        Drawing.ApplyDrawingOptions(_handle, _options);
        return new PdfPathObject(_handle);
    }
}


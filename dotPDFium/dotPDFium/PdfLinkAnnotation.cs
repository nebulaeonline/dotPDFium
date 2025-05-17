using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public sealed class PdfLinkAnnotation : PdfAnnotation
{
    internal PdfLinkAnnotation(IntPtr handle, PdfPage page)
        : base(handle, page)
    {
    }

    public FsRectF GetBoundingRect()
    {
        if (!PdfDocNative.FPDFLink_GetAnnotRect(_handle, out var rect))
            throw new dotPDFiumException("Failed to get link annotation rect.");

        return rect;
    }

    public int GetQuadPointCount()
    {
        return PdfDocNative.FPDFLink_CountQuadPoints(_handle);
    }

    public FsQuadPointsF GetQuadPoints(int index)
    {
        if (!PdfDocNative.FPDFLink_GetQuadPoints(_handle, index, out var quad))
            throw new dotPDFiumException($"Failed to get quad point at index {index}.");

        return quad;
    }

    public PdfDestination? GetDestination(PdfDocument doc)
    {
        var handle = PdfDocNative.FPDFLink_GetDest(doc.Handle, _handle);
        return handle == IntPtr.Zero ? null : new PdfDestination(handle, doc);
    }

    public PdfAction? GetAction()
    {
        var handle = PdfDocNative.FPDFLink_GetAction(_handle);
        return handle == IntPtr.Zero ? null : new PdfAction(handle);
    }
}


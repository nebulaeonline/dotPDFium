using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public sealed class PdfLink : PdfPageObject
{
    private readonly PdfDocument _document;

    internal PdfLink(IntPtr handle, PdfDocument document)
        : base(handle)
    {
        _document = document;
    }

    public PdfDestination? GetDestination()
    {
        var destHandle = PdfDocNative.FPDFLink_GetDest(_document.Handle, Handle);
        return destHandle == IntPtr.Zero ? null : new PdfDestination(destHandle, _document);
    }

    public PdfAction? GetAction()
    {
        var actionHandle = PdfDocNative.FPDFLink_GetAction(Handle);
        return actionHandle == IntPtr.Zero ? null : new PdfAction(actionHandle);
    }
}

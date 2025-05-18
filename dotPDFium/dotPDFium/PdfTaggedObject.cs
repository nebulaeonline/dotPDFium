using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public abstract class PdfTaggedObject : IDisposable
{
    protected IntPtr _handle;
    private bool _disposed;

    internal IntPtr Handle => _handle;

    internal PdfTaggedObject(IntPtr handle)
    {
        _handle = handle;
    }

    ~PdfTaggedObject() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        // Add release logic here if PDFium adds it later.
        _handle = IntPtr.Zero;
        _disposed = true;
    }
}


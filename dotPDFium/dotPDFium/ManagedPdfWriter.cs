using nebulae.dotPDFium.Native;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium;

internal class ManagedPdfWriter : IDisposable
{
    private readonly Stream _output;
    private FPDF_FILEWRITE _fileWrite;
    private readonly FPDFWriteBlockDelegate _writeCallback;

    public ref FPDF_FILEWRITE GetFileWrite() => ref _fileWrite;

    public ManagedPdfWriter(Stream output)
    {
        _output = output ?? throw new ArgumentNullException(nameof(output));
        _writeCallback = new FPDFWriteBlockDelegate(WriteBlockImpl);

        _fileWrite = new FPDF_FILEWRITE
        {
            version = 1,
            userData = IntPtr.Zero,
            WriteBlock = _writeCallback
        };
    }

    public ManagedPdfWriter(string filePath)
        : this(new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None)) { }

    private int WriteBlockImpl(IntPtr _, IntPtr data, uint size)
    {
        try
        {
            byte[] buffer = new byte[size];
            Marshal.Copy(data, buffer, 0, (int)size);
            _output.Write(buffer, 0, (int)size);
            return 1;
        }
        catch
        {
            return 0;
        }
    }

    public void Dispose()
    {
        _output?.Dispose();
    }
}



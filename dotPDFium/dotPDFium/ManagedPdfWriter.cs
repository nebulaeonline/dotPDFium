using System.Diagnostics;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct FPDF_FILEWRITE
{
    public int version;

    // IMPORTANT: must match exact native layout (no userData!)
    public FPDFWriteBlockDelegate WriteBlock;
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int FPDFWriteBlockDelegate(IntPtr fileWrite, IntPtr data, uint size);

public class ManagedPdfWriter : IDisposable
{
    private readonly Stream _output;
    private readonly FPDFWriteBlockDelegate _writeCallback;
    private GCHandle _delegateHandle;
    private IntPtr _fileWritePtr;
    private bool _disposed = false;

    public IntPtr NativePtr => _fileWritePtr;

    public ManagedPdfWriter(Stream output)
    {
        _output = output ?? throw new ArgumentNullException(nameof(output));

        // Create and pin the callback delegate
        _writeCallback = WriteBlockImpl;
        _delegateHandle = GCHandle.Alloc(_writeCallback, GCHandleType.Normal);

        // Create the FPDF_FILEWRITE structure
        var fileWrite = new FPDF_FILEWRITE
        {
            version = 1,
            WriteBlock = _writeCallback
        };

        // Allocate memory and marshal the structure to unmanaged memory
        _fileWritePtr = Marshal.AllocHGlobal(Marshal.SizeOf<FPDF_FILEWRITE>());

        // Use try/catch to help diagnose issues
        try
        {
            Marshal.StructureToPtr(fileWrite, _fileWritePtr, false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error marshaling FPDF_FILEWRITE: {ex.Message}");
            throw;
        }
    }

    public ManagedPdfWriter(string filePath)
        : this(new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
    {
    }

    private int WriteBlockImpl(IntPtr fileWrite, IntPtr data, uint size)
    {
        // Quick checks to help debug
        if (data == IntPtr.Zero)
        {
            Debug.WriteLine("Warning: data pointer is null in WriteBlockImpl");
            return 0;
        }

        if (size == 0)
        {
            Debug.WriteLine("Warning: size is 0 in WriteBlockImpl");
            return 1; // Nothing to write, consider it success
        }

        Debug.WriteLine($"WriteBlockImpl called with size: {size}");

        try
        {
            // Buffer for the data
            byte[] buffer = new byte[size];

            // Copy data from unmanaged memory
            Marshal.Copy(data, buffer, 0, (int)size);

            // Write to the output stream
            _output.Write(buffer, 0, (int)size);

            return 1; // Success
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in WriteBlockImpl: {ex.Message}");
            return 0; // Failure
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                _output?.Dispose();
            }

            // Free unmanaged resources
            if (_delegateHandle.IsAllocated)
            {
                _delegateHandle.Free();
            }

            if (_fileWritePtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_fileWritePtr);
                _fileWritePtr = IntPtr.Zero;
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~ManagedPdfWriter()
    {
        Dispose(false);
    }
}
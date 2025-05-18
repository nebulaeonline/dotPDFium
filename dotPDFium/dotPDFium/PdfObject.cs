using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

/// <summary>
/// This enum represents the type of the PDF object.
/// </summary>
public enum PdfObjectType
{
    Unknown = 0,
    Document,
    Page,
    TextPage,
    Bitmap,
    PageObject,
    Annotation,
    Font,
    Form,
    Signature,
}

/// <summary>
/// This class is the base class for all PDFium objects. It implements IDisposable to 
/// ensure proper resource management.
/// </summary>
public abstract class PdfObject : IDisposable
{
    protected IntPtr _handle;
    protected PdfObjectType _type;
    protected bool _disposed = false;

    public IntPtr Handle => _handle;
    public PdfObjectType Type => _type;
    public bool IsDisposed => _disposed;

    /// <summary>
    /// Returns the last error message from PDFium.
    /// </summary>
    /// <returns></returns>
    public static string GetPDFiumError()
    {
        var err = PdfViewNative.FPDF_GetLastError();
        return err switch
        {
            0 => "No error",
            1 => "Unknown error",
            2 => "File not found or could not be opened",
            3 => "Invalid format",
            4 => "Incorrect or missing password",
            5 => "Security error (access denied)",
            6 => "Page error (corrupted content)",
            _ => $"Unknown error code: {err}"
        };
    }

    /// <summary>
    /// Base class constructor. This constructor is protected and should not be used directly.
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="type"></param>
    protected PdfObject(IntPtr handle, PdfObjectType type)
    {
        _handle = handle;
        _type = type;
    }

    /// <summary>
    /// Base class destructor. This destructor is here for the IDisposable pattern.
    /// </summary>
    ~PdfObject()
    {
        Dispose(false);
    }

    /// <summary>
    /// Releases the resources used by this instance of the PdfObject class.
    /// </summary>
    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the resources used by the current instance of the class.
    /// </summary>
    /// <remarks>This method should be called when the instance is no longer needed to free unmanaged
    /// resources. It ensures that any associated native resources are properly released based on the type of the
    /// object.</remarks>
    /// <param name="disposing">Are we disposing managed objects?</param>
    /// <exception cref="InvalidOperationException">Thrown if the object type is not supported for disposal.</exception>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (_handle == IntPtr.Zero)
            return;

        switch (_type)
        {
            case PdfObjectType.Document:
                PdfViewNative.FPDF_CloseDocument(_handle);
                break;

            case PdfObjectType.Page:
                PdfViewNative.FPDF_ClosePage(_handle);
                break;

            case PdfObjectType.TextPage:
                PdfTextNative.FPDFText_ClosePage(_handle);
                break;

            case PdfObjectType.Bitmap:
                PdfViewNative.FPDFBitmap_Destroy(_handle);
                break;

            // Add more types here as needed
            default:
                throw new InvalidOperationException($"Dispose() not implemented for {_type}");
        }

        _handle = IntPtr.Zero;
        _disposed = true;
    }
}

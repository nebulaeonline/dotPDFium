using nebulae.dotPDFium;
using nebulae.dotPDFium.Native;

/// <summary>
/// Represents a clipping path in a PDF document, which can be used to define regions for rendering or masking content.
/// </summary>
/// <remarks>A <see cref="PdfClipPath"/> is created with specific bounds and can be inserted into a <see
/// cref="PdfPage"/> to define a clipping region. It provides methods to query the paths and segments that make up the
/// clipping path.</remarks>
public class PdfClipPath : IDisposable
{
    private IntPtr _handle;
    private bool _ownsHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfClipPath"/> class with the specified handle.
    /// </summary>
    /// <remarks>This constructor is intended for internal use and should not be called directly by user
    /// code.</remarks>
    /// <param name="handle">A pointer to the native handle representing the PDF clip path.</param>
    /// <param name="ownsHandle">A value indicating whether the instance owns the handle and is responsible for releasing it. <see
    /// langword="true"/> if the instance owns the handle; otherwise, <see langword="false"/>.</param>
    internal PdfClipPath(IntPtr handle, bool ownsHandle = true)
    {
        _handle = handle;
        _ownsHandle = ownsHandle;
    }

    /// <summary>
    /// Creates a new <see cref="PdfClipPath"/> instance with the specified rectangular bounds.
    /// </summary>
    /// <param name="bounds">The rectangular bounds that define the clipping path. The rectangle is specified using its left, bottom, right,
    /// and top coordinates.</param>
    /// <returns>A new <see cref="PdfClipPath"/> instance representing the defined clipping path.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the clip path could not be created.</exception>
    public static PdfClipPath Create(FsRectF bounds)
    {
        var handle = PdfTransformPageNative.FPDF_CreateClipPath(
            bounds.left, bounds.bottom, bounds.right, bounds.top);
        if (handle == IntPtr.Zero)
            throw new dotPDFiumException("Failed to create clip path.");
        return new PdfClipPath(handle);
    }

    /// <summary>
    /// Inserts the current clip path into the specified PDF page.
    /// </summary>
    /// <remarks>This method modifies the specified PDF page by adding the current clip path to it.  Ensure
    /// that the <paramref name="page"/> object is valid and properly initialized before calling this method.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> object representing the PDF page where the clip path will be inserted. Cannot be <see
    /// langword="null"/>.</param>
    public void InsertIntoPage(PdfPage page)
    {
        PdfTransformPageNative.FPDFPage_InsertClipPath(page.Handle, _handle);
    }

    /// <summary>
    /// Gets the number of paths in the current PDF clip path.
    /// </summary>
    /// <remarks>This property retrieves the count of paths defined in the clip path of the PDF page.  A clip
    /// path is used to define the visible region of the page content.</remarks>
    public int PathCount => PdfTransformPageNative.FPDFClipPath_CountPaths(_handle);

    /// <summary>
    /// Gets the number of path segments in the specified path index of the clip path.
    /// </summary>
    /// <param name="pathIndex">The zero-based index of the path within the clip path. Must be a valid index for the clip path.</param>
    /// <returns>The number of segments in the specified path. Returns 0 if the path index is invalid or the clip path contains
    /// no segments.</returns>
    public int GetSegmentCount(int pathIndex)
        => PdfTransformPageNative.FPDFClipPath_CountPathSegments(_handle, pathIndex);

    /// <summary>
    /// Retrieves a pointer to a specific path segment within a clip path.
    /// </summary>
    /// <remarks>The returned pointer can be used to access details about the path segment. Ensure that the
    /// indices provided are within the valid range for the clip path to avoid unexpected behavior.</remarks>
    /// <param name="pathIndex">The zero-based index of the path within the clip path.</param>
    /// <param name="segmentIndex">The zero-based index of the segment within the specified path.</param>
    /// <returns>A pointer to the path segment, or <see cref="IntPtr.Zero"/> if the specified path or segment does not exist.</returns>
    public IntPtr GetPathSegment(int pathIndex, int segmentIndex)
        => PdfTransformPageNative.FPDFClipPath_GetPathSegment(_handle, pathIndex, segmentIndex);

    /// <summary>
    /// Releases the resources used by the current instance of the class.
    /// </summary>
    /// <remarks>This method should be called to release unmanaged resources when the instance is no longer
    /// needed.  After calling this method, the instance should not be used.</remarks>
    public void Dispose()
    {
        if (_ownsHandle && _handle != IntPtr.Zero)
        {
            PdfTransformPageNative.FPDF_DestroyClipPath(_handle);
            _handle = IntPtr.Zero;
        }
    }

    /// <summary>
    /// Gets the handle associated with the current instance.
    /// </summary>
    public IntPtr Handle => _handle;
}

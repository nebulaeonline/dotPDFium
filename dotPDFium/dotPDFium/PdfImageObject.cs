using nebulae.dotPDFium.Native;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium;

public class PdfImageObject : PdfPageObject
{
    private static readonly GetBlockDelegate _sharedBlockDelegate = GetBlock;

    internal PdfImageObject(IntPtr handle)
        : base(handle, PdfPageObjectType.Image)
    {
    }

    /// <summary>
    /// Retrieves the decoded image data from the PDF image object.
    /// </summary>
    /// <returns>A byte array containing the decoded image data. Returns an empty array if no image data is available.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the decoded image data cannot be retrieved due to an error in the underlying PDF library.</exception>
    public byte[] GetDecodedImageData()
    {
        uint size = PdfEditNative.FPDFImageObj_GetImageDataDecoded(_handle, null!, 0);
        if (size == 0)
            return Array.Empty<byte>();

        var buffer = new byte[size];

        if (PdfEditNative.FPDFImageObj_GetImageDataDecoded(_handle, buffer, size) != 0)
            throw new dotPDFiumException($"Failed to retrieve decoded image data: {PdfObject.GetPDFiumError()}");

        return buffer;
    }

    /// <summary>
    /// Retrieves the raw image data from the PDF image object.
    /// </summary>
    /// <remarks>This method returns the raw byte data of the image associated with the PDF image object.  If
    /// the image object contains no data, an empty byte array is returned.</remarks>
    /// <returns>A byte array containing the raw image data. If the image object has no data, an empty array is returned.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the raw image data cannot be retrieved due to an error in the underlying PDF library.</exception>
    public byte[] GetRawImageData()
    {
        uint size = PdfEditNative.FPDFImageObj_GetImageDataRaw(_handle, null!, 0);
        if (size == 0)
            return Array.Empty<byte>();

        var buffer = new byte[size];

        if (PdfEditNative.FPDFImageObj_GetImageDataRaw(_handle, buffer, size) != 0)
            throw new dotPDFiumException($"Failed to retrieve raw image data: {PdfObject.GetPDFiumError()}");

        return buffer;
    }

    /// <summary>
    /// Retrieves the ICC (International Color Consortium) profile data associated with the specified PDF page.
    /// </summary>
    /// <remarks>The ICC profile data is used to define the color characteristics of the PDF content. This
    /// method first determines the required buffer size for the ICC profile data and then retrieves the data if
    /// available.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> object representing the PDF page from which to extract the ICC profile data.</param>
    /// <returns>A byte array containing the ICC profile data. Returns an empty array if no ICC profile data is available.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the ICC profile data cannot be retrieved due to an error in the underlying PDF processing library.</exception>
    public byte[] GetIccProfileData(PdfPage page)
    {
        // First call to get the required buffer size
        PdfEditNative.FPDFImageObj_GetIccProfileDataDecoded(
            _handle, page.Handle, Array.Empty<byte>(), UIntPtr.Zero, out var size);

        if (size == UIntPtr.Zero)
            return Array.Empty<byte>();

        var buffer = new byte[(int)size];
        if (!PdfEditNative.FPDFImageObj_GetIccProfileDataDecoded(
                _handle, page.Handle, buffer, (UIntPtr)buffer.Length, out _))
        {
            throw new dotPDFiumException("Failed to retrieve ICC profile data.");
        }

        return buffer;
    }

    /// <summary>
    /// Gets the number of image filters applied to the current image object.
    /// </summary>
    /// <remarks>This method retrieves the count of filters associated with the image object represented by
    /// this instance. Use this method to determine how many filters are applied before accessing individual filter
    /// details.</remarks>
    /// <returns>The total number of image filters applied. Returns 0 if no filters are applied.</returns>
    public int GetImageFilterCount()
    {
        return PdfEditNative.FPDFImageObj_GetImageFilterCount(_handle);
    }

    /// <summary>
    /// Retrieves the name of the image filter applied to the image at the specified index.
    /// </summary>
    /// <remarks>The method retrieves the name of the image filter associated with an image object in a PDF. 
    /// The index must correspond to a valid filter applied to the image; otherwise, an exception is thrown.</remarks>
    /// <param name="index">The zero-based index of the image filter to retrieve.</param>
    /// <returns>A string representing the name of the image filter. The string is encoded in ASCII and does not include a null
    /// terminator.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the image filter cannot be retrieved for the specified <paramref name="index"/>.</exception>
    public string GetImageFilter(int index)
    {
        const int maxLen = 64; // arbitrary; PDF filters are short
        var buffer = new byte[maxLen];

        uint actualLen = PdfEditNative.FPDFImageObj_GetImageFilter(_handle, index, buffer, (uint)buffer.Length);
        if (actualLen == 0)
            throw new dotPDFiumException($"Failed to retrieve image filter at index {index}");

        return System.Text.Encoding.ASCII.GetString(buffer, 0, (int)actualLen - 1); // omit null terminator
    }

    /// <summary>
    /// Retrieves the pixel dimensions of the image associated with this object.
    /// </summary>
    /// <returns>An <see cref="FsSize"/> structure representing the width and height of the image in pixels.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the image pixel dimensions cannot be retrieved.</exception>
    public FsSize GetImagePixelSize()
    {
        if (!PdfEditNative.FPDFImageObj_GetImagePixelSize(_handle, out uint width, out uint height))
            throw new dotPDFiumException("Failed to retrieve image pixel dimensions.");

        return new FsSize(width, height);
    }

    /// <summary>
    /// Retrieves the rendered bitmap representation of the current image object on the specified PDF page.
    /// </summary>
    /// <remarks>The dimensions of the returned bitmap are not known and must be determined by the caller if
    /// needed.</remarks>
    /// <param name="doc">The PDF document containing the page and image object. Cannot be <see langword="null"/>.</param>
    /// <param name="page">The PDF page containing the image object. Cannot be <see langword="null"/>.</param>
    /// <returns>A <see cref="PdfBitmap"/> representing the rendered image object, or <see langword="null"/> if the rendering
    /// fails.</returns>
    public PdfBitmap? GetRenderedBitmap(PdfDocument doc, PdfPage page)
    {
        var handle = PdfEditNative.FPDFImageObj_GetRenderedBitmap(doc.Handle, page.Handle, this.Handle);
        if (handle == IntPtr.Zero)
            return null;

        return new PdfBitmap(handle, 0, 0); // no dimensions known
    }

    /// <summary>
    /// Loads a JPEG image into the current PDF image object.
    /// </summary>
    /// <remarks>This method uses the native PDFium library to load a JPEG image into the current image
    /// object.  If a page is specified, the image will be associated with that page. Ensure that the  <paramref
    /// name="access"/> object provides valid access to the JPEG file.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page where the image will be loaded.  Can be <see
    /// langword="null"/> if the image is not associated with a specific page.</param>
    /// <param name="access">A <see cref="PdfFileAccess"/> object that provides access to the JPEG file.  This object must remain valid for
    /// the duration of the operation.</param>
    /// <exception cref="dotPDFiumException">Thrown if the JPEG image fails to load into the PDF image object.</exception>
    public void LoadJpeg(PdfPage? page, PdfFileAccess access)
    {
        IntPtr[]? pages = page != null ? new[] { page.Handle } : null;
        if (!PdfEditNative.FPDFImageObj_LoadJpegFile(pages, pages?.Length ?? 0, this.Handle, ref access))
            throw new dotPDFiumException("Failed to load JPEG into image object.");
    }

    /// <summary>
    /// Loads a JPEG image into the current PDF image object inline.
    /// </summary>
    /// <remarks>This method attempts to load a JPEG image directly into the PDF image object. If a page is
    /// provided,  the image is associated with that page. If no page is provided, the image is loaded without a
    /// specific  page association. Ensure that the <paramref name="fileAccess"/> parameter is properly configured to 
    /// provide access to the JPEG file.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> to associate with the image, or <see langword="null"/> if no specific page is
    /// associated.</param>
    /// <param name="fileAccess">The <see cref="FPDF_FILEACCESS"/> structure providing access to the JPEG file.</param>
    /// <exception cref="dotPDFiumException">Thrown if the JPEG image could not be loaded into the image object inline.</exception>
    public void LoadJpegInline(PdfPage? page, PdfFileAccess fileAccess)
    {
        IntPtr[]? pages = page != null ? new[] { page.Handle } : null;
        int count = pages?.Length ?? 0;

        if (!PdfEditNative.FPDFImageObj_LoadJpegFileInline(pages, count, this.Handle, ref fileAccess))
            throw new dotPDFiumException("Failed to load JPEG into image object inline.");
    }

    /// <summary>
    /// Loads a JPEG image from the specified file path and associates it with the given PDF page.
    /// </summary>
    /// <remarks>This method reads the JPEG image from the provided file path and associates it with the
    /// specified PDF page. If <paramref name="page"/> is <see langword="null"/>, the image will not be associated with
    /// any page.</remarks>
    /// <param name="page">The PDF page to which the JPEG image will be added. Can be <see langword="null"/> if no page is specified.</param>
    /// <param name="filePath">The file path of the JPEG image to load. Must not be <see langword="null"/> or empty.</param>
    public void LoadJpegFromPath(PdfPage? page, string filePath)
    {
        using var data = LoadFileAsPinned(filePath);
        LoadJpeg(page, data.Access);
    }

    /// <summary>
    /// Loads a JPEG image from the specified file path and embeds it inline within the given PDF page.
    /// </summary>
    /// <remarks>This method reads the JPEG image from the provided file path, processes it, and embeds it
    /// inline  within the specified PDF page. If <paramref name="page"/> is <see langword="null"/>, the behavior 
    /// depends on the implementation of the underlying PDF library.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> object where the JPEG image will be embedded.  This parameter can be <see
    /// langword="null"/> if no specific page is targeted.</param>
    /// <param name="filePath">The file path of the JPEG image to load. Must be a valid path to an existing file.</param>
    public void LoadJpegInlineFromPath(PdfPage? page, string filePath)
    {
        using var data = LoadFileAsPinned(filePath);
        LoadJpegInline(page, data.Access);
    }

    /// <summary>
    /// Represents a pinned memory buffer for accessing image data, ensuring the buffer remains fixed in memory.
    /// </summary>
    /// <remarks>This class is used to load an image file into memory, pin the buffer to prevent it from being
    /// moved by the garbage collector,  and provide access to the file data through a <see cref="PdfFileAccess"/>
    /// structure.  The buffer is automatically unpinned when the object is disposed.</remarks>
    private sealed class PinnedData : IDisposable
    {
        public byte[] Buffer { get; }
        public GCHandle Handle { get; }
        public PdfFileAccess Access { get; }

        public PinnedData(string filePath)
        {
            Buffer = File.ReadAllBytes(filePath);
            Handle = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
            Access = new PdfFileAccess
            {
                fileLength = (uint)Buffer.Length,
                getBlock = Marshal.GetFunctionPointerForDelegate(_sharedBlockDelegate),
                userData = GCHandle.ToIntPtr(Handle)
            };
        }

        public void Dispose()
        {
            if (Handle.IsAllocated)
                Handle.Free();
        }
    }

    /// <summary>
    /// Loads a file from the specified path and returns its data in a pinned memory structure.
    /// </summary>
    /// <param name="path">The full path to the file to be loaded. Must not be null or empty.</param>
    /// <returns>A <see cref="PinnedData"/> object containing the file's data in pinned memory.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file specified by <paramref name="path"/> does not exist.</exception>
    private static PinnedData LoadFileAsPinned(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("File does not exist", path);

        return new PinnedData(path);
    }

    /// <summary>
    /// Copies a block of data from a managed byte array to an unmanaged memory buffer.
    /// </summary>
    /// <remarks>This method assumes that <paramref name="param"/> is a valid pointer to a <see
    /// cref="GCHandle"/> referencing a managed byte array. The caller is responsible for ensuring that the unmanaged
    /// buffer referenced by <paramref name="buffer"/> is large enough to hold the specified number of bytes.</remarks>
    /// <param name="param">A pointer to a <see cref="GCHandle"/> that references the managed byte array.</param>
    /// <param name="position">The starting position, in bytes, within the managed array from which to copy data.</param>
    /// <param name="buffer">A pointer to the unmanaged memory buffer where the data will be copied.</param>
    /// <param name="size">The number of bytes to copy from the managed array to the unmanaged buffer.</param>
    /// <returns><see langword="1"/> if the operation succeeds; otherwise, <see langword="0"/> if an error occurs.</returns>
    private static int GetBlock(IntPtr param, uint position, IntPtr buffer, uint size)
    {
        try
        {
            var handle = GCHandle.FromIntPtr(param);
            var data = (byte[])handle.Target!;
            Marshal.Copy(data, (int)position, buffer, (int)size);
            return 1;
        }
        catch
        {
            return 0;
        }
    }
    /// <summary>
    /// Sets the specified bitmap as the content of this image object.
    /// </summary>
    /// <remarks>If a page is provided, the bitmap will be associated with that page. If no page is specified,
    /// the bitmap will be set without a specific page context.</remarks>
    /// <param name="bitmap">The <see cref="PdfBitmap"/> to set as the image content. Cannot be null.</param>
    /// <param name="page">The <see cref="PdfPage"/> associated with the bitmap, or <see langword="null"/> if the bitmap is not tied to a
    /// specific page.</param>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails due to an error in the underlying PDF library.</exception>
    public void SetBitmap(PdfBitmap bitmap, PdfPage? page = null)
    {
        IntPtr[]? pages = page != null ? new[] { page.Handle } : null;
        
        if (!PdfEditNative.FPDFImageObj_SetBitmap(pages, pages?.Length ?? 0, this.Handle, bitmap.Handle))
            throw new dotPDFiumException($"Failed to set bitmap: {PdfObject.GetPDFiumError()}");
    }

    /// <summary>
    /// Sets the blend mode for this page object. Valid values are "Normal" (default), "Multiply", "Screen",
    /// "Overlay", "Darken", "Lighten", "ColorDodge", "ColorBurn", "HardLight", "SoftLight",
    /// "Difference" and "Exclusion".
    /// </summary>
    /// <param name="blendMode">The PDF blend mode name as a string.</param>
    /// <exception cref="ArgumentNullException">Thrown if blendMode is null or empty.</exception>
    public void SetBlendMode(string blendMode)
    {
        if (string.IsNullOrWhiteSpace(blendMode))
            throw new ArgumentNullException(nameof(blendMode));

        PdfEditNative.FPDFPageObj_SetBlendMode(this.Handle, blendMode);
    }

    /// <summary>
    /// Sets the transformation matrix for the image object.
    /// </summary>
    /// <remarks>The transformation matrix defines how the image object is scaled, skewed, rotated, and
    /// translated within the PDF. Ensure that the provided values form a valid transformation matrix.</remarks>
    /// <param name="a">The horizontal scaling factor.</param>
    /// <param name="b">The horizontal skewing factor.</param>
    /// <param name="c">The vertical skewing factor.</param>
    /// <param name="d">The vertical scaling factor.</param>
    /// <param name="e">The horizontal translation (x-axis offset).</param>
    /// <param name="f">The vertical translation (y-axis offset).</param>
    /// <exception cref="dotPDFiumException">Thrown if the operation to set the image matrix fails.</exception>
    public void SetMatrix(double a, double b, double c, double d, double e, double f)
    {
        if (!PdfEditNative.FPDFImageObj_SetMatrix(this.Handle, a, b, c, d, e, f))
            throw new dotPDFiumException("Failed to set image matrix.");
    }
}
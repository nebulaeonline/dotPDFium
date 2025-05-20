using nebulae.dotPDFium;
using nebulae.dotPDFium.Native;

/// <summary>
/// The PdfBitmap class represents a bitmap image in the PDFium library. 
/// It provides methods to create and manipulate bitmap images from PDF pages.
/// </summary>
public class PdfBitmap : PdfObject
{
    private readonly int _width;
    private readonly int _height;

    public int HelloDocfx() => 42;

    /// <summary>
    /// Constructor for PdfBitmap. This constructor is internal and should not be used directly.
    /// </summary>
    /// <param name="handle">The PDFium pointer to the bitmap</param>
    /// <param name="width">The width of the bitmap in pixels</param>
    /// <param name="height">The height of the bitmap in pixels</param>
    internal PdfBitmap(IntPtr handle, int width, int height)
        : base(handle, PdfObjectType.Bitmap)
    {
        _width = width;
        _height = height;
    }

    /// <summary>
    /// Returns the width of the bitmap in pixels.
    /// </summary>
    public int Width => _width;

    /// <summary>
    /// Returns the height of the bitmap in pixels.
    /// </summary>
    public int Height => _height;

    /// <summary>
    /// Gets a pointer to the memory buffer containing the raw image data of the PDF bitmap.
    /// </summary>
    /// <remarks>The buffer is managed by the underlying PDF rendering library and should not be modified or
    /// freed by the caller. The caller is responsible for ensuring the associated bitmap handle remains valid while
    /// accessing the buffer.</remarks>
    public IntPtr Buffer => PdfViewNative.FPDFBitmap_GetBuffer(_handle);

    /// <summary>
    /// Gets the stride of the bitmap, which is the number of bytes in a row of pixel data.
    /// </summary>
    public int Stride => PdfViewNative.FPDFBitmap_GetStride(_handle);

    /// <summary>
    /// Gets the format of the bitmap, which indicates the color depth and pixel format.
    /// </summary>
    public PdfBitmapFormat Format => (PdfBitmapFormat)PdfViewNative.FPDFBitmap_GetFormat(_handle);

    /// <summary>
    /// Fills a rectangle in the bitmap with the specified color.
    /// </summary>
    /// <param name="left">The left pixel parameter of the origin of the rectangle</param>
    /// <param name="top">The top pixel parameter of the origin of the rectangle</param>
    /// <param name="width">The width parameter of the rectangle in pixels</param>
    /// <param name="height">The height parameter of the rectangle in pixels</param>
    /// <param name="color">The color to use to fill the rectangle</param>
    public void FillRect(int left, int top, int width, int height, uint color)
    {
        PdfViewNative.FPDFBitmap_FillRect(_handle, left, top, width, height, color);
    }

    /// <summary>
    /// Creates a new bitmap with the specified width, height, and alpha channel support.
    /// </summary>
    /// <param name="width">The width of the bitmap in pixels</param>
    /// <param name="height">The height of the bitmap in pixels</param>
    /// <param name="alpha">Whether or not the bitmap should support an alpha channel (transparency)</param>
    /// <returns>a new PdfBitmap</returns>
    /// <exception cref="dotPDFiumException">Throws on PDFium library error</exception>
    public static PdfBitmap Create(int width, int height, bool alpha = true)
    {
        var handle = PdfViewNative.FPDFBitmap_Create(width, height, alpha ? 1 : 0);
        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to create bitmap: {PdfObject.GetPDFiumError()}");

        return new PdfBitmap(handle, width, height);
    }

    /// <summary>
    /// Create a bitmap with the specified width, height, format, buffer, and stride.
    /// </summary>
    /// <param name="width">The width of the bitmap in pixels</param>
    /// <param name="height">The height of the bitmap in pixels</param>
    /// <param name="format">The format of the bitmap</param>
    /// <param name="buffer">The buffer for the newly created bitmap</param>
    /// <param name="stride">The stride of the new bitmap (width of a row)</param>
    /// <returns>a new PdfBitmap object whose buffer is user-controlled</returns>
    /// <exception cref="dotPDFiumException"></exception>
    public static PdfBitmap CreateEx(int width, int height, PdfBitmapFormat format, IntPtr buffer, int stride)
    {
        var handle = PdfViewNative.FPDFBitmap_CreateEx(width, height, (int)format, buffer, stride);
        if (handle == IntPtr.Zero)
            throw new dotPDFiumException("Failed to create bitmap (Ex).");

        return new PdfBitmap(handle, width, height);
    }

    /// <summary>
    /// Dispose method. This method is protected and should not be used directly.
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing); // triggers FPDFBitmap_Destroy
    }
}

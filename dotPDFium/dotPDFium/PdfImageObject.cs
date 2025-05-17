using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

public class PdfImageObject : PdfPageObject
{
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
}
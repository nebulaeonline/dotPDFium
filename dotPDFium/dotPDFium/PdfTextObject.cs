using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

/// <summary>
/// PdfTextObject represents a text object within a PDF document. It is derived from PdfPageObject and provides
/// the ability to manipulate text content on a PDF page.
/// </summary>
public class PdfTextObject : PdfPageObject
{
    /// <summary>
    /// Constructor for creating a new PdfTextObject instance. This constructor is internal and should not be used
    /// </summary>
    /// <param name="handle"></param>
    internal PdfTextObject(IntPtr handle)
        : base(handle, PdfPageObjectType.Text)
    {
    }

    /// <summary>
    /// Retrieves the font associated with the current text object.
    /// </summary>
    /// <returns>A <see cref="PdfFont"/> instance representing the font of the text object.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the font cannot be retrieved from the text object.</exception>
    public PdfFont GetFont()
    {
        var fontHandle = PdfEditNative.FPDFTextObj_GetFont(_handle);

        if (fontHandle == IntPtr.Zero)
            throw new dotPDFiumException("Failed to get font from text object.");

        // Name is unknown at this stage unless you track it earlier
        return new PdfFont(fontHandle, "<anonymous>");
    }

    /// <summary>
    /// Retrieves the font size of the text object.
    /// </summary>
    /// <returns>The font size of the text object as a <see cref="float"/>.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the font size cannot be retrieved from the text object.</exception>
    public float GetFontSize()
    {
        float fontSize = 0.0f;
        var retVal = PdfEditNative.FPDFTextObj_GetFontSize(_handle, out fontSize);

        return retVal ? fontSize : throw new dotPDFiumException($"Failed to get font size from text object: {PdfObject.GetPDFiumError()}");
    }

    /// <summary>
    /// Extracts the text content from the specified PDF page.
    /// </summary>
    /// <remarks>The extracted text may include a null terminator at the end, which is removed before
    /// returning the result.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> object representing the page from which to extract text.</param>
    /// <returns>A <see cref="string"/> containing the text content of the specified PDF page.  Returns an empty string if the
    /// page contains no text or if the extraction fails.</returns>
    public string GetText(PdfPage page)
    {
        var length = PdfEditNative.FPDFTextObj_GetText(_handle, page.Handle, null!, 0);
        
        if (length <= 0)
            return string.Empty;

        var buffer = new char[length];
        PdfEditNative.FPDFTextObj_GetText(_handle, page.Handle, buffer, length);
        return new string(buffer, 0, (int)length - 1); // PDFium includes null terminator
    }

    /// <summary>
    /// Retrieves the text rendering mode of the current PDF text object.
    /// </summary>
    /// <returns>A <see cref="PdfTextRenderMode"/> value representing the rendering mode of the text object.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the text rendering mode retrieved is not a valid <see cref="PdfTextRenderMode"/> value.</exception>
    public PdfTextRenderMode GetRenderMode()
    {
        int mode = PdfEditNative.FPDFTextObj_GetTextRenderMode(_handle);
        return Enum.IsDefined(typeof(PdfTextRenderMode), mode)
            ? (PdfTextRenderMode)mode
            : throw new dotPDFiumException($"Unknown text render mode: {mode}");
    }

    /// <summary>
    /// Sets the text rendering mode for the current PDF text object.
    /// </summary>
    /// <remarks>The <paramref name="mode"/> parameter specifies the rendering style for text, which can
    /// include options such as filling, stroking, or clipping text. Ensure that the text object is valid and properly
    /// initialized before calling this method.</remarks>
    /// <param name="mode">The text rendering mode to apply. This determines how text is rendered, such as fill, stroke, or a combination
    /// of both.</param>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails due to an error in the underlying PDF library.</exception>
    public void SetRenderMode(PdfTextRenderMode mode)
    {
        if (!PdfEditNative.FPDFTextObj_SetTextRenderMode(_handle, (int)mode))
            throw new dotPDFiumException($"Failed to set text render mode: {PdfObject.GetPDFiumError()}");
    }

    /// <summary>
    /// Sets the text content of the PdfTextObject. This method allows you to modify the text displayed in the text object.
    /// </summary>
    /// <param name="text"></param>
    /// <exception cref="dotPDFiumException"></exception>
    public void SetText(string text)
    {
        if (!PdfEditNative.FPDFText_SetText(_handle, text))
            throw new dotPDFiumException($"Failed to set text: {PdfObject.GetPDFiumError()}");
    }
}


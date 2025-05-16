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


using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

public static class PDFiumEngine
{
    private static bool _isInitialized = false;
    public static bool IsInitialized => _isInitialized;

    /// <summary>
    /// Initializes the PDFium library. This method should be called before using any PDFium functions.
    /// </summary>
    public static void Init()
    {
        if (!_isInitialized)
        {
            PDFiumLibraryResolver.Register();
            PdfViewNative.FPDF_InitLibrary();
            _isInitialized = true;
        }
    }

    /// <summary>
    /// Shuts down the PDFium library. This method should be called when the application is done using PDFium.
    /// Calling Shutdown() will unload PDFium. All native handles (documents, pages, bitmaps) become invalid. 
    /// Use with care.
    /// </summary>
    public static void Shutdown()
    {
        if (_isInitialized)
        {
            PdfViewNative.FPDF_DestroyLibrary();
            _isInitialized = false;
        }
    }
}

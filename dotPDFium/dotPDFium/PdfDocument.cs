using nebulae.dotPDFium.Native;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium;

/// <summary>
/// Specifies the PDF file version supported by a document or operation.
/// </summary>
/// <remarks>The values of this enumeration correspond to the major and minor versions of the PDF specification.
/// For example, <see cref="PdfFileVersion.Pdf14"/> represents PDF version 1.4.</remarks>
public enum PdfFileVersion
{
    Pdf14 = 14,
    Pdf15 = 15,
    Pdf16 = 16,
    Pdf17 = 17
}

/// <summary>
/// Specifies the type of font used in a PDF document or text object.
/// </summary>
public enum PdfFontType
{
    Type1 = 1,
    TrueType = 2
}
public class PdfDocument : PdfObject
{
    /// <summary>
    /// This is a list of open pages; it is used to keep track of the pages that are currently open.
    /// </summary>
    private readonly HashSet<PdfPage> _openPages = new();

    /// <summary>
    /// Registers a PDF page for tracking within the current context.
    /// </summary>
    /// <remarks>This method adds the specified <see cref="PdfPage"/> to an internal collection of open pages.
    /// Ensure that the <paramref name="page"/> is not already registered to avoid duplicate entries.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> instance to register. Cannot be <see langword="null"/>.</param>
    internal void RegisterPage(PdfPage page) => _openPages.Add(page);

    /// <summary>
    /// Unregisters a PDF page from tracking within the current context.
    /// </summary>
    /// <param name="page"></param>
    internal void UnregisterPage(PdfPage page) => _openPages.Remove(page);

    /// <summary>
    /// Class constructor. This constructor is private and should not be used directly.
    /// </summary>
    /// <param name="handle">The pointer to the document object</param>
    /// <exception cref="dotPDFiumException">Thrown on PDFium library error</exception>
    private PdfDocument(IntPtr handle) : base(handle, PdfObjectType.Document)
    {
        // Throw on null pointer
        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Invalid document handle ({nameof(handle)}): {PdfObject.GetPDFiumError()}");
    }

    /// <summary>
    /// Loads a PDF document from a file. The file path must be valid and the file must exist.
    /// </summary>
    /// <param name="filePath">Path to the file to load</param>
    /// <param name="password">Password to the file being loaded</param>
    /// <returns>PdfDocument on success</returns>
    /// <exception cref="ArgumentException">Thrown on null or empty filePath</exception>
    /// <exception cref="dotPDFiumException">Thrown on PDFium library error</exception>
    public static PdfDocument LoadFromFile(string filePath, string? password = null)
    {
        // Throw on null or empty file path
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        var handle = PdfViewNative.FPDF_LoadDocument(filePath, password ?? "");

        // Throw on null pointer
        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to load PDF document from file: {filePath}, {PdfObject.GetPDFiumError()}");

        return new PdfDocument(handle);
    }

    /// <summary>
    /// Loads a PDF document from a file. The file path must be valid and the file must exist.
    /// </summary>
    /// <param name="filePath">Path to the file to load</param>
    /// <param name="document">The out variable to receive the PdfDocument object</param>
    /// <param name="password">Password to the file being loaded</param>
    /// <returns>true on success, false on failure</returns>
    public static bool TryLoadFromFile(string filePath, out PdfDocument? document, string? password = null)
    {
        document = null;

        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        var handle = PdfViewNative.FPDF_LoadDocument(filePath, password ?? "");
        
        if (handle == IntPtr.Zero)
            return false;
        
        document = new PdfDocument(handle);
        return true;
    }

    /// <summary>
    /// Loads a PDF document from a byte array. The byte array must not be null or empty.
    /// </summary>
    /// <param name="buffer">The byte array containing the PDF document</param>
    /// <param name="password">Password to the file being loaded</param>
    /// <returns>PdfDocument on success</returns>
    /// <exception cref="ArgumentException">Thrown if the byte array is null or empty</exception>
    /// <exception cref="dotPDFiumException">Thrown on PDFium library error</exception>
    public static PdfDocument LoadFromMemory(byte[] buffer, string? password = null)
    {
        if (buffer == null || buffer.Length == 0)
            throw new ArgumentException("Buffer cannot be null or empty.", nameof(buffer));

        GCHandle pinned = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        try
        {
            var handle = PdfViewNative.FPDF_LoadMemDocument64(
                pinned.AddrOfPinnedObject(),
                new UIntPtr((uint)buffer.Length), // safe cast up to 2GB
                password ?? "");

            if (handle == IntPtr.Zero)
                throw new dotPDFiumException($"Failed to load PDF document from memory: {PdfObject.GetPDFiumError()}");

            return new PdfDocument(handle);
        }
        finally
        {
            if (pinned.IsAllocated)
                pinned.Free();
        }
    }

    /// <summary>
    /// Loads a PDF document from a byte array. The byte array must not be null or empty.
    /// </summary>
    /// <param name="buffer">The byte array containing the PDF document</param>
    /// <param name="document">The out variable to receive the PdfDocument object</param>
    /// <param name="password">Password to the file being loaded</param>
    /// <returns>true on success, false on failure</returns>
    public static bool TryLoadFromMemory(byte[] buffer, out PdfDocument? document, string? password = null)
    {
        document = null;

        if (buffer == null || buffer.Length == 0)
            return false;

        GCHandle pinned = GCHandle.Alloc(buffer, GCHandleType.Pinned);

        try
        {
            var handle = PdfViewNative.FPDF_LoadMemDocument64(
                pinned.AddrOfPinnedObject(),
                new UIntPtr((uint)buffer.Length), // safe cast up to 2GB
                password ?? "");
        
            if (handle == IntPtr.Zero)
                return false;
            
            document = new PdfDocument(handle);
            return true;
        }
        finally
        {
            if (pinned.IsAllocated)
                pinned.Free();
        }
    }

    /// <summary>
    /// Creates a new PDF document. This method initializes a new PDF document and returns a new document object.
    /// </summary>
    /// <returns>A new PdfDocument</returns>
    /// <exception cref="dotPDFiumException">Throws on PDFium library error.</exception>
    public static PdfDocument CreateNew()
    {
        var handle = PdfEditNative.FPDF_CreateNewDocument();
        
        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to create new document: {PdfObject.GetPDFiumError()}");

        return new PdfDocument(handle);
    }

    /// <summary>
    /// Saves the current PDF document to the specified file path.
    /// </summary>
    /// <param name="path">The path to write the document to</param>
    /// <param name="incremental">Whether to append changes to the PDF or to re-encode the entire PDF</param>
    /// <returns>true on success, false on failure</returns>
    public bool SaveTo(string path, bool incremental = false)
    {
        uint flags = incremental ? 1u : 0u;
        using var writer = new ManagedPdfWriter(path);
        return PdfSaveNative.FPDF_SaveAsCopy(_handle, writer.NativePtr, flags);
    }

    /// <summary>
    /// Saves the current PDF document to the specified file path with a specific PDF version.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pdfVersion"></param>
    /// <returns>true on success, false on failure</returns>
    public bool SaveWithVersion(string path, PdfFileVersion fileVersion)
    {
        using var writer = new ManagedPdfWriter(path);
        return PdfSaveNative.FPDF_SaveWithVersion(_handle, writer.NativePtr, 0, (int)fileVersion);
    }


    /// <summary>
    /// Creates a new page in the document at the specified index with the specified width and height.
    /// </summary>
    /// <param name="index">index == 0 will insert a new page at the beginning; index == PageCount
    /// will append a new page to the end, and index = N inserts the page *before* existing page N.
    /// Page indexes may shift, so be cautious</param>
    /// <param name="width">The width of the page to insert in points (1/72 of an inch)</param>
    /// <param name="height">The height of the page to insert in points (1/72 of an inch)</param>
    /// <returns></returns>
    public PdfPage CreatePage(int index, float width, float height)
    {
        var pageHandle = PdfEditNative.FPDFPage_New(_handle, index, width, height);
        return new PdfPage(pageHandle, this);
    }

    /// <summary>
    /// Deletes a page from the document at the specified index. This method will shift the indexes of all subsequent pages.
    /// </summary>
    /// <param name="index"></param>
    public void DeletePage(int index)
    {
        PdfEditNative.FPDFPage_Delete(_handle, index);
    }

    /// <summary>
    /// Returns the number of pages in the document.
    /// </summary>
    public int PageCount
    {
        get
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfDocument));

            return PdfViewNative.FPDF_GetPageCount(_handle);
        }
    }

    /// <summary>
    /// Returns the number of open pages in the document.
    /// </summary>
    public int OpenPageCount
    {
        get
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfDocument));

            return _openPages.Count;
        }
    }

    /// <summary>
    /// Returns the page from the document at the specified index.
    /// </summary>
    /// <param name="pageIndex">The index of the page to load</param>
    /// <returns>PdfPage on success</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the page index specified is out of range</exception>
    /// <exception cref="dotPDFiumException">Thrown on PDFium library error</exception>
    public PdfPage LoadPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= PageCount)
            throw new ArgumentOutOfRangeException(nameof(pageIndex), "Page index is out of range.");

        var pageHandle = PdfViewNative.FPDF_LoadPage(_handle, pageIndex);
        if (pageHandle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to load page {pageIndex}: {PdfObject.GetPDFiumError()}");
    
        var page = new PdfPage(pageHandle, this);
        RegisterPage(page);
        return page;
    }

    /// <summary>
    /// Returns the page from the document at the specified index.
    /// </summary>
    /// <param name="pageIndex">The index of the page to load</param>
    /// <param name="page">The out variable to receive the specfied page</param>
    /// <returns>true on success, false on failure</returns>
    public bool TryLoadPage(int pageIndex, out PdfPage? page)
    {
        page = null;
        
        if (pageIndex < 0 || pageIndex >= PageCount)
            return false;

        var pageHandle = PdfViewNative.FPDF_LoadPage(_handle, pageIndex);
        
        if (pageHandle == IntPtr.Zero)
            return false;

        page = new PdfPage(pageHandle, this);
        RegisterPage(page);
        return true;
    }

    /// <summary>
    /// Loads a font into the document for use with text objects. This method loads 
    /// a standard font, such as "Arial" or "Times-Roman".
    /// </summary>
    /// <param name="fontName">The name of the font to load</param>
    /// <exception cref="ArgumentException">Throws if the font name is null or empty</exception>
    /// <exception cref="dotPDFiumException">Throws on PDFium library error</exception>
    /// <returns>a PdfFont object</returns>
    public PdfFont LoadStandardFont(string fontName)
    {
        if (string.IsNullOrWhiteSpace(fontName))
            throw new ArgumentException("Font name cannot be null or empty.", nameof(fontName));

        var fontHandle = PdfEditNative.FPDFText_LoadStandardFont(_handle, fontName);
        if (fontHandle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to load standard font: '{fontName}'");

        return new PdfFont(fontHandle, fontName);
    }

    /// <summary>
    /// Loads an embedded font into the document for use with text objects. This method loads a font from a byte array.
    /// </summary>
    /// <param name="fontData">The font data as a byte array</param>
    /// <param name="fontType">The type of font</param>
    /// <param name="isCid">Whether the font is a character identifier font</param>
    /// <returns>A new PdfEmbeddedFont object</returns>
    /// <exception cref="ArgumentException">Throws if font data is null or empty</exception>
    /// <exception cref="dotPDFiumException">Throws on PDFium library error</exception>
    public PdfEmbeddedFont LoadEmbeddedFont(byte[] fontData, PdfFontType fontType, bool isCid = false)
    {
        if (fontData == null || fontData.Length == 0)
            throw new ArgumentException("Font data cannot be null or empty.", nameof(fontData));

        var pinned = GCHandle.Alloc(fontData, GCHandleType.Pinned);
        try
        {
            var fontHandle = PdfEditNative.FPDFText_LoadFont(
                _handle,
                pinned.AddrOfPinnedObject(),
                (uint)fontData.Length,
                (int)fontType,
                isCid ? 1 : 0);

            if (fontHandle == IntPtr.Zero)
                throw new dotPDFiumException($"Failed to load embedded font: {PdfObject.GetPDFiumError()}");

            return new PdfEmbeddedFont(fontHandle, "<embedded>");
        }
        finally
        {
            if (pinned.IsAllocated)
                pinned.Free();
        }
    }

    /// <summary>
    /// Creates a new text object in the document. This method creates a new text object with the specified font and font size.
    /// The text object can then be added to a page or manipulated as needed.
    /// </summary>
    /// <param name="font">The PdfFont to apply to the text object</param>
    /// <param name="fontSize">The font size</param>
    /// <returns>a new PdfTextObject</returns>
    /// <exception cref="ArgumentNullException">Throws if the specified font is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Throws if the font size is <= 0</exception>
    /// <exception cref="dotPDFiumException">Throws on PDFium library error</exception>
    public PdfTextObject CreateTextObject(PdfFont font, float fontSize)
    {
        if (font == null)
            throw new ArgumentNullException(nameof(font));
        if (fontSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(fontSize), "Font size must be positive.");

        var handle = PdfEditNative.FPDFPageObj_CreateTextObj(_handle, font.Handle, fontSize);

        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to create text object: {PdfObject.GetPDFiumError()}");

        return new PdfTextObject(handle);
    }

    /// <summary>
    /// Creates a new text objecct in the document. This method creates a new text object with the 
    /// specified font and font size. The text object can then be added to a page or manipulated as needed.
    /// </summary>
    /// <param name="fontName">Name of the font to use</param>
    /// <param name="fontSize">Size of the font to use</param>
    /// <returns>a new PdfTextObject</returns>
    /// <exception cref="dotPDFiumException">Thrown on PDFium library error</exception>
    public PdfTextObject CreateStandardTextObject(string fontName, float fontSize)
    {
        var handle = PdfEditNative.FPDFPageObj_NewTextObj(_handle, fontName, fontSize);

        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to create standard font text object: {PdfObject.GetPDFiumError()}");

        return new PdfTextObject(handle);
    }

    /// <summary>
    /// Closes the current PdfDocument instance and releases the resources associated with it.
    /// </summary>
    public void Close() => Dispose();

    /// <summary>
    /// Called when the PdfDocument is disposed. This method is responsible for releasing the resources used by the
    /// PdfDocument instance, including closing any open pages and freeing the native resources associated with the document.
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
#if DEBUG
            if (_openPages.Count > 0)
                Debug.WriteLine($"Warning: PdfDocument disposed while {_openPages.Count} page(s) still open.");
#endif

            foreach (var page in _openPages.ToList())
                page.Dispose();

            _openPages.Clear();
        }

        base.Dispose(disposing);
    }
}
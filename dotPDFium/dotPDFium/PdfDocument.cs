using nebulae.dotPDFium.Native;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium;

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
    /// Loads a PDF document using a custom file access descriptor.
    /// </summary>
    /// <remarks>This method uses the <see cref="FpdfFileAccess"/> structure to provide custom file access for
    /// loading the PDF document. Ensure that the <paramref name="fileAccess"/> descriptor is properly initialized
    /// before calling this method.</remarks>
    /// <param name="fileAccess">The <see cref="FpdfFileAccess"/> descriptor that provides custom file access functionality. The descriptor must
    /// have a valid <c>m_GetBlock</c> delegate and a non-zero <c>m_FileLen</c>.</param>
    /// <param name="password">An optional password to decrypt the PDF document, if it is password-protected. Pass <see langword="null"/> or an
    /// empty string if no password is required.</param>
    /// <returns>A <see cref="PdfDocument"/> instance representing the loaded PDF document.</returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="fileAccess"/> descriptor is invalid, such as when <c>m_GetBlock</c> is <see
    /// langword="null"/> or <c>m_FileLen</c> is zero.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the PDF document cannot be loaded, typically due to an error in the underlying PDF library or an
    /// invalid file format.</exception>
    public static PdfDocument LoadFromCustomAccess(FpdfFileAccess fileAccess, string? password = null)
    {
        if (fileAccess.m_GetBlock == null || fileAccess.m_FileLen == 0)
            throw new ArgumentException("Invalid FpdfFileAccess descriptor.", nameof(fileAccess));

        var handle = PdfViewNative.FPDF_LoadCustomDocument(ref fileAccess, password ?? string.Empty);

        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to load PDF document from custom file access: {PdfObject.GetPDFiumError()}");

        return new PdfDocument(handle);
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
    /// Loads a CIDType2 font into the PDF document.
    /// </summary>
    /// <remarks>CIDType2 fonts are commonly used for embedding TrueType fonts in PDF documents.  Ensure that
    /// the provided font data is valid and compatible with the PDF library.</remarks>
    /// <param name="fontData">The font data as a byte array. This cannot be null or empty.</param>
    /// <param name="name">The name of the font to be used in the PDF document.</param>
    /// <param name="toUnicodeCMap">An optional string representing the ToUnicode CMap, which maps character codes to Unicode values. If not
    /// provided, an empty string is used.</param>
    /// <param name="cidToGidMap">An optional byte array representing the CID-to-GID map, which maps character identifiers (CIDs) to glyph
    /// identifiers (GIDs). If not provided, an empty array is used.</param>
    /// <returns>A <see cref="PdfFont"/> object representing the loaded CIDType2 font.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="fontData"/> is null or empty.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the font fails to load due to an error in the underlying PDF library.</exception>
    public PdfFont LoadCidType2Font(
    byte[] fontData,
    string name,
    string toUnicodeCMap = "",
    byte[]? cidToGidMap = null)
    {
        if (fontData == null || fontData.Length == 0)
            throw new ArgumentException("Font data cannot be null or empty.", nameof(fontData));

        var handle = PdfEditNative.FPDFText_LoadCidType2Font(
            _handle,
            fontData,
            (uint)fontData.Length,
            toUnicodeCMap ?? string.Empty,
            cidToGidMap ?? Array.Empty<byte>(),
            cidToGidMap != null ? (uint)cidToGidMap.Length : 0);

        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to load CIDType2 font: {PdfObject.GetPDFiumError()}");

        return new PdfFont(handle, name);
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
    /// Creates a new path object for use in a PDF document.
    /// </summary>
    /// <returns>A <see cref="PdfPathObject"/> representing the newly created path object.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the path object could not be created due to an error in the underlying PDF library.</exception>
    public PdfPathObject CreatePathObject()
    {
        var handle = PdfEditNative.FPDFPageObj_CreateNewPath(0, 0);

        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to create path object: {PdfObject.GetPDFiumError()}");

        return new PdfPathObject(handle);
    }

    /// <summary>
    /// Creates a new image object for use in a PDF document.
    /// </summary>
    /// <returns>A <see cref="PdfImageObject"/> representing the newly created image object.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the image object could not be created due to an error in the underlying PDF library.</exception>
    public PdfImageObject CreateImageObject()
    {
        var handle = PdfEditNative.FPDFPageObj_NewImageObj(_handle);

        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to create image object: {PdfObject.GetPDFiumError()}");

        return new PdfImageObject(handle);
    }

    /// <summary>
    /// Creates a new rectangular path object with the specified dimensions.
    /// </summary>
    /// <remarks>The rectangle is defined in the coordinate space of the PDF page. Ensure that the provided
    /// dimensions are valid and fit within the desired page area.</remarks>
    /// <param name="x">The x-coordinate of the lower-left corner of the rectangle.</param>
    /// <param name="y">The y-coordinate of the lower-left corner of the rectangle.</param>
    /// <param name="width">The width of the rectangle. Must be greater than 0.</param>
    /// <param name="height">The height of the rectangle. Must be greater than 0.</param>
    /// <returns>A <see cref="PdfPathObject"/> representing the created rectangle.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the rectangle object could not be created due to an internal error.</exception>
    public PdfPathObject CreateRectObject(float x, float y, float width, float height)
    {
        var handle = PdfEditNative.FPDFPageObj_CreateNewRect(x, y, width, height);

        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to create rect object: {PdfObject.GetPDFiumError()}");

        return new PdfPathObject(handle);
    }

    /// <summary>
    /// Retrieves the size of the page at the specified index in floating-point units.
    /// </summary>
    /// <param name="pageIndex">The zero-based index of the page whose size is to be retrieved. Must be within the range of available pages.</param>
    /// <returns>An <see cref="FsSizeF"/> structure representing the width and height of the specified page in floating-point
    /// units.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the <see cref="PdfDocument"/> has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="pageIndex"/> is less than 0 or greater than or equal to the total number of pages in
    /// the document.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the page size could not be retrieved due to an internal error.</exception>
    public FsSizeF GetPageSizeByIndexF(int pageIndex)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfDocument));
        if (pageIndex < 0 || pageIndex >= PageCount)
            throw new ArgumentOutOfRangeException(nameof(pageIndex));

        if (!PdfViewNative.FPDF_GetPageSizeByIndexF(_handle, pageIndex, out FsSizeF size))
            throw new dotPDFiumException("Failed to retrieve page size using GetPageSizeByIndexF.");

        return size;
    }

    /// <summary>
    /// Retrieves the size of a page in the document specified by its index as double-precision floating-point units.
    /// </summary>
    /// <param name="pageIndex">The zero-based index of the page whose size is to be retrieved. Must be within the range of available pages.</param>
    /// <returns>An <see cref="FsSize"/> object representing the width and height of the specified page, in points.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the document has been disposed and is no longer accessible.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="pageIndex"/> is less than 0 or greater than or equal to the total number of pages in
    /// the document.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the page size could not be retrieved due to an error in the underlying PDF library.</exception>
    public FsSize GetPageSizeByIndex(int pageIndex)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfDocument));
        if (pageIndex < 0 || pageIndex >= PageCount)
            throw new ArgumentOutOfRangeException(nameof(pageIndex));

        if (PdfViewNative.FPDF_GetPageSizeByIndex(_handle, pageIndex, out double width, out double height) == 0)
            throw new dotPDFiumException("Failed to retrieve page size using GetPageSizeByIndex.");

        return new FsSize(width, height);
    }

    /// <summary>
    /// Retrieves a rendered bitmap of the specified text object on a PDF page at the given scale.
    /// </summary>
    /// <remarks>This method uses the underlying PDF library to render the specified text object into a
    /// bitmap.  Ensure that the provided <paramref name="page"/> and <paramref name="text"/> objects are valid and
    /// associated with the same PDF document.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> containing the text object to render. Must not be <c>null</c>.</param>
    /// <param name="text">The <see cref="PdfTextObject"/> to render as a bitmap. Must not be <c>null</c>.</param>
    /// <param name="scale">The scale factor to apply when rendering the bitmap. Must be greater than 0.</param>
    /// <returns>A pointer to the rendered bitmap as an <see cref="IntPtr"/>. The caller is responsible for managing the memory
    /// of the returned bitmap.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the bitmap could not be rendered due to an error in the underlying PDF library.</exception>
    public IntPtr GetRenderedBitmap(PdfPage page, PdfTextObject text, float scale)
    {
        var bmp = PdfEditNative.FPDFTextObj_GetRenderedBitmap(_handle, page.Handle, text.Handle, scale);

        if (bmp == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to get rendered bitmap from text object: {PdfObject.GetPDFiumError()}");

        return bmp;
    }

    /// <summary>
    /// Gets the total number of attachments in the PDF document.
    /// </summary>
    /// <returns>The number of attachments in the PDF document. Returns 0 if the document contains no attachments.</returns>
    public int GetAttachmentCount()
    {
        return PdfAttachmentNative.FPDFDoc_GetAttachmentCount(_handle);
    }

    /// <summary>
    /// Retrieves the attachment at the specified index from the PDF document.
    /// </summary>
    /// <remarks>Use this method to access attachments embedded in the PDF document. Ensure that the <paramref
    /// name="index"/> is within the range of available attachments to avoid exceptions.</remarks>
    /// <param name="index">The zero-based index of the attachment to retrieve. Must be within the valid range of available attachments.</param>
    /// <returns>A <see cref="PdfAttachment"/> object representing the attachment at the specified index.</returns>
    /// <exception cref="dotPDFiumException">Thrown if no attachment exists at the specified <paramref name="index"/>.</exception>
    public PdfAttachment GetAttachment(int index)
    {
        var handle = PdfAttachmentNative.FPDFDoc_GetAttachment(_handle, index);
        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"No attachment at index {index}");

        return new PdfAttachment(handle);
    }

    /// <summary>
    /// Adds a new attachment to the PDF document.
    /// </summary>
    /// <remarks>The <paramref name="name"/> parameter must be unique within the document. If an attachment
    /// with the same name already exists, the behavior is undefined. Ensure that the provided name is valid and does
    /// not conflict with existing attachments.</remarks>
    /// <param name="name">The name of the attachment to be added. This must be a non-empty string.</param>
    /// <returns>A <see cref="PdfAttachment"/> object representing the newly added attachment.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the attachment could not be added to the document.</exception>
    public PdfAttachment AddAttachment(string name)
    {
        var handle = PdfAttachmentNative.FPDFDoc_AddAttachment(_handle, name);
        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to add attachment '{name}'");

        return new PdfAttachment(handle);
    }

    /// <summary>
    /// Deletes the attachment at the specified index from the PDF document.
    /// </summary>
    /// <param name="index">The zero-based index of the attachment to delete.</param>
    /// <exception cref="dotPDFiumException">Thrown if the attachment cannot be deleted. The exception message will include the specific error details.</exception>
    public void DeleteAttachment(int index)
    {
        if (!PdfAttachmentNative.FPDFDoc_DeleteAttachment(_handle, index))
            throw new dotPDFiumException($"Failed to delete attachment '{index}': {PdfObject.GetPDFiumError()}");
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
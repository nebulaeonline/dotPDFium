using nebulae.dotPDFium.Forms;
using nebulae.dotPDFium.Native;
using nebulae.dotPDFium.Security;
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

    public PdfFormContext? Forms { get; private set; }

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
    /// Copies the viewer preferences (e.g., page layout, UI hints) from another PDF document.
    /// </summary>
    /// <param name="source">The document from which to copy viewer preferences.</param>
    /// <exception cref="ArgumentNullException">Thrown if source is null.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the copy operation fails.</exception>
    public void CopyViewerPreferencesFrom(PdfDocument source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (!PdfPpoNative.FPDF_CopyViewerPreferences(_handle, source._handle))
            throw new dotPDFiumException("Failed to copy viewer preferences from source document.");
    }

    /// <summary>
    /// Creates a new PDF document by importing pages from an existing document arranged in an N-up layout.
    /// </summary>
    /// <param name="source">The source document to import pages from.</param>
    /// <param name="width">The width of the output composite page in points.</param>
    /// <param name="height">The height of the output composite page in points.</param>
    /// <param name="columns">Number of pages horizontally.</param>
    /// <param name="rows">Number of pages vertically.</param>
    /// <returns>A new <see cref="PdfDocument"/> containing a single composite page.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the source document is null.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails.</exception>
    public static PdfDocument ImportNPagesToOne(PdfDocument source, float width, float height, int columns, int rows)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var handle = PdfPpoNative.FPDF_ImportNPagesToOne(
            source._handle,
            width,
            height,
            (UIntPtr)columns,
            (UIntPtr)rows);

        if (handle == IntPtr.Zero)
            throw new dotPDFiumException("Failed to import N pages to one.");

        return new PdfDocument(handle);
    }

    /// <summary>
    /// Imports pages from another document into this one, starting at the given index.
    /// </summary>
    /// <param name="source">The source document to import pages from.</param>
    /// <param name="pageRange">Page range string (e.g., "1-3,5"). Must use 1-based indexing.</param>
    /// <param name="insertAtIndex">Zero-based index to insert pages into this document.</param>
    /// <exception cref="ArgumentNullException">Thrown if source or pageRange is null.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails.</exception>
    public void ImportPagesFrom(PdfDocument source, string pageRange, int insertAtIndex)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (string.IsNullOrWhiteSpace(pageRange))
            throw new ArgumentNullException(nameof(pageRange));

        bool ok = PdfPpoNative.FPDF_ImportPages(_handle, source._handle, pageRange, insertAtIndex);
        if (!ok)
            throw new dotPDFiumException("Failed to import pages from source document.");
    }

    /// <summary>
    /// Imports specific pages (by zero-based index) from another document into this one.
    /// </summary>
    /// <param name="source">The source PDF document.</param>
    /// <param name="pageIndices">Array of zero-based page indices to import.</param>
    /// <param name="insertAtIndex">Zero-based insertion point in the destination document.</param>
    /// <exception cref="ArgumentNullException">Thrown if source or pageIndices is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the pageIndices array is empty.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the import fails.</exception>
    public void ImportPagesFromByIndex(PdfDocument source, int[] pageIndices, int insertAtIndex)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (pageIndices == null || pageIndices.Length == 0)
            throw new ArgumentException("Page index array must not be null or empty.", nameof(pageIndices));

        bool ok = PdfPpoNative.FPDF_ImportPagesByIndex(
            _handle,
            source._handle,
            pageIndices,
            (uint)pageIndices.Length,
            insertAtIndex);

        if (!ok)
            throw new dotPDFiumException("Failed to import pages by index.");
    }

    /// <summary>
    /// Gets the security handler revision used for encryption, or -1 if the document is not encrypted.
    /// </summary>
    /// <returns>
    /// The revision number (e.g., 2 for RC4-40, 4 for AES-128, 6 for AES-256), or -1 if not encrypted.
    /// </returns>
    public int GetSecurityHandlerRevision()
    {
        return PdfViewNative.FPDF_GetSecurityHandlerRevision(_handle);
    }

    /// <summary>
    /// Gets the duplex printing preference defined in the document's viewer preferences.
    /// </summary>
    /// <returns>The <see cref="PdfDuplexType"/> specified by the document.</returns>
    public PdfDuplexType GetViewerDuplexPreference()
    {
        return PdfViewNative.FPDF_VIEWERREF_GetDuplex(_handle);
    }

    /// <summary>
    /// Retrieves a raw name value from the document's /ViewerPreferences dictionary.
    /// </summary>
    /// <param name="key">The name key to query (e.g., "Direction", "PrintScaling").</param>
    /// <returns>The associated name string, or an empty string if not present or invalid.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the key is null or empty.</exception>
    /// <exception cref="dotPDFiumException">Thrown if decoding fails despite valid key.</exception>
    public string GetViewerPreferenceName(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        uint size = PdfViewNative.FPDF_VIEWERREF_GetName(_handle, key, null!, 0);
        if (size == 0)
            return string.Empty;

        var buffer = new byte[size];
        uint actual = PdfViewNative.FPDF_VIEWERREF_GetName(_handle, key, buffer, size);

        if (actual == 0 || actual > size)
            throw new dotPDFiumException($"Failed to read ViewerPreferences name entry for key '{key}'.");

        return System.Text.Encoding.UTF8.GetString(buffer, 0, (int)(actual - 1)); // strip null terminator
    }

    /// <summary>
    /// Gets the suggested number of copies to print as defined in the document's viewer preferences.
    /// </summary>
    /// <returns>The suggested number of copies, or 0 if unspecified.</returns>
    public int GetSuggestedPrintCopies()
    {
        return PdfViewNative.FPDF_VIEWERREF_GetNumCopies(_handle);
    }

    /// <summary>
    /// Gets the suggested print page range defined in the viewer preferences (e.g., "1-3,5").
    /// </summary>
    /// <returns>
    /// A UTF-8 encoded string with the suggested print range, or an empty string if none is defined.
    /// </returns>
    public string GetSuggestedPrintRange()
    {
        IntPtr ptr = PdfViewNative.FPDF_VIEWERREF_GetPrintPageRange(_handle);
        if (ptr == IntPtr.Zero)
            return string.Empty;

        return Marshal.PtrToStringUTF8(ptr) ?? string.Empty;
    }

    /// <summary>
    /// Retrieves the print page range specified in the PDF viewer preferences.
    /// </summary>
    /// <remarks>This method checks the PDF viewer preferences for a defined print page range and returns it
    /// as a <see cref="PdfPrintPageRange"/> object. If no range is specified, the method returns <see
    /// langword="null"/>.</remarks>
    /// <returns>A <see cref="PdfPrintPageRange"/> object representing the print page range if specified; otherwise, <see
    /// langword="null"/> if no print page range is defined.</returns>
    public PdfPrintPageRange? GetParsedPrintPageRange()
    {
        var handle = PdfViewNative.FPDF_VIEWERREF_GetPrintPageRange(_handle);
        return handle == IntPtr.Zero ? null : new PdfPrintPageRange(handle);
    }

    /// <summary>
    /// Gets the print scaling flag defined in the document's viewer preferences.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the document allows automatic scaling when printed;
    /// <see langword="false"/> if it requests no scaling (actual size).
    /// </returns>
    public bool GetPrintScalingAllowed()
    {
        return PdfViewNative.FPDF_VIEWERREF_GetPrintScaling(_handle);
    }

    /// <summary>
    /// Determines whether the document is tagged (i.e., contains a structure tree).
    /// </summary>
    /// <returns><see langword="true"/> if the document is tagged; otherwise <see langword="false"/>.</returns>
    public bool IsTagged()
    {
        return PdfCatalogNative.FPDFCatalog_IsTagged(_handle);
    }

    /// <summary>
    /// Sets the document-level default language in the PDF catalog (/Lang).
    /// </summary>
    /// <param name="language">The BCP 47 language tag (e.g., "en-US", "fr-FR").</param>
    /// <returns><see langword="true"/> if the language was successfully set; otherwise <see langword="false"/>.</returns>
    public bool SetDefaultLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("Language cannot be null or empty.", nameof(language));

        return PdfCatalogNative.FPDFCatalog_SetLanguage(_handle, language);
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
    /// Moves the specified pages to a new position within the document.
    /// </summary>
    /// <remarks>The order of the pages in <paramref name="pageIndices"/> is preserved during the move
    /// operation.  Ensure that the indices are valid and within the bounds of the document's page count.</remarks>
    /// <param name="pageIndices">An array of zero-based indices representing the pages to move. Must contain at least one index.</param>
    /// <param name="destinationIndex">The zero-based index where the pages will be moved. The pages will be inserted before this index.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="pageIndices"/> is null or empty.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails to move the pages.</exception>
    public void MovePages(int[] pageIndices, int destinationIndex)
    {
        if (pageIndices == null || pageIndices.Length == 0)
            throw new ArgumentException("At least one page index must be specified.", nameof(pageIndices));

        if (!PdfEditNative.FPDF_MovePages(_handle, pageIndices, (ulong)pageIndices.Length, destinationIndex))
            throw new dotPDFiumException($"Failed to move pages: {PdfObject.GetPDFiumError()}");
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
    /// Retrieves the page mode of the PDF document.
    /// </summary>
    /// <remarks>The page mode specifies how the document should be displayed when opened, such as whether 
    /// bookmarks, thumbnails, or other navigation panels are shown.</remarks>
    /// <returns>A <see cref="PdfPageMode"/> value representing the page mode of the document.  Returns <see
    /// cref="PdfPageMode.Unknown"/> if the page mode is not defined or cannot be determined.</returns>
    public PdfPageMode GetPageMode()
    {
        int mode = PdfExtNative.FPDFDoc_GetPageMode(_handle);
        return Enum.IsDefined(typeof(PdfPageMode), mode)
            ? (PdfPageMode)mode
            : PdfPageMode.Unknown;
    }

    /// <summary>
    /// Checks whether the PDF document has a valid cross-reference table or stream.
    /// </summary>
    /// <returns><see langword="true"/> if the cross-reference structure is valid; otherwise, <see langword="false"/>.</returns>
    public bool HasValidCrossReferenceTable()
    {
        return PdfViewNative.FPDF_DocumentHasValidCrossReferenceTable(_handle);
    }

    /// <summary>
    /// Retrieves the byte offsets of all trailer dictionaries in the PDF file.
    /// </summary>
    /// <returns>
    /// A list of unsigned 32-bit integers representing the end offsets of each trailer section.
    /// </returns>
    /// <exception cref="dotPDFiumException">Thrown if the call fails unexpectedly.</exception>
    public IReadOnlyList<uint> GetTrailerEndOffsets()
    {
        // Step 1: Get the count
        uint count = PdfViewNative.FPDF_GetTrailerEnds(_handle, null!, 0);
        if (count == 0)
            return Array.Empty<uint>();

        // Step 2: Fetch the actual trailer offsets
        var buffer = new uint[count];
        uint actual = PdfViewNative.FPDF_GetTrailerEnds(_handle, buffer, count);

        if (actual != count)
            throw new dotPDFiumException("Mismatch in trailer end retrieval.");

        return buffer;
    }

    /// <summary>
    /// Gets all digital signatures defined in this document.
    /// </summary>
    /// <returns>A list of <see cref="PdfSignature"/> objects.</returns>
    public IReadOnlyList<PdfSignature> GetSignatures()
    {
        int count = PdfSignatureNative.FPDF_GetSignatureCount(_handle);
        if (count <= 0)
            return Array.Empty<PdfSignature>();

        var result = new List<PdfSignature>(count);
        for (int i = 0; i < count; i++)
        {
            var sigHandle = PdfSignatureNative.FPDF_GetSignatureObject(_handle, i);
            if (sigHandle != IntPtr.Zero)
                result.Add(new PdfSignature(sigHandle));
        }

        return result;
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

    public PdfBookmark? FindBookmark(string title)
    {
        var handle = PdfDocNative.FPDFBookmark_Find(_handle, title);
        return handle == IntPtr.Zero ? null : new PdfBookmark(handle, this);
    }

    /// <summary>
    /// Returns the number of named destinations defined in this document.
    /// </summary>
    public int GetNamedDestinationCount()
    {
        return (int)PdfViewNative.FPDF_CountNamedDests(_handle);
    }

    /// <summary>
    /// Retrieves the permissions associated with the current PDF document.
    /// </summary>
    /// <remarks>The permissions are determined by the document's security settings.  Refer to the PDF
    /// specification for details on the meaning of each bit in the returned bitmask.</remarks>
    /// <returns>A 32-bit unsigned integer representing the permissions of the PDF document.  The value is a bitmask where each
    /// bit indicates a specific permission, such as printing, copying, or modifying the document.</returns>
    public uint GetPermissions()
    {
        return PdfViewNative.FPDF_GetDocPermissions(_handle);
    }

    /// <summary>
    /// Retrieves the user permissions for the current PDF document.
    /// </summary>
    /// <remarks>The permissions are determined by the document's security settings.  Refer to the PDF
    /// specification for details on the meaning of each bit in the bitmask.</remarks>
    /// <returns>A 32-bit unsigned integer representing the user permissions for the document.  The value is a bitmask where each
    /// bit corresponds to a specific permission,  such as printing, copying, or modifying the document.</returns>
    public uint GetUserPermissions()
    {
        return PdfViewNative.FPDF_GetDocUserPermissions(_handle);
    }

    /// <summary>
    /// Retrieves the version number of the PDF file associated with the current instance.
    /// </summary>
    /// <remarks>The version number corresponds to the PDF specification version of the file.  For example, a
    /// version number of 1 indicates PDF 1.x.</remarks>
    /// <returns>The version number of the PDF file if it is successfully retrieved; otherwise, <see langword="null"/>.</returns>
    public int? GetFileVersion()
    {
        return PdfViewNative.FPDF_GetFileVersion(_handle, out int version)
            ? version
            : null;
    }

    /// <summary>
    /// Retrieves the named destination at the specified index within the PDF document.
    /// </summary>
    /// <remarks>Named destinations are predefined locations within a PDF document that can be used for
    /// navigation.  The method returns <see langword="null"/> if the specified index does not correspond to a valid
    /// named destination.</remarks>
    /// <param name="index">The zero-based index of the named destination to retrieve.</param>
    /// <returns>A tuple containing the name of the destination and a <see cref="PdfDestination"/> object representing the
    /// destination,  or <see langword="null"/> if the index is out of range or the destination cannot be retrieved.</returns>
    public (string Name, PdfDestination? Destination)? GetNamedDestination(int index)
    {
        int charLen = 0;
        var handle = PdfViewNative.FPDF_GetNamedDest(_handle, index, IntPtr.Zero, ref charLen);
        if (charLen == 0)
            return null;

        IntPtr buffer = Marshal.AllocHGlobal(charLen);
        try
        {
            handle = PdfViewNative.FPDF_GetNamedDest(_handle, index, buffer, ref charLen);
            if (handle == IntPtr.Zero)
                return null;

            string name = Marshal.PtrToStringUni(buffer, charLen / 2) ?? string.Empty;
            return (name, new PdfDestination(handle, this));
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// Retrieves a named destination from the PDF document by its name.
    /// </summary>
    /// <remarks>Named destinations are predefined locations within the PDF document that can be used for
    /// navigation. Use this method to retrieve a destination by its name for further processing or
    /// navigation.</remarks>
    /// <param name="name">The name of the destination to retrieve. This value cannot be <see langword="null"/> or empty.</param>
    /// <returns>A <see cref="PdfDestination"/> object representing the named destination if found; otherwise, <see
    /// langword="null"/>.</returns>
    public PdfDestination? GetNamedDestinationByName(string name)
    {
        var handle = PdfViewNative.FPDF_GetNamedDestByName(_handle, name);
        return handle == IntPtr.Zero ? null : new PdfDestination(handle, this);
    }

    /// <summary>
    /// Retrieves the metadata value associated with the specified tag from the PDF document.
    /// </summary>
    /// <remarks>The returned string excludes any null terminator that may be present in the underlying
    /// data.</remarks>
    /// <param name="tag">The metadata tag to retrieve. Common tags include "Title", "Author", and "Subject".</param>
    /// <returns>The metadata value as a string, or <see langword="null"/> if the specified tag does not exist or has no value.</returns>
    public string? GetMetadata(string tag)
    {
        uint len = PdfDocNative.FPDF_GetMetaText(_handle, tag, Array.Empty<char>(), 0);
        if (len == 0) return null;

        var buffer = new char[len];
        PdfDocNative.FPDF_GetMetaText(_handle, tag, buffer, len);

        return new string(buffer, 0, (int)len - 1); // Remove null terminator
    }

    /// <summary>
    /// Retrieves the label associated with the specified page index in the document.
    /// </summary>
    /// <remarks>Page labels are often used to display custom page numbering or names in a document. If no
    /// label is defined for the specified page, the method returns <see langword="null"/>.</remarks>
    /// <param name="pageIndex">The zero-based index of the page for which to retrieve the label.</param>
    /// <returns>The label of the specified page as a string, or <see langword="null"/> if the page does not have a label.</returns>
    public string? GetPageLabel(int pageIndex)
    {
        uint len = PdfDocNative.FPDF_GetPageLabel(_handle, pageIndex, Array.Empty<char>(), 0);
        if (len == 0) return null;

        var buffer = new char[len];
        PdfDocNative.FPDF_GetPageLabel(_handle, pageIndex, buffer, len);

        return new string(buffer, 0, (int)len - 1); // remove null terminator
    }

    /// <summary>
    /// Retrieves the file identifier associated with the document.
    /// </summary>
    /// <remarks>File identifiers are typically used to uniquely identify a document. The returned identifier
    /// may vary depending on the <paramref name="idType"/> specified.</remarks>
    /// <param name="idType">The type of file identifier to retrieve. Use <c>0</c> to retrieve the primary identifier, or other values as
    /// defined by the document specification.</param>
    /// <returns>A byte array containing the file identifier, or <see langword="null"/> if no identifier is available.</returns>
    public byte[]? GetFileIdentifier(uint idType = 0)
    {
        uint len = PdfDocNative.FPDF_GetFileIdentifier(_handle, idType, Array.Empty<byte>(), 0);
        if (len == 0) return null;

        var buffer = new byte[len];
        PdfDocNative.FPDF_GetFileIdentifier(_handle, idType, buffer, len);
        return buffer;
    }

    /// <summary>
    /// Retrieves the additional action associated with the specified page action type.
    /// </summary>
    /// <param name="type">The type of page action to retrieve. This specifies the event for which the additional action is defined.</param>
    /// <returns>A <see cref="PdfAction"/> representing the additional action for the specified page action type,  or <see
    /// langword="null"/> if no additional action is defined for the specified type.</returns>
    public PdfAction? GetAdditionalAction(PdfPageAActionType type)
    {
        var handle = PdfDocNative.FPDF_GetPageAAction(_handle, (int)type);
        return handle == IntPtr.Zero ? null : new PdfAction(handle);
    }

    /// <summary>
    /// Enables form filling functionality for the PDF document.
    /// </summary>
    /// <param name="info">The configuration information required to initialize form filling.</param>
    /// <exception cref="InvalidOperationException">Thrown if forms have already been initialized for the document.</exception>
    public void EnableForms(PdfFormFillInfo info)
    {
        if (Forms is null)
            throw new InvalidOperationException("Forms already initialized.");

        Forms = new PdfFormContext(this, info);
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
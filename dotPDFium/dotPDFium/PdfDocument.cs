using nebulae.dotPDFium.Native;
using System;
using System.Diagnostics;
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
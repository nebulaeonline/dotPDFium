using nebulae.dotPDFium;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

/// <summary>
/// The PdfTextSearch class provides functionality for handling search results within a PdfText object.
/// </summary>
public class PdfTextSearch : IDisposable
{
    private IntPtr _searchHandle;
    private readonly PdfText _parentText;

    /// <summary>
    /// Constructor for PdfTextSearch. This constructor is internal and should not be used directly.
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="parent"></param>
    internal PdfTextSearch(IntPtr handle, PdfText parent)
    {
        _searchHandle = handle;
        _parentText = parent;
    }

    /// <summary>
    /// Find next search result. Moves the search cursor to thext match of the search string.
    /// </summary>
    /// <returns></returns>
    public bool FindNext() => PdfTextNative.FPDFText_FindNext(_searchHandle);

    /// <summary>
    /// Find previous search result. Moves the search cursor to the previous match of the search string.
    /// </summary>
    /// <returns></returns>
    public bool FindPrev() => PdfTextNative.FPDFText_FindPrev(_searchHandle);

    /// <summary>
    /// Gets the current index of the search cursor.
    /// </summary>
    public int CurrentIndex => PdfTextNative.FPDFText_GetSchResultIndex(_searchHandle);

    /// <summary>
    /// Returns the number of search results for the current search string.
    /// </summary>
    public int MatchCount => PdfTextNative.FPDFText_GetSchCount(_searchHandle);

    /// <summary>
    /// Releases the resources used by the current instance of the class.
    /// </summary>
    /// <remarks>This method should be called to release unmanaged resources when the instance is no longer
    /// needed. Failure to call this method may result in resource leaks.</remarks>
    public void Dispose()
    {
        if (_searchHandle != IntPtr.Zero)
        {
            PdfTextNative.FPDFText_FindClose(_searchHandle);
            _searchHandle = IntPtr.Zero;
        }
    }
}

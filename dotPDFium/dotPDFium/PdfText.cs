using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

public class PdfText : PdfObject
{
    private readonly PdfPage _parentPage;
    private int _charCount = 0;

    /// <summary>
    /// PdfText constructor. This constructor is internal and should not be used directly.
    /// </summary>
    /// <param name="textHandle">The PDFium text pointer</param>
    /// <param name="parentPage">The parent PdfPage</param>
    /// <exception cref="ArgumentException">Throws if the textHandle is null or if the parentPage is null</exception>
    internal PdfText(IntPtr textHandle, PdfPage parentPage) : base(textHandle, PdfObjectType.TextPage)
    {
        if (textHandle == IntPtr.Zero || parentPage == null)
            throw new ArgumentException("Invalid text handle or parent page:", nameof(textHandle));
        
        _parentPage = parentPage;
        _charCount = PdfTextNative.FPDFText_CountChars(_handle);
    }

    /// <summary>
    /// Gets the total number of characters in the associated PDF text.
    /// </summary>
    public int CountChars => _handle != IntPtr.Zero
        ? _charCount
        : throw new ObjectDisposedException(nameof(PdfText));

    /// <summary>
    /// Gets the character at the specified index in the associated PDF text.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public uint GetChar(int index)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));
        
        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        return PdfTextNative.FPDFText_GetUnicode(_handle, index);
    }

    /// <summary>
    /// Gets the bounding box of the character at the specified index in the associated PDF text.
    /// </summary>
    /// <param name="index">The index of the character</param>
    /// <param name="left">The out paramater for the left dimension</param>
    /// <param name="right">The out parameter for the right dimension</param>
    /// <param name="bottom">The out parameter for the bottom dimension</param>
    /// <param name="top">The out parameter for the top dimension</param>
    /// <returns>The bounding box of the chacter in the out parameters</returns>
    /// <exception cref="ObjectDisposedException">Throws if the PdfText object has been disposed</exception>
    /// <exception cref="ArgumentOutOfRangeException">Throws if the index is out of range</exception>
    public bool GetCharBox(int index, out double left, out double right, out double bottom, out double top)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));
        
        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        return PdfTextNative.FPDFText_GetCharBox(_handle, index, out left, out right, out bottom, out top);
    }

    /// <summary>
    /// Gets the bounding box of the character at the specified index in the associated PDF text.
    /// </summary>
    /// <param name="index">The index of the character</param>
    /// <param name="left">The out paramater for the left dimension</param>
    /// <param name="right">The out parameter for the right dimension</param>
    /// <param name="bottom">The out parameter for the bottom dimension</param>
    /// <param name="top">The out parameter for the top dimension</param>
    /// <returns>The bounding box of the chacter in the out parameters & true on success and false on failure</returns>
    public bool TryGetCharBox(int index, out double left, out double right, out double bottom, out double top)
    {
        if (_handle == IntPtr.Zero || index < 0 || index >= _charCount)
        {
            left = right = bottom = top = 0.0;
            return false;
        }

        PdfTextNative.FPDFText_GetCharBox(_handle, index, out left, out right, out bottom, out top);

        return true;
    }

    /// <summary>
    /// Returns the character index at the specified x, y position in the associated PDF document.
    /// </summary>
    /// <param name="x">The x paramater of the location on the document</param>
    /// <param name="y">The y parameter of the location on the document</param>
    /// <param name="xTolerance">The allowed tolerance from the x parameter to look</param>
    /// <param name="yTolerance">The allowed tolerance from the y parameter to look</param>
    /// <returns>The index of the character located at the specified x, y position</returns>
    /// <exception cref="ObjectDisposedException">Throws if the PdfText object has already been disposed</exception>
    public int GetCharIndexAtPos(double x, double y, double xTolerance = 2.0, double yTolerance = 2.0)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        return PdfTextNative.FPDFText_GetCharIndexAtPos(_handle, x, y, xTolerance, yTolerance);
    }

    /// <summary>
    /// Returns the chacters from the specified index and count from the associated PDF text.
    /// </summary>
    /// <param name="index">The starting character index</param>
    /// <param name="count">The number of characters to return</param>
    /// <returns>A string from the start charcter and reading the specified number of characters</returns>
    /// <exception cref="ObjectDisposedException">Throws if the PdfText object has been disposed</exception>
    /// <exception cref="ArgumentOutOfRangeException">Throws if the starting index or the ending index are out of bounds</exception>
    public string GetTextRange(int index, int count)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));
        if (index < 0 || index >= _charCount || count < 0 || (index + count) > _charCount)
            throw new ArgumentOutOfRangeException();

        int bufferLen = (count + 1); // +1 for null terminator
        var buffer = new ushort[bufferLen];

        int written = PdfTextNative.FPDFText_GetText(_handle, index, count, buffer);
        if (written <= 0) return string.Empty;

        // Convert ushort[] → char[] safely
        var chars = new char[written];
        for (int i = 0; i < written; i++)
            chars[i] = (char)buffer[i];

        return new string(chars);
    }

    /// <summary>
    /// Returns the x, y origin of the character at the specified index in the associated PDF text.
    /// </summary>
    /// <param name="index">The index of the character to get the origin for</param>
    /// <param name="x">The out x variable of the origin</param>
    /// <param name="y">The out y variable of the origin</param>
    /// <returns>The origin of the specified character & true on success, false on failure</returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool TryGetCharOrigin(int index, out double x, out double y)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        return PdfTextNative.FPDFText_GetCharOrigin(_handle, index, out x, out y);
    }

    /// <summary>
    /// Counts how many bounding rectangles exist for a specific range of characters in the associated PDF text.
    /// </summary>
    /// <param name="startIndex">The starting index of the character to begin counting bounding rectangles</param>
    /// <param name="count">The count of characters to count bounding rectangles for</param>
    /// <returns>The number of bounding rectangles that exist for the specified range of characters</returns>
    /// <exception cref="ObjectDisposedException"></exception>
    public int CountRects(int startIndex, int count)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        return PdfTextNative.FPDFText_CountRects(_handle, startIndex, count);
    }

    /// <summary>
    /// Searches for the specified text in the associated PDF text. The search can be case-sensitive and/or whole-word only.
    /// </summary>
    /// <param name="searchTerm">The term to search for</param>
    /// <param name="matchCase">A boolean indicating whether or not the search should be case-sensitive</param>
    /// <param name="matchWholeWord">Flag indicating whether or not the search whould match the whole-word only</param>
    /// <param name="startIndex">The index of the character to begin searching at</param>
    /// <returns>A PdfTextSearch result</returns>
    /// <exception cref="ArgumentException">Throws if the search term is null or empty</exception>
    /// <exception cref="dotPDFiumException">Throws on a PDFium library error</exception>
    public PdfTextSearch Find(string searchTerm, bool matchCase = false, bool matchWholeWord = false, int startIndex = 0)
    {
        if (string.IsNullOrEmpty(searchTerm))
            throw new ArgumentException("Search term cannot be null or empty", nameof(searchTerm));

        uint flags = 0;
        if (matchCase) flags |= 1;
        if (matchWholeWord) flags |= 2;

        IntPtr searchHandle = PdfTextNative.FPDFText_FindStart(_handle, searchTerm, flags, startIndex);
        if (searchHandle == IntPtr.Zero)
            throw new dotPDFiumException("Failed to start search");

        return new PdfTextSearch(searchHandle, this);
    }

    /// <summary>
    /// Dispose method for the PdfText object. This method is called when the object is disposed.
    /// </summary>
    /// <param name="disposing">Whether to dispose of managed resources</param>
    protected override void Dispose(bool disposing)
    {
        // Let base class call FPDFText_ClosePage
        base.Dispose(disposing);
    }
}

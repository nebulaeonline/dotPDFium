using nebulae.dotPDFium.Native;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace nebulae.dotPDFium;

public class PdfText : PdfObject
{
    private readonly PdfPage _parentPage;
    private int _charCount = 0;
    private IntPtr _weblinkHandle = IntPtr.Zero;

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
    /// Retrieves the rotation angle of the character at the specified index within the text object.
    /// </summary>
    /// <param name="index">The zero-based index of the character whose rotation angle is to be retrieved.  
    /// Must be within the range of 0 to the total number of characters minus one.</param>
    /// <returns>The rotation angle of the character at the specified index, in degrees.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or 
    /// greater than or equal to the total number of characters.</exception>
    public float GetCharAngle(int index)
    {
        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        return PdfTextNative.FPDFText_GetCharAngle(_handle, index);
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
    /// Gets the index of the character located at the specified position within the text.
    /// </summary>
    /// <param name="x">The x-coordinate of the position to check, in device-independent points.</param>
    /// <param name="y">The y-coordinate of the position to check, in device-independent points.</param>
    /// <param name="xTolerance">The horizontal tolerance, in device-independent points, for matching the position to a character. Defaults to
    /// 2.0.</param>
    /// <param name="yTolerance">The vertical tolerance, in device-independent points, for matching the position to a character. Defaults to 2.0.</param>
    /// <returns>The zero-based index of the character at the specified position, or -1 if no character is found within the
    /// specified tolerances.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the underlying text object has been disposed.</exception>
    public int GetCharIndexAtPos(double x, double y, double xTolerance = 2.0, double yTolerance = 2.0)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        return PdfTextNative.FPDFText_GetCharIndexAtPos(_handle, x, y, xTolerance, yTolerance);
    }

    /// <summary>
    /// Attempts to retrieve the character index at the specified position within the document.
    /// </summary>
    /// <remarks>This method returns <see langword="false"/> if the underlying document handle is invalid or
    /// if no character is found at the specified position within the given tolerances.</remarks>
    /// <param name="x">The x-coordinate of the position, in device-independent points.</param>
    /// <param name="y">The y-coordinate of the position, in device-independent points.</param>
    /// <param name="index">When this method returns, contains the zero-based index of the character at the specified position, if the
    /// operation succeeds. If the operation fails, this will be set to 0.</param>
    /// <param name="xTolerance">The horizontal tolerance, in device-independent points, for determining the character at the position. The
    /// default value is 2.0.</param>
    /// <param name="yTolerance">The vertical tolerance, in device-independent points, for determining the character at the position. The default
    /// value is 2.0.</param>
    /// <returns><see langword="true"/> if a character index was successfully retrieved at the specified position; otherwise,
    /// <see langword="false"/>.</returns>
    public bool TryGetCharIndexAtPos(double x, double y, out int index, double xTolerance = 2.0, double yTolerance = 2.0)
    {
        if (_handle == IntPtr.Zero)
        {
            index = 0;
            return false;
        }

        index = PdfTextNative.FPDFText_GetCharIndexAtPos(_handle, x, y, xTolerance, yTolerance);

        if (index == -1)
            return false;
        
        return true;
    }

    /// <summary>
    /// Retrieves the fill color of the character at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the character whose fill color is to be retrieved.</param>
    /// <returns>An <see cref="RgbaColor"/> representing the fill color of the character, or <see langword="null"/> if the fill
    /// color is not available.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than or equal to the total number of characters.</exception>
    public RgbaColor? GetFillColor(int index)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));
        
        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        uint red, green, blue, alpha;

        var fillColor = PdfTextNative.FPDFText_GetFillColor(_handle, index, out red, out green, out blue, out alpha);

        if (!fillColor)
            return null;

        return new RgbaColor((byte)red, (byte)green, (byte)blue, (byte)alpha);
    }
    
    /// <summary>
    /// Retrieves font information for the character at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the character for which to retrieve font information. Must be within the range of
    /// available characters.</param>
    /// <returns>A <see cref="PdfFontInfo"/> object containing the font name and style flags for the specified character,  or
    /// <see langword="null"/> if the font information cannot be retrieved. Flags can be checked using the PdfFontFlags enum and .HasFlag().</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the <see cref="PdfText"/> object has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than or equal 
    /// to the total number of characters.</exception>
    public PdfFontInfo? GetFontInfo(int index)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        // First call: get buffer size needed
        int flags;
        uint requiredSize = PdfTextNative.FPDFText_GetFontInfo(_handle, index, null!, 0, out flags);

        if (requiredSize == 0)
            return null;

        byte[] buffer = new byte[requiredSize];
        uint actualSize = PdfTextNative.FPDFText_GetFontInfo(_handle, index, buffer, requiredSize, out flags);

        if (actualSize == 0)
            return null;

        string fontName = Encoding.UTF8.GetString(buffer.AsSpan(0, (int)actualSize)).TrimEnd('\0');
        return new PdfFontInfo(fontName, flags);
    }

    /// <summary>
    /// Retrieves the font size of the character at the specified index in the text.
    /// </summary>
    /// <param name="index">The zero-based index of the character whose font size is to be retrieved.  Must be within the range of 0 to the
    /// total character count minus one.</param>
    /// <returns>The font size of the character at the specified index, expressed as a <see cref="double"/>.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the underlying text object has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than or equal to the total character count.</exception>
    public double GetFontSize(int index)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        double size = PdfTextNative.FPDFText_GetFontSize(_handle, index);
        return size;
    }

    /// <summary>
    /// Retrieves the font weight of the character at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the character whose font weight is to be retrieved.  Must be within the range of
    /// available characters.</param>
    /// <returns>An integer representing the font weight of the specified character.  The value corresponds to the font weight as
    /// defined in the PDF document.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the underlying PDF text object has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than or equal to 
    /// the total number of characters.</exception>
    public int GetFontWeight(int index)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        return PdfTextNative.FPDFText_GetFontWeight(_handle, index);
    }

    /// <summary>
    /// Retrieves the loose bounding box of a character at the specified index within the text.
    /// </summary>
    /// <remarks>The loose bounding box may include additional space around the character, such as padding or
    /// spacing,  depending on the font and rendering context.</remarks>
    /// <param name="index">The zero-based index of the character whose bounding box is to be retrieved.</param>
    /// <returns>A <see cref="FsRectF"/> structure representing the loose bounding box of the character if successful; 
    /// otherwise, <see langword="null"/>.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the text object has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater 
    /// than or equal to the total number of characters.</exception>
    public FsRectF? GetLooseCharBox(int index)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        FsRectF rect;
        bool success = PdfTextNative.FPDFText_GetLooseCharBox(_handle, index, out rect);
        return success ? rect : null;
    }

    /// <summary>
    /// Retrieves the transformation matrix for the character at the specified index.
    /// </summary>
    /// <remarks>The transformation matrix describes how the character is positioned and scaled within the
    /// document.</remarks>
    /// <param name="index">The zero-based index of the character for which to retrieve the transformation matrix.</param>
    /// <returns>An <see cref="FsMatrix"/> representing the transformation matrix of the character if the operation is
    /// successful;  otherwise, <see langword="null"/>.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than 
    /// or equal to the total number of characters.</exception>
    public FsMatrix? GetMatrix(int index)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        FsMatrix matrix;
        bool success = PdfTextNative.FPDFText_GetMatrix(_handle, index, out matrix);
        return success ? matrix : null;
    }

    /// <summary>
    /// Retrieves the stroke color of the character at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the character whose stroke color is to be retrieved.</param>
    /// <returns>An <see cref="RgbaColor"/> representing the stroke color of the character, or <see langword="null"/>  if the
    /// stroke color could not be determined.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than or equal to the total number of characters.</exception>
    public RgbaColor? GetStrokeColor(int index)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        if (!PdfTextNative.FPDFText_GetStrokeColor(_handle, index, out uint r, out uint g, out uint b, out uint a))
            return null;

        return new RgbaColor((byte)r, (byte)g, (byte)b, (byte)a);
    }

    /// <summary>
    /// Retrieves the text object at the specified index within the PDF text.
    /// </summary>
    /// <param name="index">The zero-based index of the text object to retrieve. Must be within the range of available text objects.</param>
    /// <returns>A <see cref="PdfTextObject"/> representing the text object at the specified index,  or <see langword="null"/> if
    /// no text object exists at the specified index.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the current <see cref="PdfText"/> instance has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than or equal to the total number of text objects.</exception>
    public PdfTextObject? GetTextObject(int index)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        var objHandle = PdfTextNative.FPDFText_GetTextObject(_handle, index);
        if (objHandle == IntPtr.Zero)
            return null;

        return new PdfTextObject(objHandle); // Assuming ownership is not transferred.
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
    /// Returns the chacters from the specified index and count from the associated PDF text.
    /// </summary>
    /// <param name="index">The starting character index</param>
    /// <param name="count">The number of characters to return</param>
    /// <returns>A string from the start charcter and reading the specified number of characters and
    /// true on success, false on failure</returns>
    public bool TryGetTextRange(int index, int count, out string text)
    {
        if (_handle == IntPtr.Zero || index < 0 || index >= _charCount || count < 0 || (index + count) > _charCount)
        {
            text = string.Empty;
            return false;
        }

        int bufferLen = (count + 1); // +1 for null terminator
        var buffer = new ushort[bufferLen];

        int written = PdfTextNative.FPDFText_GetText(_handle, index, count, buffer);
        if (written <= 0)
        {
            text = string.Empty;
            return false;
        }

        // Convert ushort[] → char[] safely
        var chars = new char[written];
        for (int i = 0; i < written; i++)
            chars[i] = (char)buffer[i];

        text = new string(chars);
        return true;
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
    public bool GetCharOrigin(int index, out double x, out double y)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        return PdfTextNative.FPDFText_GetCharOrigin(_handle, index, out x, out y);
    }

    /// <summary>
    /// Returns the x, y origin of the character at the specified index in the associated PDF text.
    /// </summary>
    /// <param name="index">The index of the character to get the origin for</param>
    /// <param name="x">The out x variable of the origin</param>
    /// <param name="y">The out y variable of the origin</param>
    /// <returns>The origin of the specified character & true on success, false on failure</returns>
    public bool TryGetCharOrigin(int index, out double x, out double y)
    {
        if (_handle == IntPtr.Zero || index < 0 || index >= _charCount)
        {
            x = y = 0.0;
            return false;
        }
            
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
    /// Counts how many bounding rectangles exist for a specific range of characters in the associated PDF text.
    /// </summary>
    /// <param name="startIndex">The starting index of the character to begin counting bounding rectangles</param>
    /// <param name="count">The count of characters to count bounding rectangles for</param>
    /// <param name="rects">The out parameter to hold The number of bounding rectangles that exist for the specified range of characters</param>
    /// <returns>true on success, false on failure</returns>
    public bool TryCountRects(int startIndex, int count, int rects)
    {
        if (_handle == IntPtr.Zero)
        {
            rects = 0;
            return false;
        }
            
        rects = PdfTextNative.FPDFText_CountRects(_handle, startIndex, count);
        return true;
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
    /// Converts a text index to the corresponding character index within the document.
    /// </summary>
    /// <remarks>This method maps a logical text index to its associated character index, which can be used
    /// for further text processing. Ensure that the provided <paramref name="textIndex"/> is within the bounds of the
    /// document's text content to avoid exceptions.</remarks>
    /// <param name="textIndex">The zero-based index of the text element to be converted. Must be a valid index within the document's text
    /// content.</param>
    /// <returns>The zero-based character index corresponding to the specified text index.</returns>
    public int GetCharIndexFromTextIndex(int textIndex)
    {
        return PdfSearchExNative.FPDFText_GetCharIndexFromTextIndex(_handle, textIndex);
    }

    /// <summary>
    /// Converts a character index to the corresponding text index in the document.
    /// </summary>
    /// <remarks>This method maps a character index to its equivalent text index, which may differ depending
    /// on the document's internal representation of text. Ensure that <paramref name="charIndex"/> is within the valid
    /// range of the text content to avoid exceptions.</remarks>
    /// <param name="charIndex">The zero-based index of the character within the text content.</param>
    /// <returns>The zero-based text index corresponding to the specified character index.</returns>
    public int GetTextIndexFromCharIndex(int charIndex)
    {
        return PdfSearchExNative.FPDFText_GetTextIndexFromCharIndex(_handle, charIndex);
    }

    /// <summary>
    /// Determines whether the character at the specified index has a Unicode mapping error.
    /// </summary>
    /// <param name="index">The zero-based index of the character to check. Must be within the range of valid character indices.</param>
    /// <returns><see langword="true"/> if the character at the specified index has a Unicode mapping error; otherwise, <see
    /// langword="false"/>.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than or equal 
    /// to the total character count.</exception>
    public bool HasUnicodeMapError(int index)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        return PdfTextNative.FPDFText_HasUnicodeMapError(_handle, index) != 0;
    }

    /// <summary>
    /// Determines whether the character at the specified index is a generated character.
    /// </summary>
    /// <param name="index">The zero-based index of the character to check. Must be within the valid range of characters.</param>
    /// <returns><see langword="true"/> if the character at the specified index is a generated character; otherwise, <see
    /// langword="false"/>.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the underlying object has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than or equal to 
    /// the total character count.</exception>
    public bool IsGenerated(int index)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        return PdfTextNative.FPDFText_IsGenerated(_handle, index) != 0;
    }

    /// <summary>
    /// Determines whether the character at the specified index is a hyphen.
    /// </summary>
    /// <param name="index">The zero-based index of the character to check. Must be within the range of valid character indices.</param>
    /// <returns><see langword="true"/> if the character at the specified index is a hyphen; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the underlying text resource has been disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than or equal to the 
    /// total number of characters.</exception>
    public bool IsHyphen(int index)
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException(nameof(PdfText));

        if (index < 0 || index >= _charCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        return PdfTextNative.FPDFText_IsHyphen(_handle, index) != 0;
    }
    
    /// <summary>
    /// Loads the web links associated with the current PDF document.
    /// </summary>
    /// <remarks>This method initializes the web link handle for the PDF document if it has not already been
    /// loaded.  If the web link handle is already initialized, the method returns without performing any action.  If
    /// the operation fails, an exception is thrown.</remarks>
    /// <exception cref="dotPDFiumException">Thrown if the web links cannot be loaded successfully.</exception>
    public void LoadWebLinks()
    {
        if (_weblinkHandle != IntPtr.Zero)
            return;

        _weblinkHandle = PdfTextNative.FPDFLink_LoadWebLinks(_handle);

        if (_weblinkHandle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to load weblinks: {PdfObject.GetPDFiumError()}");
    }

    /// <summary>
    /// Gets the number of web links associated with the current PDF text object.
    /// </summary>
    public int WebLinkCount => _weblinkHandle == IntPtr.Zero
        ? 0
        : PdfTextNative.FPDFLink_CountWebLinks(_weblinkHandle);

    /// <summary>
    /// Releases resources associated with the web links and resets the handle.
    /// </summary>
    /// <remarks>This method should be called to clean up resources when web link processing is no longer
    /// needed. Failing to call this method may result in resource leaks.</remarks>
    public void CloseWebLinks()
    {
        if (_weblinkHandle != IntPtr.Zero)
        {
            PdfTextNative.FPDFLink_CloseWebLinks(_weblinkHandle);
            _weblinkHandle = IntPtr.Zero;
        }
    }

    /// <summary>
    /// Dispose method for the PdfText object. This method is called when the object is disposed.
    /// </summary>
    /// <param name="disposing">Whether to dispose of managed resources</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            CloseWebLinks();

        // Let base class call FPDFText_ClosePage
        base.Dispose(disposing);
    }
}

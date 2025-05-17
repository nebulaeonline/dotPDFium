using nebulae.dotPDFium.Native;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium;

/// <summary>
/// Represents a font object in a PDF document, providing access to font properties and metrics.
/// </summary>
/// <remarks>This class allows users to retrieve information about a font, such as its name, family, weight, and
/// style, as well as access font data and glyph-specific metrics. It also provides methods to determine whether the
/// font is embedded in the PDF and to retrieve glyph paths for rendering or analysis.</remarks>
public class PdfFont : PdfObject
{
    private readonly string _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfFont"/> class with the specified handle and font name.
    /// This constructor is intended for internal use only and assumes ownership of the provided handle.
    /// </summary>
    /// <param name="handle">A pointer to the native font resource. This must be a valid, non-null handle.</param>
    /// <param name="name">The name of the font. Cannot be null or empty.</param>
    internal PdfFont(IntPtr handle, string name)
        : base(handle, PdfObjectType.Font)
    {
        _name = name;
    }

    /// <summary>
    /// Gets the name associated with the current instance.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Retrieves the base font name of the current font.
    /// </summary>
    /// <remarks>The base font name is typically used to identify the font in a PDF document. This method
    /// ensures that the font name is read as a UTF-8 encoded string.</remarks>
    /// <returns>A <see cref="string"/> containing the base font name. The returned string represents the name of the font as
    /// defined in the PDF document. Returns an empty string if the font name cannot be determined.</returns>
    public string GetBaseFontName()
    {
        return PdfUtil.ReadUtf8StringFromLengthPrefixedCall(
            len => PdfEditNative.FPDFFont_GetBaseFontName(_handle, null!, len),
            (buf, len) => PdfEditNative.FPDFFont_GetBaseFontName(_handle, buf, len)
        );
    }

    /// <summary>
    /// Retrieves the family name of the font associated with this instance.
    /// </summary>
    /// <remarks>The family name is typically used to identify the general design of the font,  such as
    /// "Arial" or "Times New Roman". This method ensures the name is returned  as a UTF-8 encoded string.</remarks>
    /// <returns>A <see cref="string"/> containing the family name of the font.  Returns an empty string if the family name
    /// cannot be determined.</returns>
    public string GetFamilyName()
    {
        return PdfUtil.ReadUtf8StringFromLengthPrefixedCall(
            len => PdfEditNative.FPDFFont_GetFamilyName(_handle, null!, len),
            (buf, len) => PdfEditNative.FPDFFont_GetFamilyName(_handle, buf, len)
        );
    }

    /// <summary>
    /// Retrieves the raw font data associated with the current font.
    /// </summary>
    /// <remarks>This method returns the font data as a byte array. If the font has no associated data, an
    /// empty array is returned. The caller can use this data for further processing, such as embedding the font in a
    /// document or analyzing its structure.</remarks>
    /// <returns>A byte array containing the raw font data. If the font has no data, an empty array is returned.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the font data cannot be retrieved due to an internal error.</exception>
    public byte[] GetFontData()
    {
        PdfEditNative.FPDFFont_GetFontData(_handle, null!, UIntPtr.Zero, out var size);
        if (size == UIntPtr.Zero)
            return Array.Empty<byte>();

        byte[] buffer = new byte[(int)size];
        if (!PdfEditNative.FPDFFont_GetFontData(_handle, buffer, (UIntPtr)buffer.Length, out _))
            throw new dotPDFiumException("Failed to get font data.");

        return buffer;
    }

    /// <summary>
    /// Gets a value indicating whether the font is embedded within the PDF document.
    /// </summary>
    /// <remarks>An embedded font ensures that the font's appearance is preserved regardless of the
    /// availability of the font on the viewing system.</remarks>
    public bool IsEmbedded => PdfEditNative.FPDFFont_GetIsEmbedded(_handle) != 0;

    /// <summary>
    /// Retrieves the font flags associated with the current font.
    /// </summary>
    /// <remarks>The returned bitmask can be used to determine various characteristics of the font. For
    /// example, specific bits may indicate whether the font is bold, italic, or symbolic. Refer to the PDF
    /// specification for details on the meaning of each bit.</remarks>
    /// <returns>An integer representing the font flags. The value is a bitmask where each bit represents a specific font
    /// property, such as whether the font is fixed-pitch, serif, symbolic, or others.</returns>
    public int GetFlags()
    {
        return PdfEditNative.FPDFFont_GetFlags(_handle);
    }

    /// <summary>
    /// Gets the weight of the font.
    /// </summary>
    /// <remarks>The font weight is a numerical representation of the font's thickness or boldness. This value
    /// can be used to determine the visual style of the font in rendering or layout operations.</remarks>
    /// <returns>An integer representing the weight of the font. The value typically ranges from 100 (Thin) to 900 (Black), where
    /// higher values indicate a bolder font weight.</returns>
    public int GetWeight()
    {
        return PdfEditNative.FPDFFont_GetWeight(_handle);
    }

    /// <summary>
    /// Retrieves the italic angle of the font.
    /// </summary>
    /// <returns>The italic angle of the font in degrees, or <see langword="null"/> if the angle cannot be determined. A positive
    /// value indicates a right-leaning italic angle, while a negative value indicates a left-leaning italic angle.</returns>
    public int? GetItalicAngle()
    {
        return PdfEditNative.FPDFFont_GetItalicAngle(_handle, out int angle) ? angle : null;
    }

    /// <summary>
    /// Calculates the ascent of the font for the specified font size.
    /// </summary>
    /// <remarks>The ascent represents the distance from the baseline to the highest point of the font's
    /// glyphs for the given font size.</remarks>
    /// <param name="fontSize">The size of the font, in points, for which the ascent is calculated. Must be greater than 0.</param>
    /// <returns>The ascent of the font as a floating-point value, or <see langword="null"/> if the ascent could not be
    /// determined.</returns>
    public float? GetAscent(float fontSize)
    {
        return PdfEditNative.FPDFFont_GetAscent(_handle, fontSize, out float ascent) ? ascent : null;
    }

    /// <summary>
    /// Calculates the descent value for the font at the specified size.
    /// </summary>
    /// <remarks>The descent value represents the distance from the baseline to the lowest point of the font's
    /// glyphs,  expressed as a negative value. This method returns <see langword="null"/> if the calculation
    /// fails.</remarks>
    /// <param name="fontSize">The size of the font, in points, for which the descent is calculated. Must be greater than zero.</param>
    /// <returns>The descent value of the font as a <see cref="float"/> if the calculation is successful; otherwise, <see
    /// langword="null"/>.</returns>
    public float? GetDescent(float fontSize)
    {
        return PdfEditNative.FPDFFont_GetDescent(_handle, fontSize, out float descent) ? descent : null;
    }

    /// <summary>
    /// Retrieves the width of a specified glyph at a given font size.
    /// </summary>
    /// <remarks>This method queries the width of a glyph using the underlying font handle. The result is
    /// dependent on the font's metrics and the specified font size. If the glyph ID is invalid or the operation fails,
    /// the method returns <see langword="null"/>.</remarks>
    /// <param name="glyphId">The identifier of the glyph whose width is to be retrieved.</param>
    /// <param name="fontSize">The size of the font, in points, used to calculate the glyph's width. Must be greater than 0.</param>
    /// <returns>The width of the glyph as a <see cref="float"/> if the operation succeeds; otherwise, <see langword="null"/>.</returns>
    public float? GetGlyphWidth(uint glyphId, float fontSize)
    {
        return PdfEditNative.FPDFFont_GetGlyphWidth(_handle, glyphId, fontSize, out float width) ? width : null;
    }

    /// <summary>
    /// Retrieves the glyph path for the specified glyph ID and font size.
    /// </summary>
    /// <remarks>The glyph path represents the vector outline of the glyph, which can be used for rendering or
    /// analysis. Ensure that the font size is a positive value to avoid unexpected behavior.</remarks>
    /// <param name="glyphId">The ID of the glyph to retrieve the path for.</param>
    /// <param name="fontSize">The size of the font, in points, used to scale the glyph path.</param>
    /// <returns>A <see cref="PdfGlyphPath"/> object representing the path of the specified glyph,  or <see langword="null"/> if
    /// the glyph path could not be retrieved.</returns>
    public PdfGlyphPath? GetGlyphPath(uint glyphId, float fontSize)
    {
        var pathHandle = PdfEditNative.FPDFFont_GetGlyphPath(_handle, glyphId, fontSize);
        return pathHandle == IntPtr.Zero ? null : new PdfGlyphPath(pathHandle);
    }
}

using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public enum FxFontCharset
{
    ANSI = 0,
    Default = 1,
    Symbol = 2,
    ShiftJIS = 128,
    Hangeul = 129,
    GB2312 = 134,
    ChineseBig5 = 136,
    Greek = 161,
    Vietnamese = 163,
    Hebrew = 177,
    Arabic = 178,
    Cyrillic = 204,
    Thai = 222,
    EasternEuropean = 238
}

[Flags]
public enum FxFontPitchFamily
{
    FixedPitch = 1 << 0,
    Roman = 1 << 4,
    Script = 4 << 4
}

public enum FxFontWeight
{
    Normal = 400,
    Bold = 700
}

public static class PdfSystemFontNative
{
    private const string PdfiumLib = "pdfium";

    [DllImport(PdfiumLib)]
    public static extern void FPDF_SetSystemFontInfo(IntPtr fontInfo);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_GetDefaultSystemFontInfo();

    [DllImport(PdfiumLib)]
    public static extern void FPDF_FreeDefaultSystemFontInfo(IntPtr fontInfo);

    [DllImport(PdfiumLib)]
    public static extern void FPDF_AddInstalledFont(IntPtr mapper, [MarshalAs(UnmanagedType.LPStr)] string face, int charset);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_GetDefaultTTFMap();

    [DllImport(PdfiumLib)]
    public static extern UIntPtr FPDF_GetDefaultTTFMapCount();

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_GetDefaultTTFMapEntry(UIntPtr index);
}

using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;
public static class PdfSystemFontNative
{
    private const string PdfiumLib = "pdfium";
    [DllImport(PdfiumLib)]
    public static extern void FPDF_AddInstalledFont(IntPtr mapper, [MarshalAs(UnmanagedType.LPStr)] string face, int charset);
    [DllImport(PdfiumLib)]
    public static extern void FPDF_FreeDefaultSystemFontInfo(IntPtr fontInfo);
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_GetDefaultSystemFontInfo();
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_GetDefaultTTFMap();
    [DllImport(PdfiumLib)]
    public static extern UIntPtr FPDF_GetDefaultTTFMapCount();
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_GetDefaultTTFMapEntry(UIntPtr index);
    [DllImport(PdfiumLib)]
    public static extern void FPDF_SetSystemFontInfo(IntPtr fontInfo);
}
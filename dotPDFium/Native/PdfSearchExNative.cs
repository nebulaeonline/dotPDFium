using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;
public static class PdfSearchExNative
{
    private const string PdfiumLib = "pdfium";
    // Convert FPDFText_GetText result index → internal character index
    [DllImport(PdfiumLib)]
    public static extern int FPDFText_GetCharIndexFromTextIndex(IntPtr textPage, int textIndex);
    // Convert internal character index → index into FPDFText_GetText output
    [DllImport(PdfiumLib)]
    public static extern int FPDFText_GetTextIndexFromCharIndex(IntPtr textPage, int charIndex);
}
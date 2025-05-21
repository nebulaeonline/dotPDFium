using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;
public static class PdfThumbnailNative
{
    private const string PdfiumLib = "pdfium";
    // Gets decoded (RGB) thumbnail image data
    [DllImport(PdfiumLib)]
    public static extern uint FPDFPage_GetDecodedThumbnailData(IntPtr page, IntPtr buffer, uint buflen);
    // Gets raw (possibly compressed) thumbnail stream data
    [DllImport(PdfiumLib)]
    public static extern uint FPDFPage_GetRawThumbnailData(IntPtr page, IntPtr buffer, uint buflen);
    // Returns a prebuilt FPDF_BITMAP for the thumbnail if available
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPage_GetThumbnailAsBitmap(IntPtr page);
}
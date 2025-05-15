using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfAnnotNative
{
    private const string PdfiumLib = "pdfium";

    [DllImport(PdfiumLib)]
    public static extern bool FPDFAnnot_IsSupportedSubtype(int subtype);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPage_CreateAnnot(IntPtr page, int subtype);

    [DllImport(PdfiumLib)]
    public static extern int FPDFPage_GetAnnotCount(IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPage_GetAnnot(IntPtr page, int index);

    [DllImport(PdfiumLib)]
    public static extern int FPDFPage_GetAnnotIndex(IntPtr page, IntPtr annot);

    [DllImport(PdfiumLib)]
    public static extern void FPDFPage_CloseAnnot(IntPtr annot);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPage_RemoveAnnot(IntPtr page, int index);

    [DllImport(PdfiumLib)]
    public static extern int FPDFAnnot_GetSubtype(IntPtr annot);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFAnnot_SetColor(IntPtr annot, int type, uint r, uint g, uint b, uint a);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFAnnot_GetColor(IntPtr annot, int type, out uint r, out uint g, out uint b, out uint a);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFAnnot_SetRect(IntPtr annot, ref FS_RECTF rect);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFAnnot_GetRect(IntPtr annot, out FS_RECTF rect);
}


using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;
public static class PdfTransformPageNative
{
    private const string PdfiumLib = "pdfium";
    [DllImport(PdfiumLib)]
    public static extern int FPDFClipPath_CountPaths(IntPtr clipPath);
    [DllImport(PdfiumLib)]
    public static extern int FPDFClipPath_CountPathSegments(IntPtr clipPath, int pathIndex);
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFClipPath_GetPathSegment(IntPtr clipPath, int pathIndex, int segmentIndex);
    // Clip path inspection
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPageObj_GetClipPath(IntPtr pageObject);
    // Transform clip path of a page object
    [DllImport(PdfiumLib)]
    public static extern void FPDFPageObj_TransformClipPath(IntPtr pageObject, double a, double b, double c, double d, double e, double f);
    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFPage_GetArtBox(IntPtr page, out float left, out float bottom, out float right, out float top);
    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFPage_GetBleedBox(IntPtr page, out float left, out float bottom, out float right, out float top);
    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFPage_GetCropBox(IntPtr page, out float left, out float bottom, out float right, out float top);
    // Box getters
    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFPage_GetMediaBox(IntPtr page, out float left, out float bottom, out float right, out float top);
    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFPage_GetTrimBox(IntPtr page, out float left, out float bottom, out float right, out float top);
    [DllImport(PdfiumLib)]
    public static extern void FPDFPage_InsertClipPath(IntPtr page, IntPtr clipPath);
    [DllImport(PdfiumLib)]
    public static extern void FPDFPage_SetArtBox(IntPtr page, float left, float bottom, float right, float top);
    [DllImport(PdfiumLib)]
    public static extern void FPDFPage_SetBleedBox(IntPtr page, float left, float bottom, float right, float top);
    [DllImport(PdfiumLib)]
    public static extern void FPDFPage_SetCropBox(IntPtr page, float left, float bottom, float right, float top);
    // Box setters
    [DllImport(PdfiumLib)]
    public static extern void FPDFPage_SetMediaBox(IntPtr page, float left, float bottom, float right, float top);
    [DllImport(PdfiumLib)]
    public static extern void FPDFPage_SetTrimBox(IntPtr page, float left, float bottom, float right, float top);
    // Transform entire page with optional clipping
    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFPage_TransFormWithClip(IntPtr page, ref FsMatrixF matrix, ref FsRectF clipRect);
    // Create/destroy/insert custom clip path
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_CreateClipPath(float left, float bottom, float right, float top);
    [DllImport(PdfiumLib)]
    public static extern void FPDF_DestroyClipPath(IntPtr clipPath);
}
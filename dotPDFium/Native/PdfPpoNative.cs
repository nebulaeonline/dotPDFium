using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfPpoNative
{
    private const string PdfiumLib = "pdfium";

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDF_ImportPagesByIndex(IntPtr destDoc, IntPtr srcDoc, int[] pageIndices, uint length, int index);

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDF_ImportPages(IntPtr destDoc, IntPtr srcDoc, [MarshalAs(UnmanagedType.LPStr)] string pageRange, int index);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_ImportNPagesToOne(IntPtr srcDoc, float outputWidth, float outputHeight, UIntPtr pagesX, UIntPtr pagesY);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_NewXObjectFromPage(IntPtr destDoc, IntPtr srcDoc, int srcPageIndex);

    [DllImport(PdfiumLib)]
    public static extern void FPDF_CloseXObject(IntPtr xobject);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_NewFormObjectFromXObject(IntPtr xobject);

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDF_CopyViewerPreferences(IntPtr destDoc, IntPtr srcDoc);
}


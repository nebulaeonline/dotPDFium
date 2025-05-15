using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfStructTreeNative
{
    private const string PdfiumLib = "pdfium";

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_StructTree_GetForPage(IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern void FPDF_StructTree_Close(IntPtr structTree);

    [DllImport(PdfiumLib)]
    public static extern int FPDF_StructTree_CountChildren(IntPtr structTree);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_StructTree_GetChildAtIndex(IntPtr structTree, int index);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_StructElement_GetParent(IntPtr element);

    [DllImport(PdfiumLib)]
    public static extern int FPDF_StructElement_CountChildren(IntPtr element);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_StructElement_GetChildAtIndex(IntPtr element, int index);

    [DllImport(PdfiumLib)]
    public static extern int FPDF_StructElement_GetMarkedContentID(IntPtr element);

    [DllImport(PdfiumLib)]
    public static extern uint FPDF_StructElement_GetAltText(IntPtr element, [Out] char[] buffer, uint length);

    [DllImport(PdfiumLib)]
    public static extern uint FPDF_StructElement_GetActualText(IntPtr element, [Out] char[] buffer, uint length);

    [DllImport(PdfiumLib)]
    public static extern uint FPDF_StructElement_GetID(IntPtr element, [Out] char[] buffer, uint length);

    [DllImport(PdfiumLib)]
    public static extern uint FPDF_StructElement_GetLang(IntPtr element, [Out] char[] buffer, uint length);

    [DllImport(PdfiumLib)]
    public static extern uint FPDF_StructElement_GetType(IntPtr element, [Out] char[] buffer, uint length);

    [DllImport(PdfiumLib)]
    public static extern uint FPDF_StructElement_GetObjType(IntPtr element, [Out] char[] buffer, uint length);

    [DllImport(PdfiumLib)]
    public static extern uint FPDF_StructElement_GetTitle(IntPtr element, [Out] char[] buffer, uint length);
}


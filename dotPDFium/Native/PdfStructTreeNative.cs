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
    public static extern uint FPDF_StructElement_GetType(IntPtr element, [Out] byte[] buffer, uint buflen);

    [DllImport(PdfiumLib)]
    public static extern uint FPDF_StructElement_GetObjType(IntPtr element, [Out] char[] buffer, uint length);

    [DllImport(PdfiumLib)]
    public static extern uint FPDF_StructElement_GetTitle(IntPtr element, [Out] char[] buffer, uint length);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_StructElement_GetAttr(IntPtr element);

    [DllImport(PdfiumLib)]
    public static extern int FPDF_StructElement_GetAttributeCount(IntPtr element);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_StructElement_GetAttributeAtIndex(IntPtr element, int index);

    [DllImport(PdfiumLib)]
    public static extern int FPDF_StructElement_Attr_GetCount(IntPtr attr);

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDF_StructElement_Attr_GetName(IntPtr attr, int index, IntPtr buffer, uint buflen, out uint outBuflen);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_StructElement_Attr_GetValue(IntPtr attr, [MarshalAs(UnmanagedType.LPStr)] string name);

    [DllImport(PdfiumLib)]
    public static extern int FPDF_StructElement_Attr_GetType(IntPtr value);

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDF_StructElement_Attr_GetBooleanValue(IntPtr value, out bool result);

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDF_StructElement_Attr_GetNumberValue(IntPtr value, out float result);

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDF_StructElement_Attr_GetStringValue(IntPtr value, IntPtr buffer, uint buflen, out uint outBuflen);

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDF_StructElement_Attr_GetBlobValue(IntPtr value, IntPtr buffer, uint buflen, out uint outBuflen);

    [DllImport(PdfiumLib)]
    public static extern int FPDF_StructElement_Attr_CountChildren(IntPtr value);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_StructElement_Attr_GetChildAtIndex(IntPtr value, int index);

    [DllImport(PdfiumLib)]
    public static extern int FPDF_StructElement_GetMarkedContentIdCount(IntPtr element);

    [DllImport(PdfiumLib)]
    public static extern int FPDF_StructElement_GetMarkedContentIdAtIndex(IntPtr element, int index);

    [DllImport(PdfiumLib)]
    public static extern int FPDF_StructElement_GetChildMarkedContentID(IntPtr element, int index);

    [DllImport(PdfiumLib)]
    public static extern uint FPDF_StructElement_GetStringAttribute(IntPtr element, [MarshalAs(UnmanagedType.LPStr)] string name, [Out] char[] buffer, uint length);

}


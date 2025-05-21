using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfAttachmentNative
{
    private const string PdfiumLib = "pdfium";

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFDoc_AddAttachment(IntPtr document, [MarshalAs(UnmanagedType.LPWStr)] string name);
    
    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFDoc_DeleteAttachment(IntPtr document, int index);
    
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFDoc_GetAttachment(IntPtr document, int index);
    
    [DllImport(PdfiumLib)]
    public static extern int FPDFDoc_GetAttachmentCount(IntPtr document);

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFAttachment_GetFile(IntPtr attachment, IntPtr buffer, uint buflen, out uint outBufLen);

    [DllImport(PdfiumLib)]
    public static extern uint FPDFAttachment_GetName(IntPtr attachment, [Out] char[] buffer, uint buflen);

    [DllImport(PdfiumLib)]
    public static extern uint FPDFAttachment_GetStringValue(IntPtr attachment, [MarshalAs(UnmanagedType.LPStr)] string key, [Out] char[] buffer, uint buflen);

    [DllImport(PdfiumLib)]
    public static extern uint FPDFAttachment_GetSubtype(IntPtr attachment, [Out] char[] buffer, uint buflen);

    [DllImport(PdfiumLib)]
    public static extern int FPDFAttachment_GetValueType(IntPtr attachment, [MarshalAs(UnmanagedType.LPStr)] string key);

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFAttachment_HasKey(IntPtr attachment, [MarshalAs(UnmanagedType.LPStr)] string key);

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFAttachment_SetFile(IntPtr attachment, IntPtr document, IntPtr contents, uint length);

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFAttachment_SetStringValue(IntPtr attachment, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPWStr)] string value);
}


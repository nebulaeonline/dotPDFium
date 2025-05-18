using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfSignatureNative
{
    private const string PdfiumLib = "pdfium";

    [DllImport(PdfiumLib)]
    public static extern int FPDF_GetSignatureCount(IntPtr document);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_GetSignatureObject(IntPtr document, int index);

    [DllImport(PdfiumLib)]
    public static extern uint FPDFSignatureObj_GetContents(IntPtr signature, IntPtr buffer, uint length);

    [DllImport(PdfiumLib)]
    public static extern uint FPDFSignatureObj_GetByteRange(IntPtr signature, [Out] int[] buffer, uint length);

    [DllImport(PdfiumLib)]
    public static extern uint FPDFSignatureObj_GetSubFilter(IntPtr signature, [Out] byte[] buffer, uint length);

    [DllImport(PdfiumLib)]
    public static extern uint FPDFSignatureObj_GetReason(IntPtr signature, [Out] char[] buffer, uint length);

    [DllImport(PdfiumLib)]
    public static extern uint FPDFSignatureObj_GetTime(IntPtr signature, [Out] byte[] buffer, uint length);

    [DllImport(PdfiumLib)]
    public static extern uint FPDFSignatureObj_GetDocMDPPermission(IntPtr signature);
}

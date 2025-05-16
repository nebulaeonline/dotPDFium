using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

[Flags]
public enum PdfSaveFlags
{
    None = 0,
    Incremental = 1,
    NoIncremental = 2,
    RemoveSecurity = 3,
}

public static class PdfSaveNative
{
    private const string PdfiumLib = "pdfium";

    [DllImport("pdfium.dll", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDF_SaveAsCopy(IntPtr document, IntPtr writer, uint flags);

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDF_SaveWithVersion(IntPtr document, IntPtr writer, uint flags, int fileVersion);
}


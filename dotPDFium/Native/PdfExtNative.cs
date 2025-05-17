using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfExtNative
{
    private const string PdfiumLib = "pdfium";

    // Set a handler to be notified when unsupported features are encountered
    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FSDK_SetUnSpObjProcessHandler(ref UnSupportInfo info);

    // Replace the time() function (test hook only)
    [DllImport(PdfiumLib)]
    public static extern void FSDK_SetTimeFunction(TimeFunc func);

    // Replace the localtime() function (test hook only)
    [DllImport(PdfiumLib)]
    public static extern void FSDK_SetLocaltimeFunction(LocalTimeFunc func);

    // Return the initial page mode (e.g. thumbnails, full screen, etc.)
    [DllImport(PdfiumLib)]
    public static extern int FPDFDoc_GetPageMode(IntPtr document);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate time_t TimeFunc();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr LocalTimeFunc(ref time_t time);    
}

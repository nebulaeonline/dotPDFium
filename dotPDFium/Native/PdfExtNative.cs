using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public enum FpdfUnsupportedFeatureType
{
    Doc_XfaForm = 1,
    Doc_PortableCollection = 2,
    Doc_Attachment = 3,
    Doc_Security = 4,
    Doc_SharedReview = 5,
    Doc_SharedForm_Acrobat = 6,
    Doc_SharedForm_Filesystem = 7,
    Doc_SharedForm_Email = 8,
    Annot_3DAnnot = 11,
    Annot_Movie = 12,
    Annot_Sound = 13,
    Annot_ScreenMedia = 14,
    Annot_ScreenRichMedia = 15,
    Annot_Attachment = 16,
    Annot_Signature = 17
}

public enum FpdfPageMode
{
    Unknown = -1,
    UseNone = 0,
    UseOutlines = 1,
    UseThumbs = 2,
    FullScreen = 3,
    UseOC = 4,
    UseAttachments = 5
}


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

using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfEditNative
{
    private const string PdfiumLib = "pdfium";

    // Create a new, empty PDF document
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_CreateNewDocument();

    // Create a new page in the document
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPage_New(IntPtr document, int pageIndex, double width, double height);

    // Delete an existing page
    [DllImport(PdfiumLib)]
    public static extern void FPDFPage_Delete(IntPtr document, int pageIndex);

    // Insert a page object into a page
    [DllImport(PdfiumLib)]
    public static extern void FPDFPage_InsertObject(IntPtr page, IntPtr pageObject);

    [DllImport("pdfium")]
    public static extern bool FPDFPage_RemoveObject(IntPtr page, IntPtr pageObject);

    // Generate the page's updated content stream
    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFPage_GenerateContent(IntPtr page);

    // Get number of page objects on a page
    [DllImport(PdfiumLib)]
    public static extern int FPDFPage_CountObjects(IntPtr page);

    // Get a page object from a page
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPage_GetObject(IntPtr page, int index);

    // Get the type of a page object
    [DllImport("pdfium")]
    public static extern int FPDFPageObj_GetType(IntPtr pageObject);

    // Destroy a page object manually
    [DllImport(PdfiumLib)]
    public static extern void FPDFPageObj_Destroy(IntPtr pageObject);

    // Transform a page object (matrix: a b c d e f)
    [DllImport(PdfiumLib)]
    public static extern void FPDFPageObj_Transform(IntPtr pageObject, double a, double b, double c, double d, double e, double f);

    // Get the bounds of a page object
    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFPageObj_GetBounds(IntPtr pageObject, out float left, out float bottom, out float right, out float top);

    // Create a new text object with a standard font
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPageObj_NewTextObj(IntPtr document, [MarshalAs(UnmanagedType.LPStr)] string font, float fontSize);

    [DllImport("pdfium")]
    public static extern IntPtr FPDFPageObj_CreateTextObj(IntPtr document, IntPtr font, float fontSize);

    // Set text on a text object (UTF-16LE)
    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFText_SetText(IntPtr textObject, [MarshalAs(UnmanagedType.LPWStr)] string text);

    // Load one of the 14 standard fonts
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFText_LoadStandardFont(IntPtr document, [MarshalAs(UnmanagedType.LPStr)] string font);

    // Load a custom font from memory
    [DllImport("pdfium")]
    public static extern IntPtr FPDFText_LoadFont(IntPtr document, IntPtr fontData, uint size, int fontType, int cid);

    [DllImport("pdfium")]
    public static extern bool FPDFTextObj_GetFontSize(IntPtr textObject, out float size);

    // Close a font
    [DllImport(PdfiumLib)]
    public static extern void FPDFFont_Close(IntPtr font);
}


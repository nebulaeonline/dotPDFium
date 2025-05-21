using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;
public static class PdfProgressiveNative
{
    private const string PdfiumLib = "pdfium";
    // Rendering status constants
    public const int FPDF_RENDER_READY = 0;
    public const int FPDF_RENDER_TOBECONTINUED = 1;
    public const int FPDF_RENDER_DONE = 2;
    public const int FPDF_RENDER_FAILED = 3;
    // Start progressive rendering with a color scheme
    [DllImport(PdfiumLib)]
    public static extern int FPDF_RenderPageBitmapWithColorScheme_Start(IntPtr bitmap, IntPtr page, int startX, int startY, int sizeX, int sizeY, int rotate, int flags, ref PdfColorScheme colorScheme, ref IfSdkPause pause);
    // Start progressive rendering
    [DllImport(PdfiumLib)]
    public static extern int FPDF_RenderPageBitmap_Start(IntPtr bitmap, IntPtr page, int startX, int startY, int sizeX, int sizeY, int rotate, int flags, ref IfSdkPause pause);
    // Finalize rendering session
    [DllImport(PdfiumLib)]
    public static extern void FPDF_RenderPage_Close(IntPtr page);
    // Continue rendering if FPDF_RENDER_TOBECONTINUED
    [DllImport(PdfiumLib)]
    public static extern int FPDF_RenderPage_Continue(IntPtr page, ref IfSdkPause pause);
}
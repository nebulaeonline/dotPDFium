using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public class PdfViewNative
{
    private const string PdfiumLib = "pdfium";

    [DllImport(PdfiumLib)]
    internal static extern void FPDF_InitLibrary();

    [DllImport(PdfiumLib)]
    internal static extern void FPDF_DestroyLibrary();

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_LoadDocument([MarshalAs(UnmanagedType.LPStr)] string file_path, [MarshalAs(UnmanagedType.LPStr)] string password);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_LoadMemDocument(IntPtr data_buf, int size, [MarshalAs(UnmanagedType.LPStr)] string password);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_LoadMemDocument64(IntPtr data_buf, UIntPtr size, [MarshalAs(UnmanagedType.LPStr)] string password);

    [DllImport(PdfiumLib)]
    public static extern void FPDF_CloseDocument(IntPtr document);

    [DllImport(PdfiumLib)]
    public static extern int FPDF_GetPageCount(IntPtr document);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_LoadPage(IntPtr document, int page_index);

    [DllImport(PdfiumLib)]
    public static extern void FPDF_ClosePage(IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern double FPDF_GetPageWidth(IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern double FPDF_GetPageHeight(IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern float FPDF_GetPageWidthF(IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern float FPDF_GetPageHeightF(IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFBitmap_Create(int width, int height, int alpha);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFBitmap_CreateEx(int width, int height, int format, IntPtr first_scan, int stride);

    [DllImport(PdfiumLib)]
    public static extern void FPDFBitmap_Destroy(IntPtr bitmap);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFBitmap_GetBuffer(IntPtr bitmap);

    [DllImport(PdfiumLib)]
    public static extern int FPDFBitmap_GetWidth(IntPtr bitmap);

    [DllImport(PdfiumLib)]
    public static extern int FPDFBitmap_GetHeight(IntPtr bitmap);

    [DllImport(PdfiumLib)]
    public static extern int FPDFBitmap_GetStride(IntPtr bitmap);

    [DllImport(PdfiumLib)]
    public static extern int FPDFBitmap_GetFormat(IntPtr bitmap);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFBitmap_FillRect(IntPtr bitmap, int left, int top, int width, int height, uint color);

    [DllImport(PdfiumLib)]
    public static extern void FPDF_RenderPageBitmap(IntPtr bitmap, IntPtr page, int start_x, int start_y, int size_x, int size_y, int rotate, int flags);

    [DllImport(PdfiumLib)]
    public static extern void FPDF_RenderPageBitmapWithMatrix(IntPtr bitmap, IntPtr page, ref FS_MATRIX matrix, ref FS_RECTF clipping, int flags);

    [DllImport(PdfiumLib)]
    public static extern bool FPDF_DeviceToPage(IntPtr page, int start_x, int start_y, int size_x, int size_y, int rotate, int device_x, int device_y, out double page_x, out double page_y);

    [DllImport(PdfiumLib)]
    public static extern bool FPDF_PageToDevice(IntPtr page, int start_x, int start_y, int size_x, int size_y, int rotate, double page_x, double page_y, out int device_x, out int device_y);

    [DllImport(PdfiumLib)]
    public static extern uint FPDF_GetLastError();
}

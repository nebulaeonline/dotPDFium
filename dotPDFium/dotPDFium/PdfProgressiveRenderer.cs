using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

public static class PdfProgressiveRenderer
{
    public static PdfRenderStatus Start(
        PdfBitmap bitmap,
        PdfPage page,
        int x,
        int y,
        int width,
        int height,
        int rotation,
        int flags,
        IfSdkPause pause)
    {
        int result = PdfProgressiveNative.FPDF_RenderPageBitmap_Start(
            bitmap.Handle,
            page.Handle,
            x,
            y,
            width,
            height,
            rotation,
            flags,
            ref pause);

        return (PdfRenderStatus)result;
    }
    public static PdfRenderStatus StartWithColorScheme(
        PdfBitmap bitmap,
        PdfPage page,
        int x,
        int y,
        int width,
        int height,
        int rotation,
        int flags,
        PdfColorScheme colorScheme,
        IfSdkPause pause)
    {
        int result = PdfProgressiveNative.FPDF_RenderPageBitmapWithColorScheme_Start(
            bitmap.Handle,
            page.Handle,
            x,
            y,
            width,
            height,
            rotation,
            flags,
            ref colorScheme,
            ref pause);

        return (PdfRenderStatus)result;
    }

    /// <summary>
    /// Continues a previously started progressive render operation.
    /// </summary>
    /// <param name="page">The page being rendered.</param>
    /// <param name="pause">A pause struct used to control pausing.</param>
    /// <returns>A <see cref="PdfRenderStatus"/> indicating the current state.</returns>
    public static PdfRenderStatus Continue(PdfPage page, IfSdkPause pause)
    {
        int result = PdfProgressiveNative.FPDF_RenderPage_Continue(page.Handle, ref pause);
        return (PdfRenderStatus)result;
    }

    public static void Close(PdfPage page)
    {
        PdfProgressiveNative.FPDF_RenderPage_Close(page.Handle);
    }
}

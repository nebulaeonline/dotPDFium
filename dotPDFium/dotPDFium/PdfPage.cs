using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium
{
    /// <summary>
    /// This enum represents the rotation of the PDF page.
    /// </summary>
    public enum PdfRotation
    {
        NoRotation = 0,
        Rotate90 = 1,
        Rotate180 = 2,
        Rotate270 = 3
    }

    [Flags]
    public enum PdfRenderFlags
    {
        None = 0,
        Annot = 0x01,
        OptimizedText = 0x02,
        DisableNativeText = 0x04,
        Grayscale = 0x08,
        ShowDebugInfo = 0x80,
        NoCatchExceptions = 0x100,
        RenderLimitedImageCache = 0x200,
        ForceHalftone = 0x400,
        ForPrinting = 0x800,
        NoSmoothText = 0x1000,
        NoSmoothImages = 0x2000,
        NoSmoothPaths = 0x4000,
    }

    public class PdfPage : PdfObject
    {
        private readonly PdfDocument _parentDoc;
        private PdfText? _text = null;

        /// <summary>
        /// PdfPage constructor. This constructor is internal and should not be used directly.
        /// </summary>
        /// <param name="pageHandle">The PDFium page pointer</param>
        /// <param name="parentDocument">The parent PdfDocument</param>
        /// <exception cref="ArgumentException">Throws if the pageHandle is null or if the parentDocument is null</exception>
        internal PdfPage(IntPtr pageHandle, PdfDocument parentDocument) : base(pageHandle, PdfObjectType.Page)
        {
            if (pageHandle == IntPtr.Zero || parentDocument == null)
                throw new ArgumentException("Invalid page handle or parent doc:", nameof(pageHandle));

            _parentDoc = parentDocument;
        }
        
        /// <summary>
        /// Gets the width of the current PDF page in points.
        /// </summary>
        /// <remarks>The width is retrieved directly from the underlying PDF page handle. The value is
        /// typically used for rendering or layout purposes.</remarks>
        public double Width => _handle == IntPtr.Zero
            ? throw new ObjectDisposedException(nameof(PdfPage))
            : PdfViewNative.FPDF_GetPageWidth(_handle);

        /// <summary>
        /// Gets the height of the current PDF page in points.
        /// </summary>
        /// <remarks>The height is retrieved directly from the underlying PDF page handle. The value is
        /// typically used for rendering or layout purposes.</remarks>
        public double Height => _handle == IntPtr.Zero
            ? throw new ObjectDisposedException(nameof(PdfPage))
            : PdfViewNative.FPDF_GetPageHeight(_handle);

        /// <summary>
        /// Gets the width of the current PDF page in points as a float.
        /// </summary>
        public float WidthF => _handle == IntPtr.Zero
            ? throw new ObjectDisposedException(nameof(PdfPage))
            : PdfViewNative.FPDF_GetPageWidthF(_handle);

        /// <summary>
        /// Gets the height of the current PDF page in points as a float.
        /// </summary>
        public float HeightF => _handle == IntPtr.Zero
            ? throw new ObjectDisposedException(nameof(PdfPage))
            : PdfViewNative.FPDF_GetPageHeightF(_handle);

        /// <summary>
        /// Gets the text content of the current PDF page if it is already loaded, otherwise loads it.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException">Throws if the page has been disposed</exception>
        /// <exception cref="dotPDFiumException">Throws on PDFium library error</exception>
        public PdfText GetOrLoadText()
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfPage));

            if (_text == null)
            {
                var textHandle = PdfTextNative.FPDFText_LoadPage(_handle);
                
                if (textHandle == IntPtr.Zero)
                    throw new dotPDFiumException($"Failed to load text page: {PdfObject.GetPDFiumError()}");

                var text = new PdfText(textHandle, this);
                _text = text;
            }
            
            return _text!;
        }

        /// <summary>
        /// Gets the text content of the current PDF page if it is already loaded, otherwise loads it.
        /// </summary>
        /// <param name="pdfText">Out parameter for receiving the PdfText object</param>
        /// <returns>true on success, false on failure</returns>
        public bool TryGetOrLoadText(out PdfText? pdfText)
        {
            pdfText = null;

            if (_handle == IntPtr.Zero)
                return false;

            if (_text == null)
            {
                var textHandle = PdfTextNative.FPDFText_LoadPage(_handle);

                if (textHandle == IntPtr.Zero)
                    return false;

                var text = new PdfText(textHandle, this);
                _text = text;
                pdfText = _text;
                return true;
            }
            else
            {
                pdfText = _text;
                return true;
            }
        }


        /// <summary>
        /// This method renders the current PDF page to a bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to render the page to</param>
        /// <param name="startX">The starting x coordinate</param>
        /// <param name="startY">The starting y coordinate</param>
        /// <param name="width">The width of the section to render</param>
        /// <param name="height">The height of the section to render</param>
        /// <param name="rotate">The rotation of the page (see PdfRotation enum)</param>
        /// <param name="flags">See PdfRenderFlags</param>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void RenderToBitmap(
            PdfBitmap bitmap,
            int startX,
            int startY,
            int width,
            int height,
            PdfRotation rotate = PdfRotation.NoRotation,
            PdfRenderFlags flags = PdfRenderFlags.None)
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfPage));

            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            PdfViewNative.FPDF_RenderPageBitmap(bitmap.Handle, _handle, startX, startY, width, height, (int)rotate, (int)flags);
        }

        /// <summary>
        /// Renders the current PDF page to a bitmap using a transformation matrix and clipping rectangle.
        /// </summary>
        /// <param name="bitmap">The bitmap to render to</param>
        /// <param name="transform">The transform matrix for the page</param>
        /// <param name="clip">The clipping rectangle for the page</param>
        /// <param name="flags">Flags - see PdfRenderFlags</param>
        /// <exception cref="ObjectDisposedException">Thrown if the PDF page has already been disposed</exception>
        /// <exception cref="ArgumentNullException">Thrown if the PDF bitmap is null</exception>
        public void RenderToBitmapWithMatrix(
            PdfBitmap bitmap,
            ref FS_MATRIX transform,
            ref FS_RECTF clip,
            PdfRenderFlags flags = PdfRenderFlags.None)
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfPage));

            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            PdfViewNative.FPDF_RenderPageBitmapWithMatrix(
                bitmap.Handle,
                _handle,
                ref transform,
                ref clip,
                (int)flags
            );
        }

        /// <summary>
        /// Converts device coordinates to page coordinates.
        /// </summary>
        /// <param name="startX">The starting x coordinate</param>
        /// <param name="startY">The starting y coordinate</param>
        /// <param name="width">The width of the page</param>
        /// <param name="height">The height of the page</param>
        /// <param name="rotate">The rotation of the page; don't swap width and height on rotation.</param>
        /// <param name="deviceX">The device x coordinate</param>
        /// <param name="deviceY">The device y coordinate</param>
        /// <param name="pageX">out variable for the page x coordinate</param>
        /// <param name="pageY">out variable for the page y coordinate</param>
        /// <exception cref="ObjectDisposedException"></exception>
        public void DeviceToPage(
            int startX,
            int startY,
            int width,
            int height,
            PdfRotation rotate,
            int deviceX,
            int deviceY,
            out double pageX,
            out double pageY)
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfPage));

            PdfViewNative.FPDF_DeviceToPage(
                _handle,
                startX, startY, width, height,
                (int)rotate,
                deviceX, deviceY,
                out pageX, out pageY
            );
        }

        /// <summary>
        /// Converts page coordinates to device coordinates.
        /// </summary>
        /// <param name="startX">The starting x coordinate</param>
        /// <param name="startY">The starting y coordinate</param>
        /// <param name="width">The width of the page</param>
        /// <param name="height">The height of the page</param>
        /// <param name="rotate">The rotation of the page; don't swap width and height on rotation.</param>
        /// <param name="pageX">The page x coordinate</param>
        /// <param name="pageY">The page y coordinate</param>
        /// <param name="deviceX">out variable for the device x coordinate</param>
        /// <param name="deviceY">out variable for the device y coordinate</param>
        /// <exception cref="ObjectDisposedException"></exception>
        public void PageToDevice(
            int startX,
            int startY,
            int width,
            int height,
            PdfRotation rotate,
            double pageX,
            double pageY,
            out int deviceX,
            out int deviceY)
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfPage));

            PdfViewNative.FPDF_PageToDevice(
                _handle,
                startX, startY, width, height,
                (int)rotate,
                pageX, pageY,
                out deviceX, out deviceY
            );
        }

        /// <summary>
        /// Closes the current page and releases the associated resources.
        /// </summary>
        public void Close() => Dispose();
        
        /// <summary>
        /// Releases the resources used by the page, optionally unregistering it from the parent document.
        /// </summary>
        /// <remarks>When <paramref name="disposing"/> is <see langword="true"/>, this method unregisters
        /// the page  from its parent document, if applicable. Ensure that this method is called to properly clean up 
        /// resources associated with the page.</remarks>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release
        /// only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Dispose of text if it exists
                if (_text != null)
                {
                    _text.Dispose();
                    _text = null;
                }
                
                // Unregister from parent document if still alive
                _parentDoc?.UnregisterPage(this);

#if DEBUG
                System.Diagnostics.Debug.WriteLine($"[PdfPage] Disposed and unregistered from parent doc.");
#endif
            }

            // Native disposal will be handled by base
            base.Dispose(disposing);
        }
    }
}
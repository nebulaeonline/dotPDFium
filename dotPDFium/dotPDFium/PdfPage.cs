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
        /// <param name="startX">The starting x coordinate in pixels</param>
        /// <param name="startY">The starting y coordinate in pixels</param>
        /// <param name="width">The width of the bitmap to render in pixels</param>
        /// <param name="height">The height of the bitmap to render in pixels</param>
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
        /// Converts device (pixel) coordinates into page-space coordinates (points).
        /// </summary>
        /// <param name="startX">The X position of the rendered area in device pixels (usually 0).</param>
        /// <param name="startY">The Y position of the rendered area in device pixels (usually 0).</param>
        /// <param name="width">The width of the rendered area in pixels (matches the bitmap width).</param>
        /// <param name="height">The height of the rendered area in pixels (matches the bitmap height).</param>
        /// <param name="rotate">The rotation of the page (0, 90, 180, 270 degrees). See PdfRotation enum.
        /// Do not swap width and height manually; PDFium handles this automatically.</param>
        /// <param name="deviceX">The device-space X coordinate (in pixels) to convert.</param>
        /// <param name="deviceY">The device-space Y coordinate (in pixels) to convert.</param>
        /// <param name="pageX">Output variable for the resulting X coordinate in page-space points.</param>
        /// <param name="pageY">Output variable for the resulting Y coordinate in page-space points.</param>
        /// <exception cref="ObjectDisposedException">Thrown if the page has been disposed.</exception>

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
        /// Converts a point from page coordinates (points) to device coordinates (pixels).
        /// </summary>
        /// <param name="startX">The X offset in device pixels where the page rendering starts (usually 0).</param>
        /// <param name="startY">The Y offset in device pixels where the page rendering starts (usually 0).</param>
        /// <param name="width">The width of the rendered area in device pixels (e.g., the bitmap width).</param>
        /// <param name="height">The height of the rendered area in device pixels (e.g., the bitmap height).</param>
        /// <param name="rotation">The rotation of the page as a <see cref="PdfRotation"/> value (0, 90, 180, 270 degrees).
        /// /// PDFium applies the rotation internally; do not swap width and height manually.</param>
        /// <param name="pageX">The X coordinate in page space (points, where 1 point = 1/72 inch).</param>
        /// <param name="pageY">The Y coordinate in page space (points).</param>
        /// <param name="deviceX">Output parameter receiving the corresponding X coordinate in device space (pixels).</param>
        /// <param name="deviceY">Output parameter receiving the corresponding Y coordinate in device space (pixels).</param>
        /// <exception cref="ObjectDisposedException">Thrown if the page has been disposed.</exception>
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
        /// Insert a PDF page object into the current page.
        /// </summary>
        /// <param name="obj">The PdfObject to insert into the page</param>
        /// <exception cref="ArgumentNullException">Throws if obj is null</exception>
        public void InsertObject(PdfPageObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            PdfEditNative.FPDFPage_InsertObject(_handle, obj.Handle);
        }

        /// <summary>
        /// Finalizes the content of the current page. This method should be called after all modifications to the page are complete.
        /// </summary>
        /// <exception cref="dotPDFiumException">Throws on PDFium library error</exception>
        public void FinalizeContent()
        {
            if (!PdfEditNative.FPDFPage_GenerateContent(_handle))
                throw new dotPDFiumException($"Failed to generate page content: {PdfObject.GetPDFiumError()}");
        }

        /// <summary>
        /// Gets the number of objects on the current page.
        /// </summary>
        /// <returns></returns>
        public int GetObjectCount()
        {
            return PdfEditNative.FPDFPage_CountObjects(_handle);
        }

        /// <summary>
        /// Gets the object at the specified index on the current page.
        /// </summary>
        /// <param name="index">The index of the object to retrieve</param>
        /// <returns>The object at the specified index</returns>
        /// <exception cref="dotPDFiumException">Throws on PDFium library error</exception>
        public PdfPageObject GetObject(int index)
        {
            var objHandle = PdfEditNative.FPDFPage_GetObject(_handle, index);

            if (objHandle == IntPtr.Zero)
                throw new dotPDFiumException($"Failed to get page object at index {index}: {PdfObject.GetPDFiumError()}");

            var objType = (PdfPageObjectType)PdfEditNative.FPDFPageObj_GetType(objHandle);

            return objType switch
            {
                PdfPageObjectType.Text => new PdfTextObject(objHandle),
                PdfPageObjectType.Image => new PdfImageObject(objHandle),
                PdfPageObjectType.Path => new PdfPathObject(objHandle),
                PdfPageObjectType.Form => new PdfFormObject(objHandle),
                _ => new PdfPageObject(objHandle)
            };
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
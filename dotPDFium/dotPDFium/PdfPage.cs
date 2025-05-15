using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium
{
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
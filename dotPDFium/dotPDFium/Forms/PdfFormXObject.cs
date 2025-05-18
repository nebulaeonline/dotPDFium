using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms
{
    /// <summary>
    /// Represents a form XObject in a PDF document, which is a reusable content stream that can be drawn on a page.
    /// </summary>
    /// <remarks>A form XObject is a self-contained description of graphical content that can be reused across
    /// multiple pages or locations within a PDF document. This class provides methods to access and manipulate the
    /// objects contained within the form XObject.</remarks>
    public sealed class PdfFormXObject : PdfPageObject
    {
        /// <summary>
        /// Represents a PDF Form XObject, which is a reusable content stream in a PDF document.
        /// </summary>
        /// <remarks>A Form XObject is a self-contained description of graphical elements that can be
        /// reused multiple times within a PDF document. This class provides functionality to work with such
        /// objects.</remarks>
        /// <param name="handle"></param>
        internal PdfFormXObject(IntPtr handle) : base(handle) { }

        /// <summary>
        /// Creates a new page object from an existing form XObject.
        /// </summary>
        /// <param name="xobjectHandle">The handle to the XObject (must be a valid form XObject).</param>
        /// <returns>A new <see cref="PdfFormXObject"/> ready to be added to a page.</returns>
        /// <exception cref="ArgumentException">Thrown if the handle is null or invalid.</exception>
        /// <exception cref="dotPDFiumException">Thrown if PDFium fails to create the form object.</exception>
        public static PdfFormXObject CreateFrom(PdfFormXObject xobject)
        {
            if (xobject.Handle == IntPtr.Zero)
                throw new ArgumentException("XObject handle must not be null.", nameof(xobject.Handle));

            var handle = PdfPpoNative.FPDF_NewFormObjectFromXObject(xobject.Handle);
            if (handle == IntPtr.Zero)
                throw new dotPDFiumException("Failed to create form object from XObject.");

            return new PdfFormXObject(handle);
        }

        /// <summary>
        /// Creates a reusable XObject from a page in another document. This XObject can be turned into a form object for rendering.
        /// </summary>
        /// <param name="destination">The document that will own the new XObject.</param>
        /// <param name="source">The source document containing the page to convert.</param>
        /// <param name="pageIndex">Zero-based index of the page to turn into an XObject.</param>
        /// <returns>A <see cref="PdfFormXObject"/> representing the new XObject.</returns>
        /// <exception cref="ArgumentNullException">Thrown if either document is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if page index is negative.</exception>
        /// <exception cref="dotPDFiumException">Thrown if creation fails.</exception>
        public static PdfFormXObject CreateFromPage(PdfDocument destination, PdfDocument source, int pageIndex)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));

            var handle = PdfPpoNative.FPDF_NewXObjectFromPage(destination.Handle, source.Handle, pageIndex);
            if (handle == IntPtr.Zero)
                throw new dotPDFiumException("Failed to create XObject from source page.");

            return new PdfFormXObject(handle);
        }

        /// <summary>
        /// Counts the number of objects contained within the PDF form XObject.
        /// </summary>
        /// <returns>The total number of objects contained within the PDF form XObject.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the PDF form XObject has been disposed.</exception>
        public int CountObjects()
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfFormXObject));

            return PdfEditNative.FPDFFormObj_CountObjects(_handle);
        }

        /// <summary>
        /// Retrieves the <see cref="PdfPageObject"/> at the specified index within the form XObject.
        /// </summary>
        /// <param name="index">The zero-based index of the object to retrieve. Must be within the valid range of objects in the form
        /// XObject.</param>
        /// <returns>A <see cref="PdfPageObject"/> representing the object at the specified index.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the form XObject has been disposed and is no longer accessible.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is outside the valid range of objects in the form XObject.</exception>
        public PdfPageObject GetObjectAt(int index)
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfFormXObject));

            var raw = PdfEditNative.FPDFFormObj_GetObject(_handle, (uint)index);
            
            if (raw == IntPtr.Zero)
                throw new ArgumentOutOfRangeException(nameof(index));

            return PdfPageObject.Create(raw); // Factory method dispatching to correct derived type
        }

        /// <summary>
        /// Releases the native XObject handle, if this object was not owned by the page.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (!_hasOwner && _handle != IntPtr.Zero)
            {
                PdfPpoNative.FPDF_CloseXObject(_handle);
            }

            base.Dispose(disposing);
        }

    }

}

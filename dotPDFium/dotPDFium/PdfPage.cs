using nebulae.dotPDFium.Forms;
using nebulae.dotPDFium.Native;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace nebulae.dotPDFium
{
    public class PdfPage : PdfObject
    {
        private readonly PdfDocument _parentDoc;
        private PdfPageText? _text = null;

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
        /// Gets the rotation of the current PDF page.
        /// </summary>
        /// <returns>A <see cref="PdfPageRotation"/> value representing the rotation of the page.</returns>
        /// <exception cref="dotPDFiumException">Thrown if the rotation value retrieved from the native PDF library is invalid.</exception>
        public PdfPageRotation GetRotation()
        {
            int raw = PdfEditNative.FPDFPage_GetRotation(_handle);
            return Enum.IsDefined(typeof(PdfPageRotation), raw)
                ? (PdfPageRotation)raw
                : throw new dotPDFiumException($"Invalid rotation value: {raw}");
        }

        /// <summary>
        /// Sets the rotation of the current PDF page.
        /// </summary>
        /// <remarks>This method updates the rotation of the page to the specified value.  The rotation is
        /// applied in 90-degree increments, as defined by the <see cref="PdfPageRotation"/> enumeration.</remarks>
        /// <param name="rotation">The desired rotation for the page, specified as a <see cref="PdfPageRotation"/> value.</param>
        public void SetRotation(PdfPageRotation rotation)
        {
            PdfEditNative.FPDFPage_SetRotation(_handle, (int)rotation);
        }

        /// <summary>
        /// Gets the text content of the current PDF page if it is already loaded, otherwise loads it.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException">Throws if the page has been disposed</exception>
        /// <exception cref="dotPDFiumException">Throws on PDFium library error</exception>
        public PdfPageText GetOrLoadText()
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfPage));

            if (_text == null)
            {
                var textHandle = PdfTextNative.FPDFText_LoadPage(_handle);
                
                if (textHandle == IntPtr.Zero)
                    throw new dotPDFiumException($"Failed to load text page: {PdfObject.GetPDFiumError()}");

                var text = new PdfPageText(textHandle, this);
                _text = text;
            }
            
            return _text!;
        }

        /// <summary>
        /// Gets the text content of the current PDF page if it is already loaded, otherwise loads it.
        /// </summary>
        /// <param name="pdfText">Out parameter for receiving the PdfText object</param>
        /// <returns>true on success, false on failure</returns>
        public bool TryGetOrLoadText(out PdfPageText? pdfText)
        {
            pdfText = null;

            if (_handle == IntPtr.Zero)
                return false;

            if (_text == null)
            {
                var textHandle = PdfTextNative.FPDFText_LoadPage(_handle);

                if (textHandle == IntPtr.Zero)
                    return false;

                var text = new PdfPageText(textHandle, this);
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
        /// Retrieves the bounding box of the current PDF page.
        /// </summary>
        /// <returns>A <see cref="FsRectF"/> structure representing the bounding box of the page.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the PDF page has been disposed.</exception>
        /// <exception cref="dotPDFiumException">Thrown if the bounding box could not be retrieved.</exception>
        public FsRectF GetBoundingBox()
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfPage));

            if (!PdfViewNative.FPDF_GetPageBoundingBox(_handle, out FsRectF rect))
                throw new dotPDFiumException("Failed to get bounding box for page.");

            return rect;
        }

        /// <summary>
        /// Gets the logical structure tree for this page, if one exists.
        /// </summary>
        /// <returns>A <see cref="PdfStructTree"/>, or <c>null</c> if the page has no tagged content.</returns>
        public PdfStructTree? GetStructTree()
        {
            var treeHandle = PdfStructTreeNative.FPDF_StructTree_GetForPage(_handle);
            return treeHandle == IntPtr.Zero ? null : new PdfStructTree(treeHandle);
        }

        /// <summary>
        /// Retrieves the link annotation at the specified index as a <see cref="PdfAnnotation"/>, if it exists.
        /// </summary>
        /// <param name="linkIndex">The zero-based index of the link annotation.</param>
        /// <returns>The corresponding <see cref="PdfAnnotation"/> or <c>null</c> if not found.</returns>
        public PdfAnnotation? GetLinkAnnotation(int linkIndex)
        {
            var annotHandle = PdfDocNative.FPDFLink_GetAnnot(_handle, linkIndex);
            return annotHandle == IntPtr.Zero ? null : new PdfAnnotation(annotHandle, this);
        }

        /// <summary>
        /// Determines whether the PDF page contains any transparent elements.
        /// </summary>
        /// <remarks>This method checks for the presence of transparency on the PDF page, which may affect
        /// rendering or printing.</remarks>
        /// <returns><see langword="true"/> if the PDF page has transparent elements; otherwise, <see langword="false"/>.</returns>
        public bool HasTransparency()
        {
            return PdfEditNative.FPDFPage_HasTransparency(_handle);
        }

        /// <summary>
        /// Removes the specified PDF page object from the current page.
        /// </summary>
        /// <param name="obj">The <see cref="PdfPageObject"/> to remove. This parameter cannot be <see langword="null"/>.</param>
        /// <exception cref="dotPDFiumException">Thrown if the removal operation fails due to an error in the underlying PDF library.</exception>
        public void RemoveObject(PdfPageObject obj)
        {
            if (!PdfEditNative.FPDFPage_RemoveObject(_handle, obj.Handle))
                throw new dotPDFiumException($"Failed to remove page object: {PdfObject.GetPDFiumError()}");
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
            PdfPageRotation rotate = PdfPageRotation.NoRotation,
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
            ref FsMatrixF transform,
            ref FsRectF clip,
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
            PdfPageRotation rotate,
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
        /// <param name="rotation">The rotation of the page as a <see cref="PdfPageRotation"/> value (0, 90, 180, 270 degrees).
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
            PdfPageRotation rotate,
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

            if (obj.IsOwned)
                throw new InvalidOperationException("Object is already owned by a page.");

            PdfEditNative.FPDFPage_InsertObject(_handle, obj.Handle);
            obj.MarkOwned();
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
                _ => new PdfPageObject(objHandle)
            };
        }

        /// <summary>
        /// Gets the total number of annotations on the current PDF page.
        /// </summary>
        /// <remarks>This method retrieves the count of annotations present on the PDF page represented by
        /// this instance.</remarks>
        /// <returns>The number of annotations on the page. Returns 0 if the page has no annotations.</returns>
        public int GetAnnotationCount()
        {
            return PdfAnnotNative.FPDFPage_GetAnnotCount(_handle);
        }

        /// <summary>
        /// Retrieves the annotation at the specified index on the PDF page.
        /// </summary>
        /// <param name="index">The zero-based index of the annotation to retrieve. Must be within the range of available annotations on the
        /// page.</param>
        /// <returns>A <see cref="PdfAnnotation"/> object representing the annotation at the specified index.</returns>
        /// <exception cref="dotPDFiumException">Thrown if the annotation at the specified <paramref name="index"/> cannot be retrieved.</exception>
        public PdfAnnotation GetAnnotation(int index)
        {
            var handle = PdfAnnotNative.FPDFPage_GetAnnot(_handle, index);

            if (handle == IntPtr.Zero)
                throw new dotPDFiumException($"Failed to retrieve annotation at index {index}");

            return new PdfAnnotation(handle, this);
        }

        /// <summary>
        /// Creates a new annotation of the specified subtype on the current PDF page.
        /// </summary>
        /// <param name="subtype">The subtype of the annotation to create. This determines the type of annotation, such as a text note,
        /// highlight, or shape.</param>
        /// <returns>A <see cref="PdfAnnotation"/> object representing the newly created annotation.</returns>
        /// <exception cref="dotPDFiumException">Thrown if the annotation could not be created for the specified <paramref name="subtype"/>.</exception>
        public PdfAnnotation CreateAnnotation(PdfAnnotationSubtype subtype)
        {
            var handle = PdfAnnotNative.FPDFPage_CreateAnnot(_handle, (int)subtype);
            if (handle == IntPtr.Zero)
                throw new dotPDFiumException($"Failed to create annotation of subtype: {subtype}");

            return new PdfAnnotation(handle, this);
        }

        /// <summary>
        /// Retrieves the index of the specified annotation within the PDF page.
        /// </summary>
        /// <param name="annotation">The annotation whose index is to be retrieved. Must not be <c>null</c>.</param>
        /// <returns>The zero-based index of the annotation within the page.</returns>
        /// <exception cref="dotPDFiumException">Thrown if the annotation cannot be located on the page.</exception>
        public int GetAnnotationIndex(PdfAnnotation annotation)
        {
            int index = PdfAnnotNative.FPDFPage_GetAnnotIndex(_handle, annotation.Handle);
            if (index < 0)
                throw new dotPDFiumException("Failed to locate annotation on page.");
            return index;
        }

        /// <summary>
        /// Removes the specified annotation from the PDF page.
        /// </summary>
        /// <remarks>Use this method to delete an annotation from the current PDF page. Ensure that the
        /// annotation  is valid and associated with this page before calling this method.</remarks>
        /// <param name="annotation">The annotation to remove from the page. This parameter cannot be <see langword="null"/>.</param>
        /// <exception cref="dotPDFiumException">Thrown if the annotation could not be removed from the page.</exception>
        public void RemoveAnnotation(PdfAnnotation annotation)
        {
            if (!PdfAnnotNative.FPDFPage_RemoveAnnot(_handle, GetAnnotationIndex(annotation)))
                throw new dotPDFiumException("Failed to remove annotation from page.");
        }

        /// <summary>
        /// Internal delegate for getting the bounding box of a page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="left"></param>
        /// <param name="bottom"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        private delegate bool BoxGetter(
            IntPtr page,
            out float left,
            out float bottom,
            out float right,
            out float top);

        /// <summary>
        /// Retrieves a rectangular bounding box using the specified delegate and handle.
        /// </summary>
        /// <param name="getter">A delegate that retrieves the bounding box coordinates.</param>
        /// <param name="handle">A pointer to the object for which the bounding box is retrieved.</param>
        /// <returns>An <see cref="FsRectF"/> representing the bounding box, with coordinates specified by the delegate.</returns>
        /// <exception cref="dotPDFiumException">Thrown if the delegate fails to retrieve the bounding box.</exception>
        private static FsRectF GetBox(BoxGetter getter, IntPtr handle)
        {
            if (!getter(handle, out float left, out float bottom, out float right, out float top))
                throw new dotPDFiumException($"Failed to get page box: {PdfObject.GetPDFiumError()}");

            return new FsRectF(left, top, right, bottom);
        }

        /// <summary>
        /// Configures a rectangular box by invoking the specified setter action with the provided handle and box
        /// dimensions.
        /// </summary>
        /// <param name="setter">A delegate that accepts the handle and the box's dimensions (left, bottom, right, top) to set the box.</param>
        /// <param name="handle">A pointer to the resource or object associated with the box.</param>
        /// <param name="box">The rectangle structure containing the dimensions of the box to be set.</param>
        private static void SetBox(Action<IntPtr, float, float, float, float> setter, IntPtr handle, FsRectF box)
        {
            setter(handle, box.left, box.bottom, box.right, box.top);
        }

        /// <summary>
        /// Retrieves the MediaBox of the current PDF page.
        /// </summary>
        /// <remarks>The MediaBox is the default boundary for the page content in a PDF document.  It
        /// specifies the dimensions of the page in user space units.</remarks>
        /// <returns>An <see cref="FsRectF"/> structure representing the MediaBox of the page.  The MediaBox defines the
        /// boundaries of the physical medium on which the page is intended to be displayed or printed.</returns>
        public FsRectF GetMediaBox() => GetBox(PdfTransformPageNative.FPDFPage_GetMediaBox, _handle);
        
        /// <summary>
        /// Sets the MediaBox for the current page.
        /// </summary>
        /// <remarks>The MediaBox defines the boundaries of the physical medium on which the page is
        /// intended to be displayed or printed. Ensure that the provided <paramref name="box"/> is valid and within the
        /// acceptable range for the page.</remarks>
        /// <param name="box">The <see cref="FsRectF"/> structure representing the new MediaBox dimensions.  The coordinates must be
        /// specified in points and follow the PDF coordinate system.</param>
        public void SetMediaBox(FsRectF box) => SetBox(PdfTransformPageNative.FPDFPage_SetMediaBox, _handle, box);

        /// <summary>
        /// Retrieves the crop box of the current page.
        /// </summary>
        /// <returns>A <see cref="FsRectF"/> structure representing the crop box of the page.  The crop box defines the visible
        /// area of the page in user space coordinates.</returns>
        public FsRectF GetCropBox() => GetBox(PdfTransformPageNative.FPDFPage_GetCropBox, _handle);
        
        /// <summary>
        /// Sets the crop box for the current page.
        /// </summary>
        /// <remarks>The crop box defines the visible area of the page when displayed or printed.  Any
        /// content outside the crop box will be clipped.</remarks>
        /// <param name="box">The <see cref="FsRectF"/> structure defining the crop box dimensions.  The coordinates are specified in
        /// points, with the origin at the bottom-left corner of the page.</param>
        public void SetCropBox(FsRectF box) => SetBox(PdfTransformPageNative.FPDFPage_SetCropBox, _handle, box);

        /// <summary>
        /// Retrieves the bleed box of the page, which defines the region to which the page's content should be clipped
        /// when printed.
        /// </summary>
        /// <remarks>The bleed box is typically used in printing workflows to account for content that
        /// extends beyond the trim box, such as bleeds.</remarks>
        /// <returns>An <see cref="FsRectF"/> structure representing the bleed box of the page. The bleed box is defined in the
        /// page's coordinate system.</returns>
        public FsRectF GetBleedBox() => GetBox(PdfTransformPageNative.FPDFPage_GetBleedBox, _handle);
        
        /// <summary>
        /// Sets the bleed box for the current page.
        /// </summary>
        /// <remarks>The bleed box is used to ensure that content intended to extend to the edge of the
        /// page  is printed correctly, even if the page is trimmed. Ensure that the <paramref name="box"/>  parameter
        /// specifies valid dimensions within the page boundaries.</remarks>
        /// <param name="box">The <see cref="FsRectF"/> structure representing the bleed box dimensions.  The bleed box defines the region
        /// to which the page content should extend,  typically used for printing purposes.</param>
        public void SetBleedBox(FsRectF box) => SetBox(PdfTransformPageNative.FPDFPage_SetBleedBox, _handle, box);

        /// <summary>
        /// Retrieves the trim box of the current page.
        /// </summary>
        /// <returns>An <see cref="FsRectF"/> structure representing the trim box of the page.  The trim box defines the intended
        /// dimensions of the page's content after trimming.</returns>
        public FsRectF GetTrimBox() => GetBox(PdfTransformPageNative.FPDFPage_GetTrimBox, _handle);
        
        /// <summary>
        /// Sets the trim box for the current page.
        /// </summary>
        /// <remarks>The trim box is typically used to specify the final dimensions of the page after any
        /// trimming or cutting operations. Ensure that the provided <paramref name="box"/> dimensions are valid and
        /// within the bounds of the page.</remarks>
        /// <param name="box">The <see cref="FsRectF"/> structure representing the trim box dimensions. The trim box defines the intended
        /// visible area of the page after trimming.</param>
        public void SetTrimBox(FsRectF box) => SetBox(PdfTransformPageNative.FPDFPage_SetTrimBox, _handle, box);

        /// <summary>
        /// Retrieves the ArtBox of the current PDF page.
        /// </summary>
        /// <remarks>The ArtBox defines the extent of the page's meaningful content as intended by the
        /// creator. This method returns the rectangle that represents the ArtBox in user space coordinates.</remarks>
        /// <returns>A <see cref="FsRectF"/> structure representing the ArtBox of the page.  The rectangle's coordinates are in
        /// user space units.</returns>
        public FsRectF GetArtBox() => GetBox(PdfTransformPageNative.FPDFPage_GetArtBox, _handle);
        
        /// <summary>
        /// Sets the ArtBox for the current page.
        /// </summary>
        /// <remarks>The ArtBox defines the extent of the page's meaningful content,  excluding any
        /// additional elements such as bleed or trim areas. Ensure that the specified <paramref name="box"/> is valid
        /// and within the page's boundaries.</remarks>
        /// <param name="box">The <see cref="FsRectF"/> structure defining the ArtBox dimensions.  The coordinates must be specified in
        /// the page's coordinate system.</param>
        public void SetArtBox(FsRectF box) => SetBox(PdfTransformPageNative.FPDFPage_SetArtBox, _handle, box);

        /// <summary>
        /// Applies a transformation matrix and a clipping rectangle to the current page.
        /// </summary>
        /// <param name="matrix">The transformation matrix to apply to the page. This defines how the page content is scaled, rotated, or
        /// translated.</param>
        /// <param name="clip">The clipping rectangle that limits the visible area of the page after the transformation is applied.</param>
        /// <exception cref="dotPDFiumException">Thrown if the transformation or clipping operation fails.</exception>
        public void TransformWithClip(FsMatrixF matrix, FsRectF clip)
        {
            if (!PdfTransformPageNative.FPDFPage_TransFormWithClip(_handle, ref matrix, ref clip))
                throw new dotPDFiumException("Failed to transform page with clip.");
        }

        /// <summary>
        /// Flattens the PDF page content into a single layer, making annotations and form fields part of the page
        /// content.
        /// </summary>
        /// <remarks>Flattening a PDF page can be useful for ensuring that annotations and form fields are
        /// no longer interactive and are instead rendered as part of the static page content. This operation is
        /// typically irreversible.</remarks>
        /// <param name="mode">The flattening mode that determines how the content is rendered. Defaults to <see
        /// cref="PdfFlattenMode.NormalDisplay"/>.</param>
        /// <returns>A <see cref="PdfFlattenResult"/> indicating the result of the flattening operation: <see
        /// cref="PdfFlattenResult.Success"/> if the operation was successful, <see
        /// cref="PdfFlattenResult.NothingToDo"/> if there was no content to flatten, or <see
        /// cref="PdfFlattenResult.Fail"/> if the operation failed.</returns>
        public PdfFlattenResult Flatten(PdfFlattenMode mode = PdfFlattenMode.NormalDisplay)
        {
            int result = PdfFlattenNative.FPDFPage_Flatten(_handle, (int)mode);
            return result switch
            {
                1 => PdfFlattenResult.Success,
                2 => PdfFlattenResult.NothingToDo,
                _ => PdfFlattenResult.Fail
            };
        }

        /// <summary>
        /// Retrieves the decoded thumbnail image data for the current PDF page.
        /// </summary>
        /// <remarks>The returned byte array represents the raw image data of the thumbnail.  Callers can
        /// process this data further as needed, such as converting it into an image format.</remarks>
        /// <returns>A byte array containing the decoded thumbnail image data.  Returns an empty array if no thumbnail data is
        /// available.</returns>
        public byte[] GetDecodedThumbnailData()
        {
            uint size = PdfThumbnailNative.FPDFPage_GetDecodedThumbnailData(_handle, IntPtr.Zero, 0);
            if (size == 0)
                return Array.Empty<byte>();

            var buffer = new byte[size];
            unsafe
            {
                fixed (byte* ptr = buffer)
                {
                    PdfThumbnailNative.FPDFPage_GetDecodedThumbnailData(_handle, (IntPtr)ptr, size);
                }
            }

            return buffer;
        }

        /// <summary>
        /// Retrieves the raw thumbnail data for the current PDF page.
        /// </summary>
        /// <remarks>The raw thumbnail data can be used for further processing or rendering.  The caller
        /// is responsible for interpreting the data appropriately.</remarks>
        /// <returns>A byte array containing the raw thumbnail data of the PDF page.  Returns an empty array if no thumbnail data
        /// is available.</returns>
        public byte[] GetRawThumbnailData()
        {
            uint size = PdfThumbnailNative.FPDFPage_GetRawThumbnailData(_handle, IntPtr.Zero, 0);
            if (size == 0)
                return Array.Empty<byte>();

            var buffer = new byte[size];
            unsafe
            {
                fixed (byte* ptr = buffer)
                {
                    PdfThumbnailNative.FPDFPage_GetRawThumbnailData(_handle, (IntPtr)ptr, size);
                }
            }

            return buffer;
        }

        /// <summary>
        /// Retrieves the thumbnail image of the current PDF page as a <see cref="PdfBitmap"/> object.
        /// </summary>
        /// <remarks>The returned <see cref="PdfBitmap"/> contains the thumbnail image of the current PDF
        /// page,  with its dimensions estimated based on the bitmap's stride and format.  If the thumbnail cannot be
        /// retrieved, the method returns <see langword="null"/>.</remarks>
        /// <returns>A <see cref="PdfBitmap"/> representing the thumbnail image of the current PDF page,  or <see
        /// langword="null"/> if no thumbnail is available.</returns>
        /// <exception cref="dotPDFiumException">Thrown if the thumbnail image is in an unsupported format.</exception>
        public PdfBitmap? GetThumbnailBitmap()
        {
            var handle = PdfThumbnailNative.FPDFPage_GetThumbnailAsBitmap(_handle);
            if (handle == IntPtr.Zero)
                return null;

            // Get stride and format to infer width/height
            int stride = PdfViewNative.FPDFBitmap_GetStride(handle);
            var format = (PdfBitmapFormat)PdfViewNative.FPDFBitmap_GetFormat(handle);

            // Estimate bytes per pixel from format
            int bpp = format switch
            {
                PdfBitmapFormat.Gray => 1,
                PdfBitmapFormat.BGR => 3,
                PdfBitmapFormat.BGRA => 4,
                _ => throw new dotPDFiumException($"Unsupported format: {format}")
            };

            int estimatedWidth = stride / bpp;
            int estimatedHeight = estimatedWidth > 0 ? 1 : 0; // best guess fallback

            return new PdfBitmap(handle, estimatedWidth, estimatedHeight);
        }

        /// <summary>
        /// Applies a transformation matrix to all annotations on the current PDF page.
        /// </summary>
        /// <remarks>This method modifies the appearance and positioning of annotations on the page by
        /// applying the specified transformation matrix. The transformation is applied directly to the annotations and
        /// does not affect the page content itself.</remarks>
        /// <param name="matrix">The transformation matrix to apply. Each element of the matrix represents a specific transformation
        /// component (e.g., scaling, rotation, translation).</param>
        public void TransformAnnotations(FsMatrix matrix)
        {
            PdfEditNative.FPDFPage_TransformAnnots(
                _handle,
                matrix.a, matrix.b,
                matrix.c, matrix.d,
                matrix.e, matrix.f);
        }

        /// <summary>
        /// Enumerates all link annotations on the current PDF page.
        /// </summary>
        /// <remarks>This method lazily enumerates the link annotations on the page. Each <see
        /// cref="PdfLinkAnnotation"/>  object represents a single link annotation and provides access to its properties
        /// and behaviors.</remarks>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="PdfLinkAnnotation"/> objects representing the link annotations
        /// found on the page. The enumeration will be empty if no link annotations are present.</returns>
        public IEnumerable<PdfLinkAnnotation> EnumerateLinks()
        {
            int pos = 0;
            while (PdfDocNative.FPDFLink_Enumerate(_handle, ref pos, out var link))
            {
                if (link != IntPtr.Zero)
                    yield return new PdfLinkAnnotation(link, this);
            }
        }

        /// <summary>
        /// Finds a link annotation at the specified point on the PDF page.
        /// </summary>
        /// <param name="x">The x-coordinate of the point, in the coordinate system of the PDF page.</param>
        /// <param name="y">The y-coordinate of the point, in the coordinate system of the PDF page.</param>
        /// <returns>A <see cref="PdfLinkAnnotation"/> object representing the link annotation at the specified point,  or <see
        /// langword="null"/> if no link annotation is found.</returns>
        public PdfLinkAnnotation? FindLinkAtPoint(double x, double y)
        {
            var link = PdfDocNative.FPDFLink_GetLinkAtPoint(_handle, x, y);
            return link == IntPtr.Zero ? null : new PdfLinkAnnotation(link, this);
        }

        /// <summary>
        /// Gets the z-order index of a link at the specified point on the page.
        /// </summary>
        /// <remarks>The z-order index determines the stacking order of links at a given point, with lower
        /// indices representing links that are visually on top. This method can be used to identify and interact with
        /// specific links in a document.</remarks>
        /// <param name="x">The x-coordinate of the point, in the page's coordinate system.</param>
        /// <param name="y">The y-coordinate of the point, in the page's coordinate system.</param>
        /// <returns>The zero-based z-order index of the link at the specified point, where a lower index indicates a link closer
        /// to the top of the z-order. Returns -1 if no link is found at the specified point.</returns>
        public int GetLinkZOrderAtPoint(double x, double y)
        {
            return PdfDocNative.FPDFLink_GetLinkZOrderAtPoint(_handle, x, y);
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
using nebulae.dotPDFium;
using nebulae.dotPDFium.Native;
using System.Runtime.InteropServices;
using System.Text;

namespace nebulae.dotPDFium;

public class PdfAnnotation : PdfObject
{
    protected readonly PdfPage _page;

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfAnnotation"/> class with the specified handle and associated
    /// page.
    /// </summary>
    /// <remarks>The <paramref name="handle"/> parameter is used to reference the underlying native PDF
    /// annotation object,  and the <paramref name="page"/> parameter links the annotation to a specific page in the PDF
    /// document.</remarks>
    /// <param name="handle">A pointer to the native PDF annotation object. Must be a valid, non-null handle.</param>
    /// <param name="page">The <see cref="PdfPage"/> object that this annotation is associated with. Cannot be null.</param>
    public PdfAnnotation(IntPtr handle, PdfPage page)
        : base(handle, PdfObjectType.Annotation)
    {
        _page = page;
    }

    /// <summary>
    /// Gets the subtype of the PDF annotation.
    /// </summary>
    /// <remarks>The subtype indicates the specific type of the annotation, such as text, link, or
    /// highlight.</remarks>
    public PdfAnnotationSubtype Subtype =>
        (PdfAnnotationSubtype)PdfAnnotNative.FPDFAnnot_GetSubtype(_handle);

    /// <summary>
    /// Adds a file attachment annotation to the PDF and returns a reference to the attached file.
    /// </summary>
    /// <remarks>The method creates a file attachment annotation in the PDF document and associates it with
    /// the specified name. If the operation fails, the method returns <see langword="null"/>.</remarks>
    /// <param name="name">The name of the file attachment to be added. This name is used to identify the attachment in the PDF.</param>
    /// <returns>A <see cref="PdfFileSpec"/> object representing the attached file if the operation is successful; otherwise,
    /// <see langword="null"/>.</returns>
    public PdfFileSpec? AddFileAttachment(string name)
    {
        IntPtr handle = PdfAnnotNative.FPDFAnnot_AddFileAttachment(_handle, name);
        return handle != IntPtr.Zero ? new PdfFileSpec(handle) : null;
    }

    /// <summary>
    /// Adds an ink stroke to the annotation using the specified collection of points.
    /// </summary>
    /// <remarks>This method adds a new ink stroke to the annotation by passing the provided points to the
    /// underlying PDF annotation system. The caller must ensure that the span contains valid points to define the
    /// stroke.</remarks>
    /// <param name="strokePoints">A read-only span of <see cref="FsPointF"/> representing the points that define the ink stroke. The span must
    /// contain at least one point.</param>
    /// <returns>An integer representing the result of the operation. The meaning of the return value depends on the underlying
    /// implementation.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="strokePoints"/> is empty.</exception>
    public int AddInkStroke(ReadOnlySpan<FsPointF> strokePoints)
    {
        if (strokePoints.Length == 0)
            throw new ArgumentException("Ink stroke must contain at least one point.");

        unsafe
        {
            fixed (FsPointF* ptr = strokePoints)
            {
                return PdfAnnotNative.FPDFAnnot_AddInkStroke(_handle, (IntPtr)ptr, (UIntPtr)strokePoints.Length);
            }
        }
    }

    /// <summary>
    /// Appends a specified PDF page object to the annotation.
    /// </summary>
    /// <remarks>The appended object becomes part of the annotation and will be rendered as part of it. Ensure
    /// that the <paramref name="obj"/> is properly initialized and compatible with the annotation before calling this
    /// method.</remarks>
    /// <param name="obj">The <see cref="PdfPageObject"/> to append. This object must be valid and associated with the same document as
    /// the annotation.</param>
    /// <returns><see langword="true"/> if the object was successfully appended; otherwise, <see langword="false"/>.</returns>
    public bool AppendObject(PdfPageObject obj)
    {
        return PdfAnnotNative.FPDFAnnot_AppendObject(_handle, obj.Handle);
    }


    /// <summary>
    /// Commits any changes made to the annotation.
    /// </summary>
    /// <remarks>This method finalizes modifications to the annotation and ensures that all changes are saved.
    /// After calling this method, the annotation handle is no longer valid and should not be used until or unless
    /// you call GetAnnotation() again.</remarks>
    public void CommitChanges()
    {
        PdfAnnotNative.FPDFPage_CloseAnnot(_handle);
    }

    /// <summary>
    /// Removes all ink strokes from the annotation.
    /// </summary>
    /// <remarks>This method clears the ink list associated with the annotation. If the annotation does not
    /// contain any ink strokes,  the method will return <see langword="true"/> without performing any action.</remarks>
    /// <returns><see langword="true"/> if all ink strokes were successfully removed; otherwise, <see langword="false"/>.</returns>
    public bool ClearInkStrokes()
    {
        return PdfAnnotNative.FPDFAnnot_RemoveInkList(_handle);
    }

    /// <summary>
    /// Retrieves the name of the appearance stream for the specified annotation appearance mode.
    /// </summary>
    /// <remarks>The appearance stream name is used to identify the visual representation of the annotation in
    /// the specified mode.</remarks>
    /// <param name="mode">The appearance mode for which to retrieve the stream name. This can be Normal, Rollover, or Down.</param>
    /// <returns>The name of the appearance stream as a string, or <see langword="null"/> if no appearance stream is defined for
    /// the specified mode.</returns>
    public string? GetAppearanceStreamName(PdfAnnotationAppearanceMode mode)
    {
        char[] buffer = new char[256];
        uint written = PdfAnnotNative.FPDFAnnot_GetAP(_handle, (int)mode, buffer, (uint)buffer.Length);

        return written == 0 ? null : new string(buffer, 0, (int)written - 1);
    }

    /// <summary>
    /// Appends an attachment point (quad) to the annotation.
    /// This is typically used for annotations like highlights, underlines, or squiggly lines.
    /// </summary>
    /// <param name="quad">The quadrilateral to attach to the annotation.</param>
    /// <returns><c>true</c> if the quad was successfully appended; otherwise, <c>false</c>.</returns>
    public bool AppendAttachmentPoints(FsQuadPointsF quad)
    {
        return PdfAnnotNative.FPDFAnnot_AppendAttachmentPoints(this.Handle, ref quad);
    }

    /// <summary>
    /// Retrieves the attachment point at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the attachment point to retrieve. Must be greater than or equal to 0.</param>
    /// <returns>An <see cref="FsQuadPointsF"/> structure representing the attachment point if found; otherwise, <see
    /// langword="null"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0.</exception>
    public FsQuadPointsF? GetAttachmentPoint(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (PdfAnnotNative.FPDFAnnot_GetAttachmentPoints(_handle, (UIntPtr)index, out FsQuadPointsF quad))
            return quad;

        return null;
    }

    /// <summary>
    /// Gets the number of attachment points associated with the annotation.
    /// </summary>
    /// <remarks>Attachment points are specific locations within the annotation that can be used for
    /// positioning or other purposes.</remarks>
    /// <returns>The total number of attachment points. Returns 0 if the annotation has no attachment points.</returns>
    public int GetAttachmentPointCount()
    {
        return (int)PdfAnnotNative.FPDFAnnot_CountAttachmentPoints(_handle);
    }

    /// <summary>
    /// Retrieves the border characteristics of the PDF annotation.
    /// </summary>
    /// <returns>A <see cref="PdfAnnotationBorder"/> object representing the horizontal radius, vertical radius,  and border
    /// width of the annotation, or <see langword="null"/> if the border information is unavailable.</returns>
    public PdfAnnotationBorder? GetBorder()
    {
        if (PdfAnnotNative.FPDFAnnot_GetBorder(_handle, out float hr, out float vr, out float width))
            return new PdfAnnotationBorder(hr, vr, width);

        return null;
    }

    /// <summary>
    /// Retrieves the file attachment associated with the annotation, if any.
    /// </summary>
    /// <returns>A <see cref="PdfFileSpec"/> object representing the file attachment, or <see langword="null"/> if no file
    /// attachment is associated with the annotation.</returns>
    public PdfFileSpec? GetFileAttachment()
    {
        IntPtr handle = PdfAnnotNative.FPDFAnnot_GetFileAttachment(_handle);
        return handle != IntPtr.Zero ? new PdfFileSpec(handle) : null;
    }

    /// <summary>
    /// Gets the number of ink strokes associated with the annotation.
    /// </summary>
    /// <remarks>This method retrieves the count of ink strokes defined in the annotation's ink list.  Use
    /// this method to determine how many individual strokes are part of the annotation.</remarks>
    /// <returns>The total number of ink strokes in the annotation. Returns 0 if no ink strokes are present.</returns>
    public int GetInkStrokeCount()
    {
        return (int)PdfAnnotNative.FPDFAnnot_GetInkListCount(_handle);
    }

    /// <summary>
    /// Retrieves the list of points that define the ink stroke for the specified path index.
    /// </summary>
    /// <remarks>This method retrieves the points for a specific ink stroke path associated with an
    /// annotation. The points are returned as a read-only list of <see cref="FsPointF"/> structures, which represent
    /// the coordinates of the ink stroke in the annotation's coordinate space.</remarks>
    /// <param name="pathIndex">The zero-based index of the ink stroke path to retrieve.</param>
    /// <returns>A read-only list of <see cref="FsPointF"/> objects representing the points in the ink stroke. Returns an empty
    /// list if the specified path index does not contain any points.</returns>
    public IReadOnlyList<FsPointF> GetInkStrokePoints(int pathIndex)
    {
        // First call with null to get point count
        uint count = PdfAnnotNative.FPDFAnnot_GetInkListPath(_handle, (uint)pathIndex, IntPtr.Zero, 0);
        if (count == 0)
            return Array.Empty<FsPointF>();

        int byteSize = (int)(count * Marshal.SizeOf<FsPointF>());
        IntPtr buffer = Marshal.AllocHGlobal(byteSize);

        try
        {
            uint written = PdfAnnotNative.FPDFAnnot_GetInkListPath(_handle, (uint)pathIndex, buffer, count);
            if (written == 0)
                return Array.Empty<FsPointF>();

            var points = new FsPointF[written];
            for (int i = 0; i < written; i++)
            {
                IntPtr entry = buffer + i * Marshal.SizeOf<FsPointF>();
                points[i] = Marshal.PtrToStructure<FsPointF>(entry);
            }

            return points;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// Retrieves the start and end points of the line annotation, if available.
    /// </summary>
    /// <returns>A tuple containing the start and end points of the line as <see cref="FsPointF"/> objects,  or <see
    /// langword="null"/> if the line annotation is not defined or cannot be retrieved.</returns>
    public (FsPointF Start, FsPointF End)? GetLine()
    {
        if (PdfAnnotNative.FPDFAnnot_GetLine(_handle, out FsPointF start, out FsPointF end))
            return (start, end);

        return null;
    }

    /// <summary>
    /// If this annotation is a link, retrieves the associated link object handle.
    /// </summary>
    /// <returns>The link handle if available, or <see cref="IntPtr.Zero"/> if not a link.</returns>
    public PdfLink? GetLink(PdfDocument context)
    {
        if (Subtype != PdfAnnotationSubtype.Link)
            return null;

        var handle = PdfAnnotNative.FPDFAnnot_GetLink(this.Handle);
        return handle == IntPtr.Zero ? null : new PdfLink(handle, context);
    }


    /// <summary>
    /// Retrieves the annotation linked to the specified key.
    /// </summary>
    /// <remarks>This method returns a nullable <see cref="IntPtr"/> to represent the linked annotation.  If
    /// no linked annotation is found, the method returns <see langword="null"/>.</remarks>
    /// <param name="key">The key identifying the linked annotation. Cannot be <see langword="null"/> or empty.</param>
    /// <returns>A pointer to the linked annotation if one exists; otherwise, <see langword="null"/>.</returns>
    public IntPtr? GetLinkedAnnotation(string key)
    {
        IntPtr linked = PdfAnnotNative.FPDFAnnot_GetLinkedAnnot(_handle, key);
        return linked != IntPtr.Zero ? linked : null;
    }

    /// <summary>
    /// Retrieves the numeric value for a given dictionary key on the annotation.
    /// </summary>
    /// <param name="key">The dictionary key (e.g. "F", "CA", etc.).</param>
    /// <returns>The float value if the key is present, or <c>null</c> if not found or not numeric.</returns>
    public float? GetNumberValue(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        if (PdfAnnotNative.FPDFAnnot_GetNumberValue(Handle, key, out float value))
            return value;

        return null;
    }

    /// <summary>
    /// Retrieves the string value associated with the specified key from the annotation.
    /// </summary>
    /// <remarks>The returned string is decoded from a null-terminated UTF-8 byte array. If the key is not
    /// found or the retrieval fails, the method returns <see langword="null"/>.</remarks>
    /// <param name="key">The key identifying the string value to retrieve. This parameter cannot be <see langword="null"/> or whitespace.</param>
    /// <returns>The string value associated with the specified key, or <see langword="null"/> if the key does not exist or the
    /// value cannot be retrieved.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is <see langword="null"/> or consists only of whitespace.</exception>
    public string? GetStringValue(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        // First call to get size
        uint size = 0;
        var dummy = new byte[0];
        PdfAnnotNative.FPDFAnnot_GetStringValue(this.Handle, key, dummy, 0);

        size = 256; // fallback default buffer size
        var buffer = new byte[size];

        if (!PdfAnnotNative.FPDFAnnot_GetStringValue(this.Handle, key, buffer, size))
            return null;

        // PDFium returns null-terminated UTF-8 string
        int actualLength = Array.IndexOf(buffer, (byte)0);
        if (actualLength < 0) actualLength = buffer.Length;

        return Encoding.UTF8.GetString(buffer, 0, actualLength);
    }

    /// <summary>
    /// Retrieves the <see cref="PdfPageObject"/> at the specified index within the annotation.
    /// </summary>
    /// <param name="index">The zero-based index of the object to retrieve. Must be within the valid range of objects in the annotation.</param>
    /// <returns>A <see cref="PdfPageObject"/> representing the object at the specified index, or <see langword="null"/> if no
    /// object exists at the given index.</returns>
    public PdfPageObject? GetObject(int index)
    {
        IntPtr objHandle = PdfAnnotNative.FPDFAnnot_GetObject(_handle, index);
        return objHandle != IntPtr.Zero ? new PdfPageObject(objHandle) : null;
    }

    /// <summary>
    /// Gets the total number of objects associated with the annotation.
    /// </summary>
    /// <remarks>This method retrieves the count of objects linked to the annotation represented by the
    /// current instance.</remarks>
    /// <returns>The number of objects associated with the annotation. Returns 0 if no objects are present.</returns>
    public int GetObjectCount()
    {
        return PdfAnnotNative.FPDFAnnot_GetObjectCount(_handle);
    }

    /// <summary>
    /// Retrieves the subtype of the PDF annotation represented by this instance.
    /// </summary>
    /// <returns>A <see cref="PdfAnnotationSubtype"/> value representing the annotation's subtype.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the annotation subtype is not recognized or is not defined in the <see cref="PdfAnnotationSubtype"/>
    /// enumeration.</exception>
    public PdfAnnotationSubtype GetSubtype()
    {
        int subtype = PdfAnnotNative.FPDFAnnot_GetSubtype(_handle);
        return Enum.IsDefined(typeof(PdfAnnotationSubtype), subtype)
            ? (PdfAnnotationSubtype)subtype
            : throw new dotPDFiumException($"Unknown annotation subtype: {subtype}");
    }

    /// <summary>
    /// Retrieves the type of the value associated with the specified key in the PDF annotation.
    /// </summary>
    /// <remarks>This method queries the underlying PDF annotation to determine the type of the value
    /// associated with the given key. Ensure that the key exists in the annotation to avoid unexpected
    /// results.</remarks>
    /// <param name="key">The key identifying the value whose type is to be retrieved. Cannot be <see langword="null"/> or empty.</param>
    /// <returns>A <see cref="PdfValueType"/> representing the type of the value associated with the specified key.</returns>
    public PdfValueType GetValueType(string key)
    {
        int type = PdfAnnotNative.FPDFAnnot_GetValueType(_handle, key);
        return (PdfValueType)type;
    }

    /// <summary>
    /// Retrieves the vertices of the annotation as a read-only list of points.
    /// </summary>
    /// <remarks>This method returns the vertices of the annotation, represented as a collection of <see
    /// cref="FsPointF"/> structures. If the annotation does not have any vertices, an empty list is returned.</remarks>
    /// <returns>A read-only list of <see cref="FsPointF"/> representing the vertices of the annotation. The list will be empty
    /// if no vertices are defined.</returns>
    public IReadOnlyList<FsPointF> GetVertices()
    {
        uint count = PdfAnnotNative.FPDFAnnot_GetVertices(_handle, IntPtr.Zero, 0);
        if (count == 0)
            return Array.Empty<FsPointF>();

        int size = (int)(count * Marshal.SizeOf<FsPointF>());
        IntPtr buffer = Marshal.AllocHGlobal(size);

        try
        {
            uint written = PdfAnnotNative.FPDFAnnot_GetVertices(_handle, buffer, count);
            if (written == 0)
                return Array.Empty<FsPointF>();

            var points = new FsPointF[written];
            for (int i = 0; i < written; i++)
            {
                IntPtr entry = buffer + i * Marshal.SizeOf<FsPointF>();
                points[i] = Marshal.PtrToStructure<FsPointF>(entry);
            }

            return points;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// Determines whether the annotation has attachment points.
    /// </summary>
    /// <remarks>Attachment points are specific locations associated with the annotation, such as coordinates
    /// or markers, that may be used for positioning or rendering purposes.</remarks>
    /// <returns><see langword="true"/> if the annotation has attachment points; otherwise, <see langword="false"/>.</returns>
    public bool HasAttachmentPoints()
    {
        return PdfAnnotNative.FPDFAnnot_HasAttachmentPoints(_handle);
    }

    /// <summary>
    /// Determines whether the annotation contains a specific key.
    /// </summary>
    /// <param name="key">The name of the key to check for. This value cannot be <see langword="null"/> or empty.</param>
    /// <returns><see langword="true"/> if the annotation contains the specified key; otherwise, <see langword="false"/>.</returns>
    public bool HasKey(string key)
    {
        return PdfAnnotNative.FPDFAnnot_HasKey(_handle, key);
    }

    /// <summary>
    /// Determines whether the specified annotation subtype is supported.
    /// </summary>
    /// <param name="subtype">The annotation subtype to check for support.</param>
    /// <returns><see langword="true"/> if the specified annotation subtype is supported; otherwise, <see langword="false"/>.</returns>
    public static bool IsObjectSupportedSubtype(PdfAnnotationSubtype subtype)
    {
        return PdfAnnotNative.FPDFAnnot_IsObjectSupportedSubtype((int)subtype);
    }


    /// <summary>
    /// Determines whether the annotation's subtype is supported.
    /// </summary>
    /// <remarks>This method checks if the current annotation's subtype is recognized and supported by the
    /// underlying system.</remarks>
    /// <returns><see langword="true"/> if the annotation's subtype is supported; otherwise, <see langword="false"/>.</returns>
    public bool IsSupportedSubtype()
    {
        return PdfAnnotNative.FPDFAnnot_IsSupportedSubtype((int)GetSubtype());
    }

    /// <summary>
    /// Retrieves the rectangular bounds of the annotation.
    /// </summary>
    /// <returns>A <see cref="FsRectF"/> structure representing the annotation's rectangular bounds.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the annotation rectangle could not be retrieved.</exception>
    public FsRectF GetRect()
    {
        if (!PdfAnnotNative.FPDFAnnot_GetRect(_handle, out FsRectF rect))
            throw new dotPDFiumException($"Failed to get annotation rectangle: {PdfObject.GetPDFiumError()}");

        return rect;
    }

    /// <summary>
    /// Sets the rectangular bounds of the annotation.
    /// </summary>
    /// <param name="rect">The rectangle defining the bounds of the annotation. Must be a valid <see cref="FsRectF"/> structure.</param>
    /// <exception cref="dotPDFiumException">Thrown if the rectangle could not be set due to an internal error.</exception>
    public void SetRect(FsRectF rect)
    {
        if (!PdfAnnotNative.FPDFAnnot_SetRect(_handle, ref rect))
            throw new dotPDFiumException($"Failed to set annotation rectangle: {PdfObject.GetPDFiumError()}");
    }

    /// <summary>
    /// Retrieves the color of the annotation as an RGBA value.
    /// </summary>
    /// <returns>An <see cref="RgbaColor"/> representing the red, green, blue, and alpha components of the annotation's color.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the annotation color cannot be retrieved.</exception>
    public RgbaColor GetColor()
    {
        if (!PdfAnnotNative.FPDFAnnot_GetColor(_handle, (int)PdfAnnotationColorType.Color, out uint r, out uint g, out uint b, out uint a))
            throw new dotPDFiumException($"Failed to get annotation color: {PdfObject.GetPDFiumError()}");

        return new RgbaColor((byte)r, (byte)g, (byte)b, (byte)a);
    }

    /// <summary>
    /// Sets the color of the annotation using the specified RGBA color.
    /// </summary>
    /// <param name="color">The <see cref="RgbaColor"/> structure representing the red, green, blue, and alpha components of the color to
    /// apply to the annotation.</param>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails to set the annotation color.</exception>
    public void SetColor(RgbaColor color)
    {
        if (!PdfAnnotNative.FPDFAnnot_SetColor(_handle, (int)PdfAnnotationColorType.Color, color.R, color.G, color.B, color.A))
            throw new dotPDFiumException($"Failed to set annotation color: {PdfObject.GetPDFiumError()}");
    }

    /// <summary>
    /// Removes an object at the specified index from the annotation.
    /// </summary>
    /// <remarks>This method attempts to remove an object from the annotation at the specified index.  If the
    /// index is invalid or the removal operation fails, the method returns <see langword="false"/>.</remarks>
    /// <param name="index">The zero-based index of the object to remove.</param>
    /// <returns><see langword="true"/> if the object was successfully removed; otherwise, <see langword="false"/>.</returns>
    public bool RemoveObject(int index)
    {
        return PdfAnnotNative.FPDFAnnot_RemoveObject(_handle, index);
    }

    /// <summary>
    /// Sets the appearance stream for the annotation based on the specified mode and name.
    /// </summary>
    /// <param name="mode">The appearance mode to set, such as normal, rollover, or down.</param>
    /// <param name="name">The name of the appearance stream to apply. This value cannot be null or empty.</param>
    /// <returns><see langword="true"/> if the appearance stream was successfully set; otherwise, <see langword="false"/>.</returns>
    public bool SetAppearanceStream(PdfAnnotationAppearanceMode mode, string name)
    {
        return PdfAnnotNative.FPDFAnnot_SetAP(_handle, (int)mode, name);
    }

    /// <summary>
    /// Sets the attachment points for the annotation at the specified index.
    /// </summary>
    /// <remarks>The attachment points define the area associated with the annotation. Ensure that the
    /// <paramref name="quad"/> parameter is properly initialized before calling this method.</remarks>
    /// <param name="index">The zero-based index of the attachment point to set.</param>
    /// <param name="quad">The quadrilateral points defining the attachment area.</param>
    /// <returns><see langword="true"/> if the attachment points were successfully set; otherwise, <see langword="false"/>.</returns>
    public bool SetAttachmentPoints(int index, FsQuadPointsF quad)
    {
        return PdfAnnotNative.FPDFAnnot_SetAttachmentPoints(_handle, (UIntPtr)index, ref quad);
    }

    /// <summary>
    /// Sets the border properties for the annotation, including corner radii and border width.
    /// </summary>
    /// <remarks>This method modifies the visual appearance of the annotation by setting its border
    /// properties. Ensure that the annotation handle is valid before calling this method.</remarks>
    /// <param name="horizontalRadius">The horizontal radius of the annotation's border corners. Must be non-negative.</param>
    /// <param name="verticalRadius">The vertical radius of the annotation's border corners. Must be non-negative.</param>
    /// <param name="borderWidth">The width of the annotation's border. Must be non-negative.</param>
    /// <returns><see langword="true"/> if the border properties were successfully set; otherwise, <see langword="false"/>.</returns>
    public bool SetBorder(float horizontalRadius, float verticalRadius, float borderWidth)
    {
        return PdfAnnotNative.FPDFAnnot_SetBorder(_handle, horizontalRadius, verticalRadius, borderWidth);
    }

    /// <summary>
    /// Sets the URI associated with the annotation.
    /// </summary>
    /// <param name="uri">The URI to associate with the annotation. Must not be null, empty, or consist only of whitespace.</param>
    /// <returns><see langword="true"/> if the URI was successfully set; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="uri"/> is null, empty, or consists only of whitespace.</exception>
    public bool SetUri(string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
            throw new ArgumentException("URI must not be null or empty.", nameof(uri));

        return PdfAnnotNative.FPDFAnnot_SetURI(_handle, uri);
    }

    /// <summary>
    /// Updates the specified PDF page object within the annotation.
    /// </summary>
    /// <param name="obj">The <see cref="PdfPageObject"/> to update. This parameter cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the object was successfully updated; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is <see langword="null"/>.</exception>
    public bool UpdateObject(PdfPageObject obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        return PdfAnnotNative.FPDFAnnot_UpdateObject(_handle, obj.Handle);
    }

}

using nebulae.dotPDFium.Forms;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

/// <summary>
/// Represents a page object within a PDF document, such as text, images, or shapes.
/// </summary>
/// <remarks>This class provides methods to manipulate and retrieve information about a specific page object. Page
/// objects are elements that make up the content of a PDF page, and they can be transformed or queried for their
/// properties, such as bounds.</remarks>
public class PdfPageObject : PdfObject
{
    private readonly PdfPageObjectType _objType;
    protected bool _hasOwner = false;
    internal bool IsOwned => _hasOwner;

    /// <summary>
    /// Constructor for creating a new PdfPageObject instance. This constructor is internal and should not be used
    /// </summary>
    /// <param name="handle"></param>
    internal PdfPageObject(IntPtr handle) : base(handle, PdfObjectType.PageObject)
    {
        _objType = PdfPageObjectType.Unknown;
    }

    /// <summary>
    /// Creates a new instance of a <see cref="PdfPageObject"/> or one of its derived types based on the specified
    /// handle.
    /// </summary>
    /// <param name="handle">A pointer to the native PDF page object. Must not be <see cref="IntPtr.Zero"/>.</param>
    /// <returns>A <see cref="PdfPageObject"/> instance or a derived type, such as <see cref="PdfTextObject"/>,  <see
    /// cref="PdfImageObject"/>, <see cref="PdfPathObject"/>, or <see cref="PdfFormXObject"/>,  depending on the type of
    /// the PDF page object represented by the handle.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="handle"/> is <see cref="IntPtr.Zero"/>.</exception>
    internal static PdfPageObject Create(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            throw new ArgumentNullException(nameof(handle));

        var type = (PdfPageObjectType)PdfEditNative.FPDFPageObj_GetType(handle);

        return type switch
        {
            PdfPageObjectType.Text => new PdfTextObject(handle),
            PdfPageObjectType.Image => new PdfImageObject(handle),
            PdfPageObjectType.Path => new PdfPathObject(handle),
            _ => new PdfPageObject(handle, type)
        };
    }

    /// <summary>
    /// Constructor for creating a new PdfPageObject instance with a specific handle and type. This constructor is internal and
    /// should not be used directly.
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="type"></param>
    internal PdfPageObject(IntPtr handle, PdfPageObjectType type)
        : base(handle, PdfObjectType.PageObject)
    {
        _objType = type;
    }

    /// <summary>
    /// Marks the current instance as owned.
    /// </summary>
    /// <remarks>This method sets the ownership state of the instance. Once marked as owned,  the instance
    /// cannot be unmarked through this method.</remarks>
    internal void MarkOwned()
    {
        _hasOwner = true;
    }

    /// <summary>
    /// Gets the marked content ID (MCID) associated with this page object.
    /// </summary>
    /// <remarks>
    /// The MCID is used in tagged/structured PDFs to associate content with logical structure.
    /// Returns -1 if no MCID is present.
    /// </remarks>
    /// <returns>The marked content ID, or -1 if none is associated.</returns>
    public int GetMarkedContentId()
    {
        return PdfEditNative.FPDFPageObj_GetMarkedContentID(this.Handle);
    }

    /// <summary>
    /// Transforms a page object using a specified transformation matrix. The transformation matrix is defined by the
    /// six parameters: a, b, c, d, e, and f. This method modifies the object's position and size on the page.
    /// </summary>
    /// <param name="matrix"></param>
    public void Transform(FsMatrix matrix)
    {
        PdfEditNative.FPDFPageObj_Transform(_handle, matrix.a, matrix.b, matrix.c, matrix.d, matrix.e, matrix.f);
    }

    /// <summary>
    /// Sets the x, y position of the page object. x is the horizontal distance from the left edge of
    /// the page and y is the vertical distance from the bottom edge of the page.
    /// </summary>
    /// <param name="x">The horizontal distance from the left edge of the page</param>
    /// <param name="y">The vertical distance from the bottom edge of the page</param>
    public void SetPosition(double x, double y)
    {
        // Pure translation matrix: no scaling, rotation, or skew
        Transform(new FsMatrix
        {
            a = 1,
            b = 0,
            c = 0,
            d = 1,
            e = x,
            f = y
        });
    }

    /// <summary>
    /// Scales the object by the specified factors in the x and y directions. The scaling is done relative to the
    /// original position of the object. The parameters sx and sy are the scaling factors in the x and y directions,
    /// </summary>
    /// <param name="sx"></param>
    /// <param name="sy"></param>
    public void ScaleBy(double sx, double sy)
    {
        Transform(new FsMatrix
        {
            a = sx,
            b = 0,
            c = 0,
            d = sy,
            e = 0,
            f = 0
        });
    }

    /// <summary>
    /// Scales the object to fit within the specified width and height. The scaling is done relative to the
    /// original position of the object. The parameters targetWidth and targetHeight are the desired dimensions
    /// </summary>
    /// <param name="targetWidth">Desired width</param>
    /// <param name="targetHeight">Desired height</param>
    /// <exception cref="InvalidOperationException">Thrown when the object bounds are zero</exception>
    public void ScaleTo(double targetWidth, double targetHeight)
    {
        FsRectF bounds = GetBounds();

        float currentWidth = bounds.right - bounds.left;
        float currentHeight = bounds.bottom - bounds.top;

        if (currentWidth == 0f || currentHeight == 0f)
            throw new InvalidOperationException("Cannot scale an object with zero size.");

        double scaleX = targetWidth / currentWidth;
        double scaleY = targetHeight / currentHeight;

        ScaleBy(scaleX, scaleY);
    }

    /// <summary>
    /// Transforms a page object using a specified transformation matrix. The transformation matrix is defined by the
    /// six parameters: a, b, c, d, e, and f. This method modifies the object's position and size on the page.
    /// </summary>
    /// <param name="matrix"></param>
    public void TransformF(FsMatrixF matrix)
    {
        PdfEditNative.FPDFPageObj_TransformF(_handle, ref matrix);
    }

    /// <summary>
    /// Sets the x, y position of the page object. x is the horizontal distance from the left edge of
    /// the page and y is the vertical distance from the bottom edge of the page.
    /// </summary>
    /// <param name="x">The horizontal distance from the left edge of the page</param>
    /// <param name="y">The vertical distance from the bottom edge of the page</param>
    public void SetPositionF(float x, float y)
    {
        // Pure translation matrix: no scaling, rotation, or skew
        TransformF(new FsMatrixF
        {
            a = 1,
            b = 0,
            c = 0,
            d = 1,
            e = x,
            f = y
        });
    }

    /// <summary>
    /// Scales the object by the specified factors in the x and y directions. The scaling is done relative to the
    /// original position of the object. The parameters sx and sy are the scaling factors in the x and y directions,
    /// </summary>
    /// <param name="sx"></param>
    /// <param name="sy"></param>
    public void ScaleByF(float sx, float sy)
    {
        TransformF(new FsMatrixF
        {
            a = sx,
            b = 0,
            c = 0,
            d = sy,
            e = 0,
            f = 0
        });
    }

    /// <summary>
    /// Scales the object to fit within the specified width and height. The scaling is done relative to the
    /// original position of the object. The parameters targetWidth and targetHeight are the desired dimensions
    /// </summary>
    /// <param name="targetWidth">Desired width</param>
    /// <param name="targetHeight">Desired height</param>
    /// <exception cref="InvalidOperationException">Thrown when the object bounds are zero</exception>
    public void ScaleToF(float targetWidth, float targetHeight)
    {
        FsRectF bounds = GetBounds();

        float currentWidth = bounds.right - bounds.left;
        float currentHeight = bounds.bottom - bounds.top;

        if (currentWidth == 0f || currentHeight == 0f)
            throw new InvalidOperationException("Cannot scale an object with zero size.");

        float scaleX = targetWidth / currentWidth;
        float scaleY = targetHeight / currentHeight;

        ScaleByF(scaleX, scaleY);
    }

    /// <summary>
    /// Determines whether the current page object is active.
    /// </summary>
    /// <remarks>This method checks the active state of the page object associated with the current instance.
    /// If the underlying operation fails, the method returns <see langword="false"/>.</remarks>
    /// <returns><see langword="true"/> if the page object is active; otherwise, <see langword="false"/>.</returns>
    public bool GetIsActive()
    {
        var retVal = PdfEditNative.FPDFPageObj_GetIsActive(_handle, out bool active);

        if (!retVal)
            return false;

        return active;
    }

    /// <summary>
    /// Sets the active state of the PDF page object.
    /// </summary>
    /// <remarks>The active state of a page object may influence its visibility or behavior in certain
    /// contexts. Ensure that the appropriate state is set based on the desired outcome.</remarks>
    /// <param name="active"><see langword="true"/> to activate the page object; <see langword="false"/> to deactivate it.</param>
    public void SetIsActive(bool active)
    {
        PdfEditNative.FPDFPageObj_SetIsActive(_handle, active);
    }

    /// <summary>
    /// Gets the bounds of the page object. The bounds are represented as a rectangle defined by its left, bottom, right, and top.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="dotPDFiumException"></exception>
    public FsRectF GetBounds()
    {
        if (!PdfEditNative.FPDFPageObj_GetBounds(_handle, out float left, out float bottom, out float right, out float top))
            throw new dotPDFiumException($"Failed to get object bounds: {PdfObject.GetPDFiumError()}");

        return new FsRectF(left, top, right, bottom);
    }

    /// <summary>
    /// Retrieves the fill color of the PDF page object.
    /// </summary>
    /// <returns>An <see cref="RgbaColor"/> structure representing the fill color of the object,  including red, green, blue, and
    /// alpha channel values.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the fill color cannot be retrieved due to an error in the underlying PDF library.</exception>
    public RgbaColor GetFillColor()
    {
        if (!PdfEditNative.FPDFPageObj_GetFillColor(_handle, out uint r, out uint g, out uint b, out uint a))
            throw new dotPDFiumException($"Failed to get fill color: {PdfObject.GetPDFiumError()}");

        return new RgbaColor((byte)r, (byte)g, (byte)b, (byte)a);
    }

    public void SetFillColor(RgbaColor color)
    {
        PdfEditNative.FPDFPageObj_SetFillColor(_handle, color.R, color.G, color.B, color.A);
    }

    /// <summary>
    /// Retrieves the transformation matrix associated with the current PDF object.
    /// </summary>
    /// <returns>An <see cref="FsMatrixF"/> representing the transformation matrix of the PDF object.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the transformation matrix cannot be retrieved due to an error in the underlying PDF library.</exception>
    public FsMatrixF GetMatrixF()
    {
        if (!PdfEditNative.FPDFPageObj_GetMatrix(_handle, out FsMatrixF matrix))
            throw new dotPDFiumException($"Failed to get object matrix: {PdfObject.GetPDFiumError()}");

        return matrix;
    }

    /// <summary>
    /// Sets the transformation matrix for the current PDF page object.
    /// </summary>
    /// <param name="matrix">The transformation matrix to apply. This defines how the page object is scaled, rotated, or translated.</param>
    /// <exception cref="dotPDFiumException">Thrown if the matrix could not be applied. The exception message will contain additional details about the
    /// failure.</exception>
    public void SetMatrix(FsMatrixF matrix)
    {
        if (!PdfEditNative.FPDFPageObj_SetMatrix(
                _handle,
                ref matrix))
        {
            throw new dotPDFiumException($"Failed to set matrix: {PdfObject.GetPDFiumError()}");
        }
    }

    /// <summary>
    /// Retrieves the rotated bounding box of the current PDF page object.
    /// </summary>
    /// <returns>A <see cref="FsQuadPointsF"/> structure representing the four corners of the rotated bounding box.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails to retrieve the rotated bounds.</exception>
    public FsQuadPointsF GetRotatedBounds()
    {
        if (!PdfEditNative.FPDFPageObj_GetRotatedBounds(_handle, out FsQuadPointsF quad))
            throw new dotPDFiumException($"Failed to get rotated bounds: {PdfObject.GetPDFiumError()}");

        return quad;
    }

    /// <summary>
    /// Retrieves the stroke color of the current PDF page object.
    /// </summary>
    /// <returns>An <see cref="RgbaColor"/> structure representing the stroke color,  including red, green, blue, and alpha
    /// channel values.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the stroke color cannot be retrieved due to an error in the underlying PDF library.</exception>
    public RgbaColor GetStrokeColor()
    {
        if (!PdfEditNative.FPDFPageObj_GetStrokeColor(_handle, out uint r, out uint g, out uint b, out uint a))
            throw new dotPDFiumException($"Failed to get stroke color: {PdfObject.GetPDFiumError()}");

        return new RgbaColor((byte)r, (byte)g, (byte)b, (byte)a);
    }

    /// <summary>
    /// Sets the stroke color for the current PDF page object.
    /// </summary>
    /// <remarks>The stroke color determines the color used for drawing the outline of shapes or paths in the
    /// PDF page object. Ensure that the <paramref name="color"/> parameter contains valid RGBA values, where each
    /// component is in the range of 0 to 255.</remarks>
    /// <param name="color">The <see cref="RgbaColor"/> structure representing the stroke color, including red, green, blue, and alpha
    /// components.</param>
    public void SetStrokeColor(RgbaColor color)
    {
        PdfEditNative.FPDFPageObj_SetStrokeColor(_handle, color.R, color.G, color.B, color.A);
    }

    /// <summary>
    /// Retrieves the stroke width of the current PDF page object.
    /// </summary>
    /// <returns>The stroke width of the PDF page object as a <see cref="float"/>.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the stroke width cannot be retrieved due to an error in the underlying PDF library.</exception>
    public float GetStrokeWidth()
    {
        if (!PdfEditNative.FPDFPageObj_GetStrokeWidth(_handle, out float width))
            throw new dotPDFiumException($"Failed to get stroke width: {PdfObject.GetPDFiumError()}");

        return width;
    }

    /// <summary>
    /// Sets the stroke width for the current PDF page object.
    /// </summary>
    /// <param name="width">The width of the stroke, in points. Must be a non-negative value.</param>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails due to an error in the underlying PDF library.</exception>
    public void SetStrokeWidth(float width)
    {
        if (!PdfEditNative.FPDFPageObj_SetStrokeWidth(_handle, width))
            throw new dotPDFiumException($"Failed to set stroke width: {PdfObject.GetPDFiumError()}");
    }

    /// <summary>
    /// Determines whether the PDF page object has transparency.
    /// </summary>
    /// <remarks>This method checks for the presence of transparency in the PDF page object, which may affect
    /// rendering or printing.</remarks>
    /// <returns><see langword="true"/> if the PDF page object contains transparent elements; otherwise, <see langword="false"/>.</returns>
    public bool HasTransparency()
    {
        return PdfEditNative.FPDFPageObj_HasTransparency(_handle);
    }

    /// <summary>
    /// Retrieves the handle to the clip path associated with the current page object.
    /// </summary>
    /// <returns>A nullable <see cref="IntPtr"/> representing the handle to the clip path.  Returns <see langword="null"/> if no
    /// clip path is associated with the page object.</returns>
    public IntPtr? GetClipPath()
    {
        var handle = PdfTransformPageNative.FPDFPageObj_GetClipPath(_handle);
        return handle == IntPtr.Zero ? null : handle;
    }

    /// <summary>
    /// Applies a transformation to the clip path of the current page object using the specified matrix.
    /// </summary>
    /// <remarks>This method modifies the clip path of the page object by applying the given transformation
    /// matrix.  Ensure that the matrix is properly defined to achieve the desired transformation. </remarks>
    /// <param name="matrix">The transformation matrix to apply. The matrix defines how the clip path should be scaled, rotated,  translated,
    /// or skewed. Each component of the matrix must be specified.</param>
    public void TransformClipPath(FsMatrix matrix)
    {
        PdfTransformPageNative.FPDFPageObj_TransformClipPath(
            _handle, matrix.a, matrix.b, matrix.c, matrix.d, matrix.e, matrix.f);
    }

    /// <summary>
    /// Adds a mark with the specified tag to the PDF page object.
    /// </summary>
    /// <param name="tag">The tag identifying the mark to be added. Cannot be null, empty, or consist only of whitespace.</param>
    /// <returns>A <see cref="PdfMark"/> instance representing the newly added mark.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="tag"/> is null, empty, or consists only of whitespace.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the mark could not be added to the page object.</exception>
    public PdfMark AddMark(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            throw new ArgumentException("Mark tag cannot be null or empty.", nameof(tag));

        var handle = PdfEditNative.FPDFPageObj_AddMark(_handle, tag);
        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to add mark '{tag}' to page object.");

        return new PdfMark(handle);
    }

    /// <summary>
    /// Removes the specified mark from the PDF page object.
    /// </summary>
    /// <param name="mark">The <see cref="PdfMark"/> to be removed. Must not be <see langword="null"/>.</param>
    /// <exception cref="dotPDFiumException">Thrown if the mark could not be removed from the page object.</exception>
    public void RemoveMark(PdfMark mark)
    {
        if (!PdfEditNative.FPDFPageObj_RemoveMark(_handle, mark.Handle))
            throw new dotPDFiumException("Failed to remove mark from page object.");
    }

    /// <summary>
    /// Gets the number of marks associated with the current PDF page object.
    /// </summary>
    /// <remarks>Marks are metadata or annotations associated with a PDF page object. This method retrieves
    /// the count of such marks.</remarks>
    /// <returns>The total number of marks present in the PDF page object. Returns 0 if no marks are associated.</returns>
    public int GetMarkCount()
    {
        return PdfEditNative.FPDFPageObj_CountMarks(_handle);
    }

    /// <summary>
    /// Retrieves the <see cref="PdfMark"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the mark to retrieve. Must be within the range of available marks.</param>
    /// <returns>The <see cref="PdfMark"/> at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is less than 0 or greater than or equal to the total number of marks.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the mark cannot be retrieved due to an internal error.</exception>
    public PdfMark GetMark(int index)
    {
        if (index < 0 || index >= GetMarkCount())
            throw new ArgumentOutOfRangeException(nameof(index));

        var handle = PdfEditNative.FPDFPageObj_GetMark(_handle, (uint)index);
        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to get mark at index {index}.");

        return new PdfMark(handle);
    }

    /// <summary>
    /// The dispose method for the PdfPageObject class. This method is called to release the resources used by this
    /// object. It overrides the base class Dispose method to ensure that the native handle is properly destroyed.
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        if (!_hasOwner)
            PdfEditNative.FPDFPageObj_Destroy(_handle);

        base.Dispose(disposing);
    }
}

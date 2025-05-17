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
    /// Gets the bounds of the page object. The bounds are represented as a rectangle defined by its left, bottom, right, and top.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="dotPDFiumException"></exception>
    public FsRectF GetBounds()
    {
        if (!PdfEditNative.FPDFPageObj_GetBounds(_handle, out float left, out float bottom, out float right, out float top))
            throw new dotPDFiumException("Failed to get object bounds.");

        return new FsRectF(left, top, right, bottom);
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

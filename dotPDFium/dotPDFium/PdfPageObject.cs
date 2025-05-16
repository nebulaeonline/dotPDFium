using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

/// <summary>
/// Specifies the type of an object on a PDF page.
/// </summary>
/// <remarks>This enumeration is used to identify the category of a PDF page object, such as text, images, or
/// paths. It can be useful for processing or analyzing the content of a PDF document.</remarks>
public enum  PdfPageObjectType
{
    Unknown = 0,
    Text,
    Path,
    Image,
    Shading,
    Form,
}

/// <summary>
/// Represents a page object within a PDF document, such as text, images, or shapes.
/// </summary>
/// <remarks>This class provides methods to manipulate and retrieve information about a specific page object. Page
/// objects are elements that make up the content of a PDF page, and they can be transformed or queried for their
/// properties, such as bounds.</remarks>
public class PdfPageObject : PdfObject
{
    private readonly PdfPageObjectType _objType;

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
    /// Transforms a page object using a specified transformation matrix. The transformation matrix is defined by the
    /// six parameters: a, b, c, d, e, and f. This method modifies the object's position and size on the page.
    /// </summary>
    /// <param name="matrix"></param>
    public void Transform(FS_MATRIX matrix)
    {
        PdfEditNative.FPDFPageObj_Transform(_handle, matrix.a, matrix.b, matrix.c, matrix.d, matrix.e, matrix.f);
    }

    /// <summary>
    /// Gets the bounds of the page object. The bounds are represented as a rectangle defined by its left, bottom, right, and top.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="dotPDFiumException"></exception>
    public FS_RECTF GetBounds()
    {
        if (!PdfEditNative.FPDFPageObj_GetBounds(_handle, out float left, out float bottom, out float right, out float top))
            throw new dotPDFiumException("Failed to get object bounds.");

        return new FS_RECTF(left, top, right, bottom);
    }

    /// <summary>
    /// The dispose method for the PdfPageObject class. This method is called to release the resources used by this
    /// object. It overrides the base class Dispose method to ensure that the native handle is properly destroyed.
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        PdfEditNative.FPDFPageObj_Destroy(_handle);
        base.Dispose(disposing);
    }
}

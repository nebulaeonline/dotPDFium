using nebulae.dotPDFium;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

public class PdfAnnotation : PdfObject
{
    protected readonly PdfPage _page;

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
        if (!PdfAnnotNative.FPDFAnnot_GetColor(_handle, (int)PdfAnnotColorType.Color, out uint r, out uint g, out uint b, out uint a))
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
        if (!PdfAnnotNative.FPDFAnnot_SetColor(_handle, (int)PdfAnnotColorType.Color, color.R, color.G, color.B, color.A))
            throw new dotPDFiumException($"Failed to set annotation color: {PdfObject.GetPDFiumError()}");
    }

}

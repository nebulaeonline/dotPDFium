using nebulae.dotPDFium;
using nebulae.dotPDFium.Forms;
using nebulae.dotPDFium.Native;

public enum PdfFormElementType
{
    Unknown = -1,
    PushButton,
    CheckBox,
    RadioButton,
    ComboBox,
    ListBox,
    TextField,
    SignatureField,
}

public class PdfFormElement : IDisposable
{
    protected readonly PdfFormElementType _type;
    protected IntPtr _annot;
    protected readonly int _annotIndex;
    protected bool _disposed;

    public PdfForm Form { get; }
    public PdfPage Page { get; }

    internal PdfFormElement(PdfFormElementType elementType, IntPtr annot, int annotIndex, PdfForm form, PdfPage page)
    {
        _type = elementType;
        _annot = annot != IntPtr.Zero
            ? annot
            : throw new ArgumentException("Annotation handle cannot be null", nameof(annot));

        _annotIndex = annotIndex;
        Form = form ?? throw new ArgumentNullException(nameof(form));
        Page = page ?? throw new ArgumentNullException(nameof(page));
    }

    public PdfFormElementType ElementType => _type;
    public IntPtr Handle => _annot;

    public FsRectF? GetRect()
    {
        return PdfAnnotNative.FPDFAnnot_GetRect(_annot, out FsRectF rect) ? rect : null;
    }

    public bool SetRect(FsRectF rect)
    {
        return PdfAnnotNative.FPDFAnnot_SetRect(_annot, ref rect);
    }

    public bool SetBorderColor(RgbaColor color)
    {
        return PdfAnnotNative.FPDFAnnot_SetColor(_annot, (int)PdfFormFieldColorType.Border, color.R, color.G, color.B, color.A);
    }

    public bool SetFillColor(RgbaColor color)
    {
        return PdfAnnotNative.FPDFAnnot_SetColor(_annot, (int)PdfFormFieldColorType.Fill, color.R, color.G, color.B, color.A);
    }

    public bool SetTextColor(RgbaColor color)
    {
        return PdfAnnotNative.FPDFAnnot_SetColor(_annot, (int)PdfFormFieldColorType.Text, color.R, color.G, color.B, color.A);
    }

    public float? GetFontSize()
    {
        return PdfAnnotNative.FPDFAnnot_GetFontSize(Form.Handle, _annot, out float size) ? size : null;
    }

    public RgbaColor? GetFontColor()
    {
        return PdfAnnotNative.FPDFAnnot_GetFontColor(Form.Handle, _annot, out uint r, out uint g, out uint b)
            ? new RgbaColor((byte)r, (byte)g, (byte)b, 255)
            : null;
    }

    public PdfAnnotationFlags GetAnnotationFlags()
    {
        return (PdfAnnotationFlags)PdfAnnotNative.FPDFAnnot_GetFlags(_annot);
    }

    public bool SetAnnotationFlags(PdfAnnotationFlags flags)
    {
        return PdfAnnotNative.FPDFAnnot_SetFlags(_annot, (int)flags);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing && _annot != IntPtr.Zero)
        {
            PdfAnnotNative.FPDFPage_RemoveAnnot(Page.Handle, _annotIndex);
            _annot = IntPtr.Zero;
        }

        _disposed = true;
    }
}



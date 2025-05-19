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

    public IntPtr Handle => _annot;
    public PdfFormElementType ElementType => _type;

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


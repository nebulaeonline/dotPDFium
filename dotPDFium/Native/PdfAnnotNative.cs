using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfAnnotNative
{
    private const string PdfiumLib = "pdfium";

    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_IsObjectSupportedSubtype(int subtype);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_UpdateObject(IntPtr annot, IntPtr obj);
    
    [DllImport(PdfiumLib)] 
    public static extern int FPDFAnnot_AddInkStroke(IntPtr annot, IntPtr points, UIntPtr pointCount);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_RemoveInkList(IntPtr annot);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_AppendObject(IntPtr annot, IntPtr obj);
    
    [DllImport(PdfiumLib)] 
    public static extern int FPDFAnnot_GetObjectCount(IntPtr annot);
    
    [DllImport(PdfiumLib)] 
    public static extern IntPtr FPDFAnnot_GetObject(IntPtr annot, int index);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_RemoveObject(IntPtr annot, int index);

    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_HasAttachmentPoints(IntPtr annot);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_SetAttachmentPoints(IntPtr annot, UIntPtr index, ref FsQuadPointsF quad);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_AppendAttachmentPoints(IntPtr annot, ref FsQuadPointsF quad);
    
    [DllImport(PdfiumLib)] 
    public static extern UIntPtr FPDFAnnot_CountAttachmentPoints(IntPtr annot);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_GetAttachmentPoints(IntPtr annot, UIntPtr index, out FsQuadPointsF quad);

    [DllImport(PdfiumLib)] 
    public static extern uint FPDFAnnot_GetVertices(IntPtr annot, IntPtr buffer, uint length);
    
    [DllImport(PdfiumLib)] 
    public static extern uint FPDFAnnot_GetInkListCount(IntPtr annot);
    
    [DllImport(PdfiumLib)] 
    public static extern uint FPDFAnnot_GetInkListPath(IntPtr annot, uint pathIndex, IntPtr buffer, uint length);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_GetLine(IntPtr annot, out FsPointF start, out FsPointF end);

    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_SetBorder(IntPtr annot, float hr, float vr, float width);

    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_GetBorder(IntPtr annot, out float hr, out float vr, out float width);

    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_HasKey(IntPtr annot, string key);

    [DllImport(PdfiumLib)] 
    public static extern int FPDFAnnot_GetValueType(IntPtr annot, string key);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_SetStringValue(IntPtr annot, string key, [MarshalAs(UnmanagedType.LPWStr)] string value);
    
    [DllImport(PdfiumLib)] 
    public static extern uint FPDFAnnot_GetStringValue(IntPtr annot, string key, [Out] char[] buffer, uint buflen);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_GetNumberValue(IntPtr annot, string key, out float value);

    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_SetAP(IntPtr annot, int mode, [MarshalAs(UnmanagedType.LPWStr)] string value);
    
    [DllImport(PdfiumLib)] 
    public static extern uint FPDFAnnot_GetAP(IntPtr annot, int mode, [Out] char[] buffer, uint buflen);

    [DllImport(PdfiumLib)] 
    public static extern IntPtr FPDFAnnot_GetLinkedAnnot(IntPtr annot, string key);
    
    [DllImport(PdfiumLib)] 
    public static extern int FPDFAnnot_GetFlags(IntPtr annot);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_SetFlags(IntPtr annot, int flags);

    [DllImport(PdfiumLib)] 
    public static extern IntPtr FPDFAnnot_GetFileAttachment(IntPtr annot);
    
    [DllImport(PdfiumLib)] 
    public static extern IntPtr FPDFAnnot_AddFileAttachment(IntPtr annot, [MarshalAs(UnmanagedType.LPWStr)] string name);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_SetURI(IntPtr annot, string uri);

    [DllImport(PdfiumLib)] 
    public static extern IntPtr FPDFAnnot_GetLink(IntPtr annot);

    [DllImport(PdfiumLib)] 
    public static extern IntPtr FPDFAnnot_GetFormFieldAtPoint(IntPtr hHandle, IntPtr page, ref FsPointF point);
    
    [DllImport(PdfiumLib)] 
    public static extern uint FPDFAnnot_GetFormFieldName(IntPtr hHandle, IntPtr annot, [Out] char[] buffer, uint buflen);
    
    [DllImport(PdfiumLib)] 
    public static extern uint FPDFAnnot_GetFormFieldAlternateName(IntPtr hHandle, IntPtr annot, [Out] char[] buffer, uint buflen);
    
    [DllImport(PdfiumLib)] 
    public static extern int FPDFAnnot_GetFormFieldType(IntPtr hHandle, IntPtr annot);
    
    [DllImport(PdfiumLib)] 
    public static extern uint FPDFAnnot_GetFormFieldValue(IntPtr hHandle, IntPtr annot, [Out] char[] buffer, uint buflen);
    
    [DllImport(PdfiumLib)] 
    public static extern uint FPDFAnnot_GetFormFieldExportValue(IntPtr hHandle, IntPtr annot, [Out] char[] buffer, uint buflen);
    
    [DllImport(PdfiumLib)] 
    public static extern int FPDFAnnot_GetFormFieldFlags(IntPtr hHandle, IntPtr annot);

    [DllImport(PdfiumLib)] 
    public static extern int FPDFAnnot_GetOptionCount(IntPtr hHandle, IntPtr annot);
    
    [DllImport(PdfiumLib)] 
    public static extern uint FPDFAnnot_GetOptionLabel(IntPtr hHandle, IntPtr annot, int index, [Out] char[] buffer, uint buflen);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_IsOptionSelected(IntPtr hHandle, IntPtr annot, int index);

    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_GetFontSize(IntPtr hHandle, IntPtr annot, out float value);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_GetFontColor(IntPtr hHandle, IntPtr annot, out uint r, out uint g, out uint b);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_IsChecked(IntPtr hHandle, IntPtr annot);

    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_SetFocusableSubtypes(IntPtr hHandle, IntPtr subtypes, UIntPtr count);
    
    [DllImport(PdfiumLib)] 
    public static extern int FPDFAnnot_GetFocusableSubtypesCount(IntPtr hHandle);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFAnnot_GetFocusableSubtypes(IntPtr hHandle, IntPtr subtypes, UIntPtr count);

    [DllImport(PdfiumLib)] 
    public static extern uint FPDFAnnot_GetFormAdditionalActionJavaScript(IntPtr hHandle, IntPtr annot, int eventId, [Out] char[] buffer, uint buflen);

    [DllImport(PdfiumLib)] 
    public static extern int FPDFAnnot_GetFormControlCount(IntPtr hHandle, IntPtr annot);
    
    [DllImport(PdfiumLib)] 
    public static extern int FPDFAnnot_GetFormControlIndex(IntPtr hHandle, IntPtr annot);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFAnnot_IsSupportedSubtype(int subtype);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPage_CreateAnnot(IntPtr page, int subtype);

    [DllImport(PdfiumLib)]
    public static extern int FPDFPage_GetAnnotCount(IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPage_GetAnnot(IntPtr page, int index);

    [DllImport(PdfiumLib)]
    public static extern int FPDFPage_GetAnnotIndex(IntPtr page, IntPtr annot);

    [DllImport(PdfiumLib)]
    public static extern void FPDFPage_CloseAnnot(IntPtr annot);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPage_RemoveAnnot(IntPtr page, int index);

    [DllImport(PdfiumLib)]
    public static extern int FPDFAnnot_GetSubtype(IntPtr annot);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFAnnot_SetColor(IntPtr annot, int type, uint r, uint g, uint b, uint a);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFAnnot_GetColor(IntPtr annot, int type, out uint r, out uint g, out uint b, out uint a);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFAnnot_SetRect(IntPtr annot, ref FsRectF rect);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFAnnot_GetRect(IntPtr annot, out FsRectF rect);

}


using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static partial class PdfEditNative
{
    private const string PdfiumLib = "pdfium";

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDF_CreateNewDocument();

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPage_New(IntPtr doc, int page_index, double width, double height);

    [DllImport(PdfiumLib)]
    public static extern void FPDFPage_Delete(IntPtr doc, int page_index);

    [DllImport(PdfiumLib)]
    public static extern int FPDFPage_GetRotation(IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern void FPDFPage_SetRotation(IntPtr page, int rotate);

    [DllImport(PdfiumLib)]
    public static extern void FPDFPage_InsertObject(IntPtr page, IntPtr pageObject);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPage_RemoveObject(IntPtr page, IntPtr pageObject);

    [DllImport(PdfiumLib)]
    public static extern int FPDFPage_CountObjects(IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPage_GetObject(IntPtr page, int index);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPage_HasTransparency(IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPage_GenerateContent(IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern void FPDFPageObj_Destroy(IntPtr pageObject);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPageObj_HasTransparency(IntPtr obj);

    [DllImport(PdfiumLib)]
    public static extern int FPDFPageObj_GetType(IntPtr obj);

    [DllImport(PdfiumLib)]
    public static extern void FPDFPageObj_Transform(IntPtr obj, double a, double b, double c, double d, double e, double f);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPageObj_TransformF(IntPtr obj, ref FsMatrixF matrix);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPageObj_GetMatrix(IntPtr obj, out FsMatrixF matrix);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPageObj_SetMatrix(IntPtr obj, ref FsMatrixF matrix);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPageObj_GetBounds(IntPtr obj, out float left, out float bottom, out float right, out float top);

    [DllImport(PdfiumLib)]
    public static extern void FPDFPageObj_SetFillColor(IntPtr obj, uint r, uint g, uint b, uint a);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPageObj_GetFillColor(IntPtr obj, out uint r, out uint g, out uint b, out uint a);

    [DllImport(PdfiumLib)]
    public static extern void FPDFPageObj_SetStrokeColor(IntPtr obj, uint r, uint g, uint b, uint a);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPageObj_GetStrokeColor(IntPtr obj, out uint r, out uint g, out uint b, out uint a);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPageObj_SetStrokeWidth(IntPtr obj, float width);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPageObj_GetStrokeWidth(IntPtr obj, out float width);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPageObj_NewTextObj(IntPtr doc, string font, float fontSize);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFText_SetText(IntPtr textObject, [MarshalAs(UnmanagedType.LPWStr)] string text);

    [DllImport("pdfium")]
    public static extern IntPtr FPDFText_LoadFont(IntPtr document, IntPtr fontData, uint size, int fontType, int cid);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFText_LoadStandardFont(IntPtr doc, string font);

    [DllImport(PdfiumLib)]
    public static extern void FPDFFont_Close(IntPtr font);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPageObj_CreateNewPath(float x, float y);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPageObj_CreateNewRect(float x, float y, float w, float h);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPageObj_NewImageObj(IntPtr document);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPath_MoveTo(IntPtr path, float x, float y);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPath_LineTo(IntPtr path, float x, float y);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPath_Close(IntPtr path);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPath_SetDrawMode(IntPtr path, int fillMode, bool stroke);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPath_GetDrawMode(IntPtr path, out int fillMode, out bool stroke);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFText_SetCharcodes(IntPtr textObject, uint[] charcodes, UIntPtr count);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFText_LoadCidType2Font(IntPtr doc, byte[] fontData, uint fontDataSize, string toUnicodeCMap, byte[] cidToGidMap, uint cidToGidMapSize);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFTextObj_GetFontSize(IntPtr textObj, out float fontSize);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFPageObj_CreateTextObj(IntPtr doc, IntPtr font, float fontSize);

    [DllImport(PdfiumLib)]
    public static extern int FPDFTextObj_GetTextRenderMode(IntPtr textObj);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFTextObj_SetTextRenderMode(IntPtr textObj, int renderMode);

    [DllImport(PdfiumLib, CharSet = CharSet.Unicode)]
    public static extern uint FPDFTextObj_GetText(IntPtr textObject, IntPtr textPage, char[] buffer, uint length);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFTextObj_GetRenderedBitmap(IntPtr doc, IntPtr page, IntPtr textObject, float scale);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFTextObj_GetFont(IntPtr textObject);

    [DllImport(PdfiumLib)]
    public static extern UIntPtr FPDFFont_GetBaseFontName(IntPtr font, byte[] buffer, UIntPtr length);

    [DllImport(PdfiumLib)]
    public static extern UIntPtr FPDFFont_GetFamilyName(IntPtr font, byte[] buffer, UIntPtr length);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFFont_GetFontData(IntPtr font, byte[] buffer, UIntPtr buflen, out UIntPtr outBufLen);

    [DllImport(PdfiumLib)]
    public static extern int FPDFFont_GetIsEmbedded(IntPtr font);

    [DllImport(PdfiumLib)]
    public static extern int FPDFFont_GetFlags(IntPtr font);

    [DllImport(PdfiumLib)]
    public static extern int FPDFFont_GetWeight(IntPtr font);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFFont_GetItalicAngle(IntPtr font, out int angle);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFFont_GetAscent(IntPtr font, float fontSize, out float ascent);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFFont_GetDescent(IntPtr font, float fontSize, out float descent);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFFont_GetGlyphWidth(IntPtr font, uint glyph, float fontSize, out float width);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFFont_GetGlyphPath(IntPtr font, uint glyph, float fontSize);

    [DllImport(PdfiumLib)]
    public static extern int FPDFGlyphPath_CountGlyphSegments(IntPtr glyphPath);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFGlyphPath_GetGlyphPathSegment(IntPtr glyphPath, int index);

    [DllImport(PdfiumLib)]
    public static extern int FPDFFormObj_CountObjects(IntPtr formObject);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFFormObj_GetObject(IntPtr formObject, uint index);

    [DllImport(PdfiumLib)] 
    public static extern int FPDFPageObj_CountMarks(IntPtr obj);
    
    [DllImport(PdfiumLib)] 
    public static extern IntPtr FPDFPageObj_GetMark(IntPtr obj, uint index);
    
    [DllImport(PdfiumLib)] 
    public static extern IntPtr FPDFPageObj_AddMark(IntPtr obj, string tag);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObj_RemoveMark(IntPtr obj, IntPtr mark);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObjMark_GetName(IntPtr mark, IntPtr buffer, uint buflen, out uint outBufLen);
    
    [DllImport(PdfiumLib)] 
    public static extern int FPDFPageObjMark_CountParams(IntPtr mark);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObjMark_GetParamKey(IntPtr mark, uint index, IntPtr buffer, uint buflen, out uint outBufLen);
    
    [DllImport(PdfiumLib)] 
    public static extern int FPDFPageObjMark_GetParamValueType(IntPtr mark, string key);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObjMark_GetParamIntValue(IntPtr mark, string key, out int value);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObjMark_GetParamStringValue(IntPtr mark, string key, IntPtr buffer, uint buflen, out uint outBufLen);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObjMark_GetParamBlobValue(IntPtr mark, string key, byte[] buffer, uint buflen, out uint outBufLen);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObjMark_SetIntParam(IntPtr doc, IntPtr obj, IntPtr mark, string key, int value);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObjMark_SetStringParam(IntPtr doc, IntPtr obj, IntPtr mark, string key, string value);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObjMark_SetBlobParam(IntPtr doc, IntPtr obj, IntPtr mark, string key, byte[] value, uint length);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObjMark_RemoveParam(IntPtr obj, IntPtr mark, string key);
        
    [DllImport(PdfiumLib)] 
    public static extern uint FPDFImageObj_GetImageDataDecoded(IntPtr obj, byte[] buffer, uint buflen);
    
    [DllImport(PdfiumLib)] 
    public static extern uint FPDFImageObj_GetImageDataRaw(IntPtr obj, byte[] buffer, uint buflen);
    
    [DllImport(PdfiumLib)] 
    public static extern int FPDFImageObj_GetImageFilterCount(IntPtr obj);
    
    [DllImport(PdfiumLib)] 
    public static extern uint FPDFImageObj_GetImageFilter(IntPtr obj, int index, byte[] buffer, uint buflen);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFImageObj_GetImageMetadata(IntPtr obj, IntPtr page, out PdfImageObjMetadata metadata);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFImageObj_GetImagePixelSize(IntPtr obj, out uint width, out uint height);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFImageObj_GetIccProfileDataDecoded(IntPtr obj, IntPtr page, byte[] buffer, UIntPtr buflen, out UIntPtr outBuflen);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObj_GetDashPhase(IntPtr obj, out float phase);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObj_SetDashPhase(IntPtr obj, float phase);
    
    [DllImport(PdfiumLib)] 
    public static extern int FPDFPageObj_GetDashCount(IntPtr obj);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObj_GetDashArray(IntPtr obj, float[] dashArray, UIntPtr count);
    
    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObj_SetDashArray(IntPtr obj, float[] dashArray, UIntPtr count, float phase);

    [DllImport(PdfiumLib)] 
    public static extern bool FPDFPageObj_GetRotatedBounds(IntPtr obj, out FsQuadPointsF quad);

    [DllImport(PdfiumLib)]
    public static extern bool FPDF_MovePages(IntPtr doc, int[] pageIndices, ulong pageIndicesLen, int destPageIndex);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPageObj_GetIsActive(IntPtr obj, out bool isActive);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPageObj_SetIsActive(IntPtr obj, bool isActive);

    [DllImport(PdfiumLib)]
    public static extern void FPDFPage_TransformAnnots(IntPtr page, double a, double b, double c, double d, double e, double f);

    [DllImport(PdfiumLib)]
    public static extern int FPDFPageObj_GetLineJoin(IntPtr obj);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPageObj_SetLineJoin(IntPtr obj, int lineJoin);

    [DllImport(PdfiumLib)]
    public static extern int FPDFPageObj_GetLineCap(IntPtr obj);

    [DllImport(PdfiumLib)]
    public static extern bool FPDFPageObj_SetLineCap(IntPtr obj, int lineCap);

}
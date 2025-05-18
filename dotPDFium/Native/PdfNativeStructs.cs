using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

[StructLayout(LayoutKind.Sequential)]
public struct FsPointF
{
    public float X;
    public float Y;

    public FsPointF(float x, float y)
    {
        X = x;
        Y = y;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct FsSizeF
{
    public float width;
    public float height;

    public FsSizeF(float width, float height)
    {
        this.width = width;
        this.height = height;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct FsSize
{
    public double width;
    public double height;

    public FsSize(double width, double height)
    {
        this.width = width;
        this.height = height;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct FsMatrixF
{
    public float a;
    public float b;
    public float c;
    public float d;
    public float e;
    public float f;

    public FsMatrixF(float a, float b, float c, float d, float e, float f)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        this.e = e;
        this.f = f;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct FsMatrix
{
    public double a;
    public double b;
    public double c;
    public double d;
    public double e;
    public double f;

    public FsMatrix(double a, double b, double c, double d, double e, double f)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        this.e = e;
        this.f = f;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct FsRectF
{
    public float left;
    public float top;
    public float right;
    public float bottom;

    public FsRectF(float left, float top, float right, float bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct FsRect
{
    public double left;
    public double top;
    public double right;
    public double bottom;

    public FsRect(double left, double top, double right, double bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct FsQuadPointsF
{
    float x1;
    float y1;
    float x2;
    float y2;
    float x3;
    float y3;
    float x4;
    float y4;

    public FsQuadPointsF(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
    {
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
        this.x3 = x3;
        this.y3 = y3;
        this.x4 = x4;
        this.y4 = y4;
    }
}

public readonly struct PdfAnnotationBorder
{
    public float HorizontalRadius { get; }
    public float VerticalRadius { get; }
    public float BorderWidth { get; }

    public PdfAnnotationBorder(float hr, float vr, float width)
    {
        HorizontalRadius = hr;
        VerticalRadius = vr;
        BorderWidth = width;
    }
}


[StructLayout(LayoutKind.Sequential)]
public struct PdfImageObjMetadata
{
    /// <summary>The image width in pixels.</summary>
    public uint width;

    /// <summary>The image height in pixels.</summary>
    public uint height;

    /// <summary>The image's horizontal DPI (dots per inch).</summary>
    public float horizontal_dpi;

    /// <summary>The image's vertical DPI (dots per inch).</summary>
    public float vertical_dpi;

    /// <summary>Bits per pixel.</summary>
    public uint bits_per_pixel;

    /// <summary>The image's color space. Use FPDF_COLORSPACE_* constants.</summary>
    public int colorspace;

    /// <summary>The image's marked content ID, or -1 if none.</summary>
    public int marked_content_id;
}

[StructLayout(LayoutKind.Sequential)]
public struct FxFileAvail
{
    public int version;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public IsDataAvailDelegate IsDataAvail;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool IsDataAvailDelegate(IntPtr pThis, UIntPtr offset, UIntPtr size);
}

[StructLayout(LayoutKind.Sequential)]
public struct FxDownloadHints
{
    public int version;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public AddSegmentDelegate AddSegment;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AddSegmentDelegate(IntPtr pThis, UIntPtr offset, UIntPtr size);
}

[StructLayout(LayoutKind.Sequential)]
public struct FpdfFileAccess
{
    public uint m_FileLen;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public GetBlockDelegate m_GetBlock;

    public IntPtr m_Param;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int GetBlockDelegate(IntPtr param, uint position, IntPtr buffer, uint size);
}

[StructLayout(LayoutKind.Sequential)]
public struct UnSupportInfo
{
    public int version;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public UnSupportHandler FSDK_UnSupport_Handler;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void UnSupportHandler(IntPtr pThis, int type);
}


// time_t = long on most platforms
[StructLayout(LayoutKind.Sequential)]
public struct time_t
{
    public long Value;

    public static implicit operator long(time_t t) => t.Value;
    public static implicit operator time_t(long l) => new time_t { Value = l };
}

[StructLayout(LayoutKind.Sequential)]
public struct FpdfSystemTime
{
    public ushort wYear;
    public ushort wMonth;
    public ushort wDayOfWeek;
    public ushort wDay;
    public ushort wHour;
    public ushort wMinute;
    public ushort wSecond;
    public ushort wMilliseconds;
}


[StructLayout(LayoutKind.Sequential)]
public unsafe struct PdfFormFillInfo
{
    public int version;

    // Optional: free resources
    public delegate* unmanaged[Cdecl]<PdfFormFillInfo*, void> Release;

    // Rendering invalidation callback
    public delegate* unmanaged[Cdecl]<PdfFormFillInfo*, IntPtr /* FPDF_PAGE */, double, double, double, double, void> FFI_Invalidate;

    // Optional: text selection display
    public delegate* unmanaged[Cdecl]<PdfFormFillInfo*, IntPtr, double, double, double, double, void> FFI_OutputSelectedRect;

    // Cursor change
    public delegate* unmanaged[Cdecl]<PdfFormFillInfo*, int, void> FFI_SetCursor;

    // Timer control
    public delegate* unmanaged[Cdecl]<PdfFormFillInfo*, int, delegate* unmanaged[Cdecl]<int, void>, int> FFI_SetTimer;
    public delegate* unmanaged[Cdecl]<PdfFormFillInfo*, int, void> FFI_KillTimer;

    // Time
    public delegate* unmanaged[Cdecl]<PdfFormFillInfo*, FpdfSystemTime> FFI_GetLocalTime;

    // Form change callback
    public delegate* unmanaged[Cdecl]<PdfFormFillInfo*, void> FFI_OnChange;

    // Page access
    public delegate* unmanaged[Cdecl]<PdfFormFillInfo*, IntPtr /* FPDF_DOCUMENT */, int, IntPtr> FFI_GetPage;

    // Optional: for JavaScript engines
    public delegate* unmanaged[Cdecl]<PdfFormFillInfo*, IntPtr /* FPDF_DOCUMENT */, IntPtr> FFI_GetCurrentPage;
    public delegate* unmanaged[Cdecl]<PdfFormFillInfo*, IntPtr /* FPDF_PAGE */, int> FFI_GetRotation;

    // Execute named action (e.g. "NextPage")
    public delegate* unmanaged[Cdecl]<PdfFormFillInfo*, IntPtr /* FPDF_BYTESTRING */, void> FFI_ExecuteNamedAction;

    // JavaScript runtime integration (unused unless m_pJsPlatform is provided)
    public IntPtr m_pJsPlatform;

    // XFA-related flags
    public byte xfa_disabled;

    // TODO: other fields as needed for version 2/3 support
}

[StructLayout(LayoutKind.Sequential)]
public struct IfSdkPause
{
    public int version;

    // Pointer to a native callback: delegate or IntPtr (we’ll handle both)
    public IntPtr NeedToPauseNow;

    // Optional user-defined context
    public IntPtr userData;
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int NeedToPauseNowDelegate(ref IfSdkPause pause);

[StructLayout(LayoutKind.Sequential)]
public struct PdfColorScheme
{
    public uint path_fill_color;
    public uint path_stroke_color;
    public uint text_fill_color;
    public uint text_stroke_color;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct PdfFontInfo
{
    public int version;

    public delegate* unmanaged[Cdecl]<PdfFontInfo*, void> Release;
    public delegate* unmanaged[Cdecl]<PdfFontInfo*, void*, void> EnumFonts;
    public delegate* unmanaged[Cdecl]<PdfFontInfo*, int, byte, int, int, byte*, byte*, void*> MapFont;
    public delegate* unmanaged[Cdecl]<PdfFontInfo*, byte*, void*> GetFont;
    public delegate* unmanaged[Cdecl]<PdfFontInfo*, void*, uint, byte*, uint, uint> GetFontData;
    public delegate* unmanaged[Cdecl]<PdfFontInfo*, void*, byte*, uint, uint> GetFaceName;
    public delegate* unmanaged[Cdecl]<PdfFontInfo*, void*, int> GetFontCharset;
    public delegate* unmanaged[Cdecl]<PdfFontInfo*, void*, void> DeleteFont;
}

[StructLayout(LayoutKind.Sequential)]
public struct CharsetFontMap
{
    public int charset;

    [MarshalAs(UnmanagedType.LPStr)]
    public string fontname;
}

public readonly record struct RgbaColor(byte R, byte G, byte B, byte A);
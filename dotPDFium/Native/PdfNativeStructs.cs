using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

[StructLayout(LayoutKind.Sequential)]
public struct FS_MATRIX
{
    public float a;
    public float b;
    public float c;
    public float d;
    public float e;
    public float f;

    public FS_MATRIX(float a, float b, float c, float d, float e, float f)
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
public struct FS_RECTF
{
    public float left;
    public float top;
    public float right;
    public float bottom;

    public FS_RECTF(float left, float top, float right, float bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct FX_FILEAVAIL
{
    public int version;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public IsDataAvailDelegate IsDataAvail;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool IsDataAvailDelegate(IntPtr pThis, UIntPtr offset, UIntPtr size);
}

[StructLayout(LayoutKind.Sequential)]
public struct FX_DOWNLOADHINTS
{
    public int version;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public AddSegmentDelegate AddSegment;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AddSegmentDelegate(IntPtr pThis, UIntPtr offset, UIntPtr size);
}

[StructLayout(LayoutKind.Sequential)]
public struct FPDF_FILEACCESS
{
    public uint m_FileLen;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public GetBlockDelegate m_GetBlock;

    public IntPtr m_Param;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int GetBlockDelegate(IntPtr param, uint position, IntPtr buffer, uint size);
}

[StructLayout(LayoutKind.Sequential)]
public struct UNSUPPORT_INFO
{
    public int version;

    [MarshalAs(UnmanagedType.FunctionPtr)]
    public UnSupportHandler FSDK_UnSupport_Handler;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void UnSupportHandler(ref UNSUPPORT_INFO info, int type);
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
public struct FPDF_SYSTEMTIME
{
    public ushort wYear;         // e.g. 2024
    public ushort wMonth;        // 1–12
    public ushort wDayOfWeek;    // 0–6 (Sunday–Saturday)
    public ushort wDay;          // 1–31
    public ushort wHour;         // 0–23
    public ushort wMinute;       // 0–59
    public ushort wSecond;       // 0–59
    public ushort wMilliseconds; // 0–999
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FPDF_FORMFILLINFO
{
    public int version;

    // Optional: free resources
    public delegate* unmanaged[Cdecl]<FPDF_FORMFILLINFO*, void> Release;

    // Rendering invalidation callback
    public delegate* unmanaged[Cdecl]<FPDF_FORMFILLINFO*, IntPtr /* FPDF_PAGE */, double, double, double, double, void> FFI_Invalidate;

    // Optional: text selection display
    public delegate* unmanaged[Cdecl]<FPDF_FORMFILLINFO*, IntPtr, double, double, double, double, void> FFI_OutputSelectedRect;

    // Cursor change
    public delegate* unmanaged[Cdecl]<FPDF_FORMFILLINFO*, int, void> FFI_SetCursor;

    // Timer control
    public delegate* unmanaged[Cdecl]<FPDF_FORMFILLINFO*, int, delegate* unmanaged[Cdecl]<int, void>, int> FFI_SetTimer;
    public delegate* unmanaged[Cdecl]<FPDF_FORMFILLINFO*, int, void> FFI_KillTimer;

    // Time
    public delegate* unmanaged[Cdecl]<FPDF_FORMFILLINFO*, FPDF_SYSTEMTIME> FFI_GetLocalTime;

    // Form change callback
    public delegate* unmanaged[Cdecl]<FPDF_FORMFILLINFO*, void> FFI_OnChange;

    // Page access
    public delegate* unmanaged[Cdecl]<FPDF_FORMFILLINFO*, IntPtr /* FPDF_DOCUMENT */, int, IntPtr> FFI_GetPage;

    // Optional: for JavaScript engines
    public delegate* unmanaged[Cdecl]<FPDF_FORMFILLINFO*, IntPtr /* FPDF_DOCUMENT */, IntPtr> FFI_GetCurrentPage;
    public delegate* unmanaged[Cdecl]<FPDF_FORMFILLINFO*, IntPtr /* FPDF_PAGE */, int> FFI_GetRotation;

    // Execute named action (e.g. "NextPage")
    public delegate* unmanaged[Cdecl]<FPDF_FORMFILLINFO*, IntPtr /* FPDF_BYTESTRING */, void> FFI_ExecuteNamedAction;

    // Optional: JavaScript runtime integration (unused unless m_pJsPlatform is provided)
    public IntPtr m_pJsPlatform;

    // Optional: XFA-related flags
    public byte xfa_disabled;

    // Add other fields as needed for version 2/3 support
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct IFSDK_PAUSE
{
    public int version;

    // PDFium will periodically call this to ask: should we pause?
    public delegate* unmanaged[Cdecl]<IFSDK_PAUSE*, int> NeedToPauseNow;

    public void* user;
}

[StructLayout(LayoutKind.Sequential)]
public struct FPDF_COLORSCHEME
{
    public uint path_fill_color;
    public uint path_stroke_color;
    public uint text_fill_color;
    public uint text_stroke_color;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct FPDF_SYSFONTINFO
{
    public int version;

    public delegate* unmanaged[Cdecl]<FPDF_SYSFONTINFO*, void> Release;
    public delegate* unmanaged[Cdecl]<FPDF_SYSFONTINFO*, void*, void> EnumFonts;
    public delegate* unmanaged[Cdecl]<FPDF_SYSFONTINFO*, int, byte, int, int, byte*, byte*, void*> MapFont;
    public delegate* unmanaged[Cdecl]<FPDF_SYSFONTINFO*, byte*, void*> GetFont;
    public delegate* unmanaged[Cdecl]<FPDF_SYSFONTINFO*, void*, uint, byte*, uint, uint> GetFontData;
    public delegate* unmanaged[Cdecl]<FPDF_SYSFONTINFO*, void*, byte*, uint, uint> GetFaceName;
    public delegate* unmanaged[Cdecl]<FPDF_SYSFONTINFO*, void*, int> GetFontCharset;
    public delegate* unmanaged[Cdecl]<FPDF_SYSFONTINFO*, void*, void> DeleteFont;
}

[StructLayout(LayoutKind.Sequential)]
public struct FPDF_CharsetFontMap
{
    public int charset;
    public IntPtr fontname;
}

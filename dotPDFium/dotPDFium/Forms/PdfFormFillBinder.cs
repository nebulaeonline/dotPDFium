using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public unsafe sealed class PdfFormFillBinder : IDisposable
{
    public PdfFormFillInfo Info;

    public PdfFormFillBinder()
    {
        Info.version = 1;

        Info.FFI_SetCursor = (delegate* unmanaged[Cdecl]<PdfFormFillInfo*, int, void>)&SetCursor;
        Info.FFI_OnChange = (delegate* unmanaged[Cdecl]<PdfFormFillInfo*, void>)&OnChange;
        Info.FFI_Invalidate = (delegate* unmanaged[Cdecl]<PdfFormFillInfo*, nint, double, double, double, double, void>)&Invalidate;

        // Everything else left null for now
        Info.Release = null;
        Info.FFI_KillTimer = null;
        Info.FFI_SetTimer = null;
        Info.FFI_GetLocalTime = null;
    }

    public void Dispose() { }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void SetCursor(PdfFormFillInfo* info, int cursorType)
    {
        Debug.WriteLine($"[dotPDFium] Cursor set to {cursorType}");
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void OnChange(PdfFormFillInfo* info)
    {
        Debug.WriteLine($"[dotPDFium] Form field changed.");
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Invalidate(PdfFormFillInfo* info, nint page, double l, double t, double r, double b)
    {
        Debug.WriteLine($"[dotPDFium] Invalidate: ({l},{t}) → ({r},{b})");
    }
}

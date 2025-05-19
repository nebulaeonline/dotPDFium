using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public unsafe sealed class PdfFormFillBinder : IDisposable
{
    public PdfFormFillInfo Info;
    private static readonly Dictionary<IntPtr, PdfFormEvents> _eventMap = new();
    private readonly GCHandle _selfHandle;
    private readonly IntPtr _infoPtr;

    public PdfFormFillBinder(PdfFormEvents events)
    {
        Info.version = 1;
        Info.FFI_SetCursor = &OnSetCursor;
        Info.FFI_OnChange = &OnChanged;
        Info.FFI_Invalidate = &OnInvalidate;

        _selfHandle = GCHandle.Alloc(events);
        _infoPtr = (IntPtr)Unsafe.AsPointer(ref Info);
        _eventMap[_infoPtr] = events;
    }

    public void Dispose()
    {
        _eventMap.Remove(_infoPtr);
        if (_selfHandle.IsAllocated)
            _selfHandle.Free();
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void OnSetCursor(PdfFormFillInfo* info, int cursorType)
    {
        if (_eventMap.TryGetValue((IntPtr)info, out var events))
            events.OnCursorChange?.Invoke(cursorType);
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void OnChanged(PdfFormFillInfo* info)
    {
        if (_eventMap.TryGetValue((IntPtr)info, out var events))
            events.OnFormChanged?.Invoke();
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void OnInvalidate(PdfFormFillInfo* info, IntPtr page, double left, double top, double right, double bottom)
    {
        if (!_eventMap.TryGetValue((IntPtr)info, out var events)) return;

        var rect = new FsRectF((float)left, (float)top, (float)right, (float)bottom);
        var pageWrapper = new PdfPage(page); // Temporary stub; should be resolved from PdfDocument
        events.OnInvalidate?.Invoke(pageWrapper, rect);
    }
}

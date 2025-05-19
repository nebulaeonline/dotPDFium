using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public unsafe sealed class PdfFormFillBinder : IDisposable
{
    public PdfFormFillInfo Info;

    private readonly PdfForm _form;
    private readonly PdfFormEvents _events;
    private readonly GCHandle _selfHandle;
    private readonly IntPtr _infoPtr;

    private static readonly Dictionary<IntPtr, PdfFormFillBinder> _binderMap = new();

    public PdfFormFillBinder(PdfForm form, PdfFormEvents events)
    {
        _form = form ?? throw new ArgumentNullException(nameof(form));
        _events = events ?? throw new ArgumentNullException(nameof(events));

        Info.version = 1;
        Info.FFI_SetCursor = &OnSetCursor;
        Info.FFI_OnChange = &OnChanged;
        Info.FFI_Invalidate = &OnInvalidate;
        Info.FFI_OutputSelectedRect = &OnOutputSelectedRect;
        Info.FFI_SetTimer = &OnSetTimer;
        Info.FFI_KillTimer = &OnKillTimer;
        Info.FFI_GetLocalTime = &OnGetLocalTime;
        Info.FFI_GetPage = &OnGetPage;
        Info.FFI_GetRotation = &OnGetRotation;
        Info.Release = &OnRelease;

        _selfHandle = GCHandle.Alloc(this);
        _infoPtr = (IntPtr)Unsafe.AsPointer(ref Info);
        _binderMap[_infoPtr] = this;
    }

    public void Dispose()
    {
        _binderMap.Remove(_infoPtr);
        if (_selfHandle.IsAllocated)
            _selfHandle.Free();
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void OnSetCursor(PdfFormFillInfo* info, int cursorType)
    {
        if (_binderMap.TryGetValue((IntPtr)info, out var binder))
        {
            binder._events.OnCursorChange?.Invoke(cursorType);
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void OnChanged(PdfFormFillInfo* info)
    {
        if (_binderMap.TryGetValue((IntPtr)info, out var binder))
        {
            binder._events.OnFormChanged?.Invoke();
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void OnInvalidate(PdfFormFillInfo* info, IntPtr page, double left, double top, double right, double bottom)
    {
        if (!_binderMap.TryGetValue((IntPtr)info, out var binder)) return;

        var resolvedPage = binder._form.Document.ResolvePage(page);
        if (resolvedPage == null) return;

        var rect = new FsRectF((float)left, (float)top, (float)right, (float)bottom);
        binder._events.OnInvalidate?.Invoke(resolvedPage, rect);
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void OnOutputSelectedRect(PdfFormFillInfo* info, IntPtr page, double l, double t, double r, double b)
    {
        if (!_binderMap.TryGetValue((IntPtr)info, out var binder)) return;

        var resolvedPage = binder._form.Document.ResolvePage(page);
        if (resolvedPage == null) return;

        var rect = new FsRectF((float)l, (float)t, (float)r, (float)b);
        binder._events.OnOutputSelectedRect?.Invoke(resolvedPage, rect);
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static int OnSetTimer(PdfFormFillInfo* info, int elapse, delegate* unmanaged[Cdecl]<int, void> callback)
    {
        if (_binderMap.TryGetValue((IntPtr)info, out var binder))
        {
            return binder._events.OnSetTimer?.Invoke(elapse, () => callback(elapse)) ?? 0;
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void OnKillTimer(PdfFormFillInfo* info, int timerId)
    {
        if (_binderMap.TryGetValue((IntPtr)info, out var binder))
        {
            binder._events.OnKillTimer?.Invoke(timerId);
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static FpdfSystemTime OnGetLocalTime(PdfFormFillInfo* info)
    {
        if (_binderMap.TryGetValue((IntPtr)info, out var binder) && binder._events.OnGetLocalTime != null)
        {
            return binder._events.OnGetLocalTime.Invoke();
        }
        return default;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static IntPtr OnGetPage(PdfFormFillInfo* info, IntPtr doc, int index)
    {
        if (_binderMap.TryGetValue((IntPtr)info, out var binder) && binder._events.OnGetPage != null)
        {
            var page = binder._events.OnGetPage.Invoke(binder._form.Document, index);
            return page?.Handle ?? IntPtr.Zero;
        }
        return IntPtr.Zero;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static int OnGetRotation(PdfFormFillInfo* info, IntPtr page)
    {
        if (_binderMap.TryGetValue((IntPtr)info, out var binder) && binder._events.OnGetRotation != null)
        {
            var resolved = binder._form.Document.ResolvePage(page);
            if (resolved != null)
                return binder._events.OnGetRotation.Invoke(resolved);
        }
        return 0;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static void OnRelease(PdfFormFillInfo* info)
    {
        if (_binderMap.TryGetValue((IntPtr)info, out var binder))
        {
            binder._events.OnRelease?.Invoke();
        }
    }
}

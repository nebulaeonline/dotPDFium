namespace nebulae.dotPDFium.Native;

public static class PdfKeyInterop
{
    // Modifier Flags (bitmask)
    public const int FWL_EVENTFLAG_ShiftKey = 1 << 0;
    public const int FWL_EVENTFLAG_ControlKey = 1 << 1;
    public const int FWL_EVENTFLAG_AltKey = 1 << 2;
    public const int FWL_EVENTFLAG_MetaKey = 1 << 3;
    public const int FWL_EVENTFLAG_KeyPad = 1 << 4;
    public const int FWL_EVENTFLAG_AutoRepeat = 1 << 5;
    public const int FWL_EVENTFLAG_LeftButtonDown = 1 << 6;
    public const int FWL_EVENTFLAG_MiddleButtonDown = 1 << 7;
    public const int FWL_EVENTFLAG_RightButtonDown = 1 << 8;

    // Common VKEY constants (add more as needed)
    public const int FWL_VKEY_Back = 0x08;
    public const int FWL_VKEY_Tab = 0x09;
    public const int FWL_VKEY_Return = 0x0D;
    public const int FWL_VKEY_Shift = 0x10;
    public const int FWL_VKEY_Control = 0x11;
    public const int FWL_VKEY_Escape = 0x1B;
    public const int FWL_VKEY_Space = 0x20;
    public const int FWL_VKEY_Left = 0x25;
    public const int FWL_VKEY_Up = 0x26;
    public const int FWL_VKEY_Right = 0x27;
    public const int FWL_VKEY_Down = 0x28;
    public const int FWL_VKEY_Delete = 0x2E;
}


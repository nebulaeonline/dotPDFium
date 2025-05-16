namespace nebulae.dotPDFium.Native;

[Flags]
public enum FpdfKeyModifierFlags
{
    None = 0,
    ShiftKey = 1 << 0,
    ControlKey = 1 << 1,
    AltKey = 1 << 2,
    MetaKey = 1 << 3,
    KeyPad = 1 << 4,
    AutoRepeat = 1 << 5,
    LeftButtonDown = 1 << 6,
    MiddleButtonDown = 1 << 7,
    RightButtonDown = 1 << 8
}

public enum FpdfVirtualKey
{
    Back = 0x08,
    Tab = 0x09,
    Return = 0x0D,
    Shift = 0x10,
    Control = 0x11,
    Escape = 0x1B,
    Space = 0x20,
    Left = 0x25,
    Up = 0x26,
    Right = 0x27,
    Down = 0x28,
    Delete = 0x2E
    // Add more keys as needed
}
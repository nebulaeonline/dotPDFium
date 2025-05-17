using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium.Native;

/// <summary>
/// Specifies the type of an object on a PDF page.
/// </summary>
/// <remarks>This enumeration is used to identify the category of a PDF page object, such as text, images, or
/// paths. It can be useful for processing or analyzing the content of a PDF document.</remarks>
public enum PdfPageObjectType
{
    Unknown = 0,
    Text,
    Path,
    Image,
    Shading,
    Form,
    Mark = 1000,
}

/// <summary>
/// Represents the type of a segment in a PDF path.
/// </summary>
/// <remarks>This enumeration is used to identify the type of drawing operation for a segment in a PDF path. It
/// includes operations such as moving to a point, drawing a line, or creating a Bézier curve.</remarks>
public enum PdfSegmentType
{
    Unknown = -1,
    LineTo = 0,
    BezierTo = 1,
    MoveTo = 2
}

/// <summary>
/// Specifies the fill mode to use when rendering paths in a PDF document.
/// </summary>
/// <remarks>The fill mode determines how the interior of a path is defined when rendering shapes. Use <see
/// cref="None"/> to disable filling, <see cref="Alternate"/> for the alternate fill rule, or <see cref="Winding"/> for
/// the nonzero winding number rule.</remarks>
public enum PdfFillMode
{
    None = 0,
    Alternate = 1,
    Winding = 2
}

/// <summary>
/// Specifies the shape to be used at the ends of open paths when stroking a line in a PDF document.
/// </summary>
/// <remarks>This enumeration defines the available line cap styles for stroking operations in PDF graphics. The
/// line cap determines how the end of a line is rendered when it does not connect to another line.</remarks>
public enum PdfLineCap
{
    Butt = 0,
    Round = 1,
    ProjectingSquare = 2
}

/// <summary>
/// Represents the color space used in a PDF document.
/// </summary>
/// <remarks>A color space defines how colors are represented and interpreted in a PDF.  This enumeration includes
/// standard color spaces such as DeviceRGB and DeviceCMYK,  as well as specialized color spaces like ICCBased and
/// Separation.</remarks>
public enum PdfColorspace
{
    Unknown = 0,
    DeviceGray = 1,
    DeviceRGB = 2,
    DeviceCMYK = 3,
    CalGray = 4,
    CalRGB = 5,
    Lab = 6,
    ICCBased = 7,
    Separation = 8,
    DeviceN = 9,
    Indexed = 10,
    Pattern = 11
}

/// <summary>
/// Represents the subtype of a PDF annotation as defined in the PDF specification.
/// </summary>
/// <remarks>This enumeration provides a comprehensive list of annotation subtypes that can be used in a PDF
/// document. Each subtype corresponds to a specific type of annotation, such as text notes, links, highlights, or
/// multimedia elements.</remarks>
public enum PdfAnnotationSubtype
{
    Unknown = 0,
    Text = 1,
    Link = 2,
    FreeText = 3,
    Line = 4,
    Square = 5,
    Circle = 6,
    Polygon = 7,
    Polyline = 8,
    Highlight = 9,
    Underline = 10,
    Squiggly = 11,
    Strikeout = 12,
    Stamp = 13,
    Caret = 14,
    Ink = 15,
    Popup = 16,
    FileAttachment = 17,
    Sound = 18,
    Movie = 19,
    Widget = 20,
    Screen = 21,
    PrinterMark = 22,
    TrapNet = 23,
    Watermark = 24,
    ThreeD = 25,
    RichMedia = 26,
    XfaWidget = 27,
    Redact = 28
}

/// <summary>
/// Specifies the flags that control the behavior and visibility of a PDF annotation.
/// </summary>
/// <remarks>These flags can be combined using a bitwise OR operation to define multiple behaviors for a single
/// annotation. For example, an annotation can be both <see cref="Invisible"/> and <see cref="NoView"/>.</remarks>
[Flags]
public enum PdfAnnotationFlags
{
    None = 0,
    Invisible = 1 << 0,
    Hidden = 1 << 1,
    Print = 1 << 2,
    NoZoom = 1 << 3,
    NoRotate = 1 << 4,
    NoView = 1 << 5,
    ReadOnly = 1 << 6,
    Locked = 1 << 7,
    ToggleNoView = 1 << 8
}

/// <summary>
/// Specifies the type of color associated with a PDF annotation.
/// </summary>
/// <remarks>This enumeration is used to distinguish between the primary color of the annotation and its interior
/// color, if applicable. The values correspond to specific color roles within a PDF annotation's appearance.</remarks>
public enum PdfAnnotColorType
{
    Color = 0,
    InteriorColor = 1
}

/// <summary>
/// Specifies the types of actions that can be associated with a PDF annotation.
/// </summary>
/// <remarks>These action types represent specific behaviors or events that can be triggered by user interaction
/// with a PDF annotation, such as entering text or performing calculations. Each action type corresponds to a
/// predefined functionality in the PDF specification.</remarks>
public enum PdfAnnotationActionType
{
    KeyStroke = 12,
    Format = 13,
    Validate = 14,
    Calculate = 15
}

/// <summary>
/// Represents the type of value that an attachment can hold.
/// </summary>
/// <remarks>This enumeration defines the possible data types that can be associated with an attachment.  It is
/// used to specify the type of value stored in an attachment, enabling type-safe handling  of attachment
/// data.</remarks>
public enum AttachmentValueType
{
    Boolean = 0,
    Number = 1,
    String = 2,
    Name = 3,
    Array = 4,
    Dictionary = 5,
    Stream = 6
}


/// <summary>
/// Specifies the type of action that can be performed in a PDF document.
/// </summary>
/// <remarks>This enumeration defines the various types of actions that can be associated with interactive
/// elements in a PDF, such as links or buttons. Each action type represents a specific behavior or operation that can
/// be triggered when the action is executed.</remarks>
public enum PdfActionType
{
    Unsupported = 0,
    GoTo = 1,
    RemoteGoTo = 2,
    Uri = 3,
    Launch = 4,
    Named = 5,
    JavaScript = 6,
}

/// <summary>
/// Specifies the view mode for a destination in a PDF document.
/// </summary>
/// <remarks>This enumeration defines the various ways a PDF viewer can display a specific destination within a
/// document. Each value corresponds to a predefined view mode, such as zooming to a specific location, fitting the page
/// to the viewer’s width or height, or displaying a specific rectangle.</remarks>
public enum PdfDestViewMode
{
    Unknown = 0,
    XYZ = 1,
    Fit = 2,
    FitH = 3,
    FitV = 4,
    FitR = 5,
    FitB = 6,
    FitBH = 7,
    FitBV = 8
}

/// <summary>
/// Specifies the type of file identifier used in a PDF document, as defined by the PDF specification.
/// </summary>
/// <remarks>A PDF file identifier consists of two parts: a "permanent" identifier and a "changing" identifier.
/// The <see cref="PdfFileIdType"/> enum is used to distinguish between these two types: <list type="bullet"> <item>
/// <term><see cref="Permanent"/></term> <description>Represents the permanent identifier, which remains constant across
/// revisions of the document.</description> </item> <item> <term><see cref="Changing"/></term> <description>Represents
/// the changing identifier, which is updated with each revision of the document.</description> </item>
/// </list></remarks>
public enum PdfFileIdType
{
    Permanent = 0,
    Changing = 1
}

/// <summary>
/// Represents the types of unsupported features that may be encountered in a PDF document.
/// </summary>
/// <remarks>This enumeration categorizes various unsupported features in PDF documents, such as specific
/// document-level features (e.g., XFA forms, security settings) or annotation types (e.g., 3D annotations, multimedia
/// content). These values can be used to identify and handle unsupported features when processing PDF files.</remarks>
public enum PdfUnsupportedFeatureType
{
    Doc_XfaForm = 1,
    Doc_PortableCollection = 2,
    Doc_Attachment = 3,
    Doc_Security = 4,
    Doc_SharedReview = 5,
    Doc_SharedForm_Acrobat = 6,
    Doc_SharedForm_Filesystem = 7,
    Doc_SharedForm_Email = 8,
    Annot_3DAnnot = 11,
    Annot_Movie = 12,
    Annot_Sound = 13,
    Annot_ScreenMedia = 14,
    Annot_ScreenRichMedia = 15,
    Annot_Attachment = 16,
    Annot_Signature = 17
}

/// <summary>
/// Specifies the page mode to be used when a PDF document is opened.
/// </summary>
/// <remarks>The page mode determines how the document is displayed in a PDF viewer upon opening.  For example, it
/// can specify whether the document is shown in full screen, with a  navigation pane, or with thumbnails. The default
/// behavior depends on the viewer  if <see cref="UseNone"/> is specified.</remarks>
public enum PdfPageMode
{
    Unknown = -1,
    UseNone = 0,
    UseOutlines = 1,
    UseThumbs = 2,
    FullScreen = 3,
    UseOC = 4,
    UseAttachments = 5
}

/// <summary>
/// Represents the result of a PDF flattening operation.
/// </summary>
/// <remarks>This enumeration indicates the outcome of a PDF flattening process, which may fail, succeed,  or
/// determine that no action was necessary. Use this to evaluate the result of the operation  and take appropriate
/// actions based on the value.</remarks>
public enum PdfFlattenResult
{
    Fail = 0,
    Success = 1,
    NothingToDo = 2
}

/// <summary>
/// Specifies the mode used to flatten a PDF document.
/// </summary>
/// <remarks>Flattening a PDF refers to merging interactive elements, such as form fields or annotations,  into
/// the static content of the document. This enumeration defines the modes that control  how the flattening process is
/// applied.</remarks>
public enum PdfFlattenMode
{
    NormalDisplay = 0,
    Print = 1
}

/// <summary>
/// Specifies the type of form contained within a PDF document.
/// </summary>
/// <remarks>This enumeration is used to identify the form type in a PDF file, which can influence how the form is
/// processed or rendered.</remarks>
public enum PdfFormType
{
    None = 0,
    AcroForm = 1,
    XfaFull = 2,
    XfaForeground = 3
}

/// <summary>
/// Specifies the type of a form field in a PDF document.
/// </summary>
/// <remarks>This enumeration is used to identify the type of a form field within a PDF document. Each value
/// corresponds to a specific type of interactive field, such as buttons, text fields, or signature fields.</remarks>
public enum PdfFormFieldType
{
    Unknown = 0,
    PushButton = 1,
    CheckBox = 2,
    RadioButton = 3,
    ComboBox = 4,
    ListBox = 5,
    TextField = 6,
    Signature = 7
    // XFA values 8–15 if needed
}

/// <summary>
/// Specifies the types of actions that can be triggered in a PDF document lifecycle.
/// </summary>
/// <remarks>These actions represent specific events that occur during the lifecycle of a PDF document,  such as
/// before closing, before saving, or after printing. They can be used to define  custom behaviors or workflows
/// associated with these events.</remarks>
public enum PdfDocActionType
{
    BeforeClose = 0x10,   // WC
    BeforeSave = 0x11,    // WS
    AfterSave = 0x12,     // DS
    BeforePrint = 0x13,   // WP
    AfterPrint = 0x14     // DP
}

/// <summary>
/// Specifies the types of actions that can be performed on a PDF page.
/// </summary>
/// <remarks>This enumeration is used to indicate whether an action should occur when a PDF page is opened or
/// closed.</remarks>
public enum PdfPageActionType
{
    Open = 0,
    Close = 1
}

/// <summary>
/// Specifies modifier flags that represent the state of keys or mouse buttons during a PDF-related input event.
/// </summary>
/// <remarks>This enumeration supports bitwise combination of its values due to the <see cref="FlagsAttribute"/>.
/// Use these flags to determine which modifier keys or mouse buttons were active during an input event.</remarks>
[Flags]
public enum PdfKeyModifierFlags
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

/// <summary>
/// Represents virtual key codes for common keyboard keys used in PDF-related operations.
/// </summary>
/// <remarks>This enumeration provides a set of virtual key codes that correspond to commonly used keyboard keys.
/// These values can be used to handle keyboard input in PDF-related applications, such as navigation or interaction
/// with PDF viewers. The values are based on standard virtual key codes.</remarks>
public enum PdfVirtualKey
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

/// <summary>
/// Represents the status of a PDF rendering operation.
/// </summary>
/// <remarks>This enumeration is used to indicate the current state of a PDF rendering process.  The status can be
/// one of the following: <list type="bullet"> <item> <description><see cref="Ready"/>: The rendering process is ready
/// to start.</description> </item> <item> <description><see cref="ToBeContinued"/>: The rendering process is in
/// progress and requires additional steps to complete.</description> </item> <item> <description><see cref="Done"/>:
/// The rendering process has completed successfully.</description> </item> <item> <description><see cref="Failed"/>:
/// The rendering process has failed.</description> </item> </list></remarks>
public enum PdfRenderStatus
{
    Ready = 0,
    ToBeContinued = 1,
    Done = 2,
    Failed = 3
}

/// <summary>
/// Specifies flags that control the behavior of saving a PDF document.
/// </summary>
/// <remarks>This enumeration supports bitwise combination of its values due to the <see cref="FlagsAttribute"/>.
/// Use these flags to customize how a PDF document is saved, such as enabling incremental updates or removing security
/// settings.</remarks>
[Flags]
public enum PdfSaveFlags
{
    None = 0,
    Incremental = 1,
    NoIncremental = 2,
    RemoveSecurity = 3,
}

/// <summary>
/// Specifies the modification permissions for a PDF document when using  Document Modification Detection and Prevention
/// (MDP).
/// </summary>
/// <remarks>These permissions define the level of changes allowed in a PDF document  while maintaining its
/// integrity under MDP. The permissions range from  disallowing all changes to allowing specific types of
/// modifications, such  as form filling or signing.</remarks>
public enum PdfDocMdpPermission
{
    NoChanges = 1,
    FormFillAndSign = 2,
    AnnotateFormFillAndSign = 3
}

/// <summary>
/// Specifies character set values used to define the encoding of text in fonts.
/// </summary>
/// <remarks>This enumeration is commonly used to specify the character set for font rendering or text encoding.
/// Each value corresponds to a specific character set, which determines how characters are mapped to glyphs.</remarks>
public enum FxFontCharset
{
    ANSI = 0,
    Default = 1,
    Symbol = 2,
    ShiftJIS = 128,
    Hangeul = 129,
    GB2312 = 134,
    ChineseBig5 = 136,
    Greek = 161,
    Vietnamese = 163,
    Hebrew = 177,
    Arabic = 178,
    Cyrillic = 204,
    Thai = 222,
    EasternEuropean = 238
}

/// <summary>
/// Specifies the pitch and family classification of a font.
/// </summary>
/// <remarks>This enumeration is used to define font characteristics such as whether the font has a fixed pitch or
/// belongs to a specific family, such as Roman or Script. The values can be combined using a bitwise OR operation due
/// to the <see cref="FlagsAttribute"/> applied to the enumeration.</remarks>
[Flags]
public enum FxFontPitchFamily
{
    FixedPitch = 1 << 0,
    Roman = 1 << 4,
    Script = 4 << 4
}

/// <summary>
/// Specifies font characteristics and attributes for a PDF font.
/// </summary>
/// <remarks>This enumeration uses the <see cref="FlagsAttribute"/> to allow a bitwise combination of its member
/// values. Each flag represents a specific font attribute, such as whether the font is italic, symbolic, or
/// serif.</remarks>
[Flags]
public enum PdfFontFlags
{
    None = 0,
    FixedPitch = 1 << 0,  // bit 1
    Serif = 1 << 1,  // bit 2
    Symbolic = 1 << 2,  // bit 3
    Script = 1 << 3,  // bit 4
    Nonsymbolic = 1 << 5,  // bit 6 (bit 5 is reserved)
    Italic = 1 << 6,  // bit 7
    AllCap = 1 << 16, // bit 17
    SmallCap = 1 << 17, // bit 18
    ForceBold = 1 << 18  // bit 19
}

/// <summary>
/// Specifies the weight of a font, which determines the thickness of the characters.
/// </summary>
/// <remarks>The font weight is represented as an integer value, where higher values indicate thicker weights.
/// Commonly used values include <see cref="Normal"/> for regular text and <see cref="Bold"/> for bold text.</remarks>
public enum FxFontWeight
{
    Normal = 400,
    Bold = 700
}

/// <summary>
/// Specifies the bitmap formats supported for rendering in a PDF context.
/// </summary>
/// <remarks>This enumeration defines various pixel formats that can be used when working with bitmap images in a
/// PDF. Each format represents a specific arrangement of color channels and, in some cases, transparency
/// information.</remarks>
public enum PdfBitmapFormat
{
    Unknown = 0,
    Gray = 1,
    BGR = 2,
    BGRx = 3,
    BGRA = 4,
    BGRAPremul = 5
}

/// <summary>
/// Represents the types of objects that can be processed in the FPF (File Processing Framework).
/// </summary>
/// <remarks>This enumeration defines the various object types that may be encountered or manipulated within the
/// FPF system. Each value corresponds to a specific type of object, such as a boolean, number, string, or more complex
/// structures like arrays and dictionaries.</remarks>
public enum FpfObjectType
{
    Unknown = 0,
    Boolean = 1,
    Number = 2,
    String = 3,
    Name = 4,
    Array = 5,
    Dictionary = 6,
    Stream = 7,
    NullObj = 8,
    Reference = 9
}

/// <summary>
/// Specifies rendering options for PDF content, allowing fine-grained control over how pages are rendered.
/// </summary>
/// <remarks>This enumeration supports bitwise combination of its values due to the <see cref="FlagsAttribute"/>.
/// Use these flags to customize rendering behavior, such as enabling annotations, adjusting text smoothing, or
/// optimizing for printing. Some flags may affect performance or output quality, so choose options based on the
/// specific rendering requirements.</remarks>
[Flags]
public enum PdfRenderFlags
{
    None = 0,
    Annot = 0x01,
    LcdText = 0x02,
    NoNativeText = 0x04,
    Grayscale = 0x08,
    DebugInfo = 0x80,
    NoCatch = 0x100,
    LimitedImageCache = 0x200,
    ForceHalftone = 0x400,
    Printing = 0x800,
    NoSmoothText = 0x1000,
    NoSmoothImage = 0x2000,
    NoSmoothPath = 0x4000,
    ReverseByteOrder = 0x10,
    ConvertFillToStroke = 0x20
}

/// <summary>
/// Specifies the duplex printing options for a PDF document.
/// </summary>
/// <remarks>This enumeration defines the possible duplex (double-sided) printing modes for a PDF document.  Use
/// these values to indicate whether and how the pages should be printed on both sides of the paper.</remarks>
public enum PdfDuplexType
{
    Undefined = 0,
    Simplex = 1,
    FlipShortEdge = 2,
    FlipLongEdge = 3
}

/// <summary>
/// This enum represents the rotation of the PDF page.
/// </summary>
public enum PdfPageRotation
{
    NoRotation = 0,
    Rotate90 = 1,
    Rotate180 = 2,
    Rotate270 = 3
}

/// <summary>
/// Specifies the PDF file version supported by a document or operation.
/// </summary>
/// <remarks>The values of this enumeration correspond to the major and minor versions of the PDF specification.
/// For example, <see cref="PdfFileVersion.Pdf14"/> represents PDF version 1.4.</remarks>
public enum PdfFileVersion
{
    Pdf14 = 14,
    Pdf15 = 15,
    Pdf16 = 16,
    Pdf17 = 17
}

/// <summary>
/// Specifies the type of font used in a PDF document or text object.
/// </summary>
public enum PdfFontType
{
    Type1 = 1,
    TrueType = 2
}

/// <summary>
/// Specifies the drawing mode for a path in a PDF document.
/// </summary>
/// <remarks>This enumeration defines the options for rendering a path in a PDF, including filling, stroking, or
/// both. It supports bitwise combination of its values due to the <see cref="FlagsAttribute"/>.</remarks>
[Flags]
public enum PdfPathDrawMode
{
    None = 0,
    Fill = 1,
    Stroke = 2,
    FillStroke = Fill | Stroke
}

public enum PdfTextRenderMode
{
    Fill = 0,
    Stroke = 1,
    FillStroke = 2,
    Invisible = 3,
    FillClip = 4,
    StrokeClip = 5,
    FillStrokeClip = 6,
    Clip = 7
}

public enum PdfMarkParamType
{
    None = 0,
    Int = 1,
    String = 2,
    Blob = 3
}
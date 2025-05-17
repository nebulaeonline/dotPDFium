using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

/// <summary>
/// Represents an action within a PDF document, such as a hyperlink or a named action.
/// </summary>
/// <remarks>A <see cref="PdfAction"/> encapsulates a specific action defined in a PDF document.  Actions can
/// include navigating to a URI, executing a named action, or other types of actions  as defined by the PDF
/// specification. Use the <see cref="Type"/> property to determine the  type of action and the appropriate methods to
/// retrieve additional details.</remarks>
public sealed class PdfAction
{
    private readonly IntPtr _handle;

    /// <summary>
    /// Represents an action within a PDF document.
    /// </summary>
    /// <remarks>This constructor is intended for internal use only and initializes the action with a native
    /// handle.</remarks>
    /// <param name="handle">A pointer to the native PDF action handle. This must be a valid handle.</param>
    internal PdfAction(IntPtr handle)
    {
        _handle = handle;
    }

    /// <summary>
    /// Gets the type of the PDF action represented by this instance.
    /// </summary>
    public PdfActionType Type
    {
        get
        {
            uint type = PdfDocNative.FPDFAction_GetType(_handle);
            return Enum.IsDefined(typeof(PdfActionType), type)
                ? (PdfActionType)type
                : PdfActionType.Unsupported;
        }
    }

    /// <summary>
    /// Retrieves the destination associated with the current action in the specified PDF document.
    /// </summary>
    /// <remarks>The destination represents a specific location within the PDF document, such as a page or a
    /// view.  Use this method to navigate to or retrieve information about the target location of the action.</remarks>
    /// <param name="doc">The <see cref="PdfDocument"/> instance representing the PDF document containing the action.</param>
    /// <returns>A <see cref="PdfDestination"/> object representing the destination associated with the action,  or <see
    /// langword="null"/> if no destination is associated.</returns>
    public PdfDestination? GetDestination(PdfDocument doc)
    {
        var dest = PdfDocNative.FPDFAction_GetDest(doc.Handle, _handle);
        return dest == IntPtr.Zero ? null : new PdfDestination(dest, doc);
    }

    /// <summary>
    /// Retrieves the file path associated with the current PDF action.
    /// </summary>
    /// <remarks>The returned file path is decoded using UTF-8 encoding and may be trimmed of any trailing
    /// null characters.</remarks>
    /// <returns>The file path as a string, or <see langword="null"/> if no file path is associated with the action.</returns>
    public string? GetFilePath()
    {
        uint length = PdfDocNative.FPDFAction_GetFilePath(_handle, Array.Empty<byte>(), 0);
        if (length == 0) return null;

        byte[] buffer = new byte[length];
        PdfDocNative.FPDFAction_GetFilePath(_handle, buffer, length);

        return Encoding.UTF8.GetString(buffer, 0, (int)length).TrimEnd('\0');
    }

    /// <summary>
    /// Retrieves the URI associated with the specified PDF document action.
    /// </summary>
    /// <remarks>The returned URI is encoded in UTF-8 and may include a null terminator, which is removed
    /// before returning the result.</remarks>
    /// <param name="doc">The <see cref="PdfDocument"/> containing the action for which the URI is retrieved.</param>
    /// <returns>The URI as a string if the action has an associated URI; otherwise, <see langword="null"/>.</returns>
    public string? GetUri(PdfDocument doc)
    {
        // First call to get the length (including null terminator)
        uint length = PdfDocNative.FPDFAction_GetURIPath(doc.Handle, _handle, Array.Empty<byte>(), 0);
        if (length == 0)
            return null;

        byte[] buffer = new byte[length];
        uint written = PdfDocNative.FPDFAction_GetURIPath(doc.Handle, _handle, buffer, length);

        return System.Text.Encoding.UTF8.GetString(buffer, 0, (int)written).TrimEnd('\0');
    }

    /// <summary>
    /// Retrieves the name of a named action.
    /// </summary>
    /// <returns>Always returns <see langword="null"/> as the underlying implementation does not support retrieving the name of
    /// named actions.</returns>
    public string? GetNamedAction()
    {
        // PDFium does not support retrieving the actual name of named actions
        return null;
    }

    /// <summary>
    /// Gets the handle associated with the current instance.
    /// </summary>
    internal IntPtr Handle => _handle;
}


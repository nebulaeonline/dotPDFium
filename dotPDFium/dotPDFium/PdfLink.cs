using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

/// <summary>
/// Represents a link annotation within a PDF document.
/// </summary>
/// <remarks>A <see cref="PdfLink"/> object provides access to the destination or action associated with a link
/// annotation in a PDF document. Use this class to retrieve information about where the link points to, such as a
/// specific page or an external action.</remarks>
public sealed class PdfLink : PdfPageObject
{
    private readonly PdfDocument _document;

    internal PdfLink(IntPtr handle, PdfDocument document)
        : base(handle)
    {
        _document = document;
    }

    /// <summary>
    /// Retrieves the destination associated with the current PDF link.
    /// </summary>
    /// <remarks>The destination provides information about where the link points to within the PDF document, 
    /// such as a specific page or location. If the link does not have an associated destination,  the method returns
    /// <see langword="null"/>.</remarks>
    /// <returns>A <see cref="PdfDestination"/> object representing the destination of the link,  or <see langword="null"/> if no
    /// destination is associated with the link.</returns>
    public PdfDestination? GetDestination()
    {
        var destHandle = PdfDocNative.FPDFLink_GetDest(_document.Handle, Handle);
        return destHandle == IntPtr.Zero ? null : new PdfDestination(destHandle, _document);
    }

    /// <summary>
    /// Retrieves the action associated with the current PDF link, if any.
    /// </summary>
    /// <returns>A <see cref="PdfAction"/> object representing the action associated with the link,  or <see langword="null"/> if
    /// no action is associated.</returns>
    public PdfAction? GetAction()
    {
        var actionHandle = PdfDocNative.FPDFLink_GetAction(Handle);
        return actionHandle == IntPtr.Zero ? null : new PdfAction(actionHandle);
    }
}

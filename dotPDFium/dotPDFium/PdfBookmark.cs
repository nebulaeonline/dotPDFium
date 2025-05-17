using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

/// <summary>
/// Represents a bookmark in a PDF document, providing access to its title, hierarchical structure,  and associated
/// actions or destinations.
/// </summary>
/// <remarks>A <see cref="PdfBookmark"/> allows navigation through the bookmark hierarchy of a PDF document.  It
/// provides methods to retrieve child bookmarks, sibling bookmarks, and associated actions or  destinations. Bookmarks
/// are typically used to represent a table of contents or navigation points  within a PDF document.</remarks>
public sealed class PdfBookmark
{
    private readonly IntPtr _handle;
    private readonly PdfDocument _document;

    /// <summary>
    /// Constructor for creating a new PdfBookmark instance. This constructor is intended for internal use only.
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="document"></param>
    internal PdfBookmark(IntPtr handle, PdfDocument document)
    {
        _handle = handle;
        _document = document;
    }

    /// <summary>
    /// Gets the title of the bookmark represented by this instance.
    /// </summary>
    public string Title
    {
        get
        {
            uint len = PdfDocNative.FPDFBookmark_GetTitle(_handle, Array.Empty<char>(), 0);
            if (len == 0)
                return string.Empty;

            var buffer = new char[len];
            uint written = PdfDocNative.FPDFBookmark_GetTitle(_handle, buffer, len);

            return new string(buffer, 0, (int)written).TrimEnd('\0');
        }
    }

    /// <summary>
    /// Gets the number of child bookmarks associated with the current bookmark.
    /// </summary>
    /// <remarks>Use this method to determine the number of child bookmarks for a given bookmark in a PDF
    /// document. This can be useful for navigating or processing hierarchical bookmark structures.</remarks>
    /// <returns>The number of child bookmarks. Returns 0 if the current bookmark has no children.</returns>
    public int GetChildCount()
    {
        return PdfDocNative.FPDFBookmark_GetCount(_handle);
    }

    /// <summary>
    /// Retrieves the first child bookmark of the current bookmark.
    /// </summary>
    /// <remarks>Use this method to navigate the hierarchy of bookmarks in a PDF document. If the current
    /// bookmark has no child bookmarks, the method returns <see langword="null"/>.</remarks>
    /// <returns>A <see cref="PdfBookmark"/> representing the first child bookmark, or <see langword="null"/> if the current
    /// bookmark has no children.</returns>
    public PdfBookmark? GetFirstChild()
    {
        var child = PdfDocNative.FPDFBookmark_GetFirstChild(_document.Handle, _handle);
        return child == IntPtr.Zero ? null : new PdfBookmark(child, _document);
    }

    /// <summary>
    /// Retrieves the next sibling of the current bookmark in the document's bookmark hierarchy.
    /// </summary>
    /// <remarks>This method navigates the bookmark hierarchy of the PDF document. If the current bookmark is
    /// the last sibling in its level, the method returns <see langword="null"/>.</remarks>
    /// <returns>A <see cref="PdfBookmark"/> object representing the next sibling of the current bookmark, or <see
    /// langword="null"/> if there is no next sibling.</returns>
    public PdfBookmark? GetNextSibling()
    {
        var next = PdfDocNative.FPDFBookmark_GetNextSibling(_document.Handle, _handle);
        return next == IntPtr.Zero ? null : new PdfBookmark(next, _document);
    }

    /// <summary>
    /// Retrieves the destination associated with the current bookmark.
    /// </summary>
    /// <remarks>The destination provides information about the location or view in the PDF document  that the
    /// bookmark points to. If the bookmark does not have an associated destination,  the method returns <see
    /// langword="null"/>.</remarks>
    /// <returns>A <see cref="PdfDestination"/> object representing the destination linked to the bookmark,  or <see
    /// langword="null"/> if no destination is associated.</returns>
    public PdfDestination? GetDestination()
    {
        var dest = PdfDocNative.FPDFBookmark_GetDest(_document.Handle, _handle);
        return dest == IntPtr.Zero ? null : new PdfDestination(dest, _document);
    }

    /// <summary>
    /// Retrieves the action associated with the current PDF bookmark, if one exists.
    /// </summary>
    /// <remarks>Use this method to obtain the action linked to a bookmark in a PDF document, such as 
    /// navigating to a specific page or executing a script. If the bookmark does not have an  associated action, the
    /// method returns <see langword="null"/>.</remarks>
    /// <returns>A <see cref="PdfAction"/> object representing the action associated with the bookmark,  or <see
    /// langword="null"/> if no action is associated.</returns>
    public PdfAction? GetAction()
    {
        var action = PdfDocNative.FPDFBookmark_GetAction(_handle);
        return action == IntPtr.Zero ? null : new PdfAction(action);
    }

    /// <summary>
    /// Gets the handle representing a native resource.
    /// </summary>
    internal IntPtr Handle => _handle;
}


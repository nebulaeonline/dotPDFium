using System;
using System.Runtime.InteropServices;
using System.Text;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public sealed class PdfFormTextEditor
{
    private readonly PdfFormContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfFormTextEditor"/> class,  providing functionality to edit text
    /// fields in a PDF form.
    /// </summary>
    /// <param name="context">The context representing the PDF form to be edited.  This parameter cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is <see langword="null"/>.</exception>
    public PdfFormTextEditor(PdfFormContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Selects all text on the specified PDF form.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> object representing the page on which to select all text. Cannot be null.</param>
    /// <returns><see langword="true"/> if all text on the page was successfully selected; otherwise, <see langword="false"/>.</returns>
    public bool SelectAllText(PdfPage page)
    {
        return PdfFormFillNative.FORM_SelectAllText(_context.Handle, page.Handle);
    }

    /// <summary>
    /// Replaces the current text selection on the specified PDF page with the provided text.
    /// </summary>
    /// <remarks>This method modifies the content of the PDF page by replacing the currently selected text
    /// with the specified text. Ensure that a valid text selection exists on the page before calling this
    /// method.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> object representing the page where the text selection will be replaced.</param>
    /// <param name="text">The text to replace the current selection with. Cannot be null.</param>
    public void ReplaceSelection(PdfPage page, string text)
    {
        PdfFormFillNative.FORM_ReplaceSelection(_context.Handle, page.Handle, text);
    }

    /// <summary>
    /// Replaces the current selection on the specified PDF page with the provided text,  while preserving the selection
    /// state.
    /// </summary>
    /// <param name="page">The PDF page on which the replacement is performed. Cannot be null.</param>
    /// <param name="text">The text to replace the current selection with. Cannot be null or empty.</param>
    public void ReplaceAndKeepSelection(PdfPage page, string text)
    {
        PdfFormFillNative.FORM_ReplaceAndKeepSelection(_context.Handle, page.Handle, text);
    }

    /// <summary>
    /// Determines whether the specified PDF page has an undoable action.
    /// </summary>
    /// <param name="page">The PDF page to check for undoable actions. Cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the specified page has an action that can be undone; otherwise, <see
    /// langword="false"/>.</returns>
    public bool CanUndo(PdfPage page) => PdfFormFillNative.FORM_CanUndo(_context.Handle, page.Handle);
    
    /// <summary>
    /// Determines whether the specified PDF page has a redo operation available.
    /// </summary>
    /// <param name="page">The PDF page to check for redo availability. Cannot be null.</param>
    /// <returns><see langword="true"/> if a redo operation is available for the specified page; otherwise, <see
    /// langword="false"/>.</returns>
    public bool CanRedo(PdfPage page) => PdfFormFillNative.FORM_CanRedo(_context.Handle, page.Handle);
    
    /// <summary>
    /// Reverts the most recent change made to the specified PDF page.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> object representing the page to undo the last change on.</param>
    /// <returns><see langword="true"/> if the undo operation was successful; otherwise, <see langword="false"/>.</returns>
    public bool Undo(PdfPage page) => PdfFormFillNative.FORM_Undo(_context.Handle, page.Handle);
    
    /// <summary>
    /// Reapplies the most recently undone action on the specified PDF page.
    /// </summary>
    /// <remarks>This method reverts the effect of the most recent undo operation performed on the specified
    /// page. If there are no actions to redo, the method returns <see langword="false"/>.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> on which to redo the last undone action. Cannot be <c>null</c>.</param>
    /// <returns><see langword="true"/> if the redo operation was successful; otherwise, <see langword="false"/>.</returns>
    public bool Redo(PdfPage page) => PdfFormFillNative.FORM_Redo(_context.Handle, page.Handle);

    /// <summary>
    /// Retrieves the text currently selected on the specified PDF page.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> object representing the page from which to retrieve the selected text. Cannot be null.</param>
    /// <returns>A string containing the selected text, or <see langword="null"/> if no text is selected.</returns>
    public string? GetSelectedText(PdfPage page)
    {
        return ReadUtf16FormText((buffer, buflen) =>
            PdfFormFillNative.FORM_GetSelectedText(_context.Handle, page.Handle, buffer, buflen));
    }

    /// <summary>
    /// Retrieves the text currently in focus within a specified PDF form field on the given page.
    /// </summary>
    /// <remarks>This method is typically used to retrieve user-entered text from a form field that currently
    /// has focus. If no form field is focused, the method returns <see langword="null"/>.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> object representing the page containing the focused form field.</param>
    /// <returns>A <see langword="string"/> containing the text of the focused form field, or <see langword="null"/> if no form
    /// field is focused.</returns>
    public string? GetFocusedText(PdfPage page)
    {
        return ReadUtf16FormText((buffer, buflen) =>
            PdfFormFillNative.FORM_GetFocusedText(_context.Handle, page.Handle, buffer, buflen));
    }

    /// <summary>
    /// Reads UTF-16 encoded text from a native function and returns it as a string.
    /// </summary>
    /// <remarks>The method allocates a buffer of up to 4096 UTF-16 characters (8192 bytes) to receive the
    /// text. If the native function writes no data (returns 0 bytes), the method returns <see langword="null"/>.
    /// Otherwise, the text is converted from UTF-16 to a managed string.</remarks>
    /// <param name="nativeCall">A delegate representing the native function to invoke. The function should write UTF-16 encoded text to the
    /// provided buffer and return the number of bytes written.</param>
    /// <returns>A string containing the UTF-16 encoded text returned by the native function, or <see langword="null"/> if no
    /// text was written.</returns>
    private static string? ReadUtf16FormText(Func<IntPtr, uint, uint> nativeCall)
    {
        const int maxChars = 4096;
        int byteCount = maxChars * 2; // UTF-16 = 2 bytes per char

        IntPtr buffer = Marshal.AllocHGlobal(byteCount);
        try
        {
            uint actualBytes = nativeCall(buffer, (uint)byteCount);
            if (actualBytes == 0)
                return null;

            return Marshal.PtrToStringUni(buffer, (int)actualBytes / 2);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}

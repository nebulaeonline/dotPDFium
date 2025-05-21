using nebulae.dotPDFium.Native;
using System;
using System.Text;

namespace nebulae.dotPDFium.Forms;

/// <summary>
/// Dispatches user-driven events to the PDFium form environment.
/// These methods mirror PDFium's FORM_* notification APIs (see fpdf_formfill.h).
/// </summary>
public static class PdfFormEventsNotifier
{
    /// <summary>
    /// Performs post-processing actions on a PDF page after it has been loaded into a form.
    /// </summary>
    /// <remarks>This method should be called after a page is loaded to ensure that any necessary form-related
    /// adjustments or initializations are applied to the page. It is typically used in workflows involving interactive
    /// forms or annotations.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> instance associated with the PDF document.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the loaded page.</param>
    public static void OnAfterLoadPage(PdfForm form, PdfPage page) =>
        PdfFormFillNative.FORM_OnAfterLoadPage(page.Handle, form.Handle);

    /// <summary>
    /// Performs necessary operations before a PDF page is closed.
    /// </summary>
    /// <remarks>This method ensures that any required cleanup or finalization tasks are performed for the
    /// specified page before it is closed. Both <paramref name="form"/> and <paramref name="page"/> must be valid and
    /// initialized.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> associated with the PDF document. Cannot be <see langword="null"/>.</param>
    /// <param name="page">The <see cref="PdfPage"/> that is about to be closed. Cannot be <see langword="null"/>.</param>
    public static void OnBeforeClosePage(PdfForm form, PdfPage page) =>
        PdfFormFillNative.FORM_OnBeforeClosePage(page.Handle, form.Handle);

    /// <summary>
    /// Executes the JavaScript action associated with the document in the specified PDF form.
    /// </summary>
    /// <remarks>This method triggers the document-level JavaScript action defined in the PDF form.  Ensure
    /// that the <see cref="PdfForm"/> instance is properly initialized before calling this method.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> object representing the PDF form on which the JavaScript action will be executed.
    /// Cannot be <see langword="null"/>.</param>
    public static void DoDocumentJSAction(PdfForm form) =>
        PdfFormFillNative.FORM_DoDocumentJSAction(form.Handle);

    /// <summary>
    /// Executes the document's open action, if defined, for the specified PDF form.
    /// </summary>
    /// <remarks>This method triggers any actions associated with the document's open event, such as scripts
    /// or other predefined behaviors. Ensure that the <paramref name="form"/> is properly initialized before calling
    /// this method.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> representing the PDF document. This parameter cannot be <see langword="null"/>.</param>
    public static void DoDocumentOpenAction(PdfForm form) =>
        PdfFormFillNative.FORM_DoDocumentOpenAction(form.Handle);

    /// <summary>
    /// Executes a document-level action on the specified PDF form.
    /// </summary>
    /// <remarks>Document-level actions are predefined actions in a PDF document that are triggered by
    /// specific events, such as opening or closing the document. Use this method to programmatically invoke such
    /// actions on a PDF form.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the PDF form on which the action will be performed. Cannot be
    /// <see langword="null"/>.</param>
    /// <param name="action">The document-level action to execute, specified as a <see cref="PdfPageAActionType"/> value.</param>
    public static void DoDocumentAAction(PdfForm form, PdfPageAActionType action) =>
        PdfFormFillNative.FORM_DoDocumentAAction(form.Handle, (int)action);

    /// <summary>
    /// Executes a specific page-level additional action on a PDF page within a form.
    /// </summary>
    /// <remarks>This method triggers a predefined additional action associated with the specified page in the
    /// context of the provided form. The action type is determined by the <paramref name="action"/> parameter, which
    /// must be a valid member of the <see cref="PdfPageAActionType"/> enumeration.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form containing the page.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page on which the action will be performed.</param>
    /// <param name="action">The type of additional action to execute, specified as a <see cref="PdfPageAActionType"/> value.</param>
    public static void DoPageAAction(PdfForm form, PdfPage page, PdfPageAActionType action) =>
        PdfFormFillNative.FORM_DoPageAAction(page.Handle, form.Handle, (int)action);

    /// <summary>
    /// Handles a left mouse button down event on a PDF form at the specified location.
    /// </summary>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form being interacted with.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page containing the form.</param>
    /// <param name="modifier">An integer representing any modifier keys pressed during the event (e.g., Shift, Ctrl).</param>
    /// <param name="x">The x-coordinate of the mouse pointer, in the page's coordinate system.</param>
    /// <param name="y">The y-coordinate of the mouse pointer, in the page's coordinate system.</param>
    public static void OnLButtonDown(PdfForm form, PdfPage page, int modifier, double x, double y) =>
        PdfFormFillNative.FORM_OnLButtonDown(form.Handle, page.Handle, modifier, x, y);

    /// <summary>
    /// Handles the left mouse button release event for a PDF form on a specified page.
    /// </summary>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form to interact with.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page where the event occurred.</param>
    /// <param name="modifier">An integer representing the state of modifier keys (e.g., Shift, Ctrl) at the time of the event.</param>
    /// <param name="x">The x-coordinate of the mouse pointer, in page coordinates, at the time of the event.</param>
    /// <param name="y">The y-coordinate of the mouse pointer, in page coordinates, at the time of the event.</param>
    public static void OnLButtonUp(PdfForm form, PdfPage page, int modifier, double x, double y) =>
        PdfFormFillNative.FORM_OnLButtonUp(form.Handle, page.Handle, modifier, x, y);

    /// <summary>
    /// Handles a left mouse button double-click event on a PDF form.
    /// </summary>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form where the event occurred.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page containing the form.</param>
    /// <param name="modifier">An integer representing the state of modifier keys (e.g., Shift, Ctrl, Alt) during the event.</param>
    /// <param name="x">The x-coordinate, in page space, where the double-click occurred.</param>
    /// <param name="y">The y-coordinate, in page space, where the double-click occurred.</param>
    public static void OnLButtonDoubleClick(PdfForm form, PdfPage page, int modifier, double x, double y) =>
        PdfFormFillNative.FORM_OnLButtonDoubleClick(form.Handle, page.Handle, modifier, x, y);

    /// <summary>
    /// Handles a right mouse button down event on a PDF form at the specified location.
    /// </summary>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form being interacted with.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page where the event occurred.</param>
    /// <param name="modifier">An integer representing the state of modifier keys (e.g., Ctrl, Alt, Shift) during the event.</param>
    /// <param name="x">The x-coordinate, in page space, where the event occurred.</param>
    /// <param name="y">The y-coordinate, in page space, where the event occurred.</param>
    public static void OnRButtonDown(PdfForm form, PdfPage page, int modifier, double x, double y) =>
        PdfFormFillNative.FORM_OnRButtonDown(form.Handle, page.Handle, modifier, x, y);

    /// <summary>
    /// Handles the right mouse button release event for a PDF form on a specified page.
    /// </summary>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form being interacted with.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page where the event occurred.</param>
    /// <param name="modifier">An integer representing the state of modifier keys (e.g., Ctrl, Alt, Shift) at the time of the event.</param>
    /// <param name="x">The x-coordinate, in page space, where the right mouse button was released.</param>
    /// <param name="y">The y-coordinate, in page space, where the right mouse button was released.</param>
    public static void OnRButtonUp(PdfForm form, PdfPage page, int modifier, double x, double y) =>
        PdfFormFillNative.FORM_OnRButtonUp(form.Handle, page.Handle, modifier, x, y);

    /// <summary>
    /// Handles the mouse move event for a PDF form on a specific page.
    /// </summary>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form to interact with.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page where the mouse event occurred.</param>
    /// <param name="modifier">An integer representing the state of modifier keys (e.g., Shift, Ctrl, Alt) during the mouse event.</param>
    /// <param name="x">The x-coordinate of the mouse pointer, in page coordinates.</param>
    /// <param name="y">The y-coordinate of the mouse pointer, in page coordinates.</param>
    public static void OnMouseMove(PdfForm form, PdfPage page, int modifier, double x, double y) =>
        PdfFormFillNative.FORM_OnMouseMove(form.Handle, page.Handle, modifier, x, y);

    /// <summary>
    /// Handles mouse wheel events for a PDF form on a specific page.
    /// </summary>
    /// <param name="form">The <see cref="PdfForm"/> instance associated with the PDF document.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page where the event occurred.</param>
    /// <param name="modifier">An integer representing the state of modifier keys (e.g., Ctrl, Alt, Shift) during the event.</param>
    /// <param name="coord">The coordinates, in points, where the mouse wheel event occurred.</param>
    /// <param name="deltaX">The horizontal scroll delta, in points, caused by the mouse wheel.</param>
    /// <param name="deltaY">The vertical scroll delta, in points, caused by the mouse wheel.</param>
    public static void OnMouseWheel(PdfForm form, PdfPage page, int modifier, FsPointF coord, int deltaX, int deltaY) =>
        PdfFormFillNative.FORM_OnMouseWheel(form.Handle, page.Handle, modifier, ref coord, deltaX, deltaY);

    /// <summary>
    /// Handles a key-down event for a PDF form on a specified page.
    /// </summary>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form to process the key event for.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page containing the form.</param>
    /// <param name="keyCode">The key code of the key that was pressed.</param>
    /// <param name="modifier">The modifier key state (e.g., Shift, Ctrl, Alt) at the time of the key press.</param>
    public static void OnKeyDown(PdfForm form, PdfPage page, int keyCode, int modifier) =>
        PdfFormFillNative.FORM_OnKeyDown(form.Handle, page.Handle, keyCode, modifier);

    /// <summary>
    /// Handles the key-up event for a PDF form on a specified page.
    /// </summary>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form where the event occurred.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page containing the form.</param>
    /// <param name="keyCode">The code of the key that was released.</param>
    /// <param name="modifier">The modifier key state at the time of the key release, such as Shift, Ctrl, or Alt.</param>
    public static void OnKeyUp(PdfForm form, PdfPage page, int keyCode, int modifier) =>
        PdfFormFillNative.FORM_OnKeyUp(form.Handle, page.Handle, keyCode, modifier);

    /// <summary>
    /// Handles a character input event for a PDF form on a specific page.
    /// </summary>
    /// <remarks>This method processes character input events, such as typing into a form field, and applies
    /// the input to the specified form and page. Ensure that both <paramref name="form"/> and <paramref name="page"/>
    /// are valid and properly initialized before calling this method.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form to which the character input is applied.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page containing the form.</param>
    /// <param name="charCode">The Unicode character code of the input character.</param>
    /// <param name="modifier">An integer representing any modifier keys (e.g., Shift, Ctrl) that were active during the input event.</param>
    public static void OnChar(PdfForm form, PdfPage page, int charCode, int modifier) =>
        PdfFormFillNative.FORM_OnChar(form.Handle, page.Handle, charCode, modifier);


    /// <summary>
    /// Handles the focus event for a PDF form field.
    /// </summary>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form containing the field.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page where the form field is located.</param>
    /// <param name="modifier">An integer representing the modifier keys pressed during the focus event.  This value is typically a bitmask of
    /// key states (e.g., Shift, Ctrl, Alt).</param>
    /// <param name="x">The x-coordinate, in points, of the focus event relative to the page.</param>
    /// <param name="y">The y-coordinate, in points, of the focus event relative to the page.</param>
    public static void OnFocus(PdfForm form, PdfPage page, int modifier, double x, double y) =>
        PdfFormFillNative.FORM_OnFocus(form.Handle, page.Handle, modifier, x, y);


    /// <summary>
    /// Replaces the current text selection on the specified PDF page with the provided text.
    /// </summary>
    /// <remarks>This method modifies the content of the specified PDF page by replacing the currently
    /// selected text with the provided string. Ensure that the <paramref name="form"/> and <paramref name="page"/>
    /// objects are valid and properly initialized before calling this method.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> object representing the form associated with the PDF document. Cannot be <see
    /// langword="null"/>.</param>
    /// <param name="page">The <see cref="PdfPage"/> object representing the page where the text selection will be replaced. Cannot be <see
    /// langword="null"/>.</param>
    /// <param name="text">The text to replace the current selection with. If <see langword="null"/> or empty, the selection will be
    /// cleared.</param>
    public static void ReplaceSelection(PdfForm form, PdfPage page, string text) =>
        PdfFormFillNative.FORM_ReplaceSelection(form.Handle, page.Handle, text);

    /// <summary>
    /// Replaces the current text in the specified PDF form field while preserving the user's selection.
    /// </summary>
    /// <remarks>This method ensures that the user's current selection within the form field is preserved
    /// after the text is replaced.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> object representing the form containing the field to update. Cannot be <see
    /// langword="null"/>.</param>
    /// <param name="page">The <see cref="PdfPage"/> object representing the page containing the form field. Cannot be <see
    /// langword="null"/>.</param>
    /// <param name="text">The new text to replace the current content of the form field. Cannot be <see langword="null"/> or empty.</param>
    public static void ReplaceAndKeepSelection(PdfForm form, PdfPage page, string text) =>
        PdfFormFillNative.FORM_ReplaceAndKeepSelection(form.Handle, page.Handle, text);

    /// <summary>
    /// Selects all text within the specified PDF form and page.
    /// </summary>
    /// <remarks>This method is typically used to programmatically select all text on a specific page of a PDF
    /// form. The selection state may depend on the underlying PDF library's implementation.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> object representing the form containing the text to select. Cannot be <see
    /// langword="null"/>.</param>
    /// <param name="page">The <see cref="PdfPage"/> object representing the page within the form. Cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the text was successfully selected; otherwise, <see langword="false"/>.</returns>
    public static bool SelectAllText(PdfForm form, PdfPage page) =>
        PdfFormFillNative.FORM_SelectAllText(form.Handle, page.Handle);


    /// <summary>
    /// Determines whether the specified index is selected in the given PDF form on the specified page.
    /// </summary>
    /// <remarks>This method checks the selection state of an index within a PDF form field on a specific
    /// page. Ensure that the <paramref name="form"/> and <paramref name="page"/> are valid and properly
    /// initialized.</remarks>
    /// <param name="form">The PDF form containing the selection state.</param>
    /// <param name="page">The page of the PDF form to check.</param>
    /// <param name="index">The index to check for selection.</param>
    /// <returns><see langword="true"/> if the specified index is selected; otherwise, <see langword="false"/>.</returns>
    public static bool IsIndexSelected(PdfForm form, PdfPage page, int index) =>
        PdfFormFillNative.FORM_IsIndexSelected(form.Handle, page.Handle, index);

    /// <summary>
    /// Sets the selection state of a specific index within a form on a given PDF page.
    /// </summary>
    /// <remarks>This method modifies the selection state of an item within a form field on a specific PDF
    /// page. Ensure that the provided <paramref name="form"/> and <paramref name="page"/> are valid and correspond to
    /// the same document.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form to modify.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page containing the form.</param>
    /// <param name="index">The zero-based index of the item to update within the form.</param>
    /// <param name="selected"><see langword="true"/> to mark the item at the specified index as selected; otherwise, <see langword="false"/>
    /// to deselect it.</param>
    /// <returns><see langword="true"/> if the selection state was successfully updated; otherwise, <see langword="false"/>.</returns>
    public static bool SetIndexSelected(PdfForm form, PdfPage page, int index, bool selected) =>
        PdfFormFillNative.FORM_SetIndexSelected(form.Handle, page.Handle, index, selected);

    /// <summary>
    /// Retrieves the currently focused annotation within the specified PDF form, if any.
    /// </summary>
    /// <remarks>This method queries the PDF form to determine if an annotation currently has focus. If an
    /// annotation is  focused, its associated page index and annotation object are returned. If no annotation is
    /// focused,  <paramref name="pageIndex"/> is set to -1 and <paramref name="annotation"/> is set to <see
    /// langword="null"/>.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form to query.</param>
    /// <param name="pageIndex">When this method returns, contains the zero-based index of the page containing the focused annotation,  or -1 if
    /// no annotation is focused.</param>
    /// <param name="annotation">When this method returns, contains the <see cref="PdfAnnotation"/> object representing the focused annotation, 
    /// or <see langword="null"/> if no annotation is focused.</param>
    /// <returns><see langword="true"/> if a focused annotation is found; otherwise, <see langword="false"/>.</returns>
    public static bool GetFocusedAnnot(PdfForm form, out int pageIndex, out PdfAnnotation? annotation)
    {
        annotation = null;
        bool result = PdfFormFillNative.FORM_GetFocusedAnnot(form.Handle, out pageIndex, out var annotHandle);
        if (result && annotHandle != IntPtr.Zero)
            annotation = new PdfAnnotation(annotHandle, form.Document.ResolvePage(pageIndex)!);
        return result;
    }

    /// <summary>
    /// Sets the specified annotation as the focused annotation within the given PDF form.
    /// </summary>
    /// <param name="form">The PDF form containing the annotation to be focused. Cannot be <see langword="null"/>.</param>
    /// <param name="annotation">The annotation to set as focused. Cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the annotation was successfully set as focused; otherwise, <see langword="false"/>.</returns>
    public static bool SetFocusedAnnot(PdfForm form, PdfAnnotation annotation) =>
        PdfFormFillNative.FORM_SetFocusedAnnot(form.Handle, annotation.Handle);

    /// <summary>
    /// Determines whether the specified form and page have an undoable action available.
    /// </summary>
    /// <param name="form">The PDF form to check for undoable actions. Cannot be <see langword="null"/>.</param>
    /// <param name="page">The PDF page associated with the form. Cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if there is an undoable action available for the specified form and page; otherwise, <see
    /// langword="false"/>.</returns>
    public static bool CanUndo(PdfForm form, PdfPage page) =>
        PdfFormFillNative.FORM_CanUndo(form.Handle, page.Handle);

    /// <summary>
    /// Determines whether the specified PDF form can redo the last undone action on the given page.
    /// </summary>
    /// <param name="form">The PDF form to check for redo capability.</param>
    /// <param name="page">The page within the PDF form to check for redo capability.</param>
    /// <returns><see langword="true"/> if the last undone action on the specified page can be redone; otherwise, <see
    /// langword="false"/>.</returns>
    public static bool CanRedo(PdfForm form, PdfPage page) =>
        PdfFormFillNative.FORM_CanRedo(form.Handle, page.Handle);

    /// <summary>
    /// Reverts the most recent change made to the specified PDF form on the given page.
    /// </summary>
    /// <param name="form">The PDF form to undo the last change for. Cannot be <see langword="null"/>.</param>
    /// <param name="page">The page of the PDF form where the undo operation is performed. Cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the undo operation was successful; otherwise, <see langword="false"/>.</returns>
    public static bool Undo(PdfForm form, PdfPage page) =>
        PdfFormFillNative.FORM_Undo(form.Handle, page.Handle);

    /// <summary>
    /// Reapplies the most recently undone action on the specified PDF form and page.
    /// </summary>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form to which the redo operation applies.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page on which the redo operation is performed.</param>
    /// <returns><see langword="true"/> if the redo operation is successful; otherwise, <see langword="false"/>.</returns>
    public static bool Redo(PdfForm form, PdfPage page) =>
        PdfFormFillNative.FORM_Redo(form.Handle, page.Handle);

    /// <summary>
    /// Retrieves the text from the currently focused field in a PDF form on the specified page.
    /// </summary>
    /// <remarks>The returned text is encoded in UTF-8. If the focused field is empty, this method returns
    /// <see langword="null"/>.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the PDF form.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page containing the focused field.</param>
    /// <returns>A <see langword="string"/> containing the text of the focused field, or <see langword="null"/> if no field is
    /// focused or the field is empty.</returns>
    public static string? GetFocusedText(PdfForm form, PdfPage page)
    {
        uint len = PdfFormFillNative.FORM_GetFocusedText(form.Handle, page.Handle, IntPtr.Zero, 0);
        if (len == 0)
            return null;

        byte[] buffer = new byte[len];
        unsafe
        {
            fixed (byte* ptr = buffer)
            {
                PdfFormFillNative.FORM_GetFocusedText(form.Handle, page.Handle, (IntPtr)ptr, len);
            }
        }

        return Encoding.UTF8.GetString(buffer, 0, (int)(len - 1));
    }

    /// <summary>
    /// Retrieves the text currently selected within the specified PDF form and page.
    /// </summary>
    /// <remarks>The returned string is encoded in UTF-8. If no text is selected, the method returns <see
    /// langword="null"/>.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> instance representing the form containing the selection.</param>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page containing the selection.</param>
    /// <returns>A <see langword="string"/> containing the selected text, or <see langword="null"/> if no text is selected.</returns>
    public static string? GetSelectedText(PdfForm form, PdfPage page)
    {
        uint len = PdfFormFillNative.FORM_GetSelectedText(form.Handle, page.Handle, IntPtr.Zero, 0);
        if (len == 0)
            return null;

        byte[] buffer = new byte[len];
        unsafe
        {
            fixed (byte* ptr = buffer)
            {
                PdfFormFillNative.FORM_GetSelectedText(form.Handle, page.Handle, (IntPtr)ptr, len);
            }
        }

        return Encoding.UTF8.GetString(buffer, 0, (int)(len - 1));
    }

    /// <summary>
    /// Forces the specified PDF form to lose focus.
    /// </summary>
    /// <remarks>This method is typically used to ensure that no form field within the specified PDF form
    /// retains focus. It may be useful in scenarios where focus-related behavior needs to be reset or
    /// overridden.</remarks>
    /// <param name="form">The <see cref="PdfForm"/> instance whose focus will be forcibly removed. Cannot be null.</param>
    public static void ForceKillFocus(PdfForm form) =>
        PdfFormFillNative.FORM_ForceToKillFocus(form.Handle);
}

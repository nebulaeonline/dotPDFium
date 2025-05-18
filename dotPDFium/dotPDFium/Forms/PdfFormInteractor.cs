using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public sealed class PdfFormInteractor
{
    private readonly PdfFormContext _context;

    public PdfFormInteractor(PdfFormContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Executes a document-level action in the PDF document.
    /// </summary>
    /// <remarks>Document-level actions are triggered by specific events in the lifecycle of a PDF document. 
    /// The <paramref name="actionType"/> parameter determines which action is executed.</remarks>
    /// <param name="actionType">The type of document-level action to execute. This specifies the predefined action to be performed,  such as
    /// opening, closing, or saving the document.</param>
    public void DoDocumentAAction(PdfDocumentAActionType actionType)
    {
        PdfFormFillNative.FORM_DoDocumentAAction(_context.Handle, (int)actionType);
    }

    /// <summary>
    /// Handles the mouse move event for a specified PDF page.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page where the mouse move event occurred.</param>
    /// <param name="modifiers">An integer representing the state of modifier keys (e.g., Shift, Ctrl, Alt) during the event.</param>
    /// <param name="x">The x-coordinate of the mouse pointer, in page coordinates.</param>
    /// <param name="y">The y-coordinate of the mouse pointer, in page coordinates.</param>
    /// <returns><see langword="true"/> if the event was successfully handled; otherwise, <see langword="false"/>.</returns>
    public bool OnMouseMove(PdfPage page, int modifiers, double x, double y)
    {
        return PdfFormFillNative.FORM_OnMouseMove(_context.Handle, page.Handle, modifiers, x, y);
    }

    /// <summary>
    /// Handles a left mouse button down event on a specified PDF page.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> where the event occurred. Cannot be <c>null</c>.</param>
    /// <param name="modifiers">A bitmask representing the state of modifier keys (e.g., Shift, Ctrl) at the time of the event.</param>
    /// <param name="x">The x-coordinate of the mouse pointer, in page coordinates.</param>
    /// <param name="y">The y-coordinate of the mouse pointer, in page coordinates.</param>
    /// <returns><see langword="true"/> if the event was successfully handled; otherwise, <see langword="false"/>.</returns>
    public bool OnLButtonDown(PdfPage page, int modifiers, double x, double y)
    {
        return PdfFormFillNative.FORM_OnLButtonDown(_context.Handle, page.Handle, modifiers, x, y);
    }

    /// <summary>
    /// Handles the left mouse button release event on a specified PDF page.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> where the event occurred. Cannot be null.</param>
    /// <param name="modifiers">A bitmask representing the state of modifier keys (e.g., Shift, Ctrl) at the time of the event.</param>
    /// <param name="x">The x-coordinate of the mouse pointer, in page coordinates, where the event occurred.</param>
    /// <param name="y">The y-coordinate of the mouse pointer, in page coordinates, where the event occurred.</param>
    /// <returns><see langword="true"/> if the event was successfully handled; otherwise, <see langword="false"/>.</returns>
    public bool OnLButtonUp(PdfPage page, int modifiers, double x, double y)
    {
        return PdfFormFillNative.FORM_OnLButtonUp(_context.Handle, page.Handle, modifiers, x, y);
    }

    /// <summary>
    /// Handles a left mouse button double-click event on a PDF page.
    /// </summary>
    /// <remarks>This method processes the double-click event in the context of a PDF form. The behavior may
    /// depend on the specific form elements present at the given coordinates.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> where the double-click occurred. Cannot be null.</param>
    /// <param name="modifiers">A bitmask representing the modifier keys (e.g., Ctrl, Alt, Shift) that were pressed during the event.</param>
    /// <param name="x">The x-coordinate of the mouse pointer, in page coordinates, at the time of the double-click.</param>
    /// <param name="y">The y-coordinate of the mouse pointer, in page coordinates, at the time of the double-click.</param>
    /// <returns><see langword="true"/> if the double-click event was handled successfully; otherwise, <see langword="false"/>.</returns>
    public bool OnLButtonDoubleClick(PdfPage page, int modifiers, double x, double y)
    {
        return PdfFormFillNative.FORM_OnLButtonDoubleClick(_context.Handle, page.Handle, modifiers, x, y);
    }

    /// <summary>
    /// Handles a right mouse button down event on a specified PDF page.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> where the event occurred. Cannot be null.</param>
    /// <param name="modifiers">A bitmask representing the modifier keys (e.g., Ctrl, Alt, Shift) that were pressed during the event.</param>
    /// <param name="x">The x-coordinate of the mouse pointer, in page coordinates, where the event occurred.</param>
    /// <param name="y">The y-coordinate of the mouse pointer, in page coordinates, where the event occurred.</param>
    /// <returns><see langword="true"/> if the event was successfully handled; otherwise, <see langword="false"/>.</returns>
    public bool OnRButtonDown(PdfPage page, int modifiers, double x, double y)
    {
        return PdfFormFillNative.FORM_OnRButtonDown(_context.Handle, page.Handle, modifiers, x, y);
    }

    /// <summary>
    /// Handles a right mouse button release event on a specified PDF page.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> where the event occurred. Cannot be <c>null</c>.</param>
    /// <param name="modifiers">A bitmask representing the state of modifier keys (e.g., Shift, Ctrl) at the time of the event.</param>
    /// <param name="x">The x-coordinate of the mouse pointer, in page coordinates, where the event occurred.</param>
    /// <param name="y">The y-coordinate of the mouse pointer, in page coordinates, where the event occurred.</param>
    /// <returns><see langword="true"/> if the event was successfully handled; otherwise, <see langword="false"/>.</returns>
    public bool OnRButtonUp(PdfPage page, int modifiers, double x, double y)
    {
        return PdfFormFillNative.FORM_OnRButtonUp(_context.Handle, page.Handle, modifiers, x, y);
    }

    /// <summary>
    /// Handles mouse wheel events for a specified PDF page.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> instance representing the page where the event occurred.</param>
    /// <param name="modifiers">A bitmask representing the state of modifier keys (e.g., Shift, Ctrl, Alt) during the event.</param>
    /// <param name="coord">The coordinates of the mouse pointer, in page space, at the time of the event.</param>
    /// <param name="deltaX">The horizontal scroll delta, in device units, caused by the mouse wheel.</param>
    /// <param name="deltaY">The vertical scroll delta, in device units, caused by the mouse wheel.</param>
    /// <returns><see langword="true"/> if the event was handled successfully; otherwise, <see langword="false"/>.</returns>
    public bool OnMouseWheel(PdfPage page, int modifiers, FsPointF coord, int deltaX, int deltaY)
    {
        return PdfFormFillNative.FORM_OnMouseWheel(_context.Handle, page.Handle, modifiers, ref coord, deltaX, deltaY);
    }

    /// <summary>
    /// Handles focus events for a PDF form field at the specified location on the page.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> where the focus event occurs. Cannot be <see langword="null"/>.</param>
    /// <param name="modifiers">A bitmask representing the modifier keys (e.g., Ctrl, Alt, Shift) active during the focus event.</param>
    /// <param name="x">The x-coordinate, in page space, where the focus event occurs.</param>
    /// <param name="y">The y-coordinate, in page space, where the focus event occurs.</param>
    /// <returns><see langword="true"/> if the focus event was successfully handled; otherwise, <see langword="false"/>.</returns>
    public bool OnFocus(PdfPage page, int modifiers, double x, double y)
    {
        return PdfFormFillNative.FORM_OnFocus(_context.Handle, page.Handle, modifiers, x, y);
    }

    /// <summary>
    /// Handles a key-down event for the specified PDF page.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> on which the key-down event occurred. Cannot be <c>null</c>.</param>
    /// <param name="keyCode">The code of the key that was pressed.</param>
    /// <param name="modifiers">The modifier keys (e.g., Shift, Ctrl, Alt) that were active during the key press.</param>
    /// <returns><see langword="true"/> if the key-down event was handled successfully; otherwise, <see langword="false"/>.</returns>
    public bool OnKeyDown(PdfPage page, int keyCode, int modifiers)
    {
        return PdfFormFillNative.FORM_OnKeyDown(_context.Handle, page.Handle, keyCode, modifiers);
    }

    /// <summary>
    /// Handles a key-up event for a specified PDF page.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> on which the key-up event occurred. Cannot be <c>null</c>.</param>
    /// <param name="keyCode">The code of the key that was released.</param>
    /// <param name="modifiers">The modifier keys (e.g., Shift, Ctrl, Alt) that were active during the key-up event.</param>
    /// <returns><see langword="true"/> if the key-up event was handled successfully; otherwise, <see langword="false"/>.</returns>
    public bool OnKeyUp(PdfPage page, int keyCode, int modifiers)
    {
        return PdfFormFillNative.FORM_OnKeyUp(_context.Handle, page.Handle, keyCode, modifiers);
    }

    /// <summary>
    /// Handles a character input event for the specified PDF page.
    /// </summary>
    /// <param name="page">The <see cref="PdfPage"/> on which the character input event occurs. Cannot be null.</param>
    /// <param name="charCode">The Unicode character code of the input character.</param>
    /// <param name="modifiers">The modifier keys (e.g., Shift, Ctrl) active during the character input event, represented as a bitmask.</param>
    /// <returns><see langword="true"/> if the character input event was successfully processed; otherwise, <see
    /// langword="false"/>.</returns>
    public bool OnChar(PdfPage page, int charCode, int modifiers)
    {
        return PdfFormFillNative.FORM_OnChar(_context.Handle, page.Handle, charCode, modifiers);
    }

    /// <summary>
    /// Forces the current PDF form field to lose focus.
    /// </summary>
    /// <remarks>This method is typically used to programmatically remove focus from a form field in a PDF
    /// document. It ensures that any active input or interaction with the field is terminated.</remarks>
    public void KillFocus()
    {
        PdfFormFillNative.FORM_ForceToKillFocus(_context.Handle);
    }

    /// <summary>
    /// Retrieves the currently focused annotation in the PDF document, if any.
    /// </summary>
    /// <returns>A tuple containing the page index and a handle to the focused annotation,  or <see langword="null"/> if no
    /// annotation is currently focused.</returns>
    public (int pageIndex, nint annot)? GetFocusedAnnotation()
    {
        if (PdfFormFillNative.FORM_GetFocusedAnnot(_context.Handle, out int pageIndex, out nint annot))
            return (pageIndex, annot);

        return null;
    }

    /// <summary>
    /// Sets the specified annotation as the focused annotation in the PDF form.
    /// </summary>
    /// <remarks>This method updates the focus to the specified annotation within the PDF form context. 
    /// Ensure that the provided annotation handle is valid and associated with the current PDF form context.</remarks>
    /// <param name="annot">The handle of the annotation to focus. Must be a valid annotation handle.</param>
    /// <returns><see langword="true"/> if the annotation was successfully set as focused; otherwise, <see langword="false"/>.</returns>
    public bool SetFocusedAnnotation(nint annot)
    {
        return PdfFormFillNative.FORM_SetFocusedAnnot(_context.Handle, annot);
    }

    /// <summary>
    /// Sets the selection state of a specific index within a PDF page.
    /// </summary>
    /// <remarks>This method interacts with the underlying PDF form fill context to update the selection state
    /// of the specified index. Ensure that the <paramref name="index"/> is within the valid range for the given
    /// <paramref name="page"/>.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> object representing the PDF page containing the index.</param>
    /// <param name="index">The zero-based index to update the selection state for.</param>
    /// <param name="selected"><see langword="true"/> to mark the index as selected; otherwise, <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the selection state was successfully updated; otherwise, <see langword="false"/>.</returns>
    public bool SetIndexSelected(PdfPage page, int index, bool selected)
    {
        return PdfFormFillNative.FORM_SetIndexSelected(_context.Handle, page.Handle, index, selected);
    }

    /// <summary>
    /// Determines whether the specified index is selected on the given PDF page.
    /// </summary>
    /// <remarks>This method checks the selection state of a specific index within the context of the provided
    /// PDF page.</remarks>
    /// <param name="page">The <see cref="PdfPage"/> object representing the PDF page to check.</param>
    /// <param name="index">The index to evaluate for selection.</param>
    /// <returns><see langword="true"/> if the specified index is selected; otherwise, <see langword="false"/>.</returns>
    public bool IsIndexSelected(PdfPage page, int index)
    {
        return PdfFormFillNative.FORM_IsIndexSelected(_context.Handle, page.Handle, index);
    }
}


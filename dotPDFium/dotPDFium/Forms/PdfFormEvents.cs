using System;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public sealed class PdfFormEvents
{
    /// <summary>
    /// Invoked when a region of a PDF page needs to be redrawn due to form updates.
    /// </summary>
    public Action<PdfPage, FsRectF>? OnInvalidate { get; set; }

    /// <summary>
    /// Invoked when the cursor should be changed (e.g., to a text beam or hand).
    /// </summary>
    public Action<int>? OnCursorChange { get; set; }

    /// <summary>
    /// Invoked when a form field value or state has changed.
    /// </summary>
    public Action? OnFormChanged { get; set; }

    // Future event hooks can go here, such as:
    // public Func<string, bool>? OnNamedAction { get; set; }
    // public Func<int, bool>? OnTimerElapsed { get; set; }
}

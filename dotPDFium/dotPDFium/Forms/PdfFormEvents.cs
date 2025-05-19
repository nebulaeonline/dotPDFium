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

    /// <summary>
    /// Invoked when PDFium wants to draw a selection rectangle (e.g., for text selection).
    /// </summary>
    public Action<PdfPage, FsRectF>? OnOutputSelectedRect { get; set; }

    /// <summary>
    /// Provides a timer callback for PDFium's internal use (e.g., cursor blink).
    /// Returns a platform-defined timer ID.
    /// </summary>
    public Func<int /* elapseMs */, Action /* callback */, int>? OnSetTimer { get; set; }

    /// <summary>
    /// Called when PDFium wants to cancel a timer.
    /// </summary>
    public Action<int /* timerId */>? OnKillTimer { get; set; }

    /// <summary>
    /// Provides the current system time to PDFium.
    /// </summary>
    public Func<FpdfSystemTime>? OnGetLocalTime { get; set; }

    /// <summary>
    /// Called when PDFium wants to retrieve a page from a document by index.
    /// </summary>
    public Func<PdfDocument, int /* pageIndex */, PdfPage?>? OnGetPage { get; set; }

    /// <summary>
    /// Called when PDFium wants to know the rotation of a given page.
    /// </summary>
    public Func<PdfPage, int>? OnGetRotation { get; set; }

    /// <summary>
    /// Optional release callback if any resources should be cleaned up when the form environment is destroyed.
    /// </summary>
    public Action? OnRelease { get; set; }
}

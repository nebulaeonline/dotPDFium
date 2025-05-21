using nebulae.dotPDFium.Drawing;
using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium.Drawing
{
    /// <summary>
    /// Provides methods for drawing shapes and paths on a PDF document.
    /// </summary>
    /// <remarks>The <see cref="Drawing"/> class includes static methods for creating and manipulating
    /// graphical elements  such as lines and rectangles on a PDF path object. These methods allow customization of
    /// drawing options,  including stroke and fill properties, and ensure proper handling of PDF path
    /// operations.</remarks>
    public static class Drawing
    {
        /// <summary>
        /// Draws a straight line between two points on a PDF path object.
        /// </summary>
        /// <remarks>This method modifies the specified <paramref name="path"/> by adding a line segment
        /// from the <paramref name="start"/> point to the <paramref name="end"/> point. If the <paramref
        /// name="options"/> specify that the path should be closed, the method will attempt to close the path after
        /// drawing the line.</remarks>
        /// <param name="path">The <see cref="PdfPathObject"/> to which the line will be added. Cannot be <see langword="null"/>.</param>
        /// <param name="options">The <see cref="DrawingOptions"/> that specify how the line should be drawn, including whether the path
        /// should be closed.</param>
        /// <param name="start">The starting point of the line, represented as an <see cref="FsPointF"/>.</param>
        /// <param name="end">The ending point of the line, represented as an <see cref="FsPointF"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is <see langword="null"/>.</exception>
        /// <exception cref="dotPDFiumException">Thrown if the operation to move to the start point, draw the line, or close the path fails.</exception>
        public static void DrawLine(PdfPathObject path, DrawingOptions options, FsPointF start, FsPointF end)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var handle = path.Handle;

            if (!PdfEditNative.FPDFPath_MoveTo(handle, start.X, start.Y))
                throw new dotPDFiumException($"Failed to move to start point for line: {PdfObject.GetPDFiumError()}");

            if (!PdfEditNative.FPDFPath_LineTo(handle, end.X, end.Y))
                throw new dotPDFiumException($"Failed to draw line to end point: {PdfObject.GetPDFiumError()}");

            if (options.ClosePath && !PdfEditNative.FPDFPath_Close(handle))
                throw new dotPDFiumException($"Failed to close path for line: {PdfObject.GetPDFiumError()}");

            ApplyDrawingOptions(handle, options);
        }

        /// <summary>
        /// Draws a rectangle on the specified PDF path object.
        /// </summary>
        /// <remarks>This method creates a rectangular path by moving to the top-left corner of the
        /// specified rectangle,  drawing lines to the other corners, and closing the path. The specified drawing
        /// options are then applied  to render the rectangle.</remarks>
        /// <param name="path">The <see cref="PdfPathObject"/> on which the rectangle will be drawn. Cannot be <see langword="null"/>.</param>
        /// <param name="options">The <see cref="DrawingOptions"/> that specify how the rectangle should be rendered, such as stroke and fill
        /// settings.</param>
        /// <param name="rect">The <see cref="FsRectF"/> structure defining the rectangle's position and dimensions.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is <see langword="null"/>.</exception>
        /// <exception cref="dotPDFiumException">Thrown if an error occurs while drawing the rectangle or closing the path.</exception>
        public static void DrawRect(PdfPathObject path, DrawingOptions options, FsRectF rect)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            float l = rect.left, t = rect.top, r = rect.right, b = rect.bottom;
            var handle = path.Handle;

            if (!PdfEditNative.FPDFPath_MoveTo(handle, l, t))
                throw new dotPDFiumException($"Failed to move to top-left corner: {PdfObject.GetPDFiumError()}");

            if (!PdfEditNative.FPDFPath_LineTo(handle, r, t) ||
                !PdfEditNative.FPDFPath_LineTo(handle, r, b) ||
                !PdfEditNative.FPDFPath_LineTo(handle, l, b))
            {
                throw new dotPDFiumException($"Failed to draw rectangle edges: {PdfObject.GetPDFiumError()}");
            }

            if (!PdfEditNative.FPDFPath_Close(handle))
                throw new dotPDFiumException($"Failed to close rectangle path: {PdfObject.GetPDFiumError()}");

            ApplyDrawingOptions(handle, options);
        }

        /// <summary>
        /// Applies the specified drawing options to a PDF path object.
        /// </summary>
        /// <remarks>This method configures the drawing behavior of the PDF path object based on the
        /// provided options.  It sets properties such as fill color, stroke color, stroke width, line cap, and line
        /// join.</remarks>
        /// <param name="path">A pointer to the PDF path object to which the drawing options will be applied.</param>
        /// <param name="opts">The <see cref="DrawingOptions"/> containing the drawing settings, such as colors, stroke width, and line
        /// styles.</param>
        internal static void ApplyDrawingOptions(IntPtr path, DrawingOptions opts)
        {
            PdfEditNative.FPDFPath_SetDrawMode(path,
                opts.FillColor.HasValue ? 1 /* fill or fill+stroke */ : 0,
                stroke: true
            );

            var stroke = opts.StrokeColor;
            PdfEditNative.FPDFPageObj_SetStrokeColor(path, stroke.R, stroke.G, stroke.B, stroke.A);
            PdfEditNative.FPDFPageObj_SetStrokeWidth(path, opts.StrokeWidth);
            PdfEditNative.FPDFPageObj_SetLineCap(path, opts.LineCap);
            PdfEditNative.FPDFPageObj_SetLineJoin(path, opts.LineJoin);

            if (opts.FillColor.HasValue)
            {
                var fill = opts.FillColor.Value;
                PdfEditNative.FPDFPageObj_SetFillColor(path, fill.R, fill.G, fill.B, fill.A);
            }
        }
    }
}

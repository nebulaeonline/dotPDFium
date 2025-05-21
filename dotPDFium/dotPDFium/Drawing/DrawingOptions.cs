using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium.Drawing
{
    public readonly struct DrawingOptions
    {
        public RgbaColor StrokeColor { get; init; }
        public RgbaColor? FillColor { get; init; } // null = no fill
        public float StrokeWidth { get; init; }
        public int LineCap { get; init; }  // 0: Butt, 1: Round, 2: Square
        public int LineJoin { get; init; } // 0: Miter, 1: Round, 2: Bevel
        public bool ClosePath { get; init; }

        public static DrawingOptions Default => new()
        {
            StrokeColor = new RgbaColor(0, 0, 0, 255),
            FillColor = null,
            StrokeWidth = 1.0f,
            LineCap = 0,
            LineJoin = 0,
            ClosePath = false
        };
    }
}

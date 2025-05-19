using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public sealed class PdfStreamAvailabilityOptions
{
    public required uint FileLength { get; init; }

    public required object UserData { get; init; }

    public required PdfGetBlockDelegate GetBlock { get; init; }
    public required PdfIsDataAvailDelegate IsDataAvailable { get; init; }
    public required PdfAddSegmentDelegate RequestSegment { get; init; }
}



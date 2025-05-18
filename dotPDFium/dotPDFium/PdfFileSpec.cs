using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

/// <summary>
/// Represents a file specification in a PDF document.
/// </summary>
/// <remarks>This class provides a representation of a file specification object within a PDF document. File
/// specifications are used to reference external files or embedded files in a PDF. Instances of this class are
/// typically obtained through PDF processing libraries and are not intended to be created directly by user
/// code.</remarks>
public sealed class PdfFileSpec
{
    internal IntPtr Handle { get; }

    internal PdfFileSpec(IntPtr handle)
    {
        Handle = handle;
    }
}


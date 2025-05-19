using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

/// <summary>
/// Delegate to read data into a buffer from a backing source (Stream, network, etc.).
/// Used with FPDF_FILEACCESS.
/// </summary>
public delegate int PdfGetBlockDelegate(IntPtr userData, uint offset, IntPtr buffer, uint size);

/// <summary>
/// Delegate to report whether a given byte range is already available.
/// Used with FX_FILEAVAIL.
/// </summary>
public delegate int PdfIsDataAvailDelegate(object userData, ulong offset, ulong length);

/// <summary>
/// Delegate called by PDFium to request that a segment be downloaded or made available.
/// Used with FX_DOWNLOADHINTS.
/// </summary>
public delegate void PdfAddSegmentDelegate(ulong offset, ulong length);


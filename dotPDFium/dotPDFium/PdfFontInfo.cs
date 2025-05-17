using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public class PdfFontInfo
{
    public string Name { get; }
    public int Flags { get; }

    public PdfFontInfo(string name, int flags)
    {
        Name = name;
        Flags = flags;
    }
}
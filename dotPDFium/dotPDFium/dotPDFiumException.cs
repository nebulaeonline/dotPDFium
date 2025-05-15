using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium
{
    public class dotPDFiumException : Exception
    {
        public dotPDFiumException() { }
        public dotPDFiumException(string message) : base(message) { }
        public dotPDFiumException(string message, Exception innerException) : base(message, innerException) { }
    }
}

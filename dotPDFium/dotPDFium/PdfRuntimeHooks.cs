using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static nebulae.dotPDFium.Native.PdfExtNative;

namespace nebulae.dotPDFium;

internal static class PdfRuntimeHooks
{
    public static void SetTimeFunction(TimeFunc func)
        => PdfExtNative.FSDK_SetTimeFunction(func);

    public static void SetLocalTimeFunction(LocalTimeFunc func)
        => PdfExtNative.FSDK_SetLocaltimeFunction(func);
}

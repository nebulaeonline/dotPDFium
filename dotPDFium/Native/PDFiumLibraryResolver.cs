using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium.Native
{
    internal class PDFiumLibraryResolver
    {
        private const string DllName = "pdfium";

        private static bool _initialized = false;

        public static void Register()
        {
            if (_initialized) return;

            NativeLibrary.SetDllImportResolver(typeof(PDFiumLibraryResolver).Assembly, Resolve);
            _initialized = true;
        }

        private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? paths)
        {
            if (!libraryName.Equals(DllName, StringComparison.OrdinalIgnoreCase))
                return IntPtr.Zero;

            string baseDir = AppContext.BaseDirectory;
            string rid = GetRuntimeIdentifier();

            string libName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "pdfium.dll" :
                             RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "libpdfium.dylib" :
                             "libpdfium.so";

            string fullPath = Path.Combine(baseDir, "runtimes", rid, "native", libName);
            if (File.Exists(fullPath))
                return NativeLibrary.Load(fullPath);

            throw new DllNotFoundException($"Failed to locate native PDFium binary at: {fullPath}");
        }

        private static string GetRuntimeIdentifier()
        {
            return (RuntimeInformation.IsOSPlatform(OSPlatform.Windows), RuntimeInformation.OSArchitecture) switch
            {
                (true, Architecture.X64) => "win-x64",
                (true, Architecture.Arm64) => "win-arm64",
                (false, Architecture.X64) when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => "linux-x64",
                (false, Architecture.Arm64) when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => "linux-arm64",
                (false, _) when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => "osx-universal", // supports both arm64 + x64
                _ => throw new PlatformNotSupportedException("Unsupported platform/architecture combination")
            };
        }
    }
}

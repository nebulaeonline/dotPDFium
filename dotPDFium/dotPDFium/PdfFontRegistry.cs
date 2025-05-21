using System.Drawing.Text;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium
{
    /// <summary>
    /// Manages system font registration and integration with the PDFium engine.
    /// </summary>
    public static class PdfFontRegistry
    {
        private static IntPtr _fontInfo;
        private static bool _initialized;

        /// <summary>
        /// Initializes the font registry using PDFium's default system font provider.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized)
                return;

            _fontInfo = PdfSystemFontNative.FPDF_GetDefaultSystemFontInfo();

            if (_fontInfo == IntPtr.Zero)
                throw new dotPDFiumException("Failed to initialize default PDFium system font info.");

            PdfSystemFontNative.FPDF_SetSystemFontInfo(_fontInfo);
            _initialized = true;
        }

        /// <summary>
        /// Registers an installed font with the specified face name and charset.
        /// </summary>
        /// <param name="faceName">Font face name (e.g., 'Arial')</param>
        /// <param name="charset">Windows charset code, usually 0 (ANSI)</param>
        public static void AddInstalledFont(string faceName, int charset = 0)
        {
            if (!_initialized)
                throw new InvalidOperationException("Font registry not initialized.");

            PdfSystemFontNative.FPDF_AddInstalledFont(_fontInfo, faceName, charset);
        }

        public static IEnumerable<string> GetInstalledFontNames()
        {
            if (OperatingSystem.IsWindows())
                return EnumerateWindowsFonts();

            if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                return EnumerateFontsFromFcList();

            return Enumerable.Empty<string>();
        }

        private static IEnumerable<string> EnumerateWindowsFonts()
        {
            if (!OperatingSystem.IsWindows() || !OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                return Enumerable.Empty<string>();

            var collection = new InstalledFontCollection();
            return collection.Families.Select(f => f.Name);
        }

        private static IEnumerable<string> EnumerateFontsFromFcList()
        {
            try
            {
                var output = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "fc-list",
                    Arguments = "--format='%{family[0]}\n'",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                })?.StandardOutput.ReadToEnd();

                return output?
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Trim('\'', ' ', '\r')) ?? Enumerable.Empty<string>();
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Frees the font system resources. Call this once on shutdown.
        /// </summary>
        public static void Dispose()
        {
            if (_fontInfo != IntPtr.Zero)
            {
                PdfSystemFontNative.FPDF_FreeDefaultSystemFontInfo(_fontInfo);
                _fontInfo = IntPtr.Zero;
                _initialized = false;
            }
        }
    }
}

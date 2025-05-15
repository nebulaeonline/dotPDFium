using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium
{
    public class PdfText : PdfObject
    {
        private readonly PdfPage _parentPage;
        private int _charCount = 0;

        /// <summary>
        /// PdfText constructor. This constructor is internal and should not be used directly.
        /// </summary>
        /// <param name="textHandle">The PDFium text pointer</param>
        /// <param name="parentPage">The parent PdfPage</param>
        /// <exception cref="ArgumentException">Throws if the textHandle is null or if the parentPage is null</exception>
        internal PdfText(IntPtr textHandle, PdfPage parentPage) : base(textHandle, PdfObjectType.TextPage)
        {
            if (textHandle == IntPtr.Zero || parentPage == null)
                throw new ArgumentException("Invalid text handle or parent page:", nameof(textHandle));
            
            _parentPage = parentPage;
            _charCount = PdfTextNative.FPDFText_CountChars(_handle);
        }

        public int CountChars => _handle != IntPtr.Zero
            ? _charCount
            : throw new ObjectDisposedException(nameof(PdfText));

        public uint GetChar(int index)
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfText));
            
            if (index < 0 || index >= _charCount)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

            return PdfTextNative.FPDFText_GetUnicode(_handle, index);
        }

        public bool GetCharBox(int index, out double left, out double right, out double bottom, out double top)
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfText));
            
            if (index < 0 || index >= _charCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            return PdfTextNative.FPDFText_GetCharBox(_handle, index, out left, out right, out bottom, out top);
        }

        public bool TryGetCharBox(int index, out double left, out double right, out double bottom, out double top)
        {
            if (_handle == IntPtr.Zero || index < 0 || index >= _charCount)
            {
                left = right = bottom = top = 0.0;
                return false;
            }

            PdfTextNative.FPDFText_GetCharBox(_handle, index, out left, out right, out bottom, out top);

            return true;
        }

        public int GetCharIndexAtPos(double x, double y, double xTolerance = 2.0, double yTolerance = 2.0)
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfText));

            return PdfTextNative.FPDFText_GetCharIndexAtPos(_handle, x, y, xTolerance, yTolerance);
        }

        public string GetTextRange(int index, int count)
        {
            if (_handle == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(PdfText));
            if (index < 0 || index >= _charCount || count < 0 || (index + count) > _charCount)
                throw new ArgumentOutOfRangeException();

            int bufferLen = (count + 1); // +1 for null terminator
            var buffer = new ushort[bufferLen];

            int written = PdfTextNative.FPDFText_GetText(_handle, index, count, buffer);
            if (written <= 0) return string.Empty;

            // Convert ushort[] → char[] safely
            var chars = new char[written];
            for (int i = 0; i < written; i++)
                chars[i] = (char)buffer[i];

            return new string(chars);
        }


        protected override void Dispose(bool disposing)
        {
            // Let base class call FPDFText_ClosePage
            base.Dispose(disposing);
        }
    }
}

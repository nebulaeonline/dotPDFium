using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium.Security;

/// <summary>
/// Represents a digital signature in a PDF document.
/// </summary>
public sealed class PdfSignature : PdfObject
{
    internal PdfSignature(IntPtr handle)
        : base(handle, PdfObjectType.Signature)
    {
    }

    /// <summary>
    /// Gets the byte range used to compute the signature hash. This typically contains 4 integers:
    /// start offset, length before signature, offset after signature, and length after.
    /// </summary>
    /// <returns>A list of signed byte range values, or empty if none are present.</returns>
    public IReadOnlyList<int> GetByteRange()
    {
        // First call to get count
        uint count = PdfSignatureNative.FPDFSignatureObj_GetByteRange(_handle, null!, 0);
        if (count == 0)
            return Array.Empty<int>();

        var buffer = new int[count];
        uint actual = PdfSignatureNative.FPDFSignatureObj_GetByteRange(_handle, buffer, count);
        if (actual != count)
            throw new dotPDFiumException("Failed to retrieve full byte range.");

        return buffer;
    }

    /// <summary>
    /// Gets the raw PKCS#7 signature contents as a DER-encoded binary blob.
    /// </summary>
    /// <returns>A <see cref="byte"/> array containing the raw signature data.</returns>
    public byte[] GetContents()
    {
        // Step 1: Get size
        uint size = PdfSignatureNative.FPDFSignatureObj_GetContents(_handle, IntPtr.Zero, 0);
        if (size == 0)
            return Array.Empty<byte>();

        var buffer = new byte[size];

        // Step 2: Pin buffer and call
        unsafe
        {
            fixed (byte* ptr = buffer)
            {
                uint written = PdfSignatureNative.FPDFSignatureObj_GetContents(_handle, (IntPtr)ptr, size);
                if (written != size)
                    throw new dotPDFiumException("Failed to retrieve full signature contents.");
            }
        }

        return buffer;
    }

    /// <summary>
    /// Gets the DocMDP permission level defined by this signature.
    /// </summary>
    /// <returns>A <see cref="PdfDocMDPPermission"/> value indicating the allowed operations after signing.</returns>
    public PdfDocMDPPermission GetDocMDPPermission()
    {
        uint value = PdfSignatureNative.FPDFSignatureObj_GetDocMDPPermission(_handle);
        return Enum.IsDefined(typeof(PdfDocMDPPermission), value)
            ? (PdfDocMDPPermission)value
            : PdfDocMDPPermission.None;
    }

    /// <summary>
    /// Gets the human-readable reason string associated with the signature (if available).
    /// </summary>
    /// <returns>The reason string provided by the signer, or an empty string if none.</returns>
    public string GetReason()
    {
        uint length = PdfSignatureNative.FPDFSignatureObj_GetReason(_handle, null!, 0);
        if (length == 0)
            return string.Empty;

        var buffer = new char[length];
        uint actual = PdfSignatureNative.FPDFSignatureObj_GetReason(_handle, buffer, length);

        if (actual == 0 || actual > buffer.Length)
            throw new dotPDFiumException("Failed to retrieve signature reason string.");

        return new string(buffer, 0, (int)(actual - 1)); // Remove trailing null
    }

    /// <summary>
    /// Gets the SubFilter string that specifies the encoding format of the signature (e.g. PKCS7, CAdES).
    /// </summary>
    /// <returns>The SubFilter string, or an empty string if not present.</returns>
    public string GetSubFilter()
    {
        uint size = PdfSignatureNative.FPDFSignatureObj_GetSubFilter(_handle, null!, 0);
        if (size == 0)
            return string.Empty;

        var buffer = new byte[size];
        uint written = PdfSignatureNative.FPDFSignatureObj_GetSubFilter(_handle, buffer, size);

        if (written == 0 || written > size)
            throw new dotPDFiumException("Failed to retrieve SubFilter string.");

        return System.Text.Encoding.UTF8.GetString(buffer, 0, (int)(written - 1)); // Strip null terminator
    }

    /// <summary>
    /// Gets the timestamp string associated with the signature (typically from the /M field).
    /// </summary>
    /// <returns>A string containing the signing time, or an empty string if unavailable.</returns>
    public string GetSigningTime()
    {
        uint size = PdfSignatureNative.FPDFSignatureObj_GetTime(_handle, null!, 0);
        if (size == 0)
            return string.Empty;

        var buffer = new byte[size];
        uint actual = PdfSignatureNative.FPDFSignatureObj_GetTime(_handle, buffer, size);

        if (actual == 0 || actual > size)
            throw new dotPDFiumException("Failed to retrieve signing time.");

        return System.Text.Encoding.UTF8.GetString(buffer, 0, (int)(actual - 1)); // Strip null terminator
    }

}

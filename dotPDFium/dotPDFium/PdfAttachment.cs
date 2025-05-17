using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public class PdfAttachment
{
    private readonly IntPtr _handle;

    internal PdfAttachment(IntPtr handle)
    {
        _handle = handle;
    }

    /// <summary>
    /// Retrieves the file contents of the PDF attachment as a byte array.
    /// </summary>
    /// <remarks>This method performs two passes to retrieve the file contents. The first pass determines the
    /// required buffer size,  and the second pass reads the actual data into the buffer. If the file cannot be
    /// retrieved, an empty byte array is returned.</remarks>
    /// <returns>A byte array containing the file contents of the PDF attachment. Returns an empty array if the file is not
    /// available.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the file contents cannot be retrieved during the second pass.</exception>
    public byte[] GetFile()
    {
        // First pass to get required buffer size
        if (!PdfAttachmentNative.FPDFAttachment_GetFile(_handle, IntPtr.Zero, 0, out uint size) || size == 0)
            return Array.Empty<byte>();

        var buffer = new byte[size];

        // Second pass to read actual data
        unsafe
        {
            fixed (byte* ptr = buffer)
            {
                if (!PdfAttachmentNative.FPDFAttachment_GetFile(_handle, (IntPtr)ptr, size, out _))
                    throw new dotPDFiumException($"Failed to retrieve attachment file contents: {PdfObject.GetPDFiumError()}");
            }
        }

        return buffer;
    }

    /// <summary>
    /// Retrieves the name of the PDF attachment.
    /// </summary>
    /// <remarks>The returned string excludes any null terminator that may be present in the underlying
    /// data.</remarks>
    /// <returns>A <see cref="string"/> containing the name of the attachment. Returns an empty string if the name is not
    /// available.</returns>
    public string GetName()
    {
        uint len = PdfAttachmentNative.FPDFAttachment_GetName(_handle, null!, 0);
        if (len == 0) return string.Empty;

        var buffer = new char[len];
        PdfAttachmentNative.FPDFAttachment_GetName(_handle, buffer, len);
        return new string(buffer, 0, (int)len - 1); // Exclude null terminator
    }

    /// <summary>
    /// Determines whether the attachment contains a specific key.
    /// </summary>
    /// <param name="key">The name of the key to check for. This value cannot be <see langword="null"/> or empty.</param>
    /// <returns><see langword="true"/> if the attachment contains the specified key; otherwise, <see langword="false"/>.</returns>
    public bool HasKey(string key)
    {
        return PdfAttachmentNative.FPDFAttachment_HasKey(_handle, key);
    }

    /// <summary>
    /// Retrieves the type of value associated with the specified key in the attachment.
    /// </summary>
    /// <param name="key">The key identifying the value in the attachment. Cannot be <see langword="null"/> or empty.</param>
    /// <returns>The <see cref="AttachmentValueType"/> representing the type of the value associated with the specified key.</returns>
    public AttachmentValueType GetValueType(string key)
    {
        return (AttachmentValueType)PdfAttachmentNative.FPDFAttachment_GetValueType(_handle, key);
    }

    /// <summary>
    /// Retrieves the string value associated with the specified key from the attachment.
    /// </summary>
    /// <param name="key">The key identifying the string value to retrieve. Cannot be <see langword="null"/> or empty.</param>
    /// <returns>The string value associated with the specified key, or an empty string if the key does not exist or the value is
    /// empty.</returns>
    public string GetStringValue(string key)
    {
        uint len = PdfAttachmentNative.FPDFAttachment_GetStringValue(_handle, key, null!, 0);
        if (len == 0) return string.Empty;

        var buffer = new char[len];
        PdfAttachmentNative.FPDFAttachment_GetStringValue(_handle, key, buffer, len);
        return new string(buffer, 0, (int)len - 1); // Exclude null terminator
    }

    /// <summary>
    /// Sets a string value associated with the specified key.
    /// </summary>
    /// <remarks>This method associates a string value with a given key. If the operation fails, an exception
    /// is thrown. Ensure that both <paramref name="key"/> and <paramref name="value"/> are valid and non-null before
    /// calling this method.</remarks>
    /// <param name="key">The key identifying the value to set. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="value">The string value to associate with the specified key. Cannot be <see langword="null"/>.</param>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails to set the string value for the specified key.</exception>
    public void SetStringValue(string key, string value)
    {
        if (!PdfAttachmentNative.FPDFAttachment_SetStringValue(_handle, key, value))
            throw new dotPDFiumException($"Failed to set string value for key '{key}'.");
    }

    /// <summary>
    /// Associates a file with the current PDF attachment.
    /// </summary>
    /// <remarks>This method sets the file data for the current attachment in the specified PDF document.  The
    /// file data is passed as a byte array and must be valid and non-empty.  If the operation fails, a <see
    /// cref="dotPDFiumException"/> is thrown.</remarks>
    /// <param name="data">The file data to associate with the attachment. Must not be null or empty.</param>
    /// <param name="doc">The PDF document to which the attachment belongs. Must not be null.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="data"/> is null or empty.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the file data could not be set on the attachment.</exception>
    public void SetFile(byte[] data, PdfDocument doc)
    {
        if (data is null || data.Length == 0)
            throw new ArgumentException("File data cannot be null or empty.", nameof(data));

        unsafe
        {
            fixed (byte* ptr = data)
            {
                if (!PdfAttachmentNative.FPDFAttachment_SetFile(
                    _handle, doc.Handle, (IntPtr)ptr, (uint)data.Length))
                {
                    throw new dotPDFiumException("Failed to set file data on attachment.");
                }
            }
        }
    }

    /// <summary>
    /// Retrieves the subtype of the PDF attachment.
    /// </summary>
    /// <remarks>The subtype typically provides additional information about the attachment's type or
    /// purpose.</remarks>
    /// <returns>A <see cref="string"/> representing the subtype of the attachment.  Returns an empty string if the subtype is
    /// not defined or cannot be retrieved.</returns>
    public string GetSubtype()
    {
        uint len = PdfAttachmentNative.FPDFAttachment_GetSubtype(_handle, null!, 0);
        if (len == 0) return string.Empty;

        var buffer = new char[len];
        PdfAttachmentNative.FPDFAttachment_GetSubtype(_handle, buffer, len);
        return new string(buffer, 0, (int)len - 1); // Exclude null terminator
    }
}


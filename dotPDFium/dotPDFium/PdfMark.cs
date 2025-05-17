using nebulae.dotPDFium.Native;
using System.Runtime.InteropServices;
using System.Text;

namespace nebulae.dotPDFium;

/// <summary>
/// Represents a semantic mark attached to a page object, such as accessibility or structure metadata.
/// </summary>
public sealed class PdfMark
{
    private readonly IntPtr _handle;

    /// <summary>
    /// Represents a mark within a PDF document.
    /// </summary>
    /// <remarks>This class is used to encapsulate a handle to a PDF mark, which may represent annotations,
    /// bookmarks, or other metadata within a PDF document. It is intended for internal use only.</remarks>
    /// <param name="handle">A pointer to the native PDF mark handle.</param>
    internal PdfMark(IntPtr handle)
    {
        _handle = handle;
    }

    /// <summary>
    /// Gets the name associated with the current PDF page object mark.
    /// </summary>
    /// <remarks>This property retrieves the name of the mark associated with the PDF page object.  If the
    /// name cannot be determined, an empty string is returned.</remarks>
    public string Name
    {
        get
        {
            uint requiredLength;
            if (!PdfEditNative.FPDFPageObjMark_GetName(_handle, IntPtr.Zero, 0, out requiredLength) || requiredLength == 0)
                return string.Empty;

            IntPtr buffer = Marshal.AllocHGlobal((int)requiredLength);
            try
            {
                if (!PdfEditNative.FPDFPageObjMark_GetName(_handle, buffer, requiredLength, out _))
                    return string.Empty;

                return Marshal.PtrToStringUTF8(buffer) ?? string.Empty;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }

    /// <summary>
    /// Gets the number of parameters associated with the current PDF page object mark.
    /// </summary>
    /// <remarks>This property retrieves the number of parameters defined for the mark associated with the PDF
    /// page object. It can be used to iterate through or inspect the parameters of the mark.</remarks>
    public int ParameterCount => PdfEditNative.FPDFPageObjMark_CountParams(_handle);

    /// <summary>
    /// Retrieves the keys of all parameters associated with the current page object mark.
    /// </summary>
    /// <remarks>This method enumerates the parameter keys for the page object mark represented by the current
    /// instance. The keys are returned as UTF-8 encoded strings. If a key cannot be retrieved, an empty string is
    /// returned for that key.</remarks>
    /// <returns>An <see cref="IEnumerable{T}"/> of strings containing the parameter keys. The collection will be empty if no
    /// parameters are associated with the page object mark.</returns>
    public IEnumerable<string> GetParamKeys()
    {
        int count = PdfEditNative.FPDFPageObjMark_CountParams(_handle);
        for (uint i = 0; i < count; i++)
        {
            yield return PdfUtil.ReadUtf8((IntPtr buffer, uint length, out uint outLen) =>
                PdfEditNative.FPDFPageObjMark_GetParamKey(_handle, i, buffer, length, out outLen)) ?? string.Empty;
        }
    }

    /// <summary>
    /// Retrieves the parameter type associated with the specified key in the PDF mark.
    /// </summary>
    /// <remarks>This method queries the underlying PDF object to determine the type of the parameter
    /// associated with the given key. Ensure that the key exists in the PDF mark to avoid unexpected results.</remarks>
    /// <param name="key">The key identifying the parameter whose type is to be retrieved. Cannot be <see langword="null"/> or empty.</param>
    /// <returns>The <see cref="PdfMarkParamType"/> representing the type of the parameter associated with the specified key.</returns>
    public PdfMarkParamType GetParamType(string key)
    {
        return (PdfMarkParamType)PdfEditNative.FPDFPageObjMark_GetParamValueType(_handle, key);
    }

    /// <summary>
    /// Retrieves the integer value associated with the specified key from the underlying object.
    /// </summary>
    /// <param name="key">The key identifying the parameter whose integer value is to be retrieved. Cannot be <see langword="null"/> or
    /// empty.</param>
    /// <returns>The integer value associated with the specified key, or <see langword="null"/> if the key does not exist or the
    /// value cannot be retrieved.</returns>
    public int? GetIntValue(string key)
    {
        return PdfEditNative.FPDFPageObjMark_GetParamIntValue(_handle, key, out int value)
            ? value
            : null;
    }

    /// <summary>
    /// Sets an integer parameter on a mark associated with a PDF page object.
    /// </summary>
    /// <remarks>This method modifies the metadata of a mark associated with a PDF page object by setting an
    /// integer parameter. Ensure that the <paramref name="key"/> is valid and that the <paramref name="doc"/> and
    /// <paramref name="obj"/>  are properly initialized before calling this method.</remarks>
    /// <param name="doc">The <see cref="PdfDocument"/> containing the page object.</param>
    /// <param name="obj">The <see cref="PdfPageObject"/> to which the mark is associated.</param>
    /// <param name="key">The key identifying the parameter to set. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="value">The integer value to assign to the specified parameter.</param>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails to set the integer parameter for the specified key.</exception>
    public void SetIntParam(PdfDocument doc, PdfPageObject obj, string key, int value)
    {
        if (!PdfEditNative.FPDFPageObjMark_SetIntParam(doc.Handle, obj.Handle, _handle, key, value))
            throw new dotPDFiumException($"Failed to set int parameter '{key}' on mark.");
    }

    /// <summary>
    /// Retrieves the string value associated with the specified key from the PDF page object mark.
    /// </summary>
    /// <param name="key">The key identifying the parameter whose string value is to be retrieved. Cannot be <see langword="null"/> or
    /// empty.</param>
    /// <returns>The string value associated with the specified key, or <see langword="null"/> if the key does not exist or the
    /// value cannot be retrieved.</returns>
    public string? GetStringValue(string key)
    {
        return PdfUtil.ReadUtf8((IntPtr buffer, uint length, out uint outLen) =>
            PdfEditNative.FPDFPageObjMark_GetParamStringValue(_handle, key, buffer, length, out outLen));
    }

    /// <summary>
    /// Sets a string parameter on a mark associated with a PDF page object.
    /// </summary>
    /// <param name="doc">The <see cref="PdfDocument"/> containing the page object.</param>
    /// <param name="obj">The <see cref="PdfPageObject"/> to which the mark is associated.</param>
    /// <param name="key">The key identifying the parameter to set. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="value">The string value to assign to the parameter. Cannot be <see langword="null"/>.</param>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails to set the string parameter.</exception>
    public void SetStringParam(PdfDocument doc, PdfPageObject obj, string key, string value)
    {
        if (!PdfEditNative.FPDFPageObjMark_SetStringParam(doc.Handle, obj.Handle, _handle, key, value))
            throw new dotPDFiumException($"Failed to set string parameter '{key}' on mark.");
    }

    /// <summary>
    /// Retrieves the binary data (blob) associated with the specified key.
    /// </summary>
    /// <remarks>This method attempts to retrieve binary data associated with the given key. If the key is not
    /// found or the blob value is empty, the method returns <see langword="null"/>.</remarks>
    /// <param name="key">The key identifying the blob value to retrieve. Cannot be <see langword="null"/> or empty.</param>
    /// <returns>A byte array containing the blob value associated with the specified key, or <see langword="null"/> if the key
    /// does not exist or the blob value is empty.</returns>
    public byte[]? GetBlobValue(string key)
    {
        // Step 1: get buffer length
        if (!PdfEditNative.FPDFPageObjMark_GetParamBlobValue(_handle, key, Array.Empty<byte>(), 0, out uint len) || len == 0)
            return null;

        var buffer = new byte[len];
        if (!PdfEditNative.FPDFPageObjMark_GetParamBlobValue(_handle, key, buffer, len, out _))
            return null;

        return buffer;
    }

    /// <summary>
    /// Sets a blob parameter on a specified mark within a PDF page object.
    /// </summary>
    /// <remarks>This method associates binary data with a specific key on a mark within a PDF page object.
    /// The operation will fail if the underlying PDF library cannot set the parameter.</remarks>
    /// <param name="doc">The <see cref="PdfDocument"/> containing the page object.</param>
    /// <param name="obj">The <see cref="PdfPageObject"/> to which the mark belongs.</param>
    /// <param name="key">The key identifying the blob parameter to set.</param>
    /// <param name="data">The binary data to associate with the specified key. Must not be null or empty.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="data"/> is null or empty.</exception>
    /// <exception cref="dotPDFiumException">Thrown if the operation fails to set the blob parameter.</exception>
    public void SetBlobParam(PdfDocument doc, PdfPageObject obj, string key, byte[] data)
    {
        if (data is null || data.Length == 0)
            throw new ArgumentException("Blob data must not be null or empty.", nameof(data));

        if (!PdfEditNative.FPDFPageObjMark_SetBlobParam(doc.Handle, obj.Handle, _handle, key, data, (uint)data.Length))
            throw new dotPDFiumException($"Failed to set blob parameter '{key}' on mark.");
    }

    /// <summary>
    /// Removes a parameter with the specified key from the mark associated with the given PDF page object.
    /// </summary>
    /// <param name="obj">The PDF page object from which the parameter will be removed. Cannot be <see langword="null"/>.</param>
    /// <param name="key">The key of the parameter to remove. Cannot be <see langword="null"/> or empty.</param>
    /// <exception cref="dotPDFiumException">Thrown if the parameter with the specified key could not be removed from the mark.</exception>
    public void RemoveParam(PdfPageObject obj, string key)
    {
        if (!PdfEditNative.FPDFPageObjMark_RemoveParam(obj.Handle, _handle, key))
            throw new dotPDFiumException($"Failed to remove parameter '{key}' from mark.");
    }

    /// <summary>
    /// Gets the handle associated with the current instance.
    /// </summary>
    internal IntPtr Handle => _handle;
}



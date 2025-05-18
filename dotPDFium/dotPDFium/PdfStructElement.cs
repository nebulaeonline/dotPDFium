using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public sealed class PdfStructElement : PdfTaggedObject
{
    internal PdfStructElement(IntPtr handle) : base(handle) { }

    /// <summary>
    /// Gets the number of child structure elements (or content references) under this element.
    /// </summary>
    public int CountChildren()
    {
        return PdfStructTreeNative.FPDF_StructElement_CountChildren(_handle);
    }

    /// <summary>
    /// Retrieves the ActualText associated with this structure element, if present.
    /// </summary>
    /// <returns>The actual text as a string, or an empty string if not defined.</returns>
    public string GetActualText()
    {
        uint size = PdfStructTreeNative.FPDF_StructElement_GetActualText(_handle, null!, 0);
        if (size == 0)
            return string.Empty;

        var buffer = new char[size];
        uint written = PdfStructTreeNative.FPDF_StructElement_GetActualText(_handle, buffer, size);

        if (written == 0 || written > size)
            throw new dotPDFiumException("Failed to retrieve ActualText.");

        return new string(buffer, 0, (int)(written - 1)); // Remove null terminator
    }

    /// <summary>
    /// Gets the alternative text (/Alt) associated with this structure element, if present.
    /// </summary>
    /// <returns>The alternative text string, or an empty string if none exists.</returns>
    public string GetAltText()
    {
        uint size = PdfStructTreeNative.FPDF_StructElement_GetAltText(_handle, null!, 0);
        if (size == 0)
            return string.Empty;

        var buffer = new char[size];
        uint written = PdfStructTreeNative.FPDF_StructElement_GetAltText(_handle, buffer, size);

        if (written == 0 || written > size)
            throw new dotPDFiumException("Failed to retrieve AltText.");

        return new string(buffer, 0, (int)(written - 1)); // Strip null terminator
    }

    /// <summary>
    /// Gets the attribute value at the specified index in the element's /A dictionary.
    /// </summary>
    /// <param name="index">Zero-based index into the attribute set.</param>
    /// <returns>A <see cref="PdfStructAttributeValue"/>, or null if invalid or missing.</returns>
    public PdfStructAttributeValue? GetAttributeAt(int index)
    {
        var attrHandle = PdfStructTreeNative.FPDF_StructElement_GetAttributeAtIndex(_handle, index);
        return attrHandle == IntPtr.Zero ? null : new PdfStructAttributeValue(attrHandle);
    }

    /// <summary>
    /// Returns the number of attributes directly associated with this structure element.
    /// </summary>
    public int GetAttributeCount()
    {
        return PdfStructTreeNative.FPDF_StructElement_GetAttributeCount(_handle);
    }

    /// <summary>
    /// Gets the child structure element at the given index.
    /// </summary>
    /// <param name="index">Index of the child.</param>
    /// <returns>The child <see cref="PdfStructElement"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If index is invalid.</exception>
    public PdfStructElement GetChildAt(int index)
    {
        int count = CountChildren();
        if (index < 0 || index >= count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var childHandle = PdfStructTreeNative.FPDF_StructElement_GetChildAtIndex(_handle, index);
        if (childHandle == IntPtr.Zero)
            throw new dotPDFiumException($"Failed to retrieve struct element at index {index}.");

        return new PdfStructElement(childHandle);
    }

    /// <summary>
    /// Gets the Marked Content ID (MCID) for the child at the specified index.
    /// </summary>
    /// <param name="index">Index of the structure child.</param>
    /// <returns>The MCID if present, or -1 if the child is not marked content.</returns>
    public int GetChildMarkedContentID(int index)
    {
        return PdfStructTreeNative.FPDF_StructElement_GetChildMarkedContentID(_handle, index);
    }

    /// <summary>
    /// Gets the value of the /ID entry for this structure element.
    /// </summary>
    /// <returns>The ID string, or an empty string if not present.</returns>
    public string GetId()
    {
        uint size = PdfStructTreeNative.FPDF_StructElement_GetID(_handle, null!, 0);
        if (size == 0)
            return string.Empty;

        var buffer = new char[size];
        uint written = PdfStructTreeNative.FPDF_StructElement_GetID(_handle, buffer, size);

        if (written == 0 || written > size)
            throw new dotPDFiumException("Failed to retrieve structure element ID.");

        return new string(buffer, 0, (int)(written - 1)); // Strip null terminator
    }

    /// <summary>
    /// Gets the language specified for this structure element (e.g., "en-US").
    /// </summary>
    /// <returns>The language string, or an empty string if not defined.</returns>
    public string GetLanguage()
    {
        uint size = PdfStructTreeNative.FPDF_StructElement_GetLang(_handle, null!, 0);
        if (size == 0)
            return string.Empty;

        var buffer = new char[size];
        uint written = PdfStructTreeNative.FPDF_StructElement_GetLang(_handle, buffer, size);

        if (written == 0 || written > size)
            throw new dotPDFiumException("Failed to retrieve structure element language.");

        return new string(buffer, 0, (int)(written - 1)); // Remove null terminator
    }

    /// <summary>
    /// Gets the primary Marked Content ID (MCID) associated with this structure element.
    /// </summary>
    /// <returns>The MCID, or -1 if not associated with any marked content.</returns>
    public int GetMarkedContentId()
    {
        return PdfStructTreeNative.FPDF_StructElement_GetMarkedContentID(_handle);
    }


    /// <summary>
    /// Gets the Marked Content ID (MCID) at the specified index, if the child at that index is a marked content item.
    /// </summary>
    /// <param name="index">Index of the child within this structure element.</param>
    /// <returns>The MCID, or -1 if the entry is not marked content.</returns>
    public int GetMarkedContentIdAt(int index)
    {
        return PdfStructTreeNative.FPDF_StructElement_GetMarkedContentIdAtIndex(_handle, index);
    }

    /// <summary>
    /// Gets the number of Marked Content IDs associated with this structure element.
    /// </summary>
    /// <returns>The count of MCIDs (can be 0).</returns>
    public int GetMarkedContentIdCount()
    {
        return PdfStructTreeNative.FPDF_StructElement_GetMarkedContentIdCount(_handle);
    }

    /// <summary>
    /// Gets the /ObjType string associated with this structure element (if present).
    /// </summary>
    /// <returns>The object type string (e.g., "Annot", "Layout"), or an empty string if none is defined.</returns>
    public string GetObjectType()
    {
        uint size = PdfStructTreeNative.FPDF_StructElement_GetObjType(_handle, null!, 0);
        if (size == 0)
            return string.Empty;

        var buffer = new char[size];
        uint written = PdfStructTreeNative.FPDF_StructElement_GetObjType(_handle, buffer, size);

        if (written == 0 || written > size)
            throw new dotPDFiumException("Failed to retrieve /ObjType from structure element.");

        return new string(buffer, 0, (int)(written - 1)); // Strip null terminator
    }

    /// <summary>
    /// Gets the parent structure element of this node, if one exists.
    /// </summary>
    /// <returns>
    /// A <see cref="PdfStructElement"/> representing the parent,
    /// or <c>null</c> if this is the root element.
    /// </returns>
    public PdfStructElement? GetParent()
    {
        var parentHandle = PdfStructTreeNative.FPDF_StructElement_GetParent(_handle);
        return parentHandle == IntPtr.Zero ? null : new PdfStructElement(parentHandle);
    }

    /// <summary>
    /// Retrieves the value of a string attribute associated with the specified key from the structure element.
    /// </summary>
    /// <param name="key">The key of the attribute to retrieve. This value cannot be null.</param>
    /// <returns>The value of the string attribute if it exists; otherwise, an empty string.</returns>
    /// <exception cref="dotPDFiumException">Thrown if the attribute retrieval fails or the written size exceeds the buffer size.</exception>
    public string GetStringAttribute(string key)
    {
        uint size = PdfStructTreeNative.FPDF_StructElement_GetStringAttribute(_handle, key, null!, 0);
        if (size == 0)
            return string.Empty;

        var buffer = new char[size];
        uint written = PdfStructTreeNative.FPDF_StructElement_GetStringAttribute(_handle, key, buffer, size);

        if (written == 0 || written > size)
            throw new dotPDFiumException($"Failed to retrieve structure attribute '{key}'.");

        return new string(buffer, 0, (int)(written - 1)); // Strip null terminator
    }

    /// <summary>
    /// Gets the /T (title) entry of this structure element, if present.
    /// </summary>
    /// <returns>The title string, or an empty string if not defined.</returns>
    public string GetTitle()
    {
        uint size = PdfStructTreeNative.FPDF_StructElement_GetTitle(_handle, null!, 0);
        if (size == 0)
            return string.Empty;

        var buffer = new char[size];
        uint written = PdfStructTreeNative.FPDF_StructElement_GetTitle(_handle, buffer, size);

        if (written == 0 || written > size)
            throw new dotPDFiumException("Failed to retrieve structure element title.");

        return new string(buffer, 0, (int)(written - 1)); // Strip null terminator
    }

    /// <summary>
    /// Gets the structure type of this element (e.g., "H1", "P", "Table").
    /// </summary>
    public string GetRawType()
    {
        uint size = PdfStructTreeNative.FPDF_StructElement_GetType(_handle, null!, 0);
        if (size == 0)
            return string.Empty;

        var buffer = new byte[size];
        uint actual = PdfStructTreeNative.FPDF_StructElement_GetType(_handle, buffer, size);

        if (actual == 0 || actual > size)
            throw new dotPDFiumException("Failed to retrieve structure element type.");

        return System.Text.Encoding.UTF8.GetString(buffer, 0, (int)(actual - 1));
    }

    /// <summary>
    /// Gets the full attribute dictionary for this structure element.
    /// </summary>
    /// <returns>A <see cref="PdfStructAttributeSet"/> representing the attributes, or null if none exist.</returns>
    public PdfStructAttributeSet? GetAttributes()
    {
        var attrHandle = PdfStructTreeNative.FPDF_StructElement_GetAttr(_handle);
        return attrHandle == IntPtr.Zero ? null : new PdfStructAttributeSet(attrHandle);
    }

    /// <summary>
    /// Gets the structure element type as an enum.
    /// </summary>
    public PdfStructElementType GetElementType()
    {
        string raw = GetRawType();
        return raw switch
        {
            "Document" => PdfStructElementType.Document,
            "H1" => PdfStructElementType.H1,
            "H2" => PdfStructElementType.H2,
            "H3" => PdfStructElementType.H3,
            "P" => PdfStructElementType.Paragraph,
            "Table" => PdfStructElementType.Table,
            "TR" => PdfStructElementType.TR,
            "TD" => PdfStructElementType.TD,
            // Add more mappings...
            _ => PdfStructElementType.Unknown
        };
    }
}

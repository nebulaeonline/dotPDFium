using System;
using System.IO;
using System.Runtime.InteropServices;

using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

/// <summary>
/// Provides an interface for managing the availability of PDF data streams, enabling incremental loading and access to
/// PDF documents and their components.
/// </summary>
/// <remarks>This class is designed to facilitate the loading of PDF documents in scenarios where the data is
/// retrieved incrementally, such as from a network stream. It provides methods to check the availability of the
/// document, individual pages, and form data, as well as to determine whether the file is linearized (optimized for
/// fast web viewing).  Instances of this class must be disposed of when no longer needed to release unmanaged
/// resources.</remarks>
public sealed class PdfStreamAvailability : IDisposable
{
    private IntPtr _availHandle;

    private GCHandle _userDataHandle;
    private GCHandle _getBlockHandle;
    private GCHandle _isDataAvailHandle;
    private GCHandle _addSegmentHandle;

    private readonly PdfGetBlockDelegate _getBlockDelegate;
    private readonly PdfIsDataAvailDelegate _isDataAvailDelegate;
    private readonly PdfAddSegmentDelegate _addSegmentDelegate;

    private FxDownloadHints _downloadHints;

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfStreamAvailability"/> class,  which provides functionality for
    /// managing PDF stream availability using custom data access and availability callbacks.
    /// </summary>
    /// <remarks>This constructor initializes the PDF stream availability context by pinning the provided user
    /// data and delegates  to prevent garbage collection during the lifetime of the instance. It also sets up the
    /// necessary native interop structures  for managing PDF data availability and download hints.</remarks>
    /// <param name="options">A <see cref="PdfStreamAvailabilityOptions"/> object that specifies the configuration for the PDF stream
    /// availability,  including user data, data access callbacks, and availability checks.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is <see langword="null"/> or if any of its required properties  (<see
    /// cref="PdfStreamAvailabilityOptions.UserData"/>, <see cref="PdfStreamAvailabilityOptions.GetBlock"/>,  <see
    /// cref="PdfStreamAvailabilityOptions.IsDataAvailable"/>, or <see
    /// cref="PdfStreamAvailabilityOptions.RequestSegment"/>) are <see langword="null"/>.</exception>
    /// <exception cref="dotPDFiumException"></exception>
    public PdfStreamAvailability(PdfStreamAvailabilityOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (options.UserData == null) throw new ArgumentNullException(nameof(options.UserData));
        if (options.GetBlock == null) throw new ArgumentNullException(nameof(options.GetBlock));
        if (options.IsDataAvailable == null) throw new ArgumentNullException(nameof(options.IsDataAvailable));
        if (options.RequestSegment == null) throw new ArgumentNullException(nameof(options.RequestSegment));

        _getBlockDelegate = options.GetBlock;
        _isDataAvailDelegate = options.IsDataAvailable;
        _addSegmentDelegate = options.RequestSegment;

        // Pin all GC-sensitive values
        _userDataHandle = GCHandle.Alloc(options.UserData);
        _getBlockHandle = GCHandle.Alloc(_getBlockDelegate);
        _isDataAvailHandle = GCHandle.Alloc(_isDataAvailDelegate);
        _addSegmentHandle = GCHandle.Alloc(_addSegmentDelegate);

        var fileAccess = new PdfFileAccess
        {
            fileLength = options.FileLength,
            getBlock = Marshal.GetFunctionPointerForDelegate(_getBlockDelegate),
            userData = GCHandle.ToIntPtr(_userDataHandle)
        };

        var fileAvail = new FxFileAvail
        {
            version = 1,
            IsDataAvail = Marshal.GetFunctionPointerForDelegate(new IsDataAvailInternalDelegate(IsDataAvailInternal))
        };

        _downloadHints = new FxDownloadHints
        {
            version = 1,
            AddSegment = Marshal.GetFunctionPointerForDelegate(new AddSegmentInternalDelegate(AddSegmentInternal))
        };

        _availHandle = PdfDataAvailNative.FPDFAvail_Create(ref fileAvail, ref fileAccess);
        if (_availHandle == IntPtr.Zero)
            throw new dotPDFiumException("Failed to create FPDFAvail context.");
    }

    /// <summary>
    /// Attempts to load the PDF document from this availability context.
    /// </summary>
    /// <param name="password">The password, or empty string for no password.</param>
    /// <returns>A <see cref="PdfDocument"/> instance if the document is available; otherwise <c>null</c>.</returns>
    /// <exception cref="dotPDFiumException">If PDFium returns an invalid handle despite IsDocAvailable returning true.</exception>
    public PdfDocument? TryLoadDocument(string password = "")
    {
        if (!IsDocAvailable())
            return null;

        var docHandle = PdfDataAvailNative.FPDFAvail_GetDocument(_availHandle, password ?? string.Empty);
        if (docHandle == IntPtr.Zero)
            return null;

        return new PdfDocument(docHandle); // ✅ User sees wrapped document, not IntPtr
    }

    /// <summary>
    /// Returns whether the underlying file is linearized (Fast Web View).
    /// </summary>
    public bool IsLinearized()
    {
        return PdfDataAvailNative.FPDFAvail_IsLinearized(_availHandle) == 1;
    }

    /// <summary>
    /// Determines whether data is available at the specified offset and size.
    /// </summary>
    /// <remarks>This method relies on a user-defined delegate to perform the actual data availability check.
    /// The <paramref name="pThis"/> parameter must point to a valid context object, and the behavior of the method
    /// depends on the implementation of the delegate.</remarks>
    /// <param name="pThis">A pointer to the user-defined context associated with the data source.</param>
    /// <param name="offset">The offset, in bytes, from the start of the data source to check for availability.</param>
    /// <param name="size">The size, in bytes, of the data to check for availability.</param>
    /// <returns>An integer indicating the result of the availability check. A non-zero value typically indicates that the data
    /// is available, while zero indicates that it is not.</returns>
    private int IsDataAvailInternal(IntPtr pThis, UIntPtr offset, UIntPtr size)
    {
        var userData = GCHandle.FromIntPtr(pThis).Target!;
        return _isDataAvailDelegate(userData, (ulong)offset, (ulong)size);
    }

    /// <summary>
    /// Adds a memory segment to the internal structure at the specified offset and size.
    /// </summary>
    /// <remarks>This method is intended for internal use and should not be called directly by user
    /// code.</remarks>
    /// <param name="pThis">A pointer to the current instance. This parameter is reserved for internal use.</param>
    /// <param name="offset">The offset, in bytes, where the segment begins.</param>
    /// <param name="size">The size, in bytes, of the segment to add.</param>
    private void AddSegmentInternal(IntPtr pThis, UIntPtr offset, UIntPtr size)
    {
        _addSegmentDelegate((ulong)offset, (ulong)size);
    }

    /// <summary>
    /// Determines whether the document is available for processing.
    /// </summary>
    /// <remarks>This method checks the availability of the document using the underlying PDF data
    /// availability mechanism. It is typically used to verify whether the document can be accessed or processed
    /// further.</remarks>
    /// <returns><see langword="true"/> if the document is available; otherwise, <see langword="false"/>.</returns>
    public bool IsDocAvailable()
    {
        return PdfDataAvailNative.FPDFAvail_IsDocAvail(_availHandle, ref _downloadHints) == 1;
    }

    /// <summary>
    /// Determines whether the specified page is available for viewing or processing.
    /// </summary>
    /// <remarks>This method checks the availability of a page in the document, which may depend on factors
    /// such as the document's loading state or the availability of required resources.</remarks>
    /// <param name="pageIndex">The zero-based index of the page to check.</param>
    /// <returns><see langword="true"/> if the specified page is available; otherwise, <see langword="false"/>.</returns>
    public bool IsPageAvailable(int pageIndex)
    {
        return PdfDataAvailNative.FPDFAvail_IsPageAvail(_availHandle, pageIndex, ref _downloadHints) == 1;
    }

    /// <summary>
    /// Determines whether the form data in the PDF document is available for use.
    /// </summary>
    /// <remarks>This method checks the availability of form data in the PDF document.  It may be useful in
    /// scenarios where form data needs to be processed or accessed.</remarks>
    /// <returns><see langword="true"/> if the form data is available; otherwise, <see langword="false"/>.</returns>
    public bool IsFormAvailable()
    {
        return PdfDataAvailNative.FPDFAvail_IsFormAvail(_availHandle, ref _downloadHints) == 1;
    }

    /// <summary>
    /// Releases all resources used by the current instance of the class.
    /// </summary>
    /// <remarks>This method should be called when the instance is no longer needed to free unmanaged
    /// resources  and release any allocated handles. After calling this method, the instance should not be
    /// used.</remarks>
    public void Dispose()
    {
        if (_availHandle != IntPtr.Zero)
        {
            PdfDataAvailNative.FPDFAvail_Destroy(_availHandle);
            _availHandle = IntPtr.Zero;
        }

        if (_userDataHandle.IsAllocated) _userDataHandle.Free();
        if (_getBlockHandle.IsAllocated) _getBlockHandle.Free();
        if (_isDataAvailHandle.IsAllocated) _isDataAvailHandle.Free();
        if (_addSegmentHandle.IsAllocated) _addSegmentHandle.Free();
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int IsDataAvailInternalDelegate(IntPtr pThis, UIntPtr offset, UIntPtr size);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void AddSegmentInternalDelegate(IntPtr pThis, UIntPtr offset, UIntPtr size);
}

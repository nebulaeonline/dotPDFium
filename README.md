# dotPDFium

**dotPDFium** is an early-stage, .NET 8+ wrapper around the native [PDFium](https://pdfium.googlesource.com/pdfium/) library, aiming to provide a safe and idiomatic C# interface for loading, parsing, and rendering PDF files.

This project is currently in a **pre-pre-alpha** state. Development is focused on establishing core object lifecycles (`PdfDocument`, `PdfPage`, `PdfText`), managing native resource ownership correctly, and wrapping the lower-level PDFium API with a clean, maintainable surface.

We're not yet feature-complete, stable, or ready for production use. This repository is for exploratory development and incremental wrapping of PDFium's 449 functions in a .NET-friendly way.

The library contains the latest PDFium binaries (v138.0.7175.0) from chromium/7175, built by bblanchon (https://github.com/bblanchon/pdfium-binaries/releases) for Windows x64 & ARM64, Linux x64 & ARM64, and MacOS (as a universal dylib supporting x64 & ARM64).

We are at v0.3.0-prealpha, which is a *very* early pre-alpha version. The goal is to hit v1.0 within 3 months or so, maybe sooner.

### Goals (eventually)

- Managed lifecycle and memory safety for native PDFium handles
- Optional exceptions or `TryX()` patterns for common workflows
- Modular text extraction and page rendering APIs
- MIT-licensed wrapper code with proper attribution of PDFium (Apache 2.0)

### Status

- Basic document and page loading implemented
- Page handling and rendering implemented
- Text extraction imlemented
- Search implemented
- Security implemented
- Forms\* are a WIP; If you have suggestions, please open an issue or PR
- Annotations imlemented
- Very few formal tests
- No documentation or examples yet
- Structured trees and progressive loading are implemented
- XFA support is not planned

#### API Docs are available at [dotPDFium API Docs](https://nebulae.online/dotPDFium/api/nebulae.dotPDFium.html)

#### Getting Started can be found [here](https://nebulae.online/dotPDFium/docs/getting-started.html)

#### Can I Create New Form Fields?

No — the underlying PDFium library does not support creating new `/Widget` annotations, which are required for checkboxes, text inputs, radio buttons, etc.

This is a limitation in the C++ PDFium core. Until it adds `FPDF_ANNOT_WIDGET` to its supported list, only editing of existing form fields is possible.

See: [FPDFAnnot_IsSupportedSubtype](https://pdfium.googlesource.com/pdfium/+/cffbd3c96f99c86fad5880db4996daa6b19fa501/fpdfsdk/fpdf_annot.cpp)

---

**Note:** This is a low-level interop library in early development. Expect sharp edges, breaking changes, and missing functionality.

---

### Example Usage

#### Create a new PDF document
```csharp
    using nebulae.dotPDFium;

    PDFiumEngine.Init();

    var doc = PdfDocument.CreateNew();
    var page = doc.CreatePage(0, 612, 792); // 8.5x11 inches

    var font = doc.LoadStandardFont("Helvetica");
    var text = doc.CreateTextObject(font, 12f);
    text.SetText("Hello from test!");
    text.SetPosition(100, 700);
    page.InsertObject(text);

    page.FinalizeContent();
    doc.SaveTo("generated.pdf");

    PDFiumEngine.Shutdown();
```

#### Cross platform rendering & drawing (using SixLabors' ImageSharp for cross-platform support)
```csharp

    PDFiumEngine.Init();

    var doc = PdfDocument.LoadFromFile("test.pdf");
    using var page = doc.LoadPage(0);

    int dpi = 144;
    float scale = dpi / 72f;

    int width = (int)(page.Width * scale);
    int height = (int)(page.Height * scale);

    using var pdfBitmap = PdfBitmap.Create(width, height);
    pdfBitmap.FillRect(0, 0, width, height, 0xFFFFFFFF); // White background

    page.RenderToBitmap(pdfBitmap, 0, 0, width, height);

    var imageBuffer = new Rgba32[width * height];

    unsafe
    {
        Buffer.MemoryCopy(
            source: (void*)pdfBitmap.Buffer,
            destination: Unsafe.AsPointer(ref imageBuffer[0]),
            destinationSizeInBytes: imageBuffer.Length * sizeof(uint), // Rgba32 is 4 bytes
            sourceBytesToCopy: imageBuffer.Length * sizeof(uint)
        );
    }

    var image = Image.WrapMemory<Rgba32>(imageBuffer, width, height);

    // Apply ImageSharp.Drawing: draw a red rectangle on top of the rendered PDF
    image.Mutate(ctx =>
    {
        ctx.DrawPolygon(Color.Red, 4f, new PointF[]
        {
            new(50, 50),
            new(width - 50, 50),
            new(width - 50, height - 50),
            new(50, height - 50),
            new(50, 50)
        });
    });

    image.SaveAsPng("rendered-drawing.png");

    PDFiumEngine.Shutdown();
```
# dotPDFium

**dotPDFium** is an early-stage, .NET 8+ wrapper around the native [PDFium](https://pdfium.googlesource.com/pdfium/) library, aiming to provide a safe and idiomatic C# interface for loading, parsing, and rendering PDF files.

This project is currently in a **pre-pre-alpha** state. Development is focused on establishing core object lifecycles (`PdfDocument`, `PdfPage`, `PdfText`), managing native resource ownership correctly, and wrapping the lower-level PDFium API with a clean, maintainable surface.

We're not yet feature-complete, stable, or ready for production use. This repository is for exploratory development and incremental wrapping of PDFium's \~425 functions in a .NET-friendly way.

The library contains the latest PDFium binaries (v138.0.7175.0) from chromium/7175, built by bblanchon (https://github.com/bblanchon/pdfium-binaries/releases) for Windows x64 & ARM64, Linux x64 & ARM64, and MacOS (as a universal dylib supporting x64 & ARM64).

We are at v0.2.4-prealpha, which is a *very* early pre-alpha version. The goal is to hit v1.0 within 3 months or so, maybe sooner.

### Goals (eventually)

- Managed lifecycle and memory safety for native PDFium handles
- Optional exceptions or `TryX()` patterns for common workflows
- Modular text extraction and page rendering APIs
- MIT-licensed wrapper code with proper attribution of PDFium (Apache 2.0)

### Status

- Basic document and page loading implemented
- Page handling and rendering implemented
- Text extraction imlemented
- Rendering and search implemented
- Security implemented
- Forms are a WIP; If you have suggestions, please open an issue or PR
- Annotations imlemented
- Very few formal tests
- No documentation or examples yet
- Structured trees and progressive loading are implemented
- XFA support is not planned

---

**Note:** This is a low-level interop library in early development. Expect sharp edges, breaking changes, and missing functionality.

### Usage
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
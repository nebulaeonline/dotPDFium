# dotPDFium

**dotPDFium** is an early-stage, .NET 8+ wrapper around the native [PDFium](https://pdfium.googlesource.com/pdfium/) library, aiming to provide a safe and idiomatic C# interface for loading, parsing, and rendering PDF files.

This project is currently in a **pre-pre-alpha** state. Development is focused on establishing core object lifecycles (`PdfDocument`, `PdfPage`, `PdfText`), managing native resource ownership correctly, and wrapping the lower-level PDFium API with a clean, maintainable surface.

We're not yet feature-complete, stable, or ready for production use. This repository is for exploratory development and incremental wrapping of PDFium's 190+ functions in a .NET-friendly way.

### Goals (eventually)

- Managed lifecycle and memory safety for native PDFium handles
- Optional exceptions or `TryX()` patterns for common workflows
- Modular text extraction and page rendering APIs
- MIT-licensed wrapper code with proper attribution of PDFium (Apache 2.0)

### Status

- Basic document and page loading
- Text extraction in progress
- Rendering and search implemented
- Very few formal tests
- No documentation or examples yet
- Need to build out: forms, annotations, extended search, and other features

---

**Note:** This is a low-level interop library in early development. Expect sharp edges, breaking changes, and missing functionality.


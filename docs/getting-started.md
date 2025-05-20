# Getting Started with dotPDFium:

The first thing to do is initialize the library before use (otherwise you'll get nasty library not found exceptions):

```csharp
PDFiumEngine.Init();
```
This handles initialization of both dotPDFium and the underlying pdfium library.

The last thing to remember to do is to shudown when you've finished with the library:

```csharp
PDFiumEngine.Shutdown();
```
---

## dotPDFium Basics

The library is designed to abstract away the pain of calling PDFium's native functions. It is a powerful library written in C++, but its external interface is (mostly) C-based. So to spare you that torture, we have wrapped up the bulk of its functionality into easy-to-use and idiomatic C# code.

The first thing you will want to do is create a [PdfDocument](https://nebulae.online/dotPDFium/api/nebulae.dotPDFium.PdfDocument.html) object. You can do that by 

- creating a completely new document:
```csharp
var doc = PdfDocument.CreateNew();
```

- loading an existing document from the file system:
```csharp
var doc = PdfDocument.LoadFromFile("/path/to/file.pdf");
```
or
```csharp
PdfDocument doc;
var result = PdfDocument.TryLoadFromFile("/path/to/file.pdf", doc)
```

- loading a document from memory:
```csharp
var doc = PdfDocument.LoadFromMemory(byteBuffer);
```
---

Once you have your document object, you can work with the pages of the document via the [PdfPage](https://nebulae.online/dotPDFium/api/nebulae.dotPDFium.PdfPage.html) class.

- creating a new page:
```csharp
var page = doc.CreatePage(0, 612, 792);
```
page units are in points, which are 1/72 of an inch (thanks Adobe); this is a standard U.S. 8.5" x 11" page.

- opening an existing page:
```csharp
using var page = doc.LoadPage(0);
```
pages are loaded by 0-based index, but be warned that page indexes can and will change out from under you if you insert or delete pages.

---

- retrieving document text can be straightforward (please see the caveats below):
```csharp
using var doc = PdfDocument.LoadFromFile("example.pdf");
var page = doc.LoadPage(0);
var textPage = page.GetOrLoadText();

string fullText = textPage.GetTextRange(0, textPage.CountChars);
Console.WriteLine(fullText);
```
- There are functions that handle extracting the text from a pdf, including locating characters at certain places on the page:
```csharp
using var doc = PdfDocument.LoadFromFile("example.pdf");
var page = doc.LoadPage(0);

// Load the text layer
var text = page.GetOrLoadText();

// Choose a point on the page in page coordinates (e.g., 72 = 1 inch from bottom-left)
double x = 150.0;
double y = 500.0;

// Find character at that point
int index = text.GetCharIndexAtPos(x, y);

if (index >= 0 && index < text.CountChars)
{
    uint ch = text.GetChar(index);
    Console.WriteLine($"Character at ({x}, {y}) is: {(char)ch} (U+{ch:X4})");
}
else
{
    Console.WriteLine("No character found at that point.");
}
```

---

page rendering is done through the [PdfBitmap](https://nebulae.online/dotPDFium/api/nebulae.dotPDFium.PdfBitmap.html) class and can be handled via System.Drawing on Windows, or you can use SixLabors' ImageSharp for true cross-platform compatibility. Another key point is that PdfDocuments are measured in points, while PdfBitmaps are measured in pixels. There are functions to convert between the two coordinate systems- see [DeviceToPage](https://nebulae.online/dotPDFium/api/nebulae.dotPDFium.PdfPage.html#nebulae_dotPDFium_PdfPage_DeviceToPage_System_Int32_System_Int32_System_Int32_System_Int32_nebulae_dotPDFium_Native_PdfPageRotation_System_Int32_System_Int32_System_Double__System_Double__) and [PageToDevice](https://nebulae.online/dotPDFium/api/nebulae.dotPDFium.PdfPage.html#nebulae_dotPDFium_PdfPage_PageToDevice_System_Int32_System_Int32_System_Int32_System_Int32_nebulae_dotPDFium_Native_PdfPageRotation_System_Double_System_Double_System_Int32__System_Int32__)

- Here's the System.Drawing example:
```csharp
using var doc = PdfDocument.LoadFromFile("example.pdf");
var page = doc.LoadPage(0);

int width = 800;
int height = (int)(width * (page.Height / page.Width));

using var pdfBmp = PdfBitmap.Create(width, height, PdfBitmapFormat.BGRA);
page.RenderToBitmap(pdfBmp, 0, 0, width, height);

using var bmp = new System.Drawing.Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, pdfBmp.Scan0);
bmp.Save("output-drawing.png", System.Drawing.Imaging.ImageFormat.Png);
```

- and the ImageSharp example:
```csharp
using var doc = PdfDocument.LoadFromFile("example.pdf");
var page = doc.LoadPage(0);

int width = 800;
int height = (int)(width * (page.Height / page.Width));

using var pdfBmp = PdfBitmap.Create(width, height, PdfBitmapFormat.BGRA);
page.RenderToBitmap(pdfBmp, 0, 0, width, height);

// Wrap PDFium memory with ImageSharp without copying
unsafe
{
    var image = Image.WrapMemory<SixLabors.ImageSharp.PixelFormats.Rgba32>(
        new Span<SixLabors.ImageSharp.PixelFormats.Rgba32>((void*)pdfBmp.Scan0, width * height),
        width,
        height
    );

    image.Save("output-sixlabors.png");
}

```

---

- dotPDFium does its best to hide any and all pointers into PDFium from the user. But if you ever need a pointer to call a native function (all are publicly exposed so you can venture out as you see fit), the object pointers are all accessed like so:
```csharp
myPdfObject.Handle
```
Handles can be passed to native functions without issue, but you are on your own if you venture over to that side of the library. Calls made outside the dotPDFium supplied interfaces are your responsibility to manage ownership and object destruction.

---

It's important to note that PDFium is a canvas-based renderer and not a text-flow based one. What that means is that each object has its own transformation matrix (rotation & scaling) plus stroke, color, fill, font and weight. What this means is that sometimes dealing with it can be finicky for text extraction, because each *character* might be (and is) its own object. There is no support for line endings or word wrapping. We are going to add some of these functions for convenience, but you have to keep in mind the limitations of the canvas rendering perspective of the underlying PDFium.

---

Those are the very basics to get you started. Check out the PdfDocument and PdfPage API docs, because most functions are fairly straightforward.

Check out the project tests on the Github site for some more complex examples like rendering to a bitmap and then painting to that bitmap and saving to a png file.

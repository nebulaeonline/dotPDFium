using nebulae.dotPDFium;
using nebulae.dotPDFium.Native;
namespace dotPDFiumTests;

public class CreateAndEditTests
{
    [Fact]
    public void SaveGeneratedTextPdf()
    {
        FS_MATRIX text_transform = new(1, 0, 0, 1, 100, 150);
        PDFiumEngine.Init();
        var doc = PdfDocument.CreateNew();
        var page = doc.CreatePage(0, 612, 792);
        var font = doc.LoadStandardFont("Helvetica");
        var text = doc.CreateTextObject(font, 12f);
        text.SetText("Hello from test!");
        text.Transform(text_transform);
        page.InsertObject(text);
        page.FinalizeContent();
        doc.SaveTo("generated.pdf");
        PDFiumEngine.Shutdown();
    }

}
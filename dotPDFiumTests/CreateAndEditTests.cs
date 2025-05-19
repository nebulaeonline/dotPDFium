using nebulae.dotPDFium;
using nebulae.dotPDFium.Native;
namespace dotPDFiumTests;

public class CreateAndEditTests
{
    [Fact]
    public void SaveGeneratedTextPdf()
    {
        PDFiumEngine.Init();
        var doc = PdfDocument.CreateNew();
        var page = doc.CreatePage(0, 612, 792);
        var font = doc.LoadStandardFont("Helvetica");
        var text = doc.CreateTextObject(font, 12f);
        text.SetText("Hello from test!");
        text.SetPosition(100, 700);
        page.InsertObject(text);
        page.FinalizeContent();
        doc.SaveTo("generated.pdf");
        PDFiumEngine.Shutdown();
    }
}
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;

using Xunit.Abstractions;
using Xunit.Sdk;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using nebulae.dotPDFium;

namespace dotPDFiumTests;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestPriorityAttribute : Attribute
{
    public int Priority { get; }

    public TestPriorityAttribute(int priority)
    {
        Priority = priority;
    }
}

public class PriorityOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        return testCases.OrderBy(tc =>
        {
            var attr = tc.TestMethod.Method
                .GetCustomAttributes(typeof(TestPriorityAttribute).AssemblyQualifiedName)
                .FirstOrDefault();

            return attr == null ? 0 : attr.GetNamedArgument<int>("Priority");
        });
    }
}

[CollectionDefinition("Sequential", DisableParallelization = true)]
public class SequentialCollection : ICollectionFixture<object> { }

[Collection("Sequential")]
public class CreateAndEditTests
{
    [Fact, TestPriority(1)]
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

    [Fact, TestPriority(2)]
    public unsafe void RenderFirstPageWithImageSharpDrawing()
    {
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
    }
}
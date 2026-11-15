using DormitoryXml.Code.Models;

namespace DormitoryXml.Code.Services;

public class HtmlExporter
{
    private readonly XslTransformer transformer;
    private readonly ResultsXmlBuilder resultsBuilder;

    public HtmlExporter(XslTransformer transformer, ResultsXmlBuilder resultsBuilder)
    {
        this.transformer = transformer;
        this.resultsBuilder = resultsBuilder;
    }

    public string Export(IEnumerable<Student> students, string xmlPath, string xslPath)
    {
        string baseName = Path.GetFileNameWithoutExtension(xmlPath) + "_result";
        string dir = Path.GetDirectoryName(xmlPath)!;
        if (!CanWrite(dir)) dir = Path.GetTempPath();

        string tempXml = Path.Combine(Path.GetTempPath(), baseName + ".xml");
        string htmlPath = Path.Combine(dir, baseName + ".html");

        try
        {
            resultsBuilder.Build(students, tempXml);
            transformer.Transform(tempXml, xslPath, htmlPath);
        }
        finally
        {
            if (File.Exists(tempXml)) File.Delete(tempXml);
        }
        return htmlPath;
    }

    private static bool CanWrite(string dir)
    {
        try
        {
            string probe = Path.Combine(dir, Guid.NewGuid().ToString("N") + ".tmp");
            File.WriteAllText(probe, "");
            File.Delete(probe);
            return true;
        }
        catch { return false; }
    }
}

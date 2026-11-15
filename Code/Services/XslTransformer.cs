using System.Xml.Xsl;

namespace DormitoryXml.Code.Services;

public class XslTransformer
{
    public void Transform(string xmlPath, string xslPath, string htmlPath)
    {
        var xslt = new XslCompiledTransform();
        xslt.Load(xslPath);
        xslt.Transform(xmlPath, htmlPath);
    }
}

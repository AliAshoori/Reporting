using System.Xml.Serialization;

namespace TechnicalTest.Shared
{
    [XmlRoot(ElementName = "Reports")]
    public class XmlReportRoot
    {
        [XmlElement(ElementName = "Report")]
        public XmlReport Report { get; set; }
    }
}

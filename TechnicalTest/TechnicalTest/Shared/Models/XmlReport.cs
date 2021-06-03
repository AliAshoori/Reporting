using System.Collections.Generic;
using System.Xml.Serialization;

namespace TechnicalTest.Shared
{
    [XmlRoot(ElementName = "Report")]
    public class XmlReport
    {
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "ReportVal")]
        public List<XmlReportItem> Items { get; set; }
    }
}

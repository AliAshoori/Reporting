using System.Xml.Serialization;

namespace TechnicalTest.Shared
{
    [XmlRoot(ElementName = "ReportVal")]
    public class XmlReportItem
    {
        [XmlElement(ElementName = "ReportRow")]
        public int? Row { get; set; }

        [XmlElement(ElementName = "ReportCol")]
        public int? Column { get; set; }

        [XmlElement(ElementName = "Val")]
        public decimal Value { get; set; }
    }
}

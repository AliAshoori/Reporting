using OfficeOpenXml;
using System.Collections.Generic;

namespace TechnicalTest.Shared
{
    public class ReportMergePayload
    {
        public ExcelWorksheet WorkSheet { get; }

        public ReportMergePayload(
            ExcelWorksheet workSheet,
            ExcelPackage package,
            IEnumerable<ReportValueCell> cells,
            XmlReportRoot xmlReportRoot)
        {
            WorkSheet = workSheet;
            Package = package;
            Cells = cells;
            XmlReportRoot = xmlReportRoot;
        }

        public ExcelPackage Package { get; }

        public IEnumerable<ReportValueCell> Cells { get; }

        public XmlReportRoot XmlReportRoot { get; }
    }
}

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
            IEnumerable<XmlReportItem> reportValues)
        {
            WorkSheet = workSheet;
            Package = package;
            Cells = cells;
            ReportValues = reportValues;
        }

        public ExcelPackage Package { get; }

        public IEnumerable<ReportValueCell> Cells { get; }

        public IEnumerable<XmlReportItem> ReportValues { get; }
    }
}

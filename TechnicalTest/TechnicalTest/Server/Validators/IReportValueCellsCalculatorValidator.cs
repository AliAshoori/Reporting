using OfficeOpenXml;

namespace TechnicalTest.Server
{
    public interface IReportValueCellsCalculatorValidator
    {
        void Validate(ExcelWorksheet worksheet);
    }

    public class ReportValueCellsCalculatorValidator : IReportValueCellsCalculatorValidator
    {
        public void Validate(ExcelWorksheet worksheet)
        {
            throw new System.NotImplementedException();
        }
    }
}

using TechnicalTest.Shared;

namespace TechnicalTest.Server
{
    public interface IReportValuesToExcelSheetMergerValidator
    {
        void Validate(ReportMergePayload payload);
    }

    public class ReportValuesToExcelSheetMergerValidator : IReportValuesToExcelSheetMergerValidator
    {
        public void Validate(ReportMergePayload payload)
        {
            throw new System.NotImplementedException();
        }
    }
}
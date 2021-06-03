using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using TechnicalTest.Shared;
using System.Linq;

namespace TechnicalTest.Server.Services
{
    public interface IReportValueCellsCalculator
    {
        IOrderedEnumerable<ReportValueCell> Calculate(ExcelWorksheet worksheet);
    }

    public class ReportValueCellsCalculator : IReportValueCellsCalculator
    {
        private readonly ILogger<ReportValueCellsCalculator> _logger;
        private readonly IReportValueCellsCalculatorValidator _validator;

        public ReportValueCellsCalculator(ILogger<ReportValueCellsCalculator> logger, IReportValueCellsCalculatorValidator validator)
        {
            _logger = logger.NotNull();
            _validator = validator.NotNull();
        }

        public IOrderedEnumerable<ReportValueCell> Calculate(ExcelWorksheet worksheet)
        {
            _validator.Validate(worksheet);

            var targetCells = Enumerable.Empty<ReportValueCell>();

            var values = worksheet.Cells.Value as object[,];

            for (int i = worksheet.Dimension.Start.Row; i < worksheet.Dimension.End.Row; i++)
            {
                for (int j = worksheet.Dimension.Start.Column; j < worksheet.Dimension.End.Column; j++)
                {
                    if (values[i, j] == null) continue;

                    var currentCellValue = values[i, j].ToString();

                    if (!string.IsNullOrWhiteSpace(currentCellValue) &&
                        currentCellValue.ToCharArray().All(c => char.IsDigit(c)))
                    {
                        var element = new ReportValueCell { Value = currentCellValue, Row = i.ExcelifyTheIndex(), Column = j.ExcelifyTheIndex() };
                        targetCells = targetCells.Append(element);
                    }
                }
            }

            return targetCells.OrderBy(cell => cell.Row);
        }
    }
}
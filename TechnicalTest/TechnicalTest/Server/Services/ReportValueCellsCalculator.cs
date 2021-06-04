using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using TechnicalTest.Shared;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechnicalTest.Server.Services
{
    public interface IReportValueCellsCalculator
    {
        IEnumerable<ReportValueCell> Calculate(ExcelWorksheet worksheet);
    }

    public class ReportValueCellsCalculator : IReportValueCellsCalculator
    {
        private readonly ILogger<ReportValueCellsCalculator> _logger;
        private readonly IReportValueCellsCalculatorValidator _validator;
        private readonly object _lock;

        public ReportValueCellsCalculator(ILogger<ReportValueCellsCalculator> logger, IReportValueCellsCalculatorValidator validator)
        {
            _logger = logger.NotNull();
            _validator = validator.NotNull();
            _lock = new object();
        }

        public IEnumerable<ReportValueCell> Calculate(ExcelWorksheet worksheet)
        {
            _validator.Validate(worksheet);

            var targetCells = Enumerable.Empty<ReportValueCell>();

            var values = worksheet.Cells.Value as object[,];

            _logger.LogInformation($"Now starting to calculate the report values cells in the excel sheet. Row: {worksheet.Cells.Rows}, Columns: {worksheet.Cells.Columns}");

            Parallel.For(worksheet.Dimension.Start.Row, worksheet.Dimension.End.Row, row =>
            {
                Parallel.For(worksheet.Dimension.Start.Column, worksheet.Dimension.End.Column, column =>
                {
                    lock (_lock)
                    {
                        if (values[row, column] == null) return;

                        var currentCellValue = values[row, column].ToString();

                        if (!string.IsNullOrWhiteSpace(currentCellValue) &&
                            currentCellValue.ToCharArray().All(c => char.IsDigit(c)))
                        {
                            var element = new ReportValueCell { Value = currentCellValue, Row = row.ExcelifyTheIndex(), Column = column.ExcelifyTheIndex() };
                            targetCells = targetCells.Append(element);
                        }
                    }
                });
            });

            _logger.LogInformation($"Successfully calculated the report value cells in the excel sheet. Found: {targetCells.Count()} cells.");

            return targetCells.OrderBy(cell => cell.Row);
        }
    }
}
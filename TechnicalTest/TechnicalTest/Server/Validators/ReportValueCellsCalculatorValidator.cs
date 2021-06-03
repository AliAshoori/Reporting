using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using TechnicalTest.Shared;

namespace TechnicalTest.Server
{
    public interface IReportValueCellsCalculatorValidator
    {
        void Validate(ExcelWorksheet worksheet);
    }

    public class ReportValueCellsCalculatorValidator : IReportValueCellsCalculatorValidator
    {
        private readonly ILogger<ReportValueCellsCalculatorValidator> _logger;

        public ReportValueCellsCalculatorValidator(ILogger<ReportValueCellsCalculatorValidator> logger)
        {
            _logger = logger.NotNull();
        }

        public void Validate(ExcelWorksheet worksheet)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException(nameof(worksheet));
            }

            if (worksheet.Cells == null)
            {
                throw new InvalidOperationException($"Worksheet {worksheet.Name} has not Cells added to it");
            }
        }
    }
}

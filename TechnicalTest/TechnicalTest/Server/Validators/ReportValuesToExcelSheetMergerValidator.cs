using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TechnicalTest.Shared;

namespace TechnicalTest.Server
{
    public interface IReportValuesToExcelSheetMergerValidator
    {
        void Validate(ReportMergePayload payload);
    }

    public class ReportValuesToExcelSheetMergerValidator : IReportValuesToExcelSheetMergerValidator
    {
        private readonly ILogger<ReportValuesToExcelSheetMergerValidator> _logger;

        public ReportValuesToExcelSheetMergerValidator(ILogger<ReportValuesToExcelSheetMergerValidator> logger)
        {
            _logger = logger.NotNull();
        }

        public void Validate(ReportMergePayload payload)
        {
            _logger.LogInformation($"Now validating the merge payload object before writing the report values.");

            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (payload.Cells == null || !payload.Cells.Any())
            {
                throw new InvalidOperationException($"{nameof(ReportMergePayload)} failed. No user defined cells found.");
            }

            IEnumerable<ReportValueCell> targetRows = payload.Cells.GroupBy(t => t.Row).Where(r => r.Count() == 1).SelectMany(item => item.ToArray());
            IEnumerable<ReportValueCell> targetColumns = payload.Cells.GroupBy(t => t.Column).Where(c => c.Count() == 1).SelectMany(item => item.ToArray());

            if (!targetRows.Any() || !targetColumns.Any())
            {
                throw new InvalidOperationException($"{nameof(ReportMergePayload)} validation failed. No target rows/columns detected.");
            }

            if (payload.ReportValues == null || !payload.ReportValues.Any())
            {
                throw new InvalidOperationException($"{nameof(ReportMergePayload)} validation failed. No report values found.");
            }

            _logger.LogInformation($"Merge payload object seems to be a valida one.");
        }
    }
}
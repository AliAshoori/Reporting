using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechnicalTest.Shared;

namespace TechnicalTest.Server.Services
{
    public interface IReportGenerator
    {
        Task<ReportFinal> GenerateAsync();
    }

    public class ReportGenerator : IReportGenerator
    {
        private readonly ILogger<ReportGenerator> _logger;
        private readonly IOptions<DatabaseSettings> _options;
        private readonly IAppXmlParser _xmlParser;
        private readonly IExcelToDataTableParser _excelToDataTableParser;
        private readonly IDataTableToRawHtmlParser _dataTableToHtmlParser;
        private readonly IReportValueCellsCalculator _valueCellCalculator;
        private readonly IReportValuesToExcelSheetMerger _merger;

        public ReportGenerator(
            ILogger<ReportGenerator> logger,
            IAppXmlParser xmlParser,
            IExcelToDataTableParser excelToDataTableParser,
            IDataTableToRawHtmlParser dataTableToHtmlParser,
            IOptions<DatabaseSettings> options,
            IReportValueCellsCalculator valueCellCalculator,
            IReportValuesToExcelSheetMerger merger)
        {
            _logger = logger.NotNull();
            _options = options.NotNull();
            _options.Value.NotNull();
            _xmlParser = xmlParser.NotNull();
            _excelToDataTableParser = excelToDataTableParser.NotNull();
            _dataTableToHtmlParser = dataTableToHtmlParser.NotNull();
            _valueCellCalculator = valueCellCalculator.NotNull();
            _merger = merger.NotNull();
        }

        public async Task<ReportFinal> GenerateAsync()
        {
            try
            {
                _logger.LogInformation($"Starting the process of report generation at {DateTime.Now}");

                var finalReport = new ReportFinal { RawHtml = string.Empty };

                FileInfo file = new FileInfo(_options.Value.ReportValueFileAddress);
                XmlReportRoot reportValues = await _xmlParser.ParseAsync(file);

                _logger.LogInformation($"Read Report values from XML file: {_options.Value.ReportValueFileAddress}.");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var fileAsByteArray = await File.ReadAllBytesAsync(_options.Value.ReportTemplateFileAddress);

                using (MemoryStream memoryStream = new MemoryStream(fileAsByteArray))
                {
                    using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
                    {
                        var worksheet = excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == _options.Value.ReportSheetName);

                        var targetCells = _valueCellCalculator.Calculate(worksheet);

                        _logger.LogInformation($"Worksheet with name: {worksheet.Name} has been read. {targetCells.Count()} calculated out of it");

                        var payload = new ReportMergePayload(worksheet, excelPackage, targetCells, reportValues);

                        var mergedFile = await _merger.MergeAsync(payload);

                        var reportDataTable = _excelToDataTableParser.ParseToDataTable(mergedFile, _options.Value.ReportSheetName);

                        finalReport.RawHtml = _dataTableToHtmlParser.Parse(reportDataTable);
                    }
                }

                _logger.LogInformation($"Report generation process has successfully compeleted at {DateTime.Now}");

                return finalReport;

            }
            catch (AggregateException aggregateExceptions)
            {
                foreach (var exception in aggregateExceptions.InnerExceptions)
                {
                    _logger.LogError($"{exception}");
                }
                throw;
            }
        }
    }
}
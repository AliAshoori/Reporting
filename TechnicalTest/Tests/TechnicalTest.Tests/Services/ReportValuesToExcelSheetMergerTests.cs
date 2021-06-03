using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechnicalTest.Server;
using TechnicalTest.Server.Services;
using TechnicalTest.Shared;

namespace TechnicalTest.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ReportValuesToExcelSheetMergerTests
    {
        MemoryStream _memoryStream;
        ExcelPackage _excelPackage;

        [TestCleanup]
        public void Cleanup()
        {
            if (_memoryStream != null)
                _memoryStream.Dispose();

            if (_excelPackage != null)
                _excelPackage.Dispose();
        }

        [TestMethod]
        public async Task MergeAsync_HappyScenario_MergeToExcelFile()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValuesToExcelSheetMerger>>();

            var mockValidator = new Mock<IReportValuesToExcelSheetMergerValidator>();
            mockValidator.Setup(m => m.Validate(It.IsAny<ReportMergePayload>())).Verifiable();

            var merger = new ReportValuesToExcelSheetMerger(mockLogger.Object, mockValidator.Object);

            var cells = new List<ReportValueCell>
            {
                new ReportValueCell
                {
                    Value = "010", Row  = 10, Column = 5
                },
                new ReportValueCell
                {
                    Value = "011", Row  = 10, Column = 6
                },
                new ReportValueCell
                {
                    Value = "012", Row  = 10, Column = 7
                },
                new ReportValueCell
                {
                    Value = "022", Row  = 10, Column = 8
                },
                new ReportValueCell
                {
                    Value = "025", Row  = 10, Column = 9
                },
                new ReportValueCell
                {
                    Value = "031", Row  = 10, Column = 10
                },
                new ReportValueCell
                {
                    Value = "040", Row  = 10, Column = 11
                },
                new ReportValueCell
                {
                    Value = "010", Row  = 11, Column = 2
                },
                new ReportValueCell
                {
                    Value = "020", Row  = 12, Column = 2
                }
            };

            var reportValues = new List<XmlReportItem>
                    {
                        new XmlReportItem
                        {
                            Row = 10, Column = 10, Value = 100
                        },
                        new XmlReportItem
                        {
                            Row = 10, Column = 11, Value = 200
                        },
                        new XmlReportItem
                        {
                            Row = 10, Column = 12, Value = 0
                        },
                        new XmlReportItem
                        {
                            Row = 20, Column = 10, Value = 600
                        },
                        new XmlReportItem
                        {
                            Row = 20, Column = 11, Value = 500
                        },
                        new XmlReportItem
                        {
                            Row = 20, Column = 12, Value = 0
                        }
                    };

            var file = await File.ReadAllBytesAsync("Data/ExcelReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            _memoryStream = new MemoryStream(file);
            _excelPackage = new ExcelPackage(_memoryStream);
            var worksheet = _excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == "F 20.04");

            var payload = new ReportMergePayload(worksheet, _excelPackage, cells, reportValues);

            // Act
            Func<Task> mergeFunction = async () => await merger.MergeAsync(payload);

            // Assert
            mergeFunction.Should().NotThrow();
        }

        [TestMethod]
        public void ReportValuesToExcelSheetMerger_WithNoLogger_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<ReportValuesToExcelSheetMerger> logger = null;
            var mockValidator = new Mock<IReportValuesToExcelSheetMergerValidator>();

            // Act
            Action initFunction = () => new ReportValuesToExcelSheetMerger(logger, mockValidator.Object);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(logger));
        }

        [TestMethod]
        public void ReportValuesToExcelSheetMerger_WithNoValidator_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValuesToExcelSheetMerger>>();
            IReportValuesToExcelSheetMergerValidator validator = null;

            // Act
            Action initFunction = () => new ReportValuesToExcelSheetMerger(mockLogger.Object, validator);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(validator));
        }
    }
}
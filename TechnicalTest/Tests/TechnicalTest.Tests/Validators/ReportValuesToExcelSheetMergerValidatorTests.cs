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
using TechnicalTest.Server;
using TechnicalTest.Shared;

namespace TechnicalTest.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ReportValuesToExcelSheetMergerValidatorTests
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
        public void Validate_HappyScenario_PassesAllValidations()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValuesToExcelSheetMergerValidator>>();
            var file = File.ReadAllBytes("Data/ExcelReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            _memoryStream = new MemoryStream(file);
            _excelPackage = new ExcelPackage(_memoryStream);
            var worksheet = _excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == "EMPTY");

            var cells = new List<ReportValueCell>
            {
                new ReportValueCell
                {
                    Row = 10, Column = 1, Value = "010"
                },
                new ReportValueCell
                {
                    Row = 10, Column = 2, Value = "011"
                },
                new ReportValueCell
                {
                    Row = 11, Column = 8, Value = "012"
                },
                new ReportValueCell
                {
                    Row = 12, Column = 4, Value = "013"
                }
            };

            var reportValues = new List<XmlReportItem>
            {
                new XmlReportItem
                {
                    Column = 1, Row = 10, Value = 900
                }
            };

            var payload = new ReportMergePayload(worksheet, _excelPackage, cells, reportValues);

            var validator = new ReportValuesToExcelSheetMergerValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.Validate(payload);

            // Assert
            validateFunction.Should().NotThrow();
        }

        [TestMethod]
        public void Validate_WithNullPayload_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValuesToExcelSheetMergerValidator>>();
            ReportMergePayload payload = null;

            var validator = new ReportValuesToExcelSheetMergerValidator(mockLogger.Object);

            // Act
            Action validatorFunction = () => validator.Validate(payload);

            // Assert
            validatorFunction.Should().ThrowExactly<ArgumentNullException>(nameof(payload));
        }

        [TestMethod]
        public void Validate_WithNoTargetRowsFound_ThrowsException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValuesToExcelSheetMergerValidator>>();
            var file = File.ReadAllBytes("Data/ExcelReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            _memoryStream = new MemoryStream(file);
            _excelPackage = new ExcelPackage(_memoryStream);
            var worksheet = _excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == "EMPTY");

            var cells = new List<ReportValueCell>
            {
                new ReportValueCell
                {
                    Row = 10, Column = 1, Value = "010"
                },
                new ReportValueCell
                {
                    Row = 10, Column = 2, Value = "011"
                }
            };

            var reportValues = Enumerable.Empty<XmlReportItem>();

            var payload = new ReportMergePayload(worksheet, _excelPackage, cells, reportValues);

            var validator = new ReportValuesToExcelSheetMergerValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.Validate(payload);

            // Assert
            validateFunction.Should().Throw<Exception>();
        }

        [TestMethod]
        public void Validate_WithNoTargetColumnsFound_ThrowsException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValuesToExcelSheetMergerValidator>>();
            var file = File.ReadAllBytes("Data/ExcelReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            _memoryStream = new MemoryStream(file);
            _excelPackage = new ExcelPackage(_memoryStream);
            var worksheet = _excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == "EMPTY");

            var cells = new List<ReportValueCell>
            {
                new ReportValueCell
                {
                    Row = 11, Column = 1, Value = "010"
                },
                new ReportValueCell
                {
                    Row = 10, Column = 1, Value = "011"
                }
            };

            var reportValues = Enumerable.Empty<XmlReportItem>();

            var payload = new ReportMergePayload(worksheet, _excelPackage, cells, reportValues);

            var validator = new ReportValuesToExcelSheetMergerValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.Validate(payload);

            // Assert
            validateFunction.Should().Throw<Exception>();
        }

        [TestMethod]
        public void Validate_WithNoReportValuesFound_ThrowsException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValuesToExcelSheetMergerValidator>>();
            var file = File.ReadAllBytes("Data/ExcelReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            _memoryStream = new MemoryStream(file);
            _excelPackage = new ExcelPackage(_memoryStream);
            var worksheet = _excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == "EMPTY");

            var cells = new List<ReportValueCell>
            {
                new ReportValueCell
                {
                    Row = 10, Column = 1, Value = "010"
                },
                new ReportValueCell
                {
                    Row = 10, Column = 2, Value = "011"
                },
                new ReportValueCell
                {
                    Row = 11, Column = 8, Value = "012"
                },
                new ReportValueCell
                {
                    Row = 12, Column = 4, Value = "013"
                }
            };

            var reportValues = Enumerable.Empty<XmlReportItem>();

            var payload = new ReportMergePayload(worksheet, _excelPackage, cells, reportValues);

            var validator = new ReportValuesToExcelSheetMergerValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.Validate(payload);

            // Assert
            validateFunction.Should().Throw<Exception>();
        }


        [TestMethod]
        public void Validate_WithCellsContainingNonNumericValue_ThrowsException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValuesToExcelSheetMergerValidator>>();
            var file = File.ReadAllBytes("Data/ExcelReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            _memoryStream = new MemoryStream(file);
            _excelPackage = new ExcelPackage(_memoryStream);
            var worksheet = _excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == "EMPTY");

            var cells = new List<ReportValueCell>
            {
                new ReportValueCell
                {
                    Row = 10, Column = 1, Value = "NON_NUMERIC"
                },
                new ReportValueCell
                {
                    Row = 10, Column = 2, Value = "012"
                },
                new ReportValueCell
                {
                    Row = 11, Column = 8, Value = "015"
                },
                new ReportValueCell
                {
                    Row = 12, Column = 4, Value = "NON_NUMERIC"
                }
            };

            var reportValues = Enumerable.Empty<XmlReportItem>();

            var payload = new ReportMergePayload(worksheet, _excelPackage, cells, reportValues);

            var validator = new ReportValuesToExcelSheetMergerValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.Validate(payload);

            // Assert
            validateFunction.Should().Throw<Exception>();
        }

        [TestMethod]
        public void ReportValuesToExcelSheetMergerValidator_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<ReportValuesToExcelSheetMergerValidator> logger = null;

            // Act
            Action initFunction = () => new ReportValuesToExcelSheetMergerValidator(logger);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(logger));
        }
    }
}

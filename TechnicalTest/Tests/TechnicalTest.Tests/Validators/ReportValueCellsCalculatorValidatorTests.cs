using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OfficeOpenXml;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using TechnicalTest.Server;

namespace TechnicalTest.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ReportValueCellsCalculatorValidatorTests
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
            var mockLogger = new Mock<ILogger<ReportValueCellsCalculatorValidator>>();

            var validator = new ReportValueCellsCalculatorValidator(mockLogger.Object);

            var file = File.ReadAllBytes("Data/ExcelReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelWorksheet worksheet = null;
            _memoryStream = new MemoryStream(file);
            _excelPackage = new ExcelPackage(_memoryStream);
            worksheet = _excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == "F 20.04");

            // Act
            Action validateFunction = () => validator.Validate(worksheet);

            // Assert
            validateFunction.Should().NotThrow();
        }

        [TestMethod]
        public void Validate_WithNullWorksheet_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValueCellsCalculatorValidator>>();
            ExcelWorksheet worksheet = null;

            var validator = new ReportValueCellsCalculatorValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.Validate(worksheet);

            // Assert
            validateFunction.Should().ThrowExactly<ArgumentNullException>(nameof(worksheet));
        }

        [TestMethod]
        public void Validate_WithNullWorksheetCells_ThrowsNullReferenceException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValueCellsCalculatorValidator>>();
            ExcelWorksheet worksheet = null;

            var validator = new ReportValueCellsCalculatorValidator(mockLogger.Object);

            var file = File.ReadAllBytes("Data/ExcelReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _memoryStream = new MemoryStream(file);
            _excelPackage = new ExcelPackage(_memoryStream);
            worksheet = _excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == "F 20.04");
            _memoryStream.Dispose();
            _excelPackage.Dispose();

            // Act
            Action validateFunction = () => validator.Validate(worksheet);

            // Assert
            validateFunction.Should().ThrowExactly<NullReferenceException>();
        }

        [TestMethod]
        public void ReportValueCellsCalculatorValidator_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<ReportValueCellsCalculatorValidator> logger = null;

            // Act
            Action initFunction = () => new ReportValueCellsCalculatorValidator(logger);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(logger));
        }
    }
}

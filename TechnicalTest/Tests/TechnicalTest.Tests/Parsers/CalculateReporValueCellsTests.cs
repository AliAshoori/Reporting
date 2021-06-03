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
    public class ReportValueCellsCalculatorTests
    {
        [TestMethod]
        public async Task Calculate_HappyScenario_ReturnsReportValueCells()
        {
            var mockLogger = new Mock<ILogger<ReportValueCellsCalculator>>();
            var mockValidator = new Mock<IReportValueCellsCalculatorValidator>();
            mockValidator.Setup(m => m.Validate(It.IsAny<ExcelWorksheet>())).Verifiable();

            var calculator = new ReportValueCellsCalculator(mockLogger.Object, mockValidator.Object);

            var expected = new List<ReportValueCell>
            {
                new ReportValueCell
                {
                    Value = "010", Row  = 9, Column = 4
                },
                new ReportValueCell
                {
                    Value = "011", Row  = 9, Column = 5
                },
                new ReportValueCell
                {
                    Value = "012", Row  = 9, Column = 6
                },
                new ReportValueCell
                {
                    Value = "022", Row  = 9, Column = 7
                },
                new ReportValueCell
                {
                    Value = "025", Row  = 9, Column = 8
                },
                new ReportValueCell
                {
                    Value = "031", Row  = 9, Column = 9
                },
                new ReportValueCell
                {
                    Value = "040", Row  = 9, Column = 10
                },
                new ReportValueCell
                {
                    Value = "010", Row  = 10, Column = 1
                },
                new ReportValueCell
                {
                    Value = "020", Row  = 11, Column = 1
                }
            };

            var actual = Enumerable.Empty<ReportValueCell>();

            var file = await File.ReadAllBytesAsync("Data/ExcelReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Act
            using (MemoryStream memoryStream = new MemoryStream(file))
            {
                using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
                {
                    var worksheet = excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == "F 20.04");
                    actual = calculator.Calculate(worksheet);
                }
            }

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task Calculate_WithNoUserDefinedIndexCellsFound_ThrowsException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValueCellsCalculator>>();
            var mockValidator = new Mock<IReportValueCellsCalculatorValidator>();
            mockValidator.Setup(m => m.Validate(It.IsAny<ExcelWorksheet>())).Verifiable();

            var calculator = new ReportValueCellsCalculator(mockLogger.Object, mockValidator.Object);

            var expected = Enumerable.Empty<ReportValueCell>();
            var actual = Enumerable.Empty<ReportValueCell>();

            var file = await File.ReadAllBytesAsync("Data/ExcelReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Act
            using (MemoryStream memoryStream = new MemoryStream(file))
            {
                using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
                {
                    var worksheet = excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == "WithNoUserDefinedIndex");
                    actual = calculator.Calculate(worksheet);
                }
            }

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void Calculate_WithValidationFails_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValueCellsCalculator>>();
            var mockValidator = new Mock<IReportValueCellsCalculatorValidator>();
            mockValidator.Setup(m => m.Validate(It.IsAny<ExcelWorksheet>())).Throws<ArgumentNullException>();

            var calculator = new ReportValueCellsCalculator(mockLogger.Object, mockValidator.Object);
            var mockWorksheet = new Mock<ExcelWorksheet>();

            // Act
            Action calculateFunction = () => calculator.Calculate(mockWorksheet.Object);

            // Assert
            calculateFunction.Should().ThrowExactly<ArgumentNullException>("worksheet");
        }

        [TestMethod]
        public void ReportValueCellsCalculator_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<ReportValueCellsCalculator> logger = null;
            var mockValidator = new Mock<IReportValueCellsCalculatorValidator>();

            // Act
            Action Init = () => new ReportValueCellsCalculator(logger, mockValidator.Object);

            // Assert
            Init.Should().ThrowExactly<ArgumentNullException>(nameof(logger));
        }

        [TestMethod]
        public void ReportValueCellsCalculator_WithNullValidator_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValueCellsCalculator>>();
            IReportValueCellsCalculatorValidator validator = null;

            // Act
            Action Init = () => new ReportValueCellsCalculator(mockLogger.Object, validator);

            // Assert
            Init.Should().ThrowExactly<ArgumentNullException>(nameof(validator));
        }
    }
}
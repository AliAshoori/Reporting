﻿using FluentAssertions;
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

namespace TechnicalTest.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ReporValueCellsCalculatorTests
    {
        [TestMethod]
        public async Task Calculate_HappyScenario_ReturnsReportValueCells()
        {
            var mockLogger = new Mock<ILogger<ReporValueCellsCalculator>>();
            var mockValidator = new Mock<ReporValueCellsCalculatorValidator>();
            mockValidator.Setup(m => m.Validate(It.IsAny<ExcelWorksheet>())).Verifiable();

            var calculator = new ReporValueCellsCalculator(mockLogger.Object, mockValidator.Object);

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

            IEnumerable<ReportValueCell> actual = Enumerable.Empty<ReportValueCell>();

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
            var mockLogger = new Mock<ILogger<ReporValueCellsCalculator>>();
            var mockValidator = new Mock<ReporValueCellsCalculatorValidator>();
            mockValidator.Setup(m => m.Validate(It.IsAny<ExcelWorksheet>())).Verifiable();

            var calculator = new ReporValueCellsCalculator(mockLogger.Object, mockValidator.Object);

            var expected = Enumerable.Empty<ReportValueCell>();

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
            var mockLogger = new Mock<ILogger<ReporValueCellsCalculator>>();
            var mockValidator = new Mock<ReporValueCellsCalculatorValidator>();
            mockValidator.Setup(m => m.Validate(It.IsAny<ExcelWorksheet>())).Throws<ArgumentNullException>();

            var calculator = new ReporValueCellsCalculator(mockLogger.Object, mockValidator.Object);
            var mockWorksheet = new Mock<ExcelWorksheet>();

            // Act
            Action calculateFunction = () => calculator.Calculate(mockWorksheet.Object);

            // Assert
            calculateFunction.Should().ThrowExactly<ArgumentNullException>("worksheet");
        }

        [TestMethod]
        public void ReporValueCellsCalculator_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<ReporValueCellsCalculator> logger = null;
            var mockValidator = new Mock<ReporValueCellsCalculatorValidator>();

            // Act
            Action Init = () => new ReporValueCellsCalculator(logger, mockValidator.Object);

            // Assert
            Init.Should().ThrowExactly<ArgumentNullException>(nameof(logger));
        }

        [TestMethod]
        public void ReporValueCellsCalculator_WithNullValidator_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReporValueCellsCalculator>>();
            IReporValueCellsCalculator validator = null;

            // Act
            Action Init = () => new ReporValueCellsCalculator(mockLogger.Object, validator);

            // Assert
            Init.Should().ThrowExactly<ArgumentNullException>(nameof(validator));
        }
    }
}
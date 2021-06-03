using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using TechnicalTest.Server;

namespace TechnicalTest.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ExcelToDataTableParserTests
    {
        [TestMethod]
        public void ParseToDataTable_HappyScenario_ReturnsDataTable()
        {
            // Arrange
            var file = new FileInfo("Data/ExcelReport.xlsx");
            const string sheetName = "F 20.04";

            var mockValidator = new Mock<IExcelFileValidator>();
            mockValidator.Setup(m => m.ValidateDataSourceFile(file, sheetName)).Verifiable();

            var mockLogger = new Mock<ILogger<ExcelToDataTableParser>>();

            var parser = new ExcelToDataTableParser(mockValidator.Object, mockLogger.Object);

            // Act
            var actual = parser.ParseToDataTable(file, sheetName);

            // Assert
            actual.Rows.Should().HaveCount(11);
            actual.Columns.Should().HaveCount(11);
        }

        [TestMethod]
        public void ParseToDataTable_WithInvalidFile_ThrowsException()
        {
            // Arrange
            var file = new FileInfo("Data/I_AM_NOT_VALID_FILE_PATH.xlsx");
            const string sheetName = "F 20.04";

            var mockValidator = new Mock<IExcelFileValidator>();
            mockValidator.Setup(m => m.ValidateDataSourceFile(file, sheetName)).Throws<FileNotFoundException>();

            var mockLogger = new Mock<ILogger<ExcelToDataTableParser>>();

            var parser = new ExcelToDataTableParser(mockValidator.Object, mockLogger.Object);

            // Act
            Action parseFunction = () => parser.ParseToDataTable(file, sheetName);

            // Assert
            parseFunction.Should().Throw<FileNotFoundException>();
        }

        [TestMethod]
        public void ParseToDataTable_WithEmptyFile_ReturnsEmptyDataTable()
        {
            // Arrange
            var file = new FileInfo("Data/ExcelReport.xlsx");
            const string sheetName = "EMPTY";

            var mockValidator = new Mock<IExcelFileValidator>();
            mockValidator.Setup(m => m.ValidateDataSourceFile(file, sheetName)).Verifiable();

            var mockLogger = new Mock<ILogger<ExcelToDataTableParser>>();

            var parser = new ExcelToDataTableParser(mockValidator.Object, mockLogger.Object);

            // Act
            var actual = parser.ParseToDataTable(file, sheetName);

            // Assert
            actual.Rows.Should().HaveCount(0);
            actual.Columns.Should().HaveCount(1);
        }

        [TestMethod]
        public void ParseToDataTable_WithInvalidSheetName_ReturnsEmptyDataTable()
        {
            // Arrange
            var file = new FileInfo("Data/HappyScenarioExcelReport.xlsx");
            const string sheetName = "I_AM_INVALID_SHEET_NAME";

            var mockValidator = new Mock<IExcelFileValidator>();
            mockValidator.Setup(m => m.ValidateDataSourceFile(file, sheetName)).Verifiable();

            var mockLogger = new Mock<ILogger<ExcelToDataTableParser>>();

            var parser = new ExcelToDataTableParser(mockValidator.Object, mockLogger.Object);

            // Act
            Action parserFunction = () => parser.ParseToDataTable(file, sheetName);

            // Assert
            parserFunction.Should().ThrowExactly<OleDbException>("'I_AM_INVALID_SHEET_NAME' is not a valid name. Make sure that it does not include invalid characters or punctuation and that it is not too long.");
        }

        [TestMethod]
        public void ExcelToDataTableParser_WithNullLoggerPassed_ThrowsArgumentNullException()
        {
            // Arrange
            var mockValidator = new Mock<IExcelFileValidator>();
            ILogger<ExcelToDataTableParser> logger = null;

            // Act
            Action initFunction = () => new ExcelToDataTableParser(mockValidator.Object, logger);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(logger));
        }

        [TestMethod]
        public void ExcelToDataTableParser_WithNullValidatorPassed_ThrowsArgumentNullException()
        {
            // Arrange
            IExcelFileValidator validator = null;
            var mockLogger = new Mock<ILogger<ExcelToDataTableParser>>();

            // Act
            Action initFunction = () => new ExcelToDataTableParser(validator, mockLogger.Object);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(validator));
        }
    }
}

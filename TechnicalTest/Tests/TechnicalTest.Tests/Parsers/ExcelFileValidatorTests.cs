using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using TechnicalTest.Server.Services;

namespace TechnicalTest.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ExcelFileValidatorTests
    {
        [TestMethod]
        public void ValidateDataSourceFile_HappyScenari_PassesValidations()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ExcelFileValidator>>();
            var file = new FileInfo("Data/HappyScenarioExcelReport.xlsx");
            const string sheetName = "F 20.04";

            var validator = new ExcelFileValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.ValidateDataSourceFile(file, sheetName);

            // Assert
            validateFunction.Should().NotThrow();
        }

        [TestMethod]
        public void ValidateDataSourceFile_WithNullFile_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ExcelFileValidator>>();
            FileInfo file = null;
            const string sheetName = "F 20.04";
            var validator = new ExcelFileValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.ValidateDataSourceFile(file, sheetName);

            // Assert
            validateFunction.Should().ThrowExactly<ArgumentNullException>(nameof(file));
        }

        [TestMethod]
        [DataRow(" ")]
        [DataRow("")]
        [DataRow(null)]
        public void ValidateDataSourceFile_WithNullSheetName_ThrowsArgumentNullException(string sheetName)
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ExcelFileValidator>>();
            FileInfo file = null;
            var validator = new ExcelFileValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.ValidateDataSourceFile(file, sheetName);

            // Assert
            validateFunction.Should().ThrowExactly<ArgumentNullException>(nameof(sheetName));
        }

        [TestMethod]
        public void ValidateDataSourceFile_WithWrongFileAddress_ThrowsFileNotFound()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ExcelFileValidator>>();
            var file = new FileInfo("Data/I_AM_NOT_VALID_FILE_PATH.xlsx");
            const string sheetName = "F 20.04";
            var validator = new ExcelFileValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.ValidateDataSourceFile(file, sheetName);

            // Assert
            validateFunction.Should().ThrowExactly<FileNotFoundException>(nameof(file.FullName));
        }

        [TestMethod]
        public void ValidateDataSourceFile_WithInvalidFileExtension_ThrowsNotSupportedException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ExcelFileValidator>>();
            var file = new FileInfo("Data/OldExcelNotSupported.xls");
            const string sheetName = "F 20.04";
            var validator = new ExcelFileValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.ValidateDataSourceFile(file, sheetName);

            // Assert
            validateFunction.Should().ThrowExactly<NotSupportedException>("Only xlsx spread sheet files are supported");
        }

        [TestMethod]
        public void ExcelFileValidator_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<ExcelFileValidator> logger = null;

            // Act
            Action initFunction = () => new ExcelFileValidator(logger);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(logger));
        }
    }
}

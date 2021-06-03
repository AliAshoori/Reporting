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
    public class XmlFileValidatorTests
    {
        [TestMethod]
        public void Validate_HappyScenario_NoExceptionThrows()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<XmlFileValidator>>();
            var file = new FileInfo("Data/HappyScenarioReport.xml");
            var validator = new XmlFileValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.ValidateFileTypeDataSourceFile(file);

            // Assert
            validateFunction.Should().NotThrow();
        }

        [TestMethod]
        public void Validate_WithFileInfoToBeNull_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<XmlFileValidator>>();
            FileInfo file = null;
            var validator = new XmlFileValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.ValidateFileTypeDataSourceFile(file);

            // Assert
            validateFunction.Should().ThrowExactly<ArgumentNullException>(nameof(file));
        }

        [TestMethod]
        public void Validate_WithFileNotFound_ThrowsFileNotFoundException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<XmlFileValidator>>();
            var file = new FileInfo("Data/I_AM_NOT_VALID_FILE_PATH.xml");
            var validator = new XmlFileValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.ValidateFileTypeDataSourceFile(file);

            // Assert
            validateFunction.Should().ThrowExactly<FileNotFoundException>(nameof(file.FullName));
        }

        [TestMethod]
        public void Validate_WithNonXmlFilePassed_ThrowsInvalidOperationException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<XmlFileValidator>>();
            var file = new FileInfo("Data/SomeTextFile.txt");
            var validator = new XmlFileValidator(mockLogger.Object);

            // Act
            Action validateFunction = () => validator.ValidateFileTypeDataSourceFile(file);

            // Assert
            validateFunction.Should().ThrowExactly<FormatException>(nameof(file.FullName));
        }

        [TestMethod]
        public void XmlFileValidator_WithNoLogPassed_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<XmlFileValidator> logger = null;

            // Act
            Action initFunction = () => new XmlFileValidator(logger);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(logger));
        }
    }
}

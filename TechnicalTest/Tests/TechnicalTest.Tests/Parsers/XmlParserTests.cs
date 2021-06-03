using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using TechnicalTest.Shared;

namespace TechnicalTest.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class XmlParserTests
    {
        [TestMethod]
        public async Task ReadAsync_HappyScenario_ReturnsReportValues()
        {
            // Arrange
            var file = new FileInfo("Data/HappyScenarioReport.xml");
            var mockLogger = new Mock<ILogger<XmlParser>>();

            var mockXmlValidator = new Mock<IXmlFileValidator>();
            mockXmlValidator.Setup(m => m.ValidateFileTypeDataSourceFile(file)).Verifiable();

            var expected = new XmlReportRoot
            {
                Report = new XmlReport
                {
                    Name = "F 20.04",
                    Items = new List<XmlReportItem>
                    {
                        new XmlReportItem { Value = 100, Row = 10, Column = 10 },
                        new XmlReportItem { Value = 200, Row = 10, Column = 11 }
                    }
                }
            };

            var reader = new XmlParser(mockLogger.Object, mockXmlValidator.Object);

            // Act
            var result = await reader.ParseAsync(file);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ReadAsync_WithElementValueMissing_ThrowException()
        {
            // Arrange
            var file = new FileInfo("Data/ElementMissingValue.xml");
            var mockLogger = new Mock<ILogger<XmlParser>>();

            var mockXmlValidator = new Mock<IXmlFileValidator>();
            mockXmlValidator.Setup(m => m.ValidateFileTypeDataSourceFile(file)).Verifiable();

            var reader = new XmlParser(mockLogger.Object, mockXmlValidator.Object);

            // Act
            Func<Task<XmlReportRoot>> readFunction = async () => await reader.ParseAsync(file);

            // Assert
            readFunction.Should().ThrowExactly<InvalidOperationException>("input string was not in the correct format");
        }

        [TestMethod]
        public void ReadAsync_WithUnExpectedDataType_ThrowsException()
        {
            // Arrange
            var file = new FileInfo("Data/UnExpectedDataTypeReport.xml");
            var mockLogger = new Mock<ILogger<XmlParser>>();

            var mockXmlValidator = new Mock<IXmlFileValidator>();
            mockXmlValidator.Setup(m => m.ValidateFileTypeDataSourceFile(file)).Verifiable();

            var reader = new XmlParser(mockLogger.Object, mockXmlValidator.Object);

            // Act
            Func<Task<XmlReportRoot>> readFunction = async () => await reader.ParseAsync(file);

            // Assert
            readFunction.Should().ThrowExactly<InvalidOperationException>("input string was not in the correct format");
        }

        [TestMethod]
        public async Task ReadAsync_WithUnexpectedElementInsteadColumn_ReturnsNullForColumn()
        {
            // Arrange
            var file = new FileInfo("Data/UnExpectedElementReport.xml");
            var mockLogger = new Mock<ILogger<XmlParser>>();

            var mockXmlValidator = new Mock<IXmlFileValidator>();
            mockXmlValidator.Setup(m => m.ValidateFileTypeDataSourceFile(file)).Verifiable();

            var expected = new XmlReportRoot
            {
                Report = new XmlReport
                {
                    Name = "F 20.04",
                    Items = new List<XmlReportItem>
                    {
                        new XmlReportItem { Value = 100, Row = 10, Column = 10 },
                        new XmlReportItem { Value = 200, Row = 10, Column = null }
                    }
                }
            };

            var reader = new XmlParser(mockLogger.Object, mockXmlValidator.Object);

            // Act
            var result = await reader.ParseAsync(file);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task ReadAsync_WithNoData_ReturnsNull()
        {
            // Arrange
            var file = new FileInfo("Data/EmptyReport.xml");
            var mockLogger = new Mock<ILogger<XmlParser>>();

            var mockXmlValidator = new Mock<IXmlFileValidator>();
            mockXmlValidator.Setup(m => m.ValidateFileTypeDataSourceFile(file)).Verifiable();

            var reader = new XmlParser(mockLogger.Object, mockXmlValidator.Object);

            var expected = new XmlReportRoot { Report = null };

            // Act
            var result = await reader.ParseAsync(file);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void AssetReportXmlReader_WithNullLoggerPassed_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<XmlParser> logger = null;
            var mockXmlValidator = new Mock<IXmlFileValidator>();

            // Act
            Action Init = () => new XmlParser(logger, mockXmlValidator.Object);

            // Assert
            Init.Should().ThrowExactly<ArgumentNullException>(nameof(logger));
        }

        [TestMethod]
        public void AssetReportXmlReader_WithNullValidatorPassed_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<XmlParser>>();
            IXmlFileValidator xmlFileValidator = null;

            // Act
            Action Init = () => new XmlParser(mockLogger.Object, xmlFileValidator);

            // Assert
            Init.Should().ThrowExactly<ArgumentNullException>(nameof(xmlFileValidator));
        }
    }
}

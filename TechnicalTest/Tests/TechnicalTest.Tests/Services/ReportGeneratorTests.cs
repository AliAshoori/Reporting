using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using TechnicalTest.Server;
using TechnicalTest.Shared;

namespace TechnicalTest.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ReportGeneratorTests
    {
        [TestMethod]
        public void GenerateAsync_HappyScenario_ReturnReportRawHtml()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportGenerator>>();

            var mockXmlParser = new Mock<IAppXmlParser>();
            mockXmlParser.Setup(m => m.ParseAsync(It.IsAny<FileInfo>())).ReturnsAsync(new XmlReportRoot());

            var mockExcelToDataTableParser = new Mock<IExcelToDataTableParser>();
            mockExcelToDataTableParser.Setup(m => m.ParseToDataTable(It.IsAny<FileInfo>(), It.IsAny<string>())).Returns(new DataTable());

            const string expected = "<table></table>";
            var mockDataTableToRawHtmlParser = new Mock<IDataTableToRawHtmlParser>();
            mockDataTableToRawHtmlParser.Setup(m => m.Parse(It.IsAny<DataTable>())).Returns(expected);

            var generator = new ReportGenerator(mockLogger.Object, mockXmlParser.Object, mockExcelToDataTableParser.Object, mockDataTableToRawHtmlParser.Object);

            // Act
            var actual = generator.GenerateAsync();

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void GenerateAsync_WithNoXmlReportValues_ThrowsInvalidOperationException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportGenerator>>();

            var mockXmlParser = new Mock<IAppXmlParser>();
            mockXmlParser.Setup(m => m.ParseAsync(It.IsAny<FileInfo>())).ReturnsAsync(It.IsAny<XmlReportRoot>());

            var mockExcelToDataTableParser = new Mock<IExcelToDataTableParser>();
            mockExcelToDataTableParser.Setup(m => m.ParseToDataTable(It.IsAny<FileInfo>(), It.IsAny<string>())).Returns(new DataTable());

            const string expected = "<table></table>";
            var mockDataTableToRawHtmlParser = new Mock<IDataTableToRawHtmlParser>();
            mockDataTableToRawHtmlParser.Setup(m => m.Parse(It.IsAny<DataTable>())).Returns(expected);

            var generator = new ReportGenerator(mockLogger.Object, mockXmlParser.Object, mockExcelToDataTableParser.Object, mockDataTableToRawHtmlParser.Object);

            // Act
            Func<Task> actual = async () => await generator.GenerateAsync();

            // Assert
            actual.Should().ThrowExactly<InvalidOperationException>("No Report Values Data Found to generate report from");
        }

        [TestMethod]
        public void ReportGenerator_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<ReportGenerator> logger = null;
            var mockXmlParser = new Mock<IAppXmlParser>();
            var mockExcelToDataTableParser = new Mock<IExcelToDataTableParser>();
            var mockDataTableToRawHtmlParser = new Mock<IDataTableToRawHtmlParser>();

            // Act
            Action initFunction = () => new ReportGenerator(logger, mockXmlParser.Object, mockExcelToDataTableParser.Object, mockDataTableToRawHtmlParser.Object);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(logger));
        }

        [TestMethod]
        public void ReportGenerator_WithNullXmlParser_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportGenerator>>();
            IAppXmlParser xmlParser = null;
            var mockExcelToDataTableParser = new Mock<IExcelToDataTableParser>();
            var mockDataTableToRawHtmlParser = new Mock<IDataTableToRawHtmlParser>();

            // Act
            Action initFunction = () => new ReportGenerator(mockLogger.Object, xmlParser, mockExcelToDataTableParser.Object, mockDataTableToRawHtmlParser.Object);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(xmlParser));
        }

        [TestMethod]
        public void ReportGenerator_WithNullExcelToDataTableParser_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportGenerator>>();
            var mockXmlParser = new Mock<IAppXmlParser>();
            IExcelToDataTableParser excelToDataTableParser = null;
            var mockDataTableToRawHtmlParser = new Mock<IDataTableToRawHtmlParser>();

            // Act
            Action initFunction = () => new ReportGenerator(mockLogger.Object, mockXmlParser.Object, excelToDataTableParser, mockDataTableToRawHtmlParser.Object);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(excelToDataTableParser));
        }

        [TestMethod]
        public void ReportGenerator_WithNullDataTableToRawHtmlParser_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportGenerator>>();
            var mockXmlParser = new Mock<IAppXmlParser>();
            var mockExcelToDataTableParser = new Mock<IExcelToDataTableParser>();
            IDataTableToRawHtmlParser dataTableToHtmlParser = null;

            // Act
            Action initFunction = () => new ReportGenerator(mockLogger.Object, mockXmlParser.Object, mockExcelToDataTableParser.Object, dataTableToHtmlParser);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(dataTableToHtmlParser));
        }
    }
}
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OfficeOpenXml;
using System;
using System.Data;
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
    public class ReportGeneratorTests
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
        public async Task GenerateAsync_HappyScenario_ReturnReportRawHtml()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportGenerator>>();

            var mockXmlParser = new Mock<IAppXmlParser>();
            mockXmlParser.Setup(m => m.ParseAsync(It.IsAny<FileInfo>())).ReturnsAsync(new XmlReportRoot());

            var mockExcelToDataTableParser = new Mock<IExcelToDataTableParser>();
            mockExcelToDataTableParser.Setup(m => m.ParseToDataTable(It.IsAny<FileInfo>(), It.IsAny<string>())).Returns(new DataTable());

            var expected = new ReportFinal { RawHtml = "<table></table>" };
            var mockDataTableToRawHtmlParser = new Mock<IDataTableToRawHtmlParser>();
            mockDataTableToRawHtmlParser.Setup(m => m.Parse(It.IsAny<DataTable>())).Returns(expected.RawHtml);

            var options = Options.Create(new DatabaseSettings
            {
                ReportSheetName = "F 20.04",
                ReportTemplateFileAddress = "Data/ExcelReport.xlsx",
                ReportValueFileAddress = "Data/HappyScenarioReport.xml",
                MergedReportFileName = "Data/ExcelReport-Merged.xlsx"
            });

            var file = await File.ReadAllBytesAsync("Data/ExcelReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _memoryStream = new MemoryStream(file);
            _excelPackage = new ExcelPackage(_memoryStream);
            var worksheet = _excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == "F 20.04");
            var targetCells = Enumerable.Empty<ReportValueCell>();
            var mockCellCalculator = new Mock<IReportValueCellsCalculator>();
            mockCellCalculator.Setup(m => m.Calculate(worksheet)).Returns(targetCells);

            var mockMerger = new Mock<IReportValuesToExcelSheetMerger>();
            mockMerger.Setup(m => m.MergeAsync(It.IsAny<ReportMergePayload>())).ReturnsAsync(new FileInfo(options.Value.MergedReportFileName));

            var generator = new ReportGenerator(
                mockLogger.Object,
                mockXmlParser.Object,
                mockExcelToDataTableParser.Object,
                mockDataTableToRawHtmlParser.Object,
                options,
                mockCellCalculator.Object,
                mockMerger.Object);

            // Act
            var actual = await generator.GenerateAsync();

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ReportGenerator_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<ReportGenerator> logger = null;
            var mockXmlParser = new Mock<IAppXmlParser>();
            var mockExcelToDataTableParser = new Mock<IExcelToDataTableParser>();
            var mockDataTableToRawHtmlParser = new Mock<IDataTableToRawHtmlParser>();
            var mockCellCalculator = new Mock<IReportValueCellsCalculator>();
            var mockMerger = new Mock<IReportValuesToExcelSheetMerger>();
            var options = Options.Create(new DatabaseSettings());

            // Act
            Action initFunction = () =>
                        new ReportGenerator(logger,
                                            mockXmlParser.Object,
                                            mockExcelToDataTableParser.Object,
                                            mockDataTableToRawHtmlParser.Object,
                                            options,
                                            mockCellCalculator.Object,
                                            mockMerger.Object);

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
            var mockCellCalculator = new Mock<IReportValueCellsCalculator>();
            var mockMerger = new Mock<IReportValuesToExcelSheetMerger>();
            var mockDataTableToRawHtmlParser = new Mock<IDataTableToRawHtmlParser>();
            var options = Options.Create(new DatabaseSettings());

            // Act
            Action initFunction = () =>
                new ReportGenerator(mockLogger.Object,
                                    xmlParser,
                                    mockExcelToDataTableParser.Object,
                                    mockDataTableToRawHtmlParser.Object,
                                    options,
                                    mockCellCalculator.Object,
                                    mockMerger.Object);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(xmlParser));
        }

        [TestMethod]
        public void ReportGenerator_WithNullExcelToDataTableParser_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportGenerator>>();
            var mockXmlParser = new Mock<IAppXmlParser>();
            var mockCellCalculator = new Mock<IReportValueCellsCalculator>();
            var mockMerger = new Mock<IReportValuesToExcelSheetMerger>();
            IExcelToDataTableParser excelToDataTableParser = null;
            var mockDataTableToRawHtmlParser = new Mock<IDataTableToRawHtmlParser>();
            var options = Options.Create(new DatabaseSettings());

            // Act
            Action initFunction = () =>
                    new ReportGenerator(mockLogger.Object,
                                        mockXmlParser.Object,
                                        excelToDataTableParser,
                                        mockDataTableToRawHtmlParser.Object,
                                        options,
                                        mockCellCalculator.Object,
                                        mockMerger.Object);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(excelToDataTableParser));
        }

        [TestMethod]
        public void ReportGenerator_WithNullDataTableToRawHtmlParser_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportGenerator>>();
            var mockXmlParser = new Mock<IAppXmlParser>();
            var mockCellCalculator = new Mock<IReportValueCellsCalculator>();
            var mockMerger = new Mock<IReportValuesToExcelSheetMerger>();
            var mockExcelToDataTableParser = new Mock<IExcelToDataTableParser>();
            IDataTableToRawHtmlParser dataTableToHtmlParser = null;
            var options = Options.Create(new DatabaseSettings());

            // Act
            Action initFunction = () =>
                        new ReportGenerator(mockLogger.Object,
                                            mockXmlParser.Object,
                                            mockExcelToDataTableParser.Object,
                                            dataTableToHtmlParser,
                                            options,
                                            mockCellCalculator.Object,
                                            mockMerger.Object);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(dataTableToHtmlParser));
        }

        [TestMethod]
        public void ReportGenerator_WithNullCellCalculator_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportGenerator>>();
            var mockXmlParser = new Mock<IAppXmlParser>();
            IReportValueCellsCalculator valueCellCalculator = null;
            var mockMerger = new Mock<IReportValuesToExcelSheetMerger>();
            var mockExcelToDataTableParser = new Mock<IExcelToDataTableParser>();
            var mockDataTableToRawHtmlParser = new Mock<IDataTableToRawHtmlParser>();
            var options = Options.Create(new DatabaseSettings());

            // Act
            Action initFunction = () =>
                        new ReportGenerator(mockLogger.Object,
                                            mockXmlParser.Object,
                                            mockExcelToDataTableParser.Object,
                                            mockDataTableToRawHtmlParser.Object,
                                            options,
                                            valueCellCalculator,
                                            mockMerger.Object);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(valueCellCalculator));
        }

        [TestMethod]
        public void ReportGenerator_WithNullMerger_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportGenerator>>();
            var mockXmlParser = new Mock<IAppXmlParser>();
            var mockCellCalculator = new Mock<IReportValueCellsCalculator>();
            IReportValuesToExcelSheetMerger merger = null;
            var mockExcelToDataTableParser = new Mock<IExcelToDataTableParser>();
            var mockDataTableToRawHtmlParser = new Mock<IDataTableToRawHtmlParser>();
            var options = Options.Create(new DatabaseSettings());

            // Act
            Action initFunction = () =>
                        new ReportGenerator(mockLogger.Object,
                                            mockXmlParser.Object,
                                            mockExcelToDataTableParser.Object,
                                            mockDataTableToRawHtmlParser.Object,
                                            options,
                                            mockCellCalculator.Object,
                                            merger);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(merger));
        }

        [TestMethod]
        public void ReportGenerator_WithNullOptions_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportGenerator>>();
            var mockXmlParser = new Mock<IAppXmlParser>();
            var mockCellCalculator = new Mock<IReportValueCellsCalculator>();
            var mockMerger = new Mock<IReportValuesToExcelSheetMerger>();
            var mockExcelToDataTableParser = new Mock<IExcelToDataTableParser>();
            var mockDataTableToRawHtmlParser = new Mock<IDataTableToRawHtmlParser>();
            IOptions<DatabaseSettings> options = null;

            // Act
            Action initFunction = () =>
                        new ReportGenerator(mockLogger.Object,
                                            mockXmlParser.Object,
                                            mockExcelToDataTableParser.Object,
                                            mockDataTableToRawHtmlParser.Object,
                                            options,
                                            mockCellCalculator.Object,
                                            mockMerger.Object);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(options));
        }
    }
}
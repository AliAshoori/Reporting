using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using TechnicalTest.Server;

namespace TechnicalTest.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DataTableToRawHtmlParserTests
    {
        [TestMethod]
        public void Parse_HappyScenario_GeneratesRawHml()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DataTableToRawHtmlParser>>();
            var mockValidator = new Mock<IDataTableToRawHtmlParserValidator>();
            mockValidator.Setup(m => m.Validate(It.IsAny<DataTable>())).Verifiable();

            DataTable table = new DataTable();
            table.Columns.Add("Dosage", typeof(int));
            table.Columns.Add("Drug", typeof(string));
            table.Columns.Add("Diagnosis", typeof(string));
            table.Columns.Add("Date", typeof(DateTime));

            table.Rows.Add(25, "Drug A", "Disease A", DateTime.Now);
            table.Rows.Add(50, "Drug Z", "Problem Z", DateTime.Now);
            table.Rows.Add(10, "Drug Q", "Disorder Q", DateTime.Now);
            table.Rows.Add(21, "Medicine A", "Diagnosis A", DateTime.Now);

            var parser = new DataTableToRawHtmlParser(mockLogger.Object, mockValidator.Object);

            var expected =
                "<table class='report-table'><tr class='report-table-row'><td class='report-column'>Dosage</td><td class='report-column'>Drug</td><td class='report-column'>Diagnosis</td><td class='report-column'>Date</td></tr><tr class='report-table-row'><td>25</td><td class='header-style'>Drug A</td><td class='header-style'>Disease A</td><td>03/06/2021 22:20:45</td></tr><tr class='report-table-row'><td>50</td><td class='header-style'>Drug Z</td><td class='header-style'>Problem Z</td><td>03/06/2021 22:20:45</td></tr><tr class='report-table-row'><td>10</td><td class='header-style'>Drug Q</td><td class='header-style'>Disorder Q</td><td>03/06/2021 22:20:45</td></tr><tr class='report-table-row'><td>21</td><td class='header-style'>Medicine A</td><td class='header-style'>Diagnosis A</td><td>03/06/2021 22:20:45</td></tr></table>";

            // Act
            var actual = parser.Parse(table);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void DataTableToRawHtmlParser_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<DataTableToRawHtmlParser> logger = null;
            var mockValidator = new Mock<IDataTableToRawHtmlParserValidator>();

            // Act
            Action initFunction = () => new DataTableToRawHtmlParser(logger, mockValidator.Object);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(logger));
        }

        [TestMethod]
        public void DataTableToRawHtmlParser_WithNullDataTableParser_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DataTableToRawHtmlParser>>();
            IDataTableToRawHtmlParserValidator validator = null;

            // Act
            Action initFunction = () => new DataTableToRawHtmlParser(mockLogger.Object, validator);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(validator));
        }
    }
}

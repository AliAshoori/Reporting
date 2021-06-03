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
    public class DataTableToRawHtmlParserValidatorTests
    {
        [TestMethod]
        public void Validate_HappyScenario_PassesAllValidations()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DataTableToRawHtmlParserValidator>>();

            DataTable table = new DataTable();
            table.Columns.Add("Dosage", typeof(int));
            table.Columns.Add("Drug", typeof(string));
            table.Columns.Add("Diagnosis", typeof(string));
            table.Columns.Add("Date", typeof(DateTime));

            table.Rows.Add(25, "Drug A", "Disease A", DateTime.Now);
            table.Rows.Add(50, "Drug Z", "Problem Z", DateTime.Now);
            table.Rows.Add(10, "Drug Q", "Disorder Q", DateTime.Now);
            table.Rows.Add(21, "Medicine A", "Diagnosis A", DateTime.Now);

            var validator = new DataTableToRawHtmlParserValidator(mockLogger.Object);

            // Act
            Action validatorFunction = () => validator.Validate(table);

            // Assert
            validatorFunction.Should().NotThrow();
        }

        [TestMethod]
        public void Validate_WithNullDataTable_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DataTableToRawHtmlParserValidator>>();

            DataTable dataTable = null;

            var validator = new DataTableToRawHtmlParserValidator(mockLogger.Object);

            // Act
            Action validatorFunction = () => validator.Validate(dataTable);

            // Assert
            validatorFunction.Should().ThrowExactly<ArgumentNullException>(nameof(dataTable));
        }

        [TestMethod]
        public void Validate_WithNoDataTableRows_ThrowsInvalidOperationException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DataTableToRawHtmlParserValidator>>();

            DataTable table = new DataTable();
            table.Columns.Add("Dosage", typeof(int));
            table.Columns.Add("Drug", typeof(string));
            table.Columns.Add("Diagnosis", typeof(string));
            table.Columns.Add("Date", typeof(DateTime));

            var validator = new DataTableToRawHtmlParserValidator(mockLogger.Object);

            // Act
            Action validatorFunction = () => validator.Validate(table);

            // Assert
            validatorFunction.Should().ThrowExactly<InvalidOperationException>("Data table must have at least one rows and columns to create raw html from");
        }

        [TestMethod]
        public void DataTableToRawHtmlParserValidator_WithNoLogger_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<DataTableToRawHtmlParserValidator> logger = null;

            // Act
            Action initFunction = () => new DataTableToRawHtmlParserValidator(logger);

            // Assert
            initFunction.Should().ThrowExactly<ArgumentNullException>(nameof(logger));
        }
    }
}
using System;
using TechnicalTest.Shared;
using System.Data;
using Microsoft.Extensions.Logging;

namespace TechnicalTest.Server
{
    public interface IDataTableToRawHtmlParserValidator
    {
        void Validate(DataTable dataTable);
    }

    public class DataTableToRawHtmlParserValidator : IDataTableToRawHtmlParserValidator
    {
        private readonly ILogger<DataTableToRawHtmlParserValidator> _logger;

        public DataTableToRawHtmlParserValidator(ILogger<DataTableToRawHtmlParserValidator> logger)
        {
            _logger = logger.NotNull();
        }

        public void Validate(DataTable dataTable)
        {
            _logger.LogInformation("Now validating data table before generating raw html out of it");

            if (dataTable == null)
            {
                throw new ArgumentNullException(nameof(dataTable));
            }

            if (dataTable.Rows.Count == 0)
            {
                throw new InvalidOperationException("Data table must have at least one rows and columns to create raw html from");
            }

            _logger.LogInformation("The data table seems to be valid for generating raw html from");
        }
    }
}
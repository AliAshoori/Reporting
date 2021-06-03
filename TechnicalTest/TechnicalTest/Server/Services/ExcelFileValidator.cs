using Microsoft.Extensions.Logging;
using System.IO;
using System;
using TechnicalTest.Shared;

namespace TechnicalTest.Server.Services
{
    public interface IExcelFileValidator
    {
        void ValidateDataSourceFile(FileInfo file, string sheetName);
    }

    public class ExcelFileValidator : BaseFileValidator, IExcelFileValidator
    {
        private readonly ILogger<ExcelFileValidator> _logger;

        public ExcelFileValidator(ILogger<ExcelFileValidator> logger)
        {
            _logger = logger.NotNull();
        }

        public void ValidateDataSourceFile(FileInfo file, string sheetName)
        {
            base.Validate(file);

            if (string.IsNullOrWhiteSpace(sheetName))
            {
                throw new ArgumentNullException(nameof(sheetName));
            }

            _logger.LogInformation("Validating Excel file before reading it...");

            string extension = Path.GetExtension(file.FullName);

            if (extension != FileTypes.Excel)
            {
                throw new NotSupportedException("Only xlsx spread sheet files are supported");
            }
        }
    }
}
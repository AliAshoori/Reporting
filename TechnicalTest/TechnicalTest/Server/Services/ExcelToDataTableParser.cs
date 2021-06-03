using Microsoft.Extensions.Logging;
using TechnicalTest.Shared;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace TechnicalTest.Server.Services
{
    public interface IExcelToDataTableParser
    {
        DataTable ParseToDataTable(FileInfo file, string sheetName);
    }

    public class ExcelToDataTableParser : IExcelToDataTableParser
    {
        private readonly IExcelFileValidator _validator;
        private readonly ILogger<ExcelToDataTableParser> _logger;

        public ExcelToDataTableParser(IExcelFileValidator validator, ILogger<ExcelToDataTableParser> logger)
        {
            _validator = validator.NotNull();
            _logger = logger.NotNull();
        }

        public DataTable ParseToDataTable(FileInfo file, string sheetName)
        {
            _validator.ValidateDataSourceFile(file, sheetName);

            _logger.LogInformation($"Reading from Excel data source: {file.FullName}");

            OleDbConnection connection = null;

            try
            {
                DataTable dataTable = null;
                string connectionString = "Provider= Microsoft.ACE.OLEDB.12.0;" + $"Data Source={file.FullName}" + ";Extended Properties='Excel 8.0;HDR=Yes'";

                using (connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    var query = sheetName.Contains(" ") ? $"select * from ['{sheetName}$']" : $"select * from [{sheetName}$]";
                    var oledbAdapter = new OleDbDataAdapter(query, connection);
                    var excelDataSet = new DataSet();
                    oledbAdapter.Fill(excelDataSet);

                    dataTable = excelDataSet.Tables[0];
                }

                return dataTable;
            }
            catch (Exception exception)
            {
                _logger.LogError($"{exception}");
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
    }
}
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq;
using System.Text;
using TechnicalTest.Shared;

namespace TechnicalTest.Server
{
    public interface IDataTableToRawHtmlParser
    {
        string Parse(DataTable dataTable);
    }

    public class DataTableToRawHtmlParser : IDataTableToRawHtmlParser
    {
        private readonly ILogger<DataTableToRawHtmlParser> _logger;
        private readonly IDataTableToRawHtmlParserValidator _validator;

        public DataTableToRawHtmlParser(
            ILogger<DataTableToRawHtmlParser> logger,
            IDataTableToRawHtmlParserValidator validator)
        {
            _logger = logger.NotNull();
            _validator = validator.NotNull();
        }

        public string Parse(DataTable dataTable)
        {
            _logger.LogInformation($"Now geenrating RAW HTML from data table. Rows: {dataTable.Rows.Count}, Columns: {dataTable.Columns.Count}");

            var rawHtml = new StringBuilder();

            rawHtml.Append("<table class='report-table'>");

            rawHtml.Append("<tr class='report-table-row'>");

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                if (dataTable.Columns[i].ColumnName.StartsWith("NoName"))
                {
                    continue;
                }

                rawHtml.Append($"<td class='report-column'>{dataTable.Columns[i].ColumnName}</td>");
            }

            rawHtml.Append("</tr>");

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                rawHtml.Append("<tr class='report-table-row'>");

                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    string currentCellValue = dataTable.Rows[i][j].ToString();

                    if (!string.IsNullOrWhiteSpace(currentCellValue) && currentCellValue.ToCharArray().Any(s => char.IsLetter(s)))
                    {
                        rawHtml.Append($"<td class='header-style'>{currentCellValue}</td>");
                    }
                    else
                    {
                        rawHtml.Append($"<td>{currentCellValue}</td>");
                    }
                }

                rawHtml.Append("</tr>");
            }

            rawHtml.Append("</table>");

            _logger.LogInformation($"Successfully generated RAW html from data table. HTML length: {rawHtml.Length}");

            return rawHtml.ToString();
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using TechnicalTest.Server.Services;

namespace TechnicalTest.Server
{
    public static class AppServicesRegistration
    {
        public static void RegisterAppServices(this IServiceCollection services)
        {
            // parsers
            services.AddScoped<IAppXmlParser, AppXmlParser>();
            services.AddScoped<IDataTableToRawHtmlParser, DataTableToRawHtmlParser>();
            services.AddScoped<IExcelToDataTableParser, ExcelToDataTableParser>();

            // validators
            services.AddScoped<IDataTableToRawHtmlParserValidator, DataTableToRawHtmlParserValidator>();
            services.AddScoped<IExcelFileValidator, ExcelFileValidator>();
            services.AddScoped<IReportValueCellsCalculatorValidator, ReportValueCellsCalculatorValidator>();
            services.AddScoped<IReportValuesToExcelSheetMergerValidator, ReportValuesToExcelSheetMergerValidator>();
            services.AddScoped<IXmlFileValidator, XmlFileValidator>();

            // services
            services.AddScoped<IReportGenerator, ReportGenerator>();
            services.AddScoped<IReportValueCellsCalculator, ReportValueCellsCalculator>();
            services.AddScoped<IReportValuesToExcelSheetMerger, ReportValuesToExcelSheetMerger>();
        }
    }
}

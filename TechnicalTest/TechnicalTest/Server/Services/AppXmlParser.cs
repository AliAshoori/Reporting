using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TechnicalTest.Shared;

namespace TechnicalTest.Server.Services
{
    public interface IAppXmlParser
    {
        Task<XmlReportRoot> ParseAsync(FileInfo file);
    }

    public class AppXmlParser : IAppXmlParser
    {
        private readonly ILogger<AppXmlParser> _logger;
        private readonly IXmlFileValidator _xmlFileValidator;

        public AppXmlParser(ILogger<AppXmlParser> logger, IXmlFileValidator xmlFileValidator)
        {
            _logger = logger.NotNull();
            _xmlFileValidator = xmlFileValidator.NotNull();
        }

        public Task<XmlReportRoot> ParseAsync(FileInfo file)
        {
            _xmlFileValidator.ValidateFileTypeDataSourceFile(file);

            _logger.LogInformation($"Reading Asset Reports from XML data source: {file.FullName}");

            try
            {
                using var streamReader = new StreamReader(file.FullName);
                var reader = XmlReader.Create(streamReader, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Document });
                return Task.FromResult(new XmlSerializer(typeof(XmlReportRoot)).Deserialize(reader) as XmlReportRoot);
            }
            catch (Exception exception)
            {
                _logger.LogError($"{exception}");
                throw;
            }
        }
    }
}
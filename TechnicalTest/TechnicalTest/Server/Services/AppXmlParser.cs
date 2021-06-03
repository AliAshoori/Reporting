using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
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
            _logger = logger;
            _xmlFileValidator = xmlFileValidator;
        }

        public Task<XmlReportRoot> ParseAsync(FileInfo file)
        {
            throw new System.NotImplementedException();
        }
    }
}

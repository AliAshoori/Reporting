using Microsoft.Extensions.Logging;
using TechnicalTest.Shared;
using System;
using System.IO;

namespace TechnicalTest.Server
{
    public interface IXmlFileValidator
    {
        void ValidateFileTypeDataSourceFile(FileInfo file);
    }

    public class XmlFileValidator : BaseFileValidator, IXmlFileValidator
    {
        private readonly ILogger<XmlFileValidator> _logger;

        public XmlFileValidator(ILogger<XmlFileValidator> logger)
        {
            _logger = logger.NotNull();
        }

        public void ValidateFileTypeDataSourceFile(FileInfo file)
        {
            base.Validate(file);

            _logger.LogInformation("Validating XML file before reading it...");

            if (Path.GetExtension(file.FullName) != FileTypes.Xml)
            {
                throw new FormatException($"File {file.Name} is not an xml file.");
            }
        }
    }
}

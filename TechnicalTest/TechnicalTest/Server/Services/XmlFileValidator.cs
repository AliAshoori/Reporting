using System;
using System.IO;

namespace TechnicalTest.Server.Services
{
    public interface IXmlFileValidator
    {
        void ValidateFileTypeDataSourceFile(FileInfo file);
    }

    public class XmlFileValidator : IXmlFileValidator
    {
        public void ValidateFileTypeDataSourceFile(FileInfo file)
        {
            throw new NotImplementedException();
        }
    }
}

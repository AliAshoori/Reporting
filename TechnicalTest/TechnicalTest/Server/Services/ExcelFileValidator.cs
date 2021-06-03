using System;
using System.IO;

namespace TechnicalTest.Server.Services
{
    public interface IExcelFileValidator
    {
        void ValidateDataSourceFile(FileInfo file, string sheetName);
    }

    public class ExcelFileValidator : BaseFileValidator, IExcelFileValidator
    {
        public void ValidateDataSourceFile(FileInfo file, string sheetName)
        {
            throw new NotImplementedException();
        }
    }
}
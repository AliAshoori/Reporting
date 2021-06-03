using System;
using System.IO;

namespace TechnicalTest.Server
{
    public interface IFileValidator
    {
        void Validate(FileInfo file);
    }

    public abstract class BaseFileValidator : IFileValidator
    {
        public void Validate(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException(nameof(file.FullName));
            }
        }
    }
}

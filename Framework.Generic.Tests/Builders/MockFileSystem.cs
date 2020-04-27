using Moq;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

namespace Framework.Generic.Tests.Builders
{
    public class MockFileSystem : Mock<IFileSystem>
    {
        private Dictionary<string, string> _storedFiles; 

        public MockFileSystem()
        {
            _storedFiles = new Dictionary<string, string>();

            Initialize();
        }

        private void Initialize()
        {
            SetupFileReadAllText().SetupFileWriteAllText();
        }

        #region Setup File.ReadAllText
        
        public MockFileSystem SetupFileReadAllText()
        {
            Setup(f => f.File.ReadAllText(It.IsAny<string>())).Returns((string filePath) =>
            {
                if (!_storedFiles.ContainsKey(filePath))
                    throw new FileNotFoundException();

                return _storedFiles[filePath];
            });

            return this;
        }

        #endregion
        #region Setup File.WriteAllText

        public MockFileSystem SetupFileWriteAllText()
        {
            Setup(f => f.File.WriteAllText(It.IsAny<string>(), It.IsAny<string>())).Callback((string filePath, string contents) =>
            {
                if (!_storedFiles.ContainsKey(filePath))
                    _storedFiles.Add(filePath, contents);
                else
                    _storedFiles[filePath] = contents;
            });

            return this;
        }

        #endregion
    }
}

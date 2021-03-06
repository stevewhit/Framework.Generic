﻿using Moq;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

namespace Framework.Generic.Tests.Builders
{
    public class MockFileSystem : Mock<IFileSystem>
    {
        public Dictionary<string, string> StoredFiles; 

        public MockFileSystem()
        {
            StoredFiles = new Dictionary<string, string>();

            Initialize();
        }

        private void Initialize()
        {
            SetupFileReadAllText().SetupFileWriteAllText().SetupFileExists();
        }

        public MockFileSystem SetupFileReadAllText()
        {
            Setup(f => f.File.ReadAllText(It.IsAny<string>())).Returns((string filePath) =>
            {
                if (!StoredFiles.ContainsKey(filePath))
                    throw new FileNotFoundException();

                return StoredFiles[filePath];
            });

            return this;
        }

        public MockFileSystem SetupFileWriteAllText()
        {
            Setup(f => f.File.WriteAllText(It.IsAny<string>(), It.IsAny<string>())).Callback((string filePath, string contents) =>
            {
                if (!StoredFiles.ContainsKey(filePath))
                    StoredFiles.Add(filePath, contents);
                else
                    StoredFiles[filePath] = contents;
            });

            return this;
        }

        public MockFileSystem SetupFileExists()
        {
            Setup(f => f.File.Exists(It.IsAny<string>())).Returns((string filePath) =>
            {
                return StoredFiles.ContainsKey(filePath);
            });

            return this;
        }
    }
}

using System;
using System.IO.Abstractions;
using Newtonsoft.Json;

namespace Framework.Generic.IO
{
    public class JsonEntityFile<FEntity> : IEntityFile<FEntity> where FEntity : ISerializedJsonObject
    {
        // Abstracted file system for better testability.
        private readonly IFileSystem _fileSystem;
        private readonly string _filePath;

        public JsonEntityFile(string filePath) : this(filePath, new FileSystem()) { }

        public JsonEntityFile(string filePath, IFileSystem fileSystem)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _filePath = filePath;
        }

        #region IEntityFile<FEntity> Support

        /// <summary>
        /// Returns the entity from the file path.
        /// </summary>
        /// <returns>Returns the entity from the file</returns>
        public virtual FEntity GetEntity()
        {
            var jsonString = _fileSystem.File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<FEntity>(jsonString);
        }

        /// <summary>
        /// Writes the serialized <paramref name="entity"/> JSON to the designated file.
        /// </summary>
        /// <param name="entity">The entity that will be serialized and written to the file.</param>
        public virtual void WriteEntity(FEntity entity)
        {
            var jsonString = JsonConvert.SerializeObject(entity, Formatting.Indented);
            _fileSystem.File.WriteAllText(_filePath, jsonString);
        }

        #endregion
    }
}

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Framework.Generic.Utility
{
    public static class ObjectUtils
    {
        /// <summary>
        /// Clones the source object.
        /// </summary>
        /// <typeparam name="T">Generic type that is serializable.</typeparam>
        /// <param name="source">The object that is being cloned.</param>
        /// <exception cref="ArgumentException">Throws when source object is not serializable.</exception>
        /// <returns>Returns a clone of the source object.</returns>
        public static T CopyObject<T>(this T source)
        {
            if (!source.GetType().IsSerializable)
                throw new ArgumentException("Source object must be serializable.");

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, source);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
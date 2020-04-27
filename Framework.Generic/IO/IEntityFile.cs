
namespace Framework.Generic.IO
{
    public interface IEntityFile<FEntity> where FEntity : class
    {
        /// <summary>
        /// Returns the entity from the file path.
        /// </summary>
        /// <returns>Returns the entity from the file path.</returns>
        FEntity GetEntity();

        /// <summary>
        /// Writes the serialized <paramref name="entity"/> JSON to the designated file..
        /// </summary>
        /// <param name="entity">The entity that will be serialized and written to the file.</param>
        void WriteEntity(FEntity entity);
    }
}

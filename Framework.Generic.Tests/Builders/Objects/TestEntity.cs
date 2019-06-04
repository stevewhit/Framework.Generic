using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace Framework.Generic.Tests.Builders
{
    public interface ITestEntity
    {
        Guid TestId { get; }
        int StoredValue { get; set; }
        int CurrentValue { get; set; }
        EntityState State { get; set; }
        bool IsVirtual { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class TestEntity : ITestEntity
    {
        [Key]
        public Guid TestId { get; private set; }
        public int StoredValue { get; set; }
        public int CurrentValue { get; set; }
        public EntityState State { get; set; }

        /// <summary>
        /// Flag to indicate if this entity is apart of the "unchanged" version of the context.
        /// Useful for mocking entity framework contexts that have similar mechanics.
        /// </summary>
        public bool IsVirtual { get; set; }

        public TestEntity() : this(false) { }
        public TestEntity(bool isVirtual = false)
        {
            TestId = Guid.NewGuid();
            State = EntityState.Unchanged;
            IsVirtual = isVirtual;
        }

        public TestEntity(int id, bool isVirtual = false) : this(isVirtual)
        {
            StoredValue = id;
            CurrentValue = id;
        }
    }
}

using Framework.Generic.IO;
using System;

namespace Framework.Generic.Tests.Builders.Objects
{
    public class TestSerializedJsonEntity : ISerializedJsonObject
    {
        public Guid Id { get; set; }
        public int Value { get; set; }
        public string Name { get; set; }

        public TestSerializedJsonEntity(int value, string name)
        {
            Id = Guid.NewGuid();
            Value = value;
            Name = name;
        }
    }
}

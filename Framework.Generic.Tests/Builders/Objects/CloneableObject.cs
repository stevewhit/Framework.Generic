using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Framework.Generic.Tests.Builders
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    internal class CloneableObject
    {
        internal enum Numbers
        {
            One, Two, Three, Four, Five
        };

        private Numbers _privateNumber = Numbers.Three;

        public string PublicString { get; set; } = "Public String";
        public Numbers PublicNumber { get; set; } = Numbers.Five;
        private int PublicValue { get; set; } = 5;
        public ICollection<CloneableObject> PublicCollection { get; set; }

        public void SetPrivateNumber(Numbers number)
        {
            _privateNumber = number;
        }
        public Numbers GetPrivateNumber()
        {
            return _privateNumber;
        }
    }

    [Serializable]
    [ExcludeFromCodeCoverage]
    internal class CloneableObjectWithNestedClass
    {
        public CloneableObject NestedObject { get; set; }

        public ICollection<CloneableObject> ObjectCollection { get; set; }
    }

    [ExcludeFromCodeCoverage]
    internal class NotSerializableObject
    {

    }
}

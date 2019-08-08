using System;
using System.Diagnostics.CodeAnalysis;

namespace Framework.Generic.Tests.Builders.Objects
{
    [ExcludeFromCodeCoverage]
    public class DisposableObject : IDisposable
    {
        public bool IsDisposed = false;

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}

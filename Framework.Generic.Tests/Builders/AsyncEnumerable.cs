using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Generic.Tests.Builders
{
    [ExcludeFromCodeCoverage]
    public class AsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        IQueryProvider IQueryable.Provider
        {
            get
            {
                return new AsyncQueryProvider<T>(this);
            }
        }

        public AsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {

        }

        public AsyncEnumerable(Expression expression)
            : base(expression)
        {

        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }
    }

    [ExcludeFromCodeCoverage]
    public class AsyncQueryProvider<T> : IDbAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public AsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var t = expression.Type;
            if (!t.IsGenericType)
            {
                return new AsyncEnumerable<T>(expression);
            }

            var genericParams = t.GetGenericArguments();
            var genericParam = genericParams[0];
            var enumerableType = typeof(AsyncEnumerable<>).MakeGenericType(genericParam);

            return (IQueryable)Activator.CreateInstance(enumerableType, expression);
        }

        public IQueryable<U> CreateQuery<U>(Expression expression)
        {
            return new AsyncEnumerable<U>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public U Execute<U>(Expression expression)
        {
            return _inner.Execute<U>(expression);
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(expression));
        }

        public Task<U> ExecuteAsync<U>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<U>(expression));
        }
    }

    [ExcludeFromCodeCoverage]
    public class AsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public T Current
        {
            get
            {
                return _inner.Current;
            }
        }

        object IDbAsyncEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public AsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }
    }
}
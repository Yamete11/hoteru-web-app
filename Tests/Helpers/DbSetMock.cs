using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Tests.Helpers
{
    public static class DbSetMock
    {
        public static Mock<DbSet<T>> Create<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IAsyncEnumerable<T>>()
                   .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                   .Returns(new AsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new AsyncQueryProvider<T>(data.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            return mockSet;
        }

        private sealed class AsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;
            public AsyncQueryProvider(IQueryProvider inner) => _inner = inner;

            public IQueryable CreateQuery(Expression expression)
            {
                var elementType = expression.Type.GetGenericArguments().First();
                var asyncEnumerableType = typeof(AsyncEnumerable<>).MakeGenericType(elementType);
                return (IQueryable)Activator.CreateInstance(asyncEnumerableType, expression)!;
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
                new AsyncEnumerable<TElement>(expression);

            public object Execute(Expression expression) => _inner.Execute(expression);
            public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) =>
                new AsyncEnumerable<TResult>(expression);

            TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) =>
                _inner.Execute<TResult>(expression);
        }

        private sealed class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public AsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
            public AsyncEnumerable(Expression expression) : base(expression) { }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
                new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

            IQueryProvider IQueryable.Provider => new AsyncQueryProvider<T>(this);
        }

        private sealed class AsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            public AsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

            public T Current => _inner.Current;
            public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
        }
    }
}

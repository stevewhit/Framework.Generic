using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using System.Data.Entity;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Data.Entity.Infrastructure;

namespace Framework.Generic.Tests.Builders
{
    [ExcludeFromCodeCoverage]
    public class MockDbSet<T> : Mock<IDbSet<T>> where T : class
    {
        protected ObservableCollection<T> Data { get; set; }
        protected IQueryable<T> Query 
        { 
            get 
            { 
                return Data.AsQueryable(); 
            } 
        }

        public MockDbSet()
        {
            Data = new ObservableCollection<T>();

            Initialize();
        }

        public MockDbSet(IEnumerable<T> data)
        {
            Data = new ObservableCollection<T>(data);

            Initialize();
        }
        
        private MockDbSet<T> Initialize()
        {
            return SetupLocal().SetupAdd().SetupAttach().SetupCreate().SetupFind().SetupRemove().SetupGetEnumerator().SetupElementType().SetupExpression().SetupProvider().SetupAsyncExtensions();
        }

        private MockDbSet<T> SetupAsyncExtensions()
        {
            As<IDbAsyncEnumerable<T>>()
                .Setup(d => d.GetAsyncEnumerator())
                .Returns(() => 
                { 
                    return new AsyncEnumerator<T>(Data.GetEnumerator()); 
                });

            As<IQueryable<T>>()
                .Setup(d => d.Provider)
                .Returns(() => 
                { 
                    return new AsyncQueryProvider<T>(Query.Provider);
                });

            return this;
        }


        private MockDbSet<T> SetupLocal()
        {
            Setup(d => d.Local)
                .Returns(() =>
                {
                    return Data;
                });

            return this;
        } 

        private MockDbSet<T> SetupAdd()
        {
            Setup(d => d.Add(It.IsAny<T>()))
                .Returns((T entity) => 
                {
                    Data.Add(entity);
                    return entity;
                });

            return this;
        }

        private MockDbSet<T> SetupAttach()
        {
            Setup(d => d.Attach(It.IsAny<T>()))
                .Returns((T entity) =>
                {
                    Data.Add(entity);
                    return entity;
                });

            return this;
        }

        private MockDbSet<T> SetupCreate()
        {
            Setup(d => d.Create())
                .Returns(() =>
                {
                    return Activator.CreateInstance<T>();
                });

            return this;
        }

        private MockDbSet<T> SetupFind()
        {
            Setup(d => d.Find(It.IsAny<object[]>()))
                .Returns((object[] keyValues) =>
                {
                    return Data.FirstOrDefault(e => keyValues.Contains(e));
                });

            return this;
        }

        private MockDbSet<T> SetupRemove()
        {
            Setup(d => d.Remove(It.IsAny<T>()))
                .Returns((T entity) =>
                {
                    Data.Remove(entity);
                    return entity;
                });

            return this;
        }

        private MockDbSet<T> SetupGetEnumerator()
        {
            Setup(d => d.GetEnumerator())
                .Returns(() =>
                {
                    return Data.GetEnumerator();
                });

            return this;
        }

        private MockDbSet<T> SetupElementType()
        {
            Setup(d => d.ElementType)
                .Returns(() =>
                {
                    return Query.ElementType;
                });

            return this;
        }

        private MockDbSet<T> SetupExpression()
        {
            Setup(d => d.Expression)
                .Returns(() =>
                {
                    return Query.Expression;
                });

            return this;
        }

        private MockDbSet<T> SetupProvider()
        {
            Setup(d => d.Provider)
                .Returns(() =>
                {
                    return Query.Provider;
                });

            return this;
        }
    }
}
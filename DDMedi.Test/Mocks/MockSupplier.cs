﻿using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi.Test.Mocks
{
    internal class MockSupplier<TInputs> :
        ISupplier<TInputs>
        where TInputs:class
    {
        public Mock<ISupplier<TInputs>> Mock { get; }
        public MockSupplier()
        {
            Mock = new Mock<ISupplier<TInputs>>();
        }

        public void Process(TInputs inputs, ISupplierContext context)
        {
            Mock.Object.Process(inputs, context);
        }
    }
    internal class MockAsyncSupplier<TInputs> :
        IAsyncSupplier<TInputs>
        where TInputs : class
    {
        public Mock<IAsyncSupplier<TInputs>> Mock { get; }
        public MockAsyncSupplier()
        {
            Mock = new Mock<IAsyncSupplier<TInputs>>();
        }

        public Task ProcessAsync(TInputs inputs, ISupplierContext context, CancellationToken token)
        {
            return Mock.Object.ProcessAsync(inputs, context, token);
        }
    }
    internal class MockESupplier<TEInputs> :
        IESupplier<TEInputs>
        where TEInputs : IEInputs
    {
        public Mock<IESupplier<TEInputs>> Mock { get; }
        public MockESupplier(Mock<IESupplier<TEInputs>> mock)
        {
            Mock = mock;
        }
        public Task ProcessAsync(TEInputs inputs, ISupplierContext context, CancellationToken token = default)
        {
            return Mock.Object.ProcessAsync(inputs, context, token);
        }
    }
    internal class MockSupplier<TInputs,TOutput> :
        ISupplier<TInputs, TOutput>
        where TInputs : class
    {
        public Mock<ISupplier<TInputs, TOutput>> Mock { get; }
        public MockSupplier()
        {
            Mock = new Mock<ISupplier<TInputs, TOutput>>();
        }

        public virtual TOutput Process(TInputs inputs, ISupplierContext context)
        {
            return Mock.Object.Process(inputs, context);
        }
    }
    internal class MockAsyncSupplier<TInputs, TOutput> :
        IAsyncSupplier<TInputs, TOutput>
        where TInputs : class
    {
        public Mock<IAsyncSupplier<TInputs, TOutput>> Mock { get; }
        public MockAsyncSupplier()
        {
            Mock = new Mock<IAsyncSupplier<TInputs, TOutput>>();
        }

        public virtual Task<TOutput> ProcessAsync(TInputs inputs, ISupplierContext context, CancellationToken token = default)
        {
            return Mock.Object.ProcessAsync(inputs, context, token);
        }
    }
    internal class MockAllSupplier<TInputs> :
        IAsyncSupplier<TInputs>,
        ISupplier<TInputs>
        where TInputs : class
    {
        public Mock<ISupplier<TInputs>> Mock { get; }
        public Mock<IAsyncSupplier<TInputs>> MockAsync { get; }
        public MockAllSupplier()
        {
            Mock = new Mock<ISupplier<TInputs>>();
            MockAsync = new Mock<IAsyncSupplier<TInputs>>();
        }

        public void Process(TInputs inputs, ISupplierContext context)
        {
            Mock.Object.Process(inputs, context);
        }

        public Task ProcessAsync(TInputs inputs, ISupplierContext context, CancellationToken token)
        {
            return MockAsync.Object.ProcessAsync(inputs, context, token);
        }
    }
    internal class MockAllSupplier<TInputs, TOutput> :
        IAsyncSupplier<TInputs, TOutput>,
        ISupplier<TInputs, TOutput>
        where TInputs : class
    {
        public Mock<ISupplier<TInputs, TOutput>> Mock { get; }
        public Mock<IAsyncSupplier<TInputs, TOutput>> MockAsync { get; }
        public MockAllSupplier()
        {
            Mock = new Mock<ISupplier<TInputs, TOutput>>();
            MockAsync = new Mock<IAsyncSupplier<TInputs, TOutput>>();
        }

        public TOutput Process(TInputs inputs, ISupplierContext context)
        {
            return Mock.Object.Process(inputs, context);
        }

        public Task<TOutput> ProcessAsync(TInputs inputs, ISupplierContext context, CancellationToken token)
        {
            return MockAsync.Object.ProcessAsync(inputs, context, token);
        }
    }
}

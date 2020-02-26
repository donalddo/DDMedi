using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi.Test.Mocks
{
    internal class MockSupplier<TInputs> :
        ISupplier<TInputs>
        where TInputs:IInputs
    {
        public Mock<ISupplier<TInputs>> Mock { get; }
        public MockSupplier()
        {
            Mock = new Mock<ISupplier<TInputs>>();
        }

        public void Process(ISupplierContext<TInputs> context)
        {
            Mock.Object.Process(context);
        }
    }
    internal class MockAsyncSupplier<TInputs> :
        IAsyncSupplier<TInputs>
        where TInputs : IInputs
    {
        public Mock<IAsyncSupplier<TInputs>> Mock { get; }
        public MockAsyncSupplier()
        {
            Mock = new Mock<IAsyncSupplier<TInputs>>();
        }

        public Task ProcessAsync(ISupplierContext<TInputs> context, CancellationToken token)
        {
            return Mock.Object.ProcessAsync(context, token);
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
        public Task ProcessAsync(ISupplierContext<TEInputs> context, CancellationToken token = default)
        {
            return Mock.Object.ProcessAsync(context, token);
        }
    }
    internal class MockSupplier<TInputs,TOutput> :
        ISupplier<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        public Mock<ISupplier<TInputs, TOutput>> Mock { get; }
        public MockSupplier()
        {
            Mock = new Mock<ISupplier<TInputs, TOutput>>();
        }

        public virtual TOutput Process(ISupplierContext<TInputs> context)
        {
            return Mock.Object.Process(context);
        }
    }
    internal class MockAsyncSupplier<TInputs, TOutput> :
        IAsyncSupplier<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        public Mock<IAsyncSupplier<TInputs, TOutput>> Mock { get; }
        public MockAsyncSupplier()
        {
            Mock = new Mock<IAsyncSupplier<TInputs, TOutput>>();
        }

        public virtual Task<TOutput> ProcessAsync(ISupplierContext<TInputs> context, CancellationToken token = default)
        {
            return Mock.Object.ProcessAsync(context, token);
        }
    }
    internal class MockAllSupplier<TInputs> :
        IAsyncSupplier<TInputs>,
        ISupplier<TInputs>
        where TInputs : IInputs
    {
        public Mock<ISupplier<TInputs>> Mock { get; }
        public Mock<IAsyncSupplier<TInputs>> MockAsync { get; }
        public MockAllSupplier()
        {
            Mock = new Mock<ISupplier<TInputs>>();
            MockAsync = new Mock<IAsyncSupplier<TInputs>>();
        }

        public void Process(ISupplierContext<TInputs> context)
        {
            Mock.Object.Process(context);
        }

        public Task ProcessAsync(ISupplierContext<TInputs> context, CancellationToken token)
        {
            return MockAsync.Object.ProcessAsync(context, token);
        }
    }
    internal class MockAllSupplier<TInputs, TOutput> :
        IAsyncSupplier<TInputs, TOutput>,
        ISupplier<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        public Mock<ISupplier<TInputs, TOutput>> Mock { get; }
        public Mock<IAsyncSupplier<TInputs, TOutput>> MockAsync { get; }
        public MockAllSupplier()
        {
            Mock = new Mock<ISupplier<TInputs, TOutput>>();
            MockAsync = new Mock<IAsyncSupplier<TInputs, TOutput>>();
        }

        public TOutput Process(ISupplierContext<TInputs> context)
        {
            return Mock.Object.Process(context);
        }

        public Task<TOutput> ProcessAsync(ISupplierContext<TInputs> context, CancellationToken token)
        {
            return MockAsync.Object.ProcessAsync(context, token);
        }
    }
}

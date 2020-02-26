using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi.Test.Mocks
{
    internal class MockDecorator<TInputs> :
        IDecorator<TInputs>
        where TInputs : IInputs
    {
        public Mock<IDecorator<TInputs>> Mock { get; }
        public MockDecorator()
        {
            Mock = new Mock<IDecorator<TInputs>>();
        }

        public void Process(IDecoratorContext<TInputs> context)
        {
            Mock.Object.Process(context);
            this.Next(context);
        }
    }
    internal class MockAsyncDecorator<TInputs> :
        IAsyncDecorator<TInputs>
        where TInputs : IInputs
    {
        public Mock<IAsyncDecorator<TInputs>> Mock { get; }
        public MockAsyncDecorator()
        {
            Mock = new Mock<IAsyncDecorator<TInputs>>();
        }

        public Task ProcessAsync(IAsyncDecoratorContext<TInputs> context, CancellationToken token = default)
        {
            Mock.Object.ProcessAsync(context, token);
            return this.Next(context, token);
        }
    }
    internal class MockEDecorator<TEInputs> :
        IEDecorator<TEInputs>
        where TEInputs : IEInputs
    {
        public Mock<IEDecorator<TEInputs>> Mock { get; }
        public MockEDecorator(Mock<IEDecorator<TEInputs>> mock)
        {
            Mock = mock;
        }
        public Task ProcessAsync(IEDecoratorContext<TEInputs> context, CancellationToken token = default)
        {
            Mock.Object.ProcessAsync(context, token);
            return this.Next(context, token);
        }
    }
    internal class MockDecorator<TInputs, TOutput> :
        IDecorator<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        public Mock<IDecorator<TInputs, TOutput>> Mock { get; }
        public MockDecorator()
        {
            Mock = new Mock<IDecorator<TInputs, TOutput>>();
        }

        public virtual TOutput Process(IDecoratorContext<TInputs, TOutput> context)
        {
            Mock.Object.Process(context);
            return this.Next(context);
        }
    }
    internal class MockAsyncDecorator<TInputs, TOutput> :
        IAsyncDecorator<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        public Mock<IAsyncDecorator<TInputs, TOutput>> Mock { get; }
        public MockAsyncDecorator()
        {
            Mock = new Mock<IAsyncDecorator<TInputs, TOutput>>();
        }

        public virtual Task<TOutput> ProcessAsync(IAsyncDecoratorContext<TInputs,TOutput> context, CancellationToken token = default)
        {
            Mock.Object.ProcessAsync(context, token);
            return this.Next(context, token);
        }
    }
    internal class MockAllDecorator<TInputs> :
        IAsyncDecorator<TInputs>,
        IDecorator<TInputs>
        where TInputs : IInputs
    {
        public Mock<IDecorator<TInputs>> Mock { get; }
        public Mock<IAsyncDecorator<TInputs>> MockAsync { get; }
        public MockAllDecorator()
        {
            Mock = new Mock<IDecorator<TInputs>>();
            MockAsync = new Mock<IAsyncDecorator<TInputs>>();
        }

        public void Process(IDecoratorContext<TInputs> context)
        {
            Mock.Object.Process(context);
            this.Next(context);
        }

        public Task ProcessAsync(IAsyncDecoratorContext<TInputs> context, CancellationToken token)
        {
            MockAsync.Object.ProcessAsync(context, token);
            return this.Next(context, token);
        }
    }
    internal class MockAllDecorator<TInputs, TOutput> :
        IAsyncDecorator<TInputs, TOutput>,
        IDecorator<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        public Mock<IDecorator<TInputs, TOutput>> Mock { get; }
        public Mock<IAsyncDecorator<TInputs, TOutput>> MockAsync { get; }
        public MockAllDecorator()
        {
            Mock = new Mock<IDecorator<TInputs, TOutput>>();
            MockAsync = new Mock<IAsyncDecorator<TInputs, TOutput>>();
        }

        public TOutput Process(IDecoratorContext<TInputs,TOutput> context)
        {
            Mock.Object.Process(context);
            return this.Next(context);
        }

        public Task<TOutput> ProcessAsync(IAsyncDecoratorContext<TInputs, TOutput> context, CancellationToken token)
        {
            MockAsync.Object.ProcessAsync(context, token);
            return this.Next(context, token);
        }
    }

    internal class Mock2EDecorator<TEInputs> :
        IEDecorator<TEInputs>
        where TEInputs : IEInputs
    {
        public Mock<IEDecorator<TEInputs>> Mock { get; }
        public Mock2EDecorator(Mock<IEDecorator<TEInputs>> mock)
        {
            Mock = mock;
        }
        public Task ProcessAsync(IEDecoratorContext<TEInputs> context, CancellationToken token = default)
        {
            Mock.Object.ProcessAsync(context, token);
            return this.Next(context, token);
        }
    }
}

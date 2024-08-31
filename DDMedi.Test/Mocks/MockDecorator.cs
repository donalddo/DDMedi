using Moq;
using Newtonsoft.Json.Linq;
using System;
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

        public void Process(TInputs inputs, IDecoratorContext context)
        {
            Mock.Object.Process(inputs, context);
            this.Next(inputs, context);
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

        public Task ProcessAsync(TInputs inputs, IAsyncDecoratorContext context, CancellationToken token = default)
        {
            Mock.Object.ProcessAsync(inputs, context, token);
            return this.Next(inputs, context, token);
        }
    }
    internal class MockEDecorator<TEInputs> :
        IEDecorator<TEInputs>
        where TEInputs : IEInputs
    {
        public IServiceProvider Provider { get; set; }
        public Mock<IEDecorator<TEInputs>> Mock { get; }
        public MockEDecorator(Mock<IEDecorator<TEInputs>> mock)
        {
            Mock = mock;
        }
        public Task ProcessAsync(TEInputs inputs, IEDecoratorContext context, CancellationToken token = default)
        {
            Mock.Object.ProcessAsync(inputs, context, token);
            if (Provider == null)
                return this.Next(inputs, context, token);
            else return this.Next(inputs, context, Provider, token);
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

        public virtual TOutput Process(TInputs inputs, IDecoratorContext<TOutput> context)
        {
            Mock.Object.Process(inputs, context);
            return this.Next(inputs, context);
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

        public virtual Task<TOutput> ProcessAsync(TInputs inputs, IAsyncDecoratorContext<TOutput> context, CancellationToken token = default)
        {
            Mock.Object.ProcessAsync(inputs, context, token);
            return this.Next(inputs, context, token);
        }
    }
    internal class MockAllDecorator<TInputs> :
        IAsyncDecorator<TInputs>,
        IDecorator<TInputs>
        where TInputs : IInputs
    {
        public IServiceProvider Provider { get; set; }
        public Mock<IDecorator<TInputs>> Mock { get; }
        public Mock<IAsyncDecorator<TInputs>> MockAsync { get; }
        public MockAllDecorator()
        {
            Mock = new Mock<IDecorator<TInputs>>();
            MockAsync = new Mock<IAsyncDecorator<TInputs>>();
        }

        public void Process(TInputs inputs, IDecoratorContext context)
        {
            Mock.Object.Process(inputs, context);
            if(Provider == null)
                this.Next(inputs, context);
            else this.Next(inputs, context, Provider);
        }

        public Task ProcessAsync(TInputs inputs, IAsyncDecoratorContext context, CancellationToken token)
        {
            MockAsync.Object.ProcessAsync(inputs, context, token);
            if (Provider == null)
                return this.Next(inputs, context, token);
            else return this.Next(inputs, context, Provider, token);
        }
    }
    internal class MockAllDecorator<TInputs, TOutput> :
        IAsyncDecorator<TInputs, TOutput>,
        IDecorator<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        public IServiceProvider Provider { get; set; }
        public Mock<IDecorator<TInputs, TOutput>> Mock { get; }
        public Mock<IAsyncDecorator<TInputs, TOutput>> MockAsync { get; }
        public MockAllDecorator()
        {
            Mock = new Mock<IDecorator<TInputs, TOutput>>();
            MockAsync = new Mock<IAsyncDecorator<TInputs, TOutput>>();
        }

        public TOutput Process(TInputs inputs, IDecoratorContext<TOutput> context)
        {
            Mock.Object.Process(inputs, context);
            if (Provider == null)
                return this.Next(inputs, context);
            else return this.Next(inputs, context, Provider);
        }

        public Task<TOutput> ProcessAsync(TInputs inputs, IAsyncDecoratorContext<TOutput> context, CancellationToken token)
        {
            MockAsync.Object.ProcessAsync(inputs, context, token);
            if (Provider == null)
                return this.Next(inputs, context, token);
            else return this.Next(inputs, context, Provider, token);
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
        public Task ProcessAsync(TEInputs inputs, IEDecoratorContext context, CancellationToken token = default)
        {
            Mock.Object.ProcessAsync(inputs, context, token);
            return this.Next(inputs, context, token);
        }
    }
}

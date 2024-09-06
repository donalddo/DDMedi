using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi.Test.Dummies
{
    internal abstract class InvalidDecorator :
        IDecorator<DummyInputs>
    {
        public abstract void Process(DummyInputs inputs, IDecoratorContext context);
    }
    internal abstract class InvalidDecorator2<T> :
        IDecorator<T> where T : class
    {
        public abstract void Process(T inputs, IDecoratorContext context);
    }
    internal class SuperDummySupplier :
        IAsyncSupplier<IInputs>,
        IESupplier<IEInputs>,
        IAsyncDecorator<IInputs>,
        IEDecorator<IEInputs>
    {
        public Task ProcessAsync(IInputs inputs, ISupplierContext context, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public Task ProcessAsync(IEInputs inputs, ISupplierContext context, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public Task ProcessAsync(IInputs inputs, IAsyncDecoratorContext context, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public Task ProcessAsync(IEInputs inputs, IEDecoratorContext context, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
    }
    internal class DummyAllDecorator :
        IAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput>,
        IDecorator<DummyInputs<DummyOutput>, DummyOutput>,
        IAsyncDecorator<DummyInputs>,
        IDecorator<DummyInputs>,
        IEDecorator<DummyEInputs>
    {
        public bool EInputsCalled { get; set; }
        public bool AsyncCalled { get; set; }
        public bool Called { get; set; }
        public bool AsyncOutputCalled { get; set; }
        public bool OutputCalled { get; set; }
        public DummyOutput Process(DummyInputs<DummyOutput> inputs, IDecoratorContext<DummyOutput> context)
        {
            OutputCalled = true;
            return this.Next(inputs, context);
        }

        public void Process(DummyInputs inputs, IDecoratorContext context)
        {
            Called = true;
            this.Next(inputs, context);
        }

        public Task ProcessAsync(DummyEInputs inputs, IEDecoratorContext context, CancellationToken token = default)
        {
            EInputsCalled = true;
            return this.Next(inputs, context, token);
        }

        public Task<DummyOutput> ProcessAsync(DummyInputs<DummyOutput>  inputs, IAsyncDecoratorContext<DummyOutput> context, CancellationToken token = default)
        {
            AsyncOutputCalled = true;
            return this.Next(inputs, context, token);
        }

        public Task ProcessAsync(DummyInputs inputs, IAsyncDecoratorContext context, CancellationToken token = default)
        {
            AsyncCalled = true;
            return this.Next(inputs, context, token);
        }
    }

    internal class DummyAllDecorator<T> :
        IAsyncDecorator<T>,
        IDecorator<T>
        where T : class
    {
        public bool Called { get; set; }
        public bool AsyncCalled { get; set; }

        public void Process(T inputs, IDecoratorContext context)
        {
            Called = true;
            this.Next(inputs, context);
        }

        public Task ProcessAsync(T inputs, IAsyncDecoratorContext context, CancellationToken token = default)
        {
            AsyncCalled = true;
            return this.Next(inputs, context, token);
        }
    }

    internal class DummyAllDecorator<T,R> :
        IAsyncDecorator<T, R>,
        IDecorator<T, R>
        where T: class
    {
        public bool Called { get; set; }
        public bool AsyncCalled { get; set; }
        public R Process(T inputs, IDecoratorContext<R> context)
        {
            Called = true;
            return this.Next(inputs, context);
        }

        public Task<R> ProcessAsync(T inputs, IAsyncDecoratorContext<R> context, CancellationToken token = default)
        {
            AsyncCalled = true;
            return this.Next(inputs, context, token);
        }
    }
    internal class DummyEInputsDecorator :
        IEDecorator<DummyEInputs>
    {
        public Task ProcessAsync(DummyEInputs inputs, IEDecoratorContext context, CancellationToken token = default)
        {
            return this.Next(inputs, context, token);
        }
    }
    internal class DummyDecorator :
        IDecorator<DummyInputs>
    {
        public bool Called { get; set; }
        public void Process(DummyInputs inputs, IDecoratorContext context)
        {
            Called = true;
            this.Next(inputs, context);
        }
    }
    internal class DummyAsyncDecorator :
        IAsyncDecorator<DummyInputs>
    {
        public bool Called { get; set; }
        public Task ProcessAsync(DummyInputs inputs, IAsyncDecoratorContext context, CancellationToken token = default)
        {
            Called = true;
            return this.Next(inputs, context, token);
        }
    }
    internal class DummyOutputDecorator :
        IDecorator<DummyInputs<DummyOutput>, DummyOutput>
    {
        public bool Called { get; set; }
        public DummyOutput Process(DummyInputs<DummyOutput>  inputs, IDecoratorContext<DummyOutput> context)
        {
            Called = true;
            return this.Next(inputs, context);
        }
    }
    internal class DummyAsyncOutputDecorator :
        IAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput>
    {
        public bool Called { get; set; }
        public Task<DummyOutput> ProcessAsync(DummyInputs<DummyOutput>  inputs, IAsyncDecoratorContext<DummyOutput> context, CancellationToken token = default)
        {
            Called = true;
            return this.Next(inputs, context, token);
        }
    }

    internal class DummyDecorator<TInputs, TOutput> :
        IDecorator<TInputs, TOutput>
        where TInputs : class
    {
        public bool Called { get; set; }
        public TOutput Process(TInputs inputs, IDecoratorContext<TOutput> context = null)
        {
            Called = true;
            Debug.WriteLine($"{nameof(DummyDecorator<IInputs<TOutput>, TOutput>)} {nameof(TOutput)} {nameof(Process)} called");
            return this.Next(inputs, context);
        }
    }
    internal class DummyEDecorator<TEInputs> :
        IEDecorator<TEInputs>
        where TEInputs : IEInputs
    {
        public Task ProcessAsync(TEInputs inputs, IEDecoratorContext context, CancellationToken token = default)
        {
            Debug.WriteLine($"{nameof(DummyEDecorator<IEInputs>)} {nameof(ProcessAsync)} called");
            return this.Next(inputs, context, token);
        }
    }
    internal class Dummy2EDecorator<TEInputs> :
        IEDecorator<TEInputs>
        where TEInputs : IEInputs
    {

        public Task ProcessAsync(TEInputs inputs, IEDecoratorContext context, CancellationToken token = default)
        {
            Debug.WriteLine($"{nameof(DummyEDecorator<IEInputs>)} {nameof(ProcessAsync)} called");
            return this.Next(inputs, context, token);
        }
    }
    internal class DummyDecorator<TInputs> :
        IDecorator<TInputs>
        where TInputs : class
    {
        public bool Called { get; set; }
        public void Process(TInputs inputs, IDecoratorContext context)
        {
            Called = true;
            Debug.WriteLine($"{nameof(DummyDecorator<IInputs>)} {nameof(Process)} called");
            this.Next(inputs, context);
        }
    }
    internal class DummyAsyncDecorator<TInputs> :
        IAsyncDecorator<TInputs>
        where TInputs : class
    {
        public bool Called { get; set; }
        public Task ProcessAsync(TInputs inputs, IAsyncDecoratorContext context, CancellationToken token = default)
        {
            Called = true;
            Debug.WriteLine($"{nameof(DummyDecorator<IInputs>)} {nameof(ProcessAsync)} called");
            return this.Next(inputs, context, token);
        }
    }
    internal class DummyAsyncDecorator<TInputs, TOutput> :
        IAsyncDecorator<TInputs, TOutput>
        where TInputs : class
    {
        public bool Called { get; set; }
        public Task<TOutput> ProcessAsync(TInputs inputs, IAsyncDecoratorContext<TOutput> context, CancellationToken token = default)
        {
            Called = true;
            Debug.WriteLine($"{nameof(DummyDecorator<IInputs>)} {nameof(ProcessAsync)} called");
            return this.Next(inputs, context, token);
        }
    }
}

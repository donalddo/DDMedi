using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi.Test.Dummies
{
    internal abstract class InvalidDecorator :
        IDecorator<DummyInputs>
    {
        public abstract void Process(IDecoratorContext<DummyInputs> context);
    }
    internal abstract class InvalidDecorator2<T> :
        IDecorator<T> where T : IInputs
    {
        public abstract void Process(IDecoratorContext<T> context);
    }
    internal class SuperDummySupplier :
        IAsyncSupplier<IInputs>,
        IESupplier<IEInputs>,
        IAsyncDecorator<IInputs>,
        IEDecorator<IEInputs>
    {
        public Task ProcessAsync(ISupplierContext<IInputs> context, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public Task ProcessAsync(ISupplierContext<IEInputs> context, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public Task ProcessAsync(IAsyncDecoratorContext<IInputs> context, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public Task ProcessAsync(IEDecoratorContext<IEInputs> context, CancellationToken token = default)
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
        public DummyOutput Process(IDecoratorContext<DummyInputs<DummyOutput>, DummyOutput> context)
        {
            OutputCalled = true;
            return this.Next(context);
        }

        public void Process(IDecoratorContext<DummyInputs> context)
        {
            Called = true;
            this.Next(context);
        }

        public Task ProcessAsync(IEDecoratorContext<DummyEInputs> context, CancellationToken token = default)
        {
            EInputsCalled = true;
            return this.Next(context, token);
        }

        public Task<DummyOutput> ProcessAsync(IAsyncDecoratorContext<DummyInputs<DummyOutput>, DummyOutput> context, CancellationToken token = default)
        {
            AsyncOutputCalled = true;
            return this.Next(context, token);
        }

        public Task ProcessAsync(IAsyncDecoratorContext<DummyInputs> context, CancellationToken token = default)
        {
            AsyncCalled = true;
            return this.Next(context, token);
        }
    }

    internal class DummyAllDecorator<T> :
        IAsyncDecorator<T>,
        IDecorator<T>
        where T : IInputs
    {
        public bool Called { get; set; }
        public bool AsyncCalled { get; set; }

        public void Process(IDecoratorContext<T> context)
        {
            Called = true;
            this.Next(context);
        }

        public Task ProcessAsync(IAsyncDecoratorContext<T> context, CancellationToken token = default)
        {
            AsyncCalled = true;
            return this.Next(context, token);
        }
    }

    internal class DummyAllDecorator<T,R> :
        IAsyncDecorator<T, R>,
        IDecorator<T, R>
        where T: IInputs<R>
    {
        public bool Called { get; set; }
        public bool AsyncCalled { get; set; }
        public R Process(IDecoratorContext<T, R> context)
        {
            Called = true;
            return this.Next(context);
        }

        public Task<R> ProcessAsync(IAsyncDecoratorContext<T, R> context, CancellationToken token = default)
        {
            AsyncCalled = true;
            return this.Next(context, token);
        }
    }
    internal class DummyEInputsDecorator :
        IEDecorator<DummyEInputs>
    {
        public Task ProcessAsync(IEDecoratorContext<DummyEInputs> context, CancellationToken token = default)
        {
            return this.Next(context, token);
        }
    }
    internal class DummyDecorator :
        IDecorator<DummyInputs>
    {
        public bool Called { get; set; }
        public void Process(IDecoratorContext<DummyInputs> context)
        {
            Called = true;
            this.Next(context);
        }
    }
    internal class DummyAsyncDecorator :
        IAsyncDecorator<DummyInputs>
    {
        public bool Called { get; set; }
        public Task ProcessAsync(IAsyncDecoratorContext<DummyInputs> context, CancellationToken token = default)
        {
            Called = true;
            return this.Next(context, token);
        }
    }
    internal class DummyOutputDecorator :
        IDecorator<DummyInputs<DummyOutput>, DummyOutput>
    {
        public bool Called { get; set; }
        public DummyOutput Process(IDecoratorContext<DummyInputs<DummyOutput>, DummyOutput> context)
        {
            Called = true;
            return this.Next(context);
        }
    }
    internal class DummyAsyncOutputDecorator :
        IAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput>
    {
        public bool Called { get; set; }
        public Task<DummyOutput> ProcessAsync(IAsyncDecoratorContext<DummyInputs<DummyOutput>, DummyOutput> context, CancellationToken token = default)
        {
            Called = true;
            return this.Next(context, token);
        }
    }

    internal class DummyDecorator<TInputs, TOutput> :
        IDecorator<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        public bool Called { get; set; }
        public TOutput Process(IDecoratorContext<TInputs,TOutput> context = null)
        {
            Called = true;
            Debug.WriteLine($"{nameof(DummyDecorator<IInputs<TOutput>, TOutput>)} {nameof(TOutput)} {nameof(Process)} called");
            return this.Next(context);
        }
    }
    internal class DummyEDecorator<TEInputs> :
        IEDecorator<TEInputs>
        where TEInputs : IEInputs
    {
        public Task ProcessAsync(IEDecoratorContext<TEInputs> context, CancellationToken token = default)
        {
            Debug.WriteLine($"{nameof(DummyEDecorator<IEInputs>)} {nameof(ProcessAsync)} called");
            return this.Next(context, token);
        }
    }
    internal class Dummy2EDecorator<TEInputs> :
        IEDecorator<TEInputs>
        where TEInputs : IEInputs
    {

        public Task ProcessAsync(IEDecoratorContext<TEInputs> context, CancellationToken token = default)
        {
            Debug.WriteLine($"{nameof(DummyEDecorator<IEInputs>)} {nameof(ProcessAsync)} called");
            return this.Next(context, token);
        }
    }
    internal class DummyDecorator<TInputs> :
        IDecorator<TInputs>
        where TInputs : IInputs
    {
        public bool Called { get; set; }
        public void Process(IDecoratorContext<TInputs> context)
        {
            Called = true;
            Debug.WriteLine($"{nameof(DummyDecorator<IInputs>)} {nameof(Process)} called");
            this.Next(context);
        }
    }
    internal class DummyAsyncDecorator<TInputs> :
        IAsyncDecorator<TInputs>
        where TInputs : IInputs
    {
        public bool Called { get; set; }
        public Task ProcessAsync(IAsyncDecoratorContext<TInputs> context, CancellationToken token = default)
        {
            Called = true;
            Debug.WriteLine($"{nameof(DummyDecorator<IInputs>)} {nameof(ProcessAsync)} called");
            return this.Next(context, token);
        }
    }
    internal class DummyAsyncDecorator<TInputs, TOutput> :
        IAsyncDecorator<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        public bool Called { get; set; }
        public Task<TOutput> ProcessAsync(IAsyncDecoratorContext<TInputs, TOutput> context, CancellationToken token = default)
        {
            Called = true;
            Debug.WriteLine($"{nameof(DummyDecorator<IInputs>)} {nameof(ProcessAsync)} called");
            return this.Next(context, token);
        }
    }
}

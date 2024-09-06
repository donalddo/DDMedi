using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi.Test.Dummies
{   public interface IInputs { }
    public interface IInputs<out Output> { }
    public class DummyInputs : IInputs { }
    internal class DummyInputs<TOutput> : IInputs<TOutput> { }
    public class Dummy2Inputs : IInputs { }
    internal class Dummy2Inputs<TOutput> : IInputs<TOutput> { }
    internal class DummyOutput { }
    public class DummyEInputs : IEInputs { }
    public class Dummy2EInputs : IEInputs { }

    internal class InvalidSupplier : ISupplier { }
    internal abstract class InvalidSupplier2 :
        IAsyncSupplier<DummyInputs>
    {
        public Task ProcessAsync(DummyInputs inputs, ISupplierContext context, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
    internal class InvalidESupplier : IESupplier { }
    internal abstract class InvalidESupplier2 :
        IESupplier<DummyEInputs>
    {
        public Task ProcessAsync(DummyEInputs inputs, ISupplierContext context, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
    internal class Dummy2Supplier :
        IAsyncSupplier<DummyInputs>
    {
        public Task ProcessAsync(DummyInputs inputs, ISupplierContext context, CancellationToken token = default)
        {
            Debug.WriteLine($"{GetType().Name} {nameof(DummyInputs)} {nameof(ProcessAsync)} Called");
            return Task.CompletedTask;
        }
    }
    internal class DummySupplier :
        IAsyncSupplier<DummyInputs<DummyOutput>, DummyOutput>,
        IAsyncSupplier<DummyInputs>,
        IESupplier<DummyEInputs>,
        ISupplier<DummyInputs<DummyOutput>, DummyOutput>,
        ISupplier<DummyInputs>,
        ICloneable
    {
        public bool AsyncCalled { get; set; }
        public bool Called { get; set; }
        public bool AsyncOutputCalled { get; set; }
        public bool OutputCalled { get; set; }
        public object Clone()
        {
            return new object();
        }

        public DummyOutput Process(DummyInputs<DummyOutput> inputs, ISupplierContext context)
        {
            OutputCalled = true;
            Debug.WriteLine($"{GetType().Name} {nameof(DummyInputs<DummyOutput>)} {nameof(Process)} Called");
            return null;
        }

        public void Process(DummyInputs inputs, ISupplierContext context)
        {
            Called = true;
            Debug.WriteLine($"{GetType().Name} {nameof(DummyInputs)} {nameof(Process)} Called");
        }

        public Task<DummyOutput> ProcessAsync(DummyInputs<DummyOutput> inputs, ISupplierContext context, CancellationToken token = default)
        {
            AsyncOutputCalled = true;
            Debug.WriteLine($"{GetType().Name} {nameof(DummyInputs<DummyOutput>)} {nameof(ProcessAsync)} Called");
            return null;
        }

        public Task ProcessAsync(DummyInputs inputs, ISupplierContext context, CancellationToken token = default)
        {
            AsyncCalled = true;
            Debug.WriteLine($"{GetType().Name} {nameof(DummyInputs)} {nameof(ProcessAsync)} Called");
            return Task.CompletedTask;
        }

        public Task ProcessAsync(DummyEInputs inputs, ISupplierContext context, CancellationToken token = default)
        {
            Debug.WriteLine($"{GetType().Name} {nameof(DummyEInputs)} {nameof(ProcessAsync)} Called");
            return Task.CompletedTask;
        }
    }

    internal class DummySupplier<TInputs> :
        IAsyncSupplier<TInputs>,
        ISupplier<TInputs>
        where TInputs:class
    {
        public bool Called { get; set; }
        public bool AsyncCalled { get; set; }
        public object Clone()
        {
            return new object();
        }

        public void Process(TInputs inputs, ISupplierContext context)
        {
            Called = true;
            Debug.WriteLine($"{GetType().Name}<> {typeof(TInputs)} {nameof(Process)} Called");
        }

        public Task ProcessAsync(TInputs inputs, ISupplierContext context, CancellationToken token = default)
        {
            AsyncCalled = true;
            Debug.WriteLine($"{GetType().Name}<> {typeof(TInputs)} {nameof(ProcessAsync)} Called");
            return Task.CompletedTask;
        }
    }

    internal class DummyESupplier<TEInputs> :
        IESupplier<TEInputs>
        where TEInputs : IEInputs
    {
        public DummyESupplier(TEInputs inputs)
        {

        }
        public Task ProcessAsync(TEInputs inputs, ISupplierContext context, CancellationToken token = default)
        {
            Debug.WriteLine($"{GetType().Name}<> {typeof(TEInputs)} {nameof(ProcessAsync)} Called");
            return Task.CompletedTask;
        }
    }

    internal class Dummy3Supplier: ISupplier<Dummy2Inputs>
    {
        public Dummy2Inputs Inputs { get; }
        public Dummy3Supplier(Dummy2Inputs inputs)
        {
            Inputs = inputs;
        }
        public void Process(Dummy2Inputs inputs, ISupplierContext context)
        {
            Debug.WriteLine($"{GetType().Name}<> {typeof(Dummy2Inputs)} {nameof(Process)} Called");
        }
    }
    internal class DummySupplier<TInputs,TOutput> :
        IAsyncSupplier<TInputs, TOutput>,
        ISupplier<TInputs, TOutput>
        where TInputs : class
    {
        public object Clone()
        {
            return new object();
        }

        public virtual TOutput Process(TInputs inputs, ISupplierContext context)
        {
            Debug.WriteLine($"{GetType().Name}<Output> {typeof(TInputs)} {nameof(Process)} Called");
            return default(TOutput);
        }

        public virtual Task<TOutput> ProcessAsync(TInputs inputs, ISupplierContext context, CancellationToken token = default)
        {
            Debug.WriteLine($"{GetType().Name}<Output> {typeof(TInputs)} {nameof(ProcessAsync)} Called");
            return Task.FromResult(default(TOutput));
        }
    }
}

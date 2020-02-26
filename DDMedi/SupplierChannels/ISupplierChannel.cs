using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public interface IBaseSupplierChannel
    {
        IDDBroker DDBroker { get; }
    }
    public interface IAsyncSupplierChannel<TInputs, TOutput> : IBaseSupplierChannel where TInputs : IInputs<TOutput>
    {
        Task<TOutput> ProcessAsync(TInputs inputs = default, CancellationToken token = default);
    }
    public interface IAsyncSupplierChannel<TInputs> : IBaseSupplierChannel where TInputs : IInputs
    {
        Task ProcessAsync(TInputs inputs = default, CancellationToken token = default);
    }
    public interface ISupplierChannel<TInputs, TOutput> : IBaseSupplierChannel where TInputs : IInputs<TOutput>
    {
        TOutput Process(TInputs inputs = default);
    }
    public interface ISupplierChannel<TInputs> : IBaseSupplierChannel where TInputs : IInputs
    {
        void Process(TInputs inputs = default);
    }
    internal abstract class BaseSupplierChannel<TInputs, TSupplier>
        where TSupplier : class
    {
        protected readonly TSupplier _supplier;
        public IDDBroker DDBroker { get; }
        protected readonly BaseSupplierContext<TInputs> _context;
        protected BaseSupplierChannel(IInternalDDBroker ddBroker, SupplierDescriptor descriptor)
        {
            _supplier = ddBroker.Provider.GetService(descriptor.ImplementDescriptor.RegisterType) as TSupplier;
            DDBroker = ddBroker;
            _context = new BaseSupplierContext<TInputs>(ddBroker);
        }
    }
    internal sealed class AsyncSupplierChannel<TInputs, TOutput> :
        BaseSupplierChannel<TInputs, IAsyncSupplier<TInputs, TOutput>>,
        IAsyncSupplierChannel<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        internal AsyncSupplierChannel(IInternalDDBroker ddBroker, SupplierDescriptor descriptor) :
            base(ddBroker, descriptor) { }
        public Task<TOutput> ProcessAsync(TInputs inputs = default, CancellationToken token = default)
        {
            _context.Inputs = inputs;
            return _supplier.ProcessAsync(_context, token);
        }

    }
    internal sealed class AsyncSupplierChannel<TInputs> :
        BaseSupplierChannel<TInputs, IAsyncSupplier<TInputs>>,
        IAsyncSupplierChannel<TInputs>
        where TInputs : IInputs
    {
        internal AsyncSupplierChannel(IInternalDDBroker ddBroker, SupplierDescriptor descriptor) :
            base(ddBroker, descriptor) { }
        public Task ProcessAsync(TInputs inputs = default, CancellationToken token = default)
        {
            _context.Inputs = inputs;
            return _supplier.ProcessAsync(_context, token);
        }

    }
    internal sealed class SupplierChannel<TInputs, TOutput> :
        BaseSupplierChannel<TInputs, ISupplier<TInputs, TOutput>>,
        ISupplierChannel<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        internal SupplierChannel(IInternalDDBroker ddBroker, SupplierDescriptor descriptor) :
            base(ddBroker, descriptor) { }
        public TOutput Process(TInputs inputs = default)
        {
            _context.Inputs = inputs;
            return _supplier.Process(_context);
        }

    }
    internal sealed class SupplierChannel<TInputs> :
        BaseSupplierChannel<TInputs, ISupplier<TInputs>>,
        ISupplierChannel<TInputs>
        where TInputs : IInputs
    {
        internal SupplierChannel(IInternalDDBroker ddBroker, SupplierDescriptor descriptor) :
            base(ddBroker, descriptor) { }
        public void Process(TInputs inputs = default)
        {
            _context.Inputs = inputs;
            _supplier.Process(_context);
        }

    }
    internal sealed class ESupplierChannel<TEInputs> :
        BaseSupplierChannel<TEInputs, IESupplier<TEInputs>>
        where TEInputs : IEInputs
    {
        internal ESupplierChannel(IInternalDDBroker ddBroker, SupplierDescriptor descriptor) :
            base(ddBroker, descriptor) { }
        public Task ProcessAsync(TEInputs inputs = default, CancellationToken token = default)
        {
            _context.Inputs = inputs;
            return _supplier.ProcessAsync(_context, token);
        }

    }
}

using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public interface ISupplierContext
    {
        IDDBroker DDBroker { get; }
        string CorrelationId { get; }
    }
    internal abstract class BaseSupplierContext<TInputs, TSupplier>:
        ISupplierContext
        where TSupplier : class
    {
        public string CorrelationId { get; }
        public IDDBroker DDBroker => _broker;
        protected readonly IInternalDDBroker _broker;
        protected readonly SupplierDescriptor _descriptor;
        protected TSupplier _instance;
        protected BaseSupplierContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
        {
            _descriptor = descriptor;
            _broker = broker;
            CorrelationId = broker.CorrelationId;
            _instance = broker.Provider.GetService(_descriptor.ImplementDescriptor.RegisterType) as TSupplier;
        }
    }
    internal sealed class AsyncSupplierContext<TInputs, TOutput> :
        BaseSupplierContext<TInputs, IAsyncSupplier<TInputs, TOutput>>,
        IAsyncSupplierChannel<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        internal AsyncSupplierContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
            : base(broker, descriptor) { }
        public Task<TOutput> ProcessAsync(TInputs inputs, CancellationToken cancellationToken = default) => _instance.ProcessAsync(inputs, this, cancellationToken);
    }
    internal sealed class AsyncSupplierContext<TInputs> :
        BaseSupplierContext<TInputs, IAsyncSupplier<TInputs>>,
        IAsyncSupplierChannel<TInputs>
        where TInputs : IInputs
    {
        internal AsyncSupplierContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
            : base(broker, descriptor) { }
        public Task ProcessAsync(TInputs inputs, CancellationToken cancellationToken = default) => _instance.ProcessAsync(inputs, this, cancellationToken);
    }
    internal sealed class SupplierContext<TInputs, TOutput> :
        BaseSupplierContext<TInputs, ISupplier<TInputs, TOutput>>,
        ISupplierChannel<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        internal SupplierContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
            : base(broker, descriptor) { }
        public TOutput Process(TInputs inputs) => _instance.Process(inputs, this);
    }
    internal sealed class SupplierContext<TInputs> :
        BaseSupplierContext<TInputs, ISupplier<TInputs>>,
        ISupplierChannel<TInputs>
        where TInputs : IInputs
    {
        internal SupplierContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
            : base(broker, descriptor) { }
        public void Process(TInputs inputs) => _instance.Process(inputs, this);
    }
    internal sealed class ESupplierContext<TEInputs> :
        BaseSupplierContext<TEInputs, IESupplier<TEInputs>>,
        IESupplierChannel<TEInputs>
        where TEInputs : IEInputs
    {
        internal ESupplierContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
            : base(broker, descriptor) { }
        public Task ProcessAsync(TEInputs inputs, CancellationToken cancellationToken = default) => _instance.ProcessAsync(inputs, this, cancellationToken);
    }
}
    

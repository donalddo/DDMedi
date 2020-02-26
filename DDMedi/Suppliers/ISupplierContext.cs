using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public interface IBaseContext
    {
        object Instance { get; }
    }
    public interface ISupplierContext<TInputs>
    {
        IDDBroker DDBroker { get; }
        TInputs Inputs { get; }
        string CorrelationId { get; }
    }
    internal interface IInternalBaseContext : IBaseContext
    {
        void InitInstance();
    }
    internal interface IInternalAsyncSupplierContext<TInputs, TOutput> : IInternalBaseContext
        where TInputs : IInputs<TOutput>
    {
        Task<TOutput> ProcessAsync(CancellationToken cancellationToken = default);
    }
    internal interface IInternalAsyncSupplierContext<TInputs> : IInternalBaseContext
        where TInputs : IInputs
    {
        Task ProcessAsync(CancellationToken cancellationToken = default);
    }
    internal interface IInternalSupplierContext<TInputs, TOutput> : IInternalBaseContext
        where TInputs : IInputs<TOutput>
    {
        TOutput Process();
    }
    internal interface IInternalSupplierContext<TInputs> : IInternalBaseContext
        where TInputs : IInputs
    {
        void Process();
    }
    internal interface IInternalESupplierContext<TEInputs> : IInternalBaseContext
        where TEInputs : IEInputs
    {
        Task ProcessAsync(CancellationToken cancellationToken = default);
    }
    internal class BaseSupplierContext<TInputs> : ISupplierContext<TInputs>
    {
        public BaseSupplierContext(IDDBroker ddBroker)
        {
            DDBroker = ddBroker;
            CorrelationId = ddBroker.CorrelationId;
        }
        public IDDBroker DDBroker { get; }
        public TInputs Inputs { get; internal set; }
        public string CorrelationId { get; }
    }
    internal abstract class BaseSupplierContext<TInputs, TDecoratorChannel, TSupplier>:
        ISupplierContext<TInputs>, IInternalBaseContext
        where TDecoratorChannel : IBaseDecoratorChannel<TInputs>
        where TSupplier : class
    {
        public TInputs Inputs => _channel.Inputs;
        public string CorrelationId { get; }
        public IDDBroker DDBroker { get; }
        protected readonly TDecoratorChannel _channel;
        protected readonly SupplierDescriptor _descriptor;
        protected TSupplier _instance;
        public object Instance { get => _instance; protected set => _instance = value as TSupplier; }
        protected BaseSupplierContext(TDecoratorChannel channel, SupplierDescriptor descriptor, object instance) :
            this(channel, descriptor) => Instance = instance;
        protected BaseSupplierContext(TDecoratorChannel channel, SupplierDescriptor descriptor)
        {
            _descriptor = descriptor;
            _channel = channel;
            DDBroker = _channel.DDBroker;
            CorrelationId = _channel.DDBroker.CorrelationId;
        }
        public void InitInstance()
        => Instance = _channel.Provider.GetService(_descriptor.ImplementDescriptor.RegisterType);
    }
    internal sealed class AsyncSupplierContext<TInputs, TOutput> :
        BaseSupplierContext<TInputs,
            AsyncDecoratorChannel<TInputs, TOutput>,
            IAsyncSupplier<TInputs, TOutput>>,
        IInternalAsyncSupplierContext<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        internal AsyncSupplierContext(AsyncDecoratorChannel<TInputs, TOutput> channel, SupplierDescriptor descriptor)
            : base(channel, descriptor) { }
        internal AsyncSupplierContext(AsyncDecoratorChannel<TInputs, TOutput> channel, SupplierDescriptor descriptor, object instance)
            : base(channel, descriptor, instance) { }
        public Task<TOutput> ProcessAsync(CancellationToken cancellationToken = default) => _instance.ProcessAsync(this, cancellationToken);
    }
    internal sealed class AsyncSupplierContext<TInputs> :
        BaseSupplierContext<TInputs,
            AsyncDecoratorChannel<TInputs>,
            IAsyncSupplier<TInputs>>,
        IInternalAsyncSupplierContext<TInputs>
        where TInputs : IInputs
    {
        internal AsyncSupplierContext(AsyncDecoratorChannel<TInputs> channel, SupplierDescriptor descriptor)
            : base(channel, descriptor) { }
        internal AsyncSupplierContext(AsyncDecoratorChannel<TInputs> channel, SupplierDescriptor descriptor, object instance)
            : base(channel, descriptor, instance) { }
        public Task ProcessAsync(CancellationToken cancellationToken = default) => _instance.ProcessAsync(this, cancellationToken);
    }
    internal sealed class SupplierContext<TInputs, TOutput> :
        BaseSupplierContext<TInputs,
            DecoratorChannel<TInputs, TOutput>,
            ISupplier<TInputs, TOutput>>,
        IInternalSupplierContext<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        internal SupplierContext(DecoratorChannel<TInputs, TOutput> channel, SupplierDescriptor descriptor)
            : base(channel, descriptor) { }
        internal SupplierContext(DecoratorChannel<TInputs, TOutput> channel, SupplierDescriptor descriptor, object instance)
            : base(channel, descriptor, instance) { }
        public TOutput Process() => _instance.Process(this);
    }
    internal sealed class SupplierContext<TInputs> :
        BaseSupplierContext<TInputs,
            DecoratorChannel<TInputs>,
            ISupplier<TInputs>>,
        IInternalSupplierContext<TInputs>
        where TInputs : IInputs
    {
        internal SupplierContext(DecoratorChannel<TInputs> channel, SupplierDescriptor descriptor)
            : base(channel, descriptor) { }
        internal SupplierContext(DecoratorChannel<TInputs> channel, SupplierDescriptor descriptor, object instance)
            : base(channel, descriptor, instance) { }
        public void Process() => _instance.Process(this);
    }
    internal sealed class ESupplierContext<TEInputs> :
        BaseSupplierContext<TEInputs,
            EDecoratorChannel<TEInputs>,
            IESupplier<TEInputs>>,
        IInternalESupplierContext<TEInputs>
        where TEInputs : IEInputs
    {
        internal ESupplierContext(EDecoratorChannel<TEInputs> channel, SupplierDescriptor descriptor)
            : base(channel, descriptor) { }
        internal ESupplierContext(EDecoratorChannel<TEInputs> channel, SupplierDescriptor descriptor, object instance)
            : base(channel, descriptor, instance) { }
        public Task ProcessAsync(CancellationToken cancellationToken = default) => _instance.ProcessAsync(this, cancellationToken);
    }
}
    

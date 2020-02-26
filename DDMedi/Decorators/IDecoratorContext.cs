using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public interface IAsyncDecoratorContext<TInputs, TOutput> : ISupplierContext<TInputs>
        where TInputs : IInputs<TOutput>
    {
        Task<TOutput> Next(CancellationToken cancellationToken = default);
    }
    public interface IAsyncDecoratorContext<TInputs> : ISupplierContext<TInputs>
         where TInputs : IInputs
    {
        Task Next(CancellationToken cancellationToken = default);
    }
    public interface IDecoratorContext<TInputs, TOutput> : ISupplierContext<TInputs>
        where TInputs : IInputs<TOutput>
    {
        TOutput Next();
    }
    public interface IDecoratorContext<TInputs> : ISupplierContext<TInputs>
        where TInputs : IInputs
    {
        void Next();
    }
    public interface IEDecoratorContext<TInputs> : ISupplierContext<TInputs>
        where TInputs : IEInputs
    {
        Task Next(CancellationToken cancellationToken = default);
    }
    internal sealed class AsyncDecoratorContext<TInputs, TOutput> :
        BaseSupplierContext<TInputs,
            AsyncDecoratorChannel<TInputs, TOutput>,
            IAsyncDecorator<TInputs, TOutput>>,
        IInternalAsyncSupplierContext<TInputs, TOutput>,
        IAsyncDecoratorContext<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        private readonly IInternalAsyncSupplierContext<TInputs, TOutput> _nextSupplier;
        internal AsyncDecoratorContext(AsyncDecoratorChannel<TInputs, TOutput> channel, SupplierDescriptor descriptor, IInternalAsyncSupplierContext<TInputs, TOutput> nextSupplier)
            : base(channel, descriptor) => _nextSupplier = nextSupplier;
        internal AsyncDecoratorContext(AsyncDecoratorChannel<TInputs, TOutput> channel, SupplierDescriptor descriptor, object instance, IInternalAsyncSupplierContext<TInputs, TOutput> nextSupplier)
            : base(channel, descriptor, instance) => _nextSupplier = nextSupplier;
        public Task<TOutput> ProcessAsync(CancellationToken cancellationToken = default) => _instance.ProcessAsync(this, cancellationToken);
        public Task<TOutput> Next(CancellationToken cancellationToken = default)
        {
            if (_nextSupplier.Instance != null)
                return _nextSupplier.ProcessAsync(cancellationToken);
            _nextSupplier.InitInstance();
            return _nextSupplier.ProcessAsync(cancellationToken);
        }
    }
    internal sealed class AsyncDecoratorContext<TInputs> :
        BaseSupplierContext<TInputs,
            AsyncDecoratorChannel<TInputs>,
            IAsyncDecorator<TInputs>>,
        IInternalAsyncSupplierContext<TInputs>,
        IAsyncDecoratorContext<TInputs>
        where TInputs : IInputs
    {
        private readonly IInternalAsyncSupplierContext<TInputs> _nextSupplier;
        internal AsyncDecoratorContext(AsyncDecoratorChannel<TInputs> channel, SupplierDescriptor descriptor, IInternalAsyncSupplierContext<TInputs> nextSupplier)
            : base(channel, descriptor) => _nextSupplier = nextSupplier;
        internal AsyncDecoratorContext(AsyncDecoratorChannel<TInputs> channel, SupplierDescriptor descriptor, object instance, IInternalAsyncSupplierContext<TInputs> nextSupplier)
            : base(channel, descriptor, instance) => _nextSupplier = nextSupplier;
        public Task ProcessAsync(CancellationToken cancellationToken = default) => _instance.ProcessAsync(this, cancellationToken);
        public Task Next(CancellationToken cancellationToken = default)
        {
            if (_nextSupplier.Instance != null)
                return _nextSupplier.ProcessAsync(cancellationToken);
            _nextSupplier.InitInstance();
            return _nextSupplier.ProcessAsync(cancellationToken);
        }
    }
    internal sealed class DecoratorContext<TInputs, TOutput> :
        BaseSupplierContext<TInputs,
            DecoratorChannel<TInputs, TOutput>,
            IDecorator<TInputs, TOutput>>,
        IInternalSupplierContext<TInputs, TOutput>,
        IDecoratorContext<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        private readonly IInternalSupplierContext<TInputs, TOutput> _nextSupplier;
        internal DecoratorContext(DecoratorChannel<TInputs, TOutput> channel, SupplierDescriptor descriptor, IInternalSupplierContext<TInputs, TOutput> nextSupplier)
            : base(channel, descriptor) => _nextSupplier = nextSupplier;
        internal DecoratorContext(DecoratorChannel<TInputs, TOutput> channel, SupplierDescriptor descriptor, object instance, IInternalSupplierContext<TInputs, TOutput> nextSupplier)
            : base(channel, descriptor, instance) => _nextSupplier = nextSupplier;
        public TOutput Process() => _instance.Process(this);
        public TOutput Next()
        {
            if (_nextSupplier.Instance != null)
                return _nextSupplier.Process();
            _nextSupplier.InitInstance();
            return _nextSupplier.Process();
        }
    }
    internal sealed class DecoratorContext<TInputs> :
        BaseSupplierContext<TInputs,
            DecoratorChannel<TInputs>,
            IDecorator<TInputs>>,
        IInternalSupplierContext<TInputs>,
        IDecoratorContext<TInputs>
        where TInputs : IInputs
    {
        private readonly IInternalSupplierContext<TInputs> _nextSupplier;
        internal DecoratorContext(DecoratorChannel<TInputs> channel, SupplierDescriptor descriptor, IInternalSupplierContext<TInputs> nextSupplier)
            : base(channel, descriptor) => _nextSupplier = nextSupplier;
        internal DecoratorContext(DecoratorChannel<TInputs> channel, SupplierDescriptor descriptor, object instance, IInternalSupplierContext<TInputs> nextSupplier)
            : base(channel, descriptor, instance) => _nextSupplier = nextSupplier;
        public void Process() => _instance.Process(this);
        public void Next()
        {
            if (_nextSupplier.Instance != null)
            {
                _nextSupplier.Process();
                return;
            }
            _nextSupplier.InitInstance();
            _nextSupplier.Process();
        }
    }
    internal sealed class EDecoratorContext<TEInputs> :
        BaseSupplierContext<TEInputs,
            EDecoratorChannel<TEInputs>,
            IEDecorator<TEInputs>>,
        IInternalESupplierContext<TEInputs>,
        IEDecoratorContext<TEInputs>
        where TEInputs : IEInputs
    {
        private readonly IInternalESupplierContext<TEInputs> _nextSupplier;
        internal EDecoratorContext(EDecoratorChannel<TEInputs> channel, SupplierDescriptor descriptor, IInternalESupplierContext<TEInputs> nextSupplier)
            : base(channel, descriptor) => _nextSupplier = nextSupplier;
        internal EDecoratorContext(EDecoratorChannel<TEInputs> channel, SupplierDescriptor descriptor, object instance, IInternalESupplierContext<TEInputs> nextSupplier)
            : base(channel, descriptor, instance) => _nextSupplier = nextSupplier;
        public Task ProcessAsync(CancellationToken cancellationToken = default) => _instance.ProcessAsync(this, cancellationToken);
        public Task Next(CancellationToken cancellationToken = default)
        {
            if (_nextSupplier.Instance != null)
                return _nextSupplier.ProcessAsync(cancellationToken);
            _nextSupplier.InitInstance();
            return _nextSupplier.ProcessAsync(cancellationToken);
        }
    }
}

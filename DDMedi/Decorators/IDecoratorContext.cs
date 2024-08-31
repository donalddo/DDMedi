using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public interface IAsyncDecoratorContext<TOutput> : ISupplierContext
    {
        Task<TOutput> Next(IInputs<TOutput> inputs, CancellationToken cancellationToken = default);
        Task<TOutput> Next(IInputs<TOutput> inputs, IServiceProvider provider, CancellationToken cancellationToken = default);
    }
    public interface IAsyncDecoratorContext : ISupplierContext
    {
        Task Next(IInputs inputs, CancellationToken cancellationToken = default);
        Task Next(IInputs inputs, IServiceProvider provider, CancellationToken cancellationToken = default);
    }
    public interface IDecoratorContext<TOutput> : ISupplierContext
    {
        TOutput Next(IInputs<TOutput> inputs);
        TOutput Next(IInputs<TOutput> inputs, IServiceProvider provider);
    }
    public interface IDecoratorContext : ISupplierContext
    {
        void Next(IInputs inputs);
        void Next(IInputs inputs, IServiceProvider provider);
    }
    public interface IEDecoratorContext : ISupplierContext
    {
        Task Next(IEInputs inputs, CancellationToken cancellationToken = default);
        Task Next(IEInputs inputs, IServiceProvider provider, CancellationToken cancellationToken = default);
    }
    internal sealed class AsyncDecoratorContext<TInputs, TOutput> :
        BaseSupplierContext<TInputs, IAsyncDecorator<TInputs, TOutput>>,
        IAsyncSupplierChannel<TInputs, TOutput>,
        IAsyncDecoratorContext<TOutput>
        where TInputs : IInputs<TOutput>
    {
        private IAsyncSupplierChannel<TInputs, TOutput> _nextSupplier;
        internal AsyncDecoratorContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
            : base(broker, descriptor) { }
        public Task<TOutput> ProcessAsync(TInputs inputs, CancellationToken cancellationToken = default) => _instance.ProcessAsync(inputs, this, cancellationToken);
        public Task<TOutput> Next(IInputs<TOutput> inputs, CancellationToken cancellationToken = default)
        {
            if (_nextSupplier != null)
                return _nextSupplier.ProcessAsync((TInputs)inputs, cancellationToken);
            _nextSupplier = _broker.CreateAsyncSupplierChannel<TInputs, TOutput>(_descriptor.Next);
            return _nextSupplier.ProcessAsync((TInputs)inputs, cancellationToken);
        }
        public Task<TOutput> Next(IInputs<TOutput> inputs, IServiceProvider provider, CancellationToken cancellationToken = default)
        {
            if (provider == _broker.Provider)
                return Next(inputs, cancellationToken);
            var newBroker = provider.CreateBroker(CorrelationId);
            return newBroker.CreateAsyncSupplierChannel<TInputs, TOutput>(_descriptor.Next).ProcessAsync((TInputs)inputs, cancellationToken);
        }
    }
    internal sealed class AsyncDecoratorContext<TInputs> :
        BaseSupplierContext<TInputs, IAsyncDecorator<TInputs>>,
        IAsyncSupplierChannel<TInputs>,
        IAsyncDecoratorContext
        where TInputs : IInputs
    {
        private IAsyncSupplierChannel<TInputs> _nextSupplier;
        internal AsyncDecoratorContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
            : base(broker, descriptor) { }
        public Task ProcessAsync(TInputs inputs, CancellationToken cancellationToken = default) => _instance.ProcessAsync(inputs, this, cancellationToken);
        public Task Next(IInputs inputs, CancellationToken cancellationToken = default)
        {
            if (_nextSupplier != null)
                return _nextSupplier.ProcessAsync((TInputs)inputs, cancellationToken);
            _nextSupplier = _broker.CreateAsyncSupplierChannel<TInputs>(_descriptor.Next);
            return _nextSupplier.ProcessAsync((TInputs)inputs, cancellationToken);
        }
        public Task Next(IInputs inputs, IServiceProvider provider, CancellationToken cancellationToken = default)
        {
            if (provider == _broker.Provider)
                return Next(inputs, cancellationToken);
            var newBroker = provider.CreateBroker(CorrelationId);
            return newBroker.CreateAsyncSupplierChannel<TInputs>(_descriptor.Next).ProcessAsync((TInputs)inputs, cancellationToken);
        }
    }
    internal sealed class DecoratorContext<TInputs, TOutput> :
        BaseSupplierContext<TInputs, IDecorator<TInputs, TOutput>>,
        ISupplierChannel<TInputs, TOutput>,
        IDecoratorContext<TOutput>
        where TInputs : IInputs<TOutput>
    {
        private ISupplierChannel<TInputs, TOutput> _nextSupplier;
        internal DecoratorContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
            : base(broker, descriptor) { }
        public TOutput Process(TInputs inputs) => _instance.Process(inputs, this);
        public TOutput Next(IInputs<TOutput> inputs)
        {
            if (_nextSupplier != null)
                return _nextSupplier.Process((TInputs)inputs);
            _nextSupplier = _broker.CreateSupplierChannel<TInputs, TOutput>(_descriptor.Next);
            return _nextSupplier.Process((TInputs)inputs);
        }
        public TOutput Next(IInputs<TOutput> inputs, IServiceProvider provider)
        {
            if (provider == _broker.Provider)
                return Next(inputs);
            var newBroker = provider.CreateBroker(CorrelationId);
            return newBroker.CreateSupplierChannel<TInputs, TOutput>(_descriptor.Next).Process((TInputs)inputs);
        }
    }
    internal sealed class DecoratorContext<TInputs> :
        BaseSupplierContext<TInputs, IDecorator<TInputs>>,
        ISupplierChannel<TInputs>,
        IDecoratorContext
        where TInputs : IInputs
    {
        private ISupplierChannel<TInputs> _nextSupplier;
        internal DecoratorContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
            : base(broker, descriptor) { }
        public void Process(TInputs inputs) => _instance.Process(inputs, this);
        public void Next(IInputs inputs)
        {
            if (_nextSupplier != null)
            {
                _nextSupplier.Process((TInputs)inputs);
                return;
            }
            _nextSupplier = _broker.CreateSupplierChannel<TInputs>(_descriptor.Next);
            _nextSupplier.Process((TInputs)inputs);
        }
        public void Next(IInputs inputs, IServiceProvider provider)
        {
            if (provider == _broker.Provider)
            {
                Next(inputs);
                return;
            }
            var newBroker = provider.CreateBroker(CorrelationId);
            newBroker.CreateSupplierChannel<TInputs>(_descriptor.Next).Process((TInputs)inputs);
        }
    }
    internal sealed class EDecoratorContext<TEInputs> :
        BaseSupplierContext<TEInputs, IEDecorator<TEInputs>>,
        IESupplierChannel<TEInputs>,
        IEDecoratorContext
        where TEInputs : IEInputs
    {
        private IESupplierChannel<TEInputs> _nextSupplier;
        internal EDecoratorContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
            : base(broker, descriptor) { }
        public Task ProcessAsync(TEInputs inputs, CancellationToken cancellationToken = default) => _instance.ProcessAsync(inputs, this, cancellationToken);
        public Task Next(IEInputs inputs, CancellationToken cancellationToken = default)
        {
            if (_nextSupplier != null)
                return _nextSupplier.ProcessAsync((TEInputs)inputs, cancellationToken);
            _nextSupplier = _broker.CreateESupplierChannel<TEInputs>(_descriptor.Next);
            return _nextSupplier.ProcessAsync((TEInputs)inputs, cancellationToken);
        }
        public Task Next(IEInputs inputs, IServiceProvider provider, CancellationToken cancellationToken = default)
        {
            if (provider == _broker.Provider)
                return Next(inputs, cancellationToken);
            var newBroker = provider.CreateBroker(CorrelationId);
            return newBroker.CreateESupplierChannel<TEInputs>(_descriptor.Next).ProcessAsync((TEInputs)inputs, cancellationToken);
        }
    }
}

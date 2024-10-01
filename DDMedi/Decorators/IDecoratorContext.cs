using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public interface IAsyncDecoratorContext<TOutput> : ISupplierContext
    {
        Task<TOutput> Next<TInputs>(TInputs inputs, CancellationToken cancellationToken = default) where TInputs : class;
        Task<TOutput> Next<TInputs>(TInputs inputs, ISupplierScope scope, CancellationToken cancellationToken = default) where TInputs : class;
    }
    public interface IAsyncDecoratorContext : ISupplierContext
    {
        Task Next<TInputs>(TInputs inputs, CancellationToken cancellationToken = default) where TInputs : class;
        Task Next<TInputs>(TInputs inputs, ISupplierScope scope, CancellationToken cancellationToken = default) where TInputs : class;
    }
    public interface IDecoratorContext<TOutput> : ISupplierContext
    {
        TOutput Next<TInputs>(TInputs inputs) where TInputs : class;
        TOutput Next<TInputs>(TInputs inputs, ISupplierScope scope) where TInputs : class;
    }
    public interface IDecoratorContext : ISupplierContext
    {
        void Next<TInputs>(TInputs inputs) where TInputs : class;
        void Next<TInputs>(TInputs inputs, ISupplierScope scope) where TInputs : class;
    }
    public interface IEDecoratorContext : ISupplierContext
    {
        Task Next(IEInputs inputs, CancellationToken cancellationToken = default);
        Task Next(IEInputs inputs, ISupplierScope scope, CancellationToken cancellationToken = default);
    }
    internal sealed class AsyncDecoratorContext<TInputs, TOutput> :
        BaseSupplierContext<IAsyncDecorator<TInputs, TOutput>>,
        IAsyncSupplierChannel<TInputs, TOutput>,
        IAsyncDecoratorContext<TOutput>
        where TInputs : class
    {
        private IAsyncSupplierChannel<TInputs, TOutput> _nextSupplier;
        internal AsyncDecoratorContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
            : base(broker, descriptor) { }
        public Task<TOutput> ProcessAsync(TInputs inputs, CancellationToken cancellationToken = default) => _instance.ProcessAsync(inputs, this, cancellationToken);
        public Task<TOutput> Next<Inputs>(Inputs inputs, CancellationToken cancellationToken = default) where Inputs : class
        {
            if (_nextSupplier != null)
                return _nextSupplier.ProcessAsync(inputs as TInputs, cancellationToken);
            _nextSupplier = _broker.CreateAsyncSupplierChannel<TInputs, TOutput>(_descriptor.Next);
            return _nextSupplier.ProcessAsync(inputs as TInputs, cancellationToken);
        }
        public Task<TOutput> Next<Inputs>(Inputs inputs, ISupplierScope scope, CancellationToken cancellationToken = default) where Inputs : class
        {
            if (scope.ServiceProvider == _broker.Provider)
                return Next(inputs, cancellationToken);
            var newBroker = scope.ServiceProvider.GetService<IInternalDDBroker>();
            return newBroker.CreateAsyncSupplierChannel<TInputs, TOutput>(_descriptor.Next).ProcessAsync(inputs as TInputs, cancellationToken);
        }
    }
    internal sealed class AsyncDecoratorContext<TInputs> :
        BaseSupplierContext<IAsyncDecorator<TInputs>>,
        IAsyncSupplierChannel<TInputs>,
        IAsyncDecoratorContext
        where TInputs : class
    {
        private IAsyncSupplierChannel<TInputs> _nextSupplier;
        internal AsyncDecoratorContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
            : base(broker, descriptor) { }
        public Task ProcessAsync(TInputs inputs, CancellationToken cancellationToken = default) => _instance.ProcessAsync(inputs, this, cancellationToken);
        public Task Next<Inputs>(Inputs inputs, CancellationToken cancellationToken = default) where Inputs : class
        {
            if (_nextSupplier != null)
                return _nextSupplier.ProcessAsync(inputs as TInputs, cancellationToken);
            _nextSupplier = _broker.CreateAsyncSupplierChannel<TInputs>(_descriptor.Next);
            return _nextSupplier.ProcessAsync(inputs as TInputs, cancellationToken);
        }
        public Task Next<Inputs>(Inputs inputs, ISupplierScope scope, CancellationToken cancellationToken = default) where Inputs : class
        {
            if (scope.ServiceProvider == _broker.Provider)
                return Next(inputs, cancellationToken);
            var newBroker = scope.ServiceProvider.GetService<IInternalDDBroker>();
            return newBroker.CreateAsyncSupplierChannel<TInputs>(_descriptor.Next).ProcessAsync(inputs as TInputs, cancellationToken);
        }
    }
    internal sealed class DecoratorContext<TInputs, TOutput> :
        BaseSupplierContext<IDecorator<TInputs, TOutput>>,
        ISupplierChannel<TInputs, TOutput>,
        IDecoratorContext<TOutput>
        where TInputs : class
    {
        private ISupplierChannel<TInputs, TOutput> _nextSupplier;
        internal DecoratorContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
            : base(broker, descriptor) { }
        public TOutput Process(TInputs inputs) => _instance.Process(inputs, this);
        public TOutput Next<Inputs>(Inputs inputs) where Inputs : class
        {
            if (_nextSupplier != null)
                return _nextSupplier.Process(inputs as TInputs);
            _nextSupplier = _broker.CreateSupplierChannel<TInputs, TOutput>(_descriptor.Next);
            return _nextSupplier.Process(inputs as TInputs);
        }
        public TOutput Next<Inputs>(Inputs inputs, ISupplierScope scope) where Inputs : class
        {
            if (scope.ServiceProvider == _broker.Provider)
                return Next(inputs);
            var newBroker = scope.ServiceProvider.GetService<IInternalDDBroker>();
            return newBroker.CreateSupplierChannel<TInputs, TOutput>(_descriptor.Next).Process(inputs as TInputs);
        }
    }
    internal sealed class DecoratorContext<TInputs> :
        BaseSupplierContext<IDecorator<TInputs>>,
        ISupplierChannel<TInputs>,
        IDecoratorContext
        where TInputs : class
    {
        private ISupplierChannel<TInputs> _nextSupplier;
        internal DecoratorContext(IInternalDDBroker broker, SupplierDescriptor descriptor)
            : base(broker, descriptor) { }
        public void Process(TInputs inputs) => _instance.Process(inputs, this);
        public void Next<Inputs>(Inputs inputs) where Inputs : class
        {
            if (_nextSupplier != null)
            {
                _nextSupplier.Process(inputs as TInputs);
                return;
            }
            _nextSupplier = _broker.CreateSupplierChannel<TInputs>(_descriptor.Next);
            _nextSupplier.Process(inputs as TInputs);
        }
        public void Next<Inputs>(Inputs inputs, ISupplierScope scope) where Inputs : class
        {
            if (scope.ServiceProvider == _broker.Provider)
            {
                Next(inputs);
                return;
            }
            var newBroker = scope.ServiceProvider.GetService<IInternalDDBroker>();
            newBroker.CreateSupplierChannel<TInputs>(_descriptor.Next).Process(inputs as TInputs);
        }
    }
    internal sealed class EDecoratorContext<TEInputs> :
        BaseSupplierContext<IEDecorator<TEInputs>>,
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
        public Task Next(IEInputs inputs, ISupplierScope scope, CancellationToken cancellationToken = default)
        {
            if (scope.ServiceProvider == _broker.Provider)
                return Next(inputs, cancellationToken);
            var newBroker = scope.ServiceProvider.GetService<IInternalDDBroker>();
            return newBroker.CreateESupplierChannel<TEInputs>(_descriptor.Next).ProcessAsync((TEInputs)inputs, cancellationToken);
        }
    }
}

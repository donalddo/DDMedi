using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public interface IDDBroker
    {
        string CorrelationId { get; }

        Task<TOutput> ProcessAsync<TInputs, TOutput>(TInputs inputs = default, CancellationToken token = default) where TInputs : IInputs<TOutput>;
        Task ProcessAsync<TInputs>(TInputs inputs = default, CancellationToken token = default) where TInputs : IInputs;
        TOutput Process<TInputs, TOutput>(TInputs inputs = default) where TInputs : IInputs<TOutput>;
        void Process<TInputs>(TInputs inputs = default) where TInputs : IInputs;

        IAsyncSupplierChannel<TInputs, TOutput> CreateAsyncSupplierChannel<TInputs, TOutput>() where TInputs : IInputs<TOutput>;
        IAsyncSupplierChannel<TInputs> CreateAsyncSupplierChannel<TInputs>() where TInputs : IInputs;
        ISupplierChannel<TInputs, TOutput> CreateSupplierChannel<TInputs, TOutput>() where TInputs : IInputs<TOutput>;
        ISupplierChannel<TInputs> CreateSupplierChannel<TInputs>() where TInputs : IInputs;

        Task Publish(IEInputs inputs = default, CancellationToken token = default);
        Task Publish<TEInputs>(TEInputs inputs = default, CancellationToken token = default) where TEInputs : IEInputs;
    }
    internal interface IInternalDDBroker: IDDBroker
    {
        string CorrelationId { get; set; }
        IServiceProvider Provider { get; }
    }
    internal class DDBroker : IInternalDDBroker
    {
        public IServiceProvider Provider { get; }
        readonly IInternalSupplierFactory _supplierFactory;
        readonly IInternalEInputsQueueFactory _queueFactory;
        public string CorrelationId { get; set; }
        public DDBroker(IServiceProvider provider, DDMediFactory factory): this(provider, factory, Guid.NewGuid().ToString()) { }
        internal DDBroker(IServiceProvider provider, DDMediFactory factory, string correlationId)
        {
            Provider = provider ?? throw new ArgumentNullException();
            if (factory == null) throw new ArgumentNullException();
            _supplierFactory = factory.SupplierFactory;
            _queueFactory = factory.EInputsQueueFactory;
            CorrelationId = correlationId;
        }

        public Task<TOutput> ProcessAsync<TInputs, TOutput>(TInputs inputs = default, CancellationToken token = default) where TInputs : IInputs<TOutput>
        => _supplierFactory.CreateAsyncSupplierChannel<TInputs, TOutput>(this).ProcessAsync(inputs, token);
        public Task ProcessAsync<TInputs>(TInputs inputs = default, CancellationToken token = default) where TInputs : IInputs
        => _supplierFactory.CreateAsyncSupplierChannel<TInputs>(this).ProcessAsync(inputs, token);
        public TOutput Process<TInputs, TOutput>(TInputs inputs = default) where TInputs : IInputs<TOutput>
        => _supplierFactory.CreateSupplierChannel<TInputs, TOutput>(this).Process(inputs);
        public void Process<TInputs>(TInputs inputs = default) where TInputs : IInputs
        => _supplierFactory.CreateSupplierChannel<TInputs>(this).Process(inputs);

        public IAsyncSupplierChannel<TInputs, TOutput> CreateAsyncSupplierChannel<TInputs, TOutput>() where TInputs : IInputs<TOutput>
        => _supplierFactory.CreateAsyncSupplierChannel<TInputs, TOutput>(this);
        public IAsyncSupplierChannel<TInputs> CreateAsyncSupplierChannel<TInputs>() where TInputs : IInputs
        => _supplierFactory.CreateAsyncSupplierChannel<TInputs>(this);
        public ISupplierChannel<TInputs, TOutput> CreateSupplierChannel<TInputs, TOutput>() where TInputs : IInputs<TOutput>
        => _supplierFactory.CreateSupplierChannel<TInputs, TOutput>(this);
        public ISupplierChannel<TInputs> CreateSupplierChannel<TInputs>() where TInputs : IInputs
        => _supplierFactory.CreateSupplierChannel<TInputs>(this);

        public Task Publish(IEInputs inputs = default, CancellationToken token = default)
        {
            if (inputs == null) throw new ArgumentNullException("Cannot get EInputs type to Publish!!");
            return Publish(inputs.GetType(), inputs, token);
        }
        public Task Publish<TEInputs>(TEInputs inputs = default, CancellationToken token = default) where TEInputs : IEInputs
        => Publish(typeof(TEInputs), inputs, token);
        Task Publish(Type eInputsType, IEInputs inputs, CancellationToken token = default)
        {
            _queueFactory.QueueItem(eInputsType, inputs, CorrelationId);
            return Task.CompletedTask;
        }
    }

}

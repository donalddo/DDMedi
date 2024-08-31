using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    internal class EInputsModel
    {
        internal EInputsModel(Type eInputsType, IEInputs inputs, string correlationId, CancellationToken cancellationToken)
        {
            EInputsType = eInputsType;
            Inputs = inputs;
            CorrelationId = correlationId;
            CancellationToken = cancellationToken;
        }

        public CancellationToken CancellationToken { get; }
        internal Type EInputsType { get; }
        internal IEInputs Inputs { get; }
        internal string CorrelationId { get; }
    }
    internal sealed class EInputsQueue
    {
        private ConcurrentQueue<EInputsModel> _queue = new ConcurrentQueue<EInputsModel>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);
        public void QueueItem(Type eInputsType, IEInputs inputs, string correlationId, CancellationToken cancellationToken = default)
        {
            _queue.Enqueue(new EInputsModel(eInputsType, inputs, correlationId, cancellationToken));
            _signal.Release();
        }
        public void QueueItem<T>(T inputs, string correlationId, CancellationToken cancellationToken = default) where T : IEInputs =>
            QueueItem(typeof(T), inputs, correlationId, cancellationToken);

        public async Task<EInputsModel> DequeueAsync(CancellationToken cancellationToken = default)
        {
            await _signal.WaitAsync(cancellationToken);
            _queue.TryDequeue(out var eInputsModel);
            return eInputsModel;
        }

    }
    public class EInputsQueueDescriptor : IDisposable
    {
        internal Dictionary<Type, SupplierDescriptor[]> Descriptors { get; private set; }
        internal int NumberOfExecutors { get; }
        internal EInputsQueue Queue { get; private set; }
        internal int QueueNum { get; }
        internal EInputsQueueDescriptor(int queueNum, Dictionary<Type, SupplierDescriptor[]> descriptors, int numberOfExecutors = 1)
        {
            Queue = new EInputsQueue();
            Descriptors = descriptors;
            NumberOfExecutors = numberOfExecutors;
            QueueNum = queueNum;
        }

        public void Dispose()
        {
            Descriptors = null;
            Queue = null;
        }
    }
    public interface IEInputsQueueFactory
    {
    }
    internal interface IInternalEInputsQueueFactory : IEInputsQueueFactory, IDisposable
    {
        void QueueItem(Type eInputsType, IEInputs inputs, string correlationId);
        Task StartAsync(ISupplierScopeFactory supplierScopeFactory, CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
    internal class EInputsQueueFactory : BaseFactory, IInternalEInputsQueueFactory
    {
        private ISupplierScopeFactory _serviceScopeFactory;
        private Dictionary<Type, EInputsQueueDescriptor[]> _descriptorDic;
        private List<EInputsQueueDescriptor> _descriptors;
        private EInputsQueueDescriptor[] _exceptionDescriptors;
        private readonly CancellationTokenSource _stoppingCts;
        private List<Task> _executors;
        private Task _executingTask;
        private Dictionary<Type, Func<SupplierDescriptor, EInputsModel, Task>> _createAndInvokeProcessAsyncMethods;
        internal DDMediFactory DDMediFactory { get; set; }
        internal EInputsQueueFactory() : base(TypeConstant.IGeneralESupplierType)
        {
            _descriptorDic = new Dictionary<Type, EInputsQueueDescriptor[]>();
            _descriptors = new List<EInputsQueueDescriptor>();
            _stoppingCts = new CancellationTokenSource();
            _executors = new List<Task>();
            _createAndInvokeProcessAsyncMethods = new Dictionary<Type, Func<SupplierDescriptor, EInputsModel, Task>>();
        }
        static readonly MethodInfo GenericCreateAndInvokeProcessAsyncMethod =
            typeof(EInputsQueueFactory).GetMethod(nameof(PrivateCreateAndInvokeProcessAsync), BindingFlags.NonPublic | BindingFlags.Instance);
        private async Task PrivateCreateAndInvokeProcessAsync<TEInputs>(SupplierDescriptor eSupplierDescriptor, EInputsModel eInputsModel)
            where TEInputs : class, IEInputs
        {
            using (var newScope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    var ddBroker = newScope.ServiceProvider.CreateBroker(eInputsModel.CorrelationId);
                    await ddBroker.CreateESupplierChannel<TEInputs>(eSupplierDescriptor)
                        .ProcessAsync(eInputsModel.Inputs as TEInputs, eInputsModel.CancellationToken);
                }
                catch (Exception ex)
                {
                    var exceptionEInputs = new ExceptionEInputs(ex, eInputsModel.Inputs);
                    _exceptionDescriptors.QueueItem(exceptionEInputs, eInputsModel.CorrelationId, eInputsModel.CancellationToken);
                }
            }
        }
        EInputsQueueDescriptor CreateOrGet(IInternalESupplierCollection eSupplierCollection)
        {
            foreach(var descriptor in _descriptors)
                if (descriptor.QueueNum == eSupplierCollection.QueueNum)
                    return descriptor;
            var descriptorDic = new Dictionary<Type,SupplierDescriptor[]>();
            var supplierDescriptorDic = SupplierDescriptorDic[TypeConstant.IGenericESupplierType];
            foreach (var pair in eSupplierCollection.TypeCollections)
            {
                var eInputsType = pair.Key.GetGenericArguments()[0];
                var cachedSupplierDescriptors = new SupplierDescriptor[pair.Value.Count];
                var supplierDescriptors = supplierDescriptorDic[eInputsType];
                var i = 0;
                foreach (var implementDescriptor in pair.Value)
                    foreach (var supplierDescriptor in supplierDescriptors)
                        if (supplierDescriptor.ImplementDescriptor.RegisterType == implementDescriptor.ImplementType)
                        {
                            cachedSupplierDescriptors[i++] = supplierDescriptor;
                            break;
                        }
                descriptorDic[eInputsType] = cachedSupplierDescriptors;
            }
            var newDescriptor = new EInputsQueueDescriptor(eSupplierCollection.QueueNum, descriptorDic, eSupplierCollection.NumberOfExecutors);
            _descriptors.Add(newDescriptor);
            return newDescriptor;
        }
        internal void AddQueues(Dictionary<Type, List<IInternalESupplierCollection>> eSupplierCollectionDic)
        {
            foreach(var pair in eSupplierCollectionDic)
            {
                var eInputsType = pair.Key.GetGenericArguments()[0];
                var descriptors = new EInputsQueueDescriptor[pair.Value.Count];
                var i = 0;
                foreach (var eSupplierCollection in pair.Value)
                    descriptors[i++] = CreateOrGet(eSupplierCollection);
                _descriptorDic[eInputsType] = descriptors;
            }
        }
        public void QueueItem(Type eInputsType, IEInputs inputs, string correlationId)
        {
            var descriptors = _descriptorDic.GetValueOrDefault(eInputsType);
            if (descriptors != null)
            {
                var token = _stoppingCts.Token;
                foreach (var descriptor in descriptors)
                    descriptor.Queue.QueueItem(eInputsType, inputs, correlationId, token);
            }
        }
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public Task StartAsync(ISupplierScopeFactory supplierScopeFactory, CancellationToken cancellationToken = default)
        {
            _serviceScopeFactory = supplierScopeFactory ?? throw new ArgumentNullException();
            foreach (var pair in _descriptorDic)
                _createAndInvokeProcessAsyncMethods[pair.Key] =
                    Delegate.CreateDelegate(typeof(Func<SupplierDescriptor, EInputsModel, Task>), this,
                        GenericCreateAndInvokeProcessAsyncMethod.MakeGenericMethod(pair.Key))
                        as Func<SupplierDescriptor, EInputsModel, Task>;
            _exceptionDescriptors = _descriptorDic.GetValueOrDefault(TypeConstant.ExceptionEInputType);
            foreach (var descriptor in _descriptors)
                for(var i = 0; i < descriptor.NumberOfExecutors; i++)
                    _executors.Add(ExecuteAsync(descriptor, _stoppingCts.Token));
            _executingTask = Task.WhenAll(_executors);
            if (_executingTask.IsCompleted)
                return _executingTask;
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _stoppingCts.Cancel();
            _createAndInvokeProcessAsyncMethods = null;
            _descriptorDic = null;
            _descriptors = null;
            _exceptionDescriptors = null;
            _executors = null;
            base.Dispose();
        }
        protected async Task ExecuteAsync(EInputsQueueDescriptor queueDescriptor, CancellationToken stoppingToken)
        {
            if (queueDescriptor == null || queueDescriptor.Queue == null || queueDescriptor.Descriptors.Count == 0)
                return;
            var createAndInvokeProcessAsyncMethods = new Dictionary<Type, Func<SupplierDescriptor, EInputsModel, Task>>();
            foreach(var pair in queueDescriptor.Descriptors)
                createAndInvokeProcessAsyncMethods[pair.Key] = _createAndInvokeProcessAsyncMethods[pair.Key];
            while (!stoppingToken.IsCancellationRequested)
            {
                var eInputsModel = await queueDescriptor.Queue.DequeueAsync(stoppingToken);
                var eSupplierDescriptors = queueDescriptor.Descriptors.GetValueOrDefault(eInputsModel.EInputsType);
                var createAndInvokeProcessAsyncMethod = createAndInvokeProcessAsyncMethods[eInputsModel.EInputsType];
                var tasks = new Task[eSupplierDescriptors.Length];
                var i = 0;
                foreach(var eSupplierDescriptor in eSupplierDescriptors)
                    tasks[i++] = createAndInvokeProcessAsyncMethod.Invoke(eSupplierDescriptor, eInputsModel);
                await Task.WhenAll(tasks);
            }
        }
    }
}

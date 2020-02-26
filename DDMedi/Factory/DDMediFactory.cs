using System;

namespace DDMedi
{
    public sealed class DDMediFactory : IDisposable
    {
        public IBaseDescriptorCollection BaseDescriptorCollection => BasicDescriptorCollection;
        internal IInternalBaseDescriptorCollection BasicDescriptorCollection { get; }
        internal IInternalSupplierFactory SupplierFactory { get; }
        internal IInternalEInputsQueueFactory EInputsQueueFactory { get; }

        private ISupplierScopeFactory _scopeFactory;

        internal DDMediFactory(
            IInternalBaseDescriptorCollection baseDescriptorCollection,
            IInternalSupplierFactory supplierfactory,
            IInternalEInputsQueueFactory queueFactory)
        {
            BasicDescriptorCollection = baseDescriptorCollection ?? throw new ArgumentNullException();
            SupplierFactory = supplierfactory ?? throw new ArgumentNullException();
            EInputsQueueFactory = queueFactory ?? throw new ArgumentNullException();
        }
        public DDMediFactory SetScopeFactory(ISupplierScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            EInputsQueueFactory.StartAsync(scopeFactory);
            return this;
        }
        
        public void Dispose()
        {
            _scopeFactory = null;
            BasicDescriptorCollection.Dispose();
            SupplierFactory.Dispose();
            EInputsQueueFactory.Dispose();
        }

    }
}

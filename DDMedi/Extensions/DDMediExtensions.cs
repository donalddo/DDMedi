using System;

namespace DDMedi
{
    public static class DDMediExtensions
    {
        public static DDMediFactory BuildSuppliers(this DDMediCollection ddCollection)
        {
            var baseDescriptorCollection = new BaseDescriptorCollection();
            var supplierFactory = ddCollection.BuildSupplierFactory(baseDescriptorCollection);
            var queueFactory = ddCollection.BuildEInputsQueuesFactory(baseDescriptorCollection);
            ddCollection.WrapAllDecorators(supplierFactory, baseDescriptorCollection);
            ddCollection.WrapAllDecorators(queueFactory, baseDescriptorCollection);
            var ddFactory = new DDMediFactory(
                baseDescriptorCollection,
                supplierFactory,
                queueFactory);
            queueFactory.DDMediFactory = ddFactory;
            baseDescriptorCollection.AddDDMediFactory(ddFactory);
            baseDescriptorCollection.AddIDDBroker();
            return ddFactory;
        }

        public static ISupplierScope CreateScope(this IDDBroker broker)
        {
            var factory = broker.Provider.GetService(typeof(ISupplierScopeFactory)) as ISupplierScopeFactory;
            return factory.CreateScope(broker.CorrelationId);
        }

        public static ISupplierScope CreateScope(this ISupplierScopeFactory factory, string correlationId)
        {
            var newScope = factory.CreateScope();
            var newBroker = newScope.ServiceProvider.GetService<IInternalDDBroker>();
            newBroker.CorrelationId = correlationId;
            return newScope;
        }
    }
}

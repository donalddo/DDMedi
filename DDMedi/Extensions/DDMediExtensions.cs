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
    }
}

using System;
using System.Collections.Generic;

namespace DDMedi
{
    public sealed class DDMediCollection : IDisposable
    {
        internal SupplierCollection SupplierCollection { get; }
        internal EInputsQueueCollection EInputsQueueCollection { get; }
        internal DecoratorCollection DecoratorCollection { get; }
        public DDMediCollection()
        {
            SupplierCollection = new SupplierCollection(TypeConstant.IGeneralSupplierType);
            var eSupplierCollection = new SupplierCollection(TypeConstant.IGeneralESupplierType);
            EInputsQueueCollection = new EInputsQueueCollection(eSupplierCollection);
            DecoratorCollection = new DecoratorCollection(TypeConstant.IGeneralSupplierType, TypeConstant.IGeneralESupplierType);
        }
        static void BuildBaseFactory(BaseFactory factory, ISupplierCollection supplierCollection, BaseDescriptorCollection baseDescriptorCollection)
        {
            foreach (var genericPair in supplierCollection.TypeCollections)
            {
                foreach (var pair in genericPair.Value)
                {
                    var baseDescriptors = new BaseDescriptor[pair.Value.Count];
                    var i = 0;
                    foreach (var implementDescriptor in pair.Value)
                        baseDescriptors[i++] =
                            baseDescriptorCollection.Create(implementDescriptor.ImplementType, implementDescriptor.Lifetime);
                    factory.AddSuppliers(genericPair.Key, pair.Key, baseDescriptors);
                }
            }

        }
        internal SupplierFactory BuildSupplierFactory(BaseDescriptorCollection BaseDescriptorCollection)
        {
            var factory = new SupplierFactory();
            BuildBaseFactory(factory, SupplierCollection, BaseDescriptorCollection);
            return factory;
        }
        internal EInputsQueueFactory BuildEInputsQueuesFactory(BaseDescriptorCollection BaseDescriptorCollection)
        {
            var factory = new EInputsQueueFactory();
            BuildBaseFactory(factory, EInputsQueueCollection.SupplierCollection, BaseDescriptorCollection);
            factory.AddQueues(EInputsQueueCollection.ESupplierCollectionDic);
            return factory;
        }
        internal void WrapAllDecorators(BaseFactory factory, BaseDescriptorCollection BaseDescriptorCollection)
        {
            var implementTypeDic = DecoratorCollection.TypeCollection[factory.IGeneralSupplierType];
            foreach (var decoratorDescriptor in implementTypeDic)
            {
                var iGenericSupplierType = decoratorDescriptor.ISupplierType.GetGenericTypeDefinition();
                if (!decoratorDescriptor.IsGeneric)
                    WrapDecorators(factory.SupplierDescriptorDic[iGenericSupplierType], decoratorDescriptor, BaseDescriptorCollection);
                else
                    WrapGenericDecorators(factory.SupplierDescriptorDic, decoratorDescriptor, BaseDescriptorCollection);
            }
        }
        DecoratorInfo CreateDecoratorInfo(Type iSupplierType, Type implementType, SupplierDescriptor wrappedDescriptor)
        {
            var layers = new List<Type>();
            do
            {
                layers.Add(wrappedDescriptor.ImplementDescriptor.RegisterType);
                wrappedDescriptor = wrappedDescriptor.Next;
            } while (wrappedDescriptor != null) ;
            return new DecoratorInfo(iSupplierType, implementType, layers, layers.Count > 1);
        }
        void WrapDecorators(Type implementType, BaseDescriptorCollection BaseDescriptorCollection,
            IReadOnlyList<SupplierDescriptor> wrappedDescriptors, Func<IDecoratorInfo, bool> condition)
        {
            if (wrappedDescriptors == null || wrappedDescriptors.Count == 0) return;
            for (var i = 0; i < wrappedDescriptors.Count; i++)
            {
                var oldDescriptor = wrappedDescriptors[i];
                oldDescriptor.CheckDuplicateDecorator(implementType);
                if (condition != null &&
                    !condition(CreateDecoratorInfo(oldDescriptor.ISupplierType, implementType, oldDescriptor)))
                    continue;
                var newDescriptor = new SupplierDescriptor
                (
                    oldDescriptor.ISupplierType,
                    BaseDescriptorCollection.Create(implementType, oldDescriptor.ImplementDescriptor.Lifetime)
                );
                newDescriptor.Link(oldDescriptor);
            }
        }
        void WrapDecorators(Dictionary<Type, SupplierDescriptor[]> supplierDescriptorDic, DecoratorDescriptor decoratorDescriptor, BaseDescriptorCollection BaseDescriptorCollection)
        {
            var inputsType = decoratorDescriptor.ISupplierType.GetGenericArguments()[0];
            var wrappedDescriptors = supplierDescriptorDic.GetValueOrDefault(inputsType) ??
                throw new InvalidOperationException($"{decoratorDescriptor.ISupplierType.FullName} is not registered");
            WrapDecorators(decoratorDescriptor.ImplementType, BaseDescriptorCollection,
                wrappedDescriptors, decoratorDescriptor.Condition);
        }
        void WrapGenericDecorators(Dictionary<Type, Dictionary<Type, SupplierDescriptor[]>> supplierDescriptorDic, DecoratorDescriptor decoratorDescriptor, BaseDescriptorCollection BaseDescriptorCollection)
        {
            var wrappedDescriptors = supplierDescriptorDic[decoratorDescriptor.ISupplierType];
            foreach (var pair in wrappedDescriptors)
                WrapDecorators(decoratorDescriptor.ImplementType.MakeGenericType(pair.Value[0].ISupplierType.GetGenericArguments()),
                    BaseDescriptorCollection, pair.Value, decoratorDescriptor.Condition);
        }
        public void Dispose()
        {
            SupplierCollection.Dispose();
            EInputsQueueCollection.Dispose();
            DecoratorCollection.Dispose();
        }

    }
}

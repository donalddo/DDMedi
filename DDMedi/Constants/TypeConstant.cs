using System;
using System.Collections.Generic;

namespace DDMedi
{
    internal static class TypeConstant
    {
        internal readonly static Type IGenericAsyncSupplierOutputType = typeof(IAsyncSupplier<,>);
        internal readonly static Type IGenericAsyncSupplierType = typeof(IAsyncSupplier<>);
        internal readonly static Type IGenericSupplierOutputType = typeof(ISupplier<,>);
        internal readonly static Type IGenericSupplierType = typeof(ISupplier<>);
        internal readonly static IReadOnlyList<Type> IGenericSupplierTypes = new Type[]
        {
            IGenericAsyncSupplierOutputType,
            IGenericAsyncSupplierType,
            IGenericSupplierOutputType,
            IGenericSupplierType
        };
        internal readonly static Type IGeneralSupplierType = typeof(ISupplier);

        internal readonly static Type IGenericESupplierType = typeof(IESupplier<>);
        internal readonly static IReadOnlyList<Type> IGenericESupplierTypes = new Type[]
        {
            IGenericESupplierType
        };
        internal readonly static Type IGeneralESupplierType = typeof(IESupplier);

        internal readonly static IReadOnlyDictionary<Type, IReadOnlyList<Type>> IGenericSupplierTypeDic =
        new Dictionary<Type, IReadOnlyList<Type>>
        {
            [IGeneralSupplierType] = IGenericSupplierTypes,
            [IGeneralESupplierType] = IGenericESupplierTypes
        };

        internal readonly static Type IGeneralDecoratorType = typeof(IDecorator);
        internal readonly static Type IGenericAsyncDecoratorOutputType = typeof(IAsyncDecorator<,>);
        internal readonly static Type IGenericAsyncDecoratorType = typeof(IAsyncDecorator<>);
        internal readonly static Type IGenericDecoratorOutputType = typeof(IDecorator<,>);
        internal readonly static Type IGenericDecoratorType = typeof(IDecorator<>);
        internal readonly static Type IGenericEDecoratorType = typeof(IEDecorator<>);
        internal readonly static IReadOnlyDictionary<Type, Type> MappedSupplierToDecorator = new Dictionary<Type, Type>
        {
            [IGenericAsyncSupplierOutputType] = IGenericAsyncDecoratorOutputType,
            [IGenericAsyncSupplierType] = IGenericAsyncDecoratorType,
            [IGenericESupplierType] = IGenericEDecoratorType,
            [IGenericSupplierOutputType] = IGenericDecoratorOutputType,
            [IGenericSupplierType] = IGenericDecoratorType
        };

        internal readonly static Type IDDBrokerType = typeof(IDDBroker);
        internal readonly static Type SupplierDescriptorType = typeof(SupplierDescriptor);
        internal readonly static Type IGeneralEInputType = typeof(IEInputs);
        internal readonly static Type ExceptionEInputType = typeof(ExceptionEInputs);
    }
}

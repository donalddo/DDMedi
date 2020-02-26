using System;
using System.Collections.Generic;
using System.Linq;

namespace DDMedi
{
    public interface IDecoratorInfo
    {
        bool IsDecoratorApplied { get; }
        Type ISupplierType { get; }
        Type DecoratorImplementType { get; }
        IReadOnlyList<Type> AppliedLayers { get; }
    }
    internal class DecoratorInfo : IDecoratorInfo
    {
        public DecoratorInfo(Type iSupplierType, Type decoratorImplementType, IReadOnlyList<Type> appliedLayers, bool isDecoratorApplied)
        {
            ISupplierType = iSupplierType;
            DecoratorImplementType = decoratorImplementType;
            IsDecoratorApplied = isDecoratorApplied;
            AppliedLayers = appliedLayers;
        }

        public bool IsDecoratorApplied { get; }

        public Type ISupplierType { get; }
        public IReadOnlyList<Type> AppliedLayers { get; }

        public Type DecoratorImplementType { get; }
    }
    internal class DecoratorDescriptor
    {
        internal DecoratorDescriptor(Type iSupplierType, Type decoratorImplementType, bool isGeneric, Func<IDecoratorInfo, bool> condition)
        {
            ISupplierType = iSupplierType;
            ImplementType = decoratorImplementType;
            Condition = condition;
            IsGeneric = isGeneric;
        }

        internal Type ISupplierType { get; }

        internal Type ImplementType { get; }
        internal Func<IDecoratorInfo, bool> Condition { get; }
        internal bool IsGeneric { get; }
    }
    internal interface IDecoratorCollection : IDisposable
    {
        IDecoratorCollection RegisterAsyncDecorator<TInputs, TOutput, TDecorator>(Func<IDecoratorInfo, bool> condition = null)
            where TInputs : IInputs<TOutput>
            where TDecorator : IAsyncDecorator<TInputs, TOutput>;
        IDecoratorCollection RegisterAsyncDecorator<TInputs, TDecorator>(Func<IDecoratorInfo, bool> condition = null)
            where TInputs : IInputs
            where TDecorator : IAsyncDecorator<TInputs>;
        IDecoratorCollection RegisterDecorator<TInputs, TOutput, TDecorator>(Func<IDecoratorInfo, bool> condition = null)
            where TInputs : IInputs<TOutput>
            where TDecorator : IDecorator<TInputs, TOutput>;
        IDecoratorCollection RegisterDecorator<TInputs, TDecorator>(Func<IDecoratorInfo, bool> condition = null)
            where TInputs : IInputs
            where TDecorator : IDecorator<TInputs>;
        IDecoratorCollection RegisterEDecorator<TEInput, TDecorator>(Func<IDecoratorInfo, bool> condition = null)
            where TEInput : IEInputs
            where TDecorator : IEDecorator<TEInput>;
        IDecoratorCollection RegisterGenericDecorator(Type iGenericSupplierType, Type genericImplementType, Func<IDecoratorInfo, bool> condition = null);
    }

    internal class DecoratorCollection : IDecoratorCollection
    {
        internal Dictionary<Type, List<DecoratorDescriptor>> TypeCollection { get; private set; }
        internal DecoratorCollection(params Type[] iGeneralSupplierTypes)
        {
            TypeCollection = new Dictionary<Type, List<DecoratorDescriptor>>();
            if (iGeneralSupplierTypes == null || iGeneralSupplierTypes.Length == 0)
                throw new ArgumentNullException();
            foreach (var iGeneralSupplierType in iGeneralSupplierTypes)
                TypeCollection[iGeneralSupplierType] = new List<DecoratorDescriptor>();
        }

        public IDecoratorCollection RegisterAsyncDecorator<TInputs, TOutput, TDecorator>(Func<IDecoratorInfo, bool> condition)
            where TInputs : IInputs<TOutput>
            where TDecorator : IAsyncDecorator<TInputs, TOutput> =>
            CheckAndAddDecorator(TypeConstant.IGeneralSupplierType, typeof(IAsyncSupplier<TInputs, TOutput>), typeof(TDecorator), condition);
        public IDecoratorCollection RegisterAsyncDecorator<TInputs, TDecorator>(Func<IDecoratorInfo, bool> condition)
            where TInputs : IInputs
            where TDecorator : IAsyncDecorator<TInputs> =>
            CheckAndAddDecorator(TypeConstant.IGeneralSupplierType, typeof(IAsyncSupplier<TInputs>), typeof(TDecorator), condition);
        public IDecoratorCollection RegisterDecorator<TInputs, TOutput, TDecorator>(Func<IDecoratorInfo, bool> condition)
            where TInputs : IInputs<TOutput>
            where TDecorator : IDecorator<TInputs, TOutput> =>
            CheckAndAddDecorator(TypeConstant.IGeneralSupplierType, typeof(ISupplier<TInputs, TOutput>), typeof(TDecorator), condition);
        public IDecoratorCollection RegisterDecorator<TInputs, TDecorator>(Func<IDecoratorInfo, bool> condition)
            where TInputs : IInputs
            where TDecorator : IDecorator<TInputs> =>
            CheckAndAddDecorator(TypeConstant.IGeneralSupplierType, typeof(ISupplier<TInputs>), typeof(TDecorator), condition);
        public IDecoratorCollection RegisterEDecorator<TEInputs, TEDecorator>(Func<IDecoratorInfo, bool> condition)
            where TEInputs : IEInputs
            where TEDecorator : IEDecorator<TEInputs> =>
            CheckAndAddDecorator(TypeConstant.IGeneralESupplierType, typeof(IESupplier<TEInputs>), typeof(TEDecorator), condition);
        public IDecoratorCollection RegisterGenericDecorator(Type iGenericSupplierType, Type genericImplementType, Func<IDecoratorInfo, bool> Condition = null)
        {
            if (!genericImplementType.IsGenericTypeDefinition)
                throw new ArgumentException($"{genericImplementType.Name} must be a generic definition");
            if (!genericImplementType.IsClass || genericImplementType.IsAbstract)
                throw new ArgumentException($"{genericImplementType.Name} must be a class");
            var iGeneralIDecoratorType = TypeConstant.IGeneralDecoratorType;
            if (!iGeneralIDecoratorType.IsAssignableFrom(genericImplementType))
                throw new ArgumentException($"{genericImplementType.Name} must be an IDecorator");

            var iGenericDecoratorType = TypeConstant.MappedSupplierToDecorator.GetValueOrDefault(iGenericSupplierType)
                ?? throw new ArgumentException($"{iGenericSupplierType.Name} Cannot be converted into any IDecorator");
            Type iGeneralSupplierType = iGenericSupplierType.ConvertToIGeneralSupplierType();
            var iDecoratorTypes = genericImplementType.GetInterfaces();
            if(!iDecoratorTypes.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == iGenericDecoratorType))
                throw new ArgumentException($"{genericImplementType.Name} Cannot cover {iGenericSupplierType.Name}");
            return AddDecorator(iGeneralSupplierType, iGenericSupplierType, genericImplementType, Condition, true);
        }

        public virtual void Dispose()
        {
            TypeCollection = null;
        }

        IDecoratorCollection CheckAndAddDecorator(Type iGeneralSupplierType, Type Supplier, Type implementType, Func<IDecoratorInfo, bool> condition = null)
        {
            if (!implementType.IsClass || implementType.IsAbstract)
                throw new ArgumentException($"{implementType.FullName} must be a class");
            return AddDecorator(iGeneralSupplierType, Supplier, implementType, condition);
        }
        internal bool IsEmpty() => TypeCollection.Count < 1;
        internal IDecoratorCollection AddDecorator(Type iGeneralSupplierType, Type iSupplierType, Type implementType, Func<IDecoratorInfo, bool> Condition = null, bool isGeneric = false)
        {
            var implementTypeDic = TypeCollection[iGeneralSupplierType];
            if (!implementTypeDic.Any(e => e.ImplementType == implementType && e.ISupplierType == iSupplierType))
                implementTypeDic.Add(new DecoratorDescriptor(iSupplierType, implementType, isGeneric, Condition));
            return this;
        }
    }
}

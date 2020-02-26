using System;
using System.Collections.Generic;

namespace DDMedi
{
    public sealed class SupplierDescriptor
    {
        internal SupplierDescriptor(Type iSupplierType, BaseDescriptor implementDescriptor)
        {
            ISupplierType = iSupplierType;
            ImplementDescriptor = implementDescriptor;
        }

        public Type ISupplierType { get; }
        public SupplierDescriptor Next { get; private set; }
        public BaseDescriptor ImplementDescriptor { get; private set; }
        internal void Link(SupplierDescriptor descriptor)
        {
            var temp = ImplementDescriptor;
            ImplementDescriptor = descriptor.ImplementDescriptor;
            descriptor.ImplementDescriptor = temp;

            Next = descriptor.Next;
            descriptor.Next = this;
        }
    }
    public interface ISupplierFactory
    {
    }
    internal interface IInternalSupplierFactory : ISupplierFactory, IDisposable
    {
        IAsyncSupplierChannel<T, R> CreateAsyncSupplierChannel<T, R>(IInternalDDBroker ddBroker) where T : IInputs<R>;
        IAsyncSupplierChannel<T> CreateAsyncSupplierChannel<T>(IInternalDDBroker ddBroker) where T : IInputs;
        ISupplierChannel<T, R> CreateSupplierChannel<T, R>(IInternalDDBroker ddBroker) where T : IInputs<R>;
        ISupplierChannel<T> CreateSupplierChannel<T>(IInternalDDBroker ddBroker) where T : IInputs;
    }
    internal abstract class BaseFactory : IDisposable
    {
        internal Dictionary<Type, Dictionary<Type, SupplierDescriptor[]>> SupplierDescriptorDic { get; private set; }
        internal Type IGeneralSupplierType { get; }
        internal BaseFactory(Type iGeneralSupplierType)
        {
            IGeneralSupplierType = iGeneralSupplierType;
            SupplierDescriptorDic = new Dictionary<Type, Dictionary<Type, SupplierDescriptor[]>>();
            var iGenericSupplerTypes = TypeConstant.IGenericSupplierTypeDic[iGeneralSupplierType];
            foreach (var iGenericSupplerType in iGenericSupplerTypes)
                SupplierDescriptorDic[iGenericSupplerType] = new Dictionary<Type, SupplierDescriptor[]>();

        }
        internal SupplierDescriptor[] AddSuppliers(Type iGenericSupplierType, Type iSupplierType, BaseDescriptor[] baseDescriptors)
        {
            var supplierDescriptors = new SupplierDescriptor[baseDescriptors.Length];
            SupplierDescriptorDic[iGenericSupplierType][iSupplierType.GetGenericArguments()[0]] = supplierDescriptors;
            var i = 0;
            for(var lastIndex = baseDescriptors.Length; lastIndex > 0;)
                supplierDescriptors[i++] = new SupplierDescriptor(iSupplierType, baseDescriptors[--lastIndex]);
            return supplierDescriptors;
        }

        public virtual void Dispose()
        {
            SupplierDescriptorDic = null;
        }
    }
    internal class SupplierFactory : BaseFactory, IInternalSupplierFactory
    {
        Dictionary<Type, SupplierDescriptor[]> _asyncSupplierOutputDic;
        Dictionary<Type, SupplierDescriptor[]> _asyncSupplierDic;
        Dictionary<Type, SupplierDescriptor[]> _supplierOutputDic;
        Dictionary<Type, SupplierDescriptor[]> _supplierDic;
        public SupplierFactory() : base(TypeConstant.IGeneralSupplierType)
        {
            _asyncSupplierOutputDic = SupplierDescriptorDic[TypeConstant.IGenericAsyncSupplierOutputType];
            _asyncSupplierDic = SupplierDescriptorDic[TypeConstant.IGenericAsyncSupplierType];
            _supplierOutputDic = SupplierDescriptorDic[TypeConstant.IGenericSupplierOutputType];
            _supplierDic = SupplierDescriptorDic[TypeConstant.IGenericSupplierType];
        }
        public IAsyncSupplierChannel<T, R> CreateAsyncSupplierChannel<T, R>(IInternalDDBroker ddBroker) where T : IInputs<R>
        {
            var descriptor = _asyncSupplierOutputDic.GetValueOrDefault(typeof(T))?[0] ??
                throw new NotImplementedException();
            if(descriptor.Next == null)
                return new AsyncSupplierChannel<T, R>(ddBroker, descriptor);
            return new AsyncDecoratorChannel<T, R>(ddBroker, descriptor);
        }
        public IAsyncSupplierChannel<T> CreateAsyncSupplierChannel<T>(IInternalDDBroker ddBroker) where T : IInputs
        {
            var descriptor = _asyncSupplierDic.GetValueOrDefault(typeof(T))?[0] ??
                throw new NotImplementedException();
            if (descriptor.Next == null)
                return new AsyncSupplierChannel<T>(ddBroker, descriptor);
            return new AsyncDecoratorChannel<T>(ddBroker, descriptor);
        }
        public ISupplierChannel<T, R> CreateSupplierChannel<T, R>(IInternalDDBroker ddBroker) where T : IInputs<R>
        {
            var descriptor = _supplierOutputDic.GetValueOrDefault(typeof(T))?[0] ??
                throw new NotImplementedException();
            if (descriptor.Next == null)
                return new SupplierChannel<T, R>(ddBroker, descriptor);
            return new DecoratorChannel<T, R>(ddBroker, descriptor);
        }
        public ISupplierChannel<T> CreateSupplierChannel<T>(IInternalDDBroker ddBroker) where T : IInputs
        {
            var descriptor = _supplierDic.GetValueOrDefault(typeof(T))?[0] ??
              throw new NotImplementedException();
            if (descriptor.Next == null)
                return new SupplierChannel<T>(ddBroker, descriptor);
            return new DecoratorChannel<T>(ddBroker, descriptor);
        }
        public override void Dispose()
        {
            _asyncSupplierOutputDic = null;
            _asyncSupplierDic = null;
            _supplierOutputDic = null;
            _supplierDic = null;
            base.Dispose();
        }
    }
}

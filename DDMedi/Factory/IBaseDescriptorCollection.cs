using System;
using System.Collections.Generic;
using System.Linq;

namespace DDMedi
{
    public sealed class BaseDescriptor
    {
        public Type RegisterType { get; internal set; }
        public Type ImplementType { get; internal set; }
        public SupplierLifetime Lifetime { get; internal set; }
        public Func<IServiceProvider, object> GetInstance { get; internal set; }
    }
    public interface IBaseDescriptorCollection: IDisposable
    {
        IReadOnlyList<BaseDescriptor> Descriptors { get; }
    }
    internal interface IInternalBaseDescriptorCollection : IBaseDescriptorCollection
    {
        void AddIDDBroker();
        void AddDDMediFactory(DDMediFactory factory);
        BaseDescriptor Create(Type implementType, SupplierLifetime lifetime);
    }
    internal class BaseDescriptorCollection : IInternalBaseDescriptorCollection
    {
        List<BaseDescriptor> _descriptors;
        public IReadOnlyList<BaseDescriptor> Descriptors => _descriptors;
        internal BaseDescriptorCollection()
        {
            _descriptors = new List<BaseDescriptor>();
        }

        public void AddIDDBroker()
        {
            _descriptors.Insert(0, new BaseDescriptor
            { 
                Lifetime = SupplierLifetime.Scoped,
                RegisterType = TypeConstant.IDDBrokerType,
                ImplementType = typeof(DDBroker)
            });
        }

        public void AddDDMediFactory(DDMediFactory factory)
        {
            _descriptors.Insert(0, new BaseDescriptor
            {
                Lifetime = SupplierLifetime.Singleton,
                RegisterType = typeof(DDMediFactory),
                GetInstance = (provider) =>
                {
                    factory.SetScopeFactory(provider.GetService(typeof(ISupplierScopeFactory)) as ISupplierScopeFactory);
                    return factory;
                }
            });
        }
        public BaseDescriptor Create(Type implementType, SupplierLifetime lifetime)
        {
            var descriptor = _descriptors.FirstOrDefault(e => e.ImplementType == implementType);
            if(descriptor == null)
            {
                descriptor = new BaseDescriptor
                {
                    Lifetime = lifetime,
                    RegisterType = implementType,
                    ImplementType = implementType
                };
                _descriptors.Add(descriptor);
                return descriptor;
            }
            if (lifetime > descriptor.Lifetime) descriptor.Lifetime = lifetime;
            return descriptor;
        }

        public void Dispose()
        {
            _descriptors = null;
        }
    }
}

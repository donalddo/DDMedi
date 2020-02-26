using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DDMedi
{
    internal class SupplierImplementDescriptor
    {
        public SupplierImplementDescriptor(Type iGeneralSupplierType, Type implementType, SupplierLifetime lifetime, IReadOnlyList<Type> iSupplierTypes)
        {
            IGeneralSupplierType = iGeneralSupplierType;
            ImplementType = implementType;
            Lifetime = lifetime;
            ISupplierTypes = iSupplierTypes;
        }
        internal Type IGeneralSupplierType { get; }
        internal Type ImplementType { get; }
        internal SupplierLifetime Lifetime { get; }
        internal IReadOnlyList<Type> ISupplierTypes { get; }
    }
    internal interface ISupplierCollection : IDisposable
    {
        ISupplierCollection AddSupplier(Type implementType, SupplierLifetime supplierLifetime);
        ISupplierCollection AddSuppliers(Assembly assembly, SupplierLifetime supplierLifetime);
        ISupplierCollection AddSuppliers(Type[] implementTypes, SupplierLifetime supplierLifetime);
        bool IsRegistered(Type iSupplierType);
        bool IsEmpty();
        SupplierImplementDescriptor CheckAndRegisterSupplier(Type implementType, SupplierLifetime supplierLifetime, out Exception exception);
        Type IGeneralSupplierType { get; }
        Dictionary<Type, Dictionary<Type, List<SupplierImplementDescriptor>>> TypeCollections { get; }
    }
    internal class SupplierCollection : ISupplierCollection
    {
        public Dictionary<Type, Dictionary<Type, List<SupplierImplementDescriptor>>> TypeCollections { get; private set; }
        public Type IGeneralSupplierType { get; }
        public IReadOnlyList<Type> IGenericSupplierTypes { get; private set; }
        public virtual void Dispose()
        {
            TypeCollections = null;
            IGenericSupplierTypes = null;
        }

        public ISupplierCollection AddSupplier(Type implementType, SupplierLifetime supplierLifetime)
        {
            CheckAndRegisterSupplier(implementType, supplierLifetime, out var exception);
            if (exception != null) throw exception;
            return this;
        }
        public ISupplierCollection AddSuppliers(Assembly assembly, SupplierLifetime supplierLifetime)
        {
            var implementTypes = assembly.GetTypes();
            foreach (var implementType in implementTypes)
                CheckAndRegisterSupplier(implementType, supplierLifetime, out _);
            return this;
        }
        public ISupplierCollection AddSuppliers(Type[] implementTypes, SupplierLifetime supplierLifetime)
        {
            if (implementTypes == null)
                throw new ArgumentNullException($"{nameof(implementTypes)} not allow null");
            if (implementTypes.Length < 1)
                throw new ArgumentException($"Must include at least 1 {IGeneralSupplierType.Name} type to register");
            foreach (var implementType in implementTypes)
            {
                CheckAndRegisterSupplier(implementType, supplierLifetime, out var exception);
                if (exception != null) throw exception;
            }
            return this;
        }

        public SupplierImplementDescriptor CheckAndRegisterSupplier(Type implementType, SupplierLifetime supplierLifetime, out Exception exception)
        {
            exception = null;
            if (TypeCollections.Any(e => e.Value.Any( x => x.Value.Any(r => r.ImplementType == implementType))))
                return null;
            var iSupplierTypes = implementType.GetISupplierTypes(IGeneralSupplierType, IGenericSupplierTypes, out exception);
            if (exception != null) return null;
            var supplierInfo = new SupplierImplementDescriptor(IGeneralSupplierType, implementType, supplierLifetime, iSupplierTypes.ToList());
            foreach(var iSupplierType in iSupplierTypes)
                TypeCollections[iSupplierType.GetGenericTypeDefinition()]
                    .AddWithAutoCreateList(iSupplierType, supplierInfo);
            return supplierInfo;
        }
        public bool IsRegistered(Type iSupplierType)
        {
            if (!iSupplierType.IsGenericType) return false;
            return TypeCollections[iSupplierType.GetGenericTypeDefinition()].Any(e => e.Key == iSupplierType);
        }
        public bool IsEmpty() =>
            !TypeCollections.Any(e => e.Value.Count > 0);

        internal void ClearTypeCollections()
        {
            TypeCollections = new Dictionary<Type, Dictionary<Type, List<SupplierImplementDescriptor>>>();
            foreach (var iGenericSupplier in IGenericSupplierTypes)
                TypeCollections[iGenericSupplier] = new Dictionary<Type, List<SupplierImplementDescriptor>>();
        }
        internal SupplierCollection(Type iGeneralSupplierType)
        {
            IGeneralSupplierType = iGeneralSupplierType;
            IGenericSupplierTypes = TypeConstant.IGenericSupplierTypeDic[IGeneralSupplierType];
            ClearTypeCollections();
        }
    }
}

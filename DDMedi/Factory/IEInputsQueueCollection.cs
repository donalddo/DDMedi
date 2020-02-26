using System;
using System.Collections.Generic;
using System.Reflection;

namespace DDMedi
{
    public interface IESupplierCollection
    {
        IESupplierCollection AddESupplier<TESupplier>(SupplierLifetime lifetime = SupplierLifetime.Scoped)
               where TESupplier : IESupplier;
        IESupplierCollection AddESuppliers(Type[] implementTypes, SupplierLifetime lifetime = SupplierLifetime.Scoped);
        IESupplierCollection AddESuppliers(Assembly assembly, SupplierLifetime lifetime = SupplierLifetime.Scoped);
        IESupplierCollection AddESuppliers(SupplierLifetime lifetime = SupplierLifetime.Scoped);
    }
    internal interface IInternalESupplierCollection : IESupplierCollection, IDisposable
    {
        int NumberOfExecutors { get; }
        bool IsEmpty();
        Dictionary<Type, List<SupplierImplementDescriptor>> TypeCollections { get; }
        int QueueNum { get; }
    }
    internal interface IEInputsQueueCollection : IDisposable
    {
        Dictionary<Type, List<IInternalESupplierCollection>> ESupplierCollectionDic { get; }
        ISupplierCollection SupplierCollection { get; }
        IEInputsQueueCollection AddQueue(Action<IESupplierCollection> registerSuppliers, int numberOfExecutors = 1);
        bool IsEmpty();
    }
    internal class ESupplierCollection :
        IInternalESupplierCollection
    {
        public Dictionary<Type, List<SupplierImplementDescriptor>> TypeCollections { get; private set; }
        public int NumberOfExecutors { get; }
        public int QueueNum { get; }
        readonly ISupplierCollection _suppliersCollection;
        internal void ClearTypeCollections()
        {
            TypeCollections = new Dictionary<Type, List<SupplierImplementDescriptor>>();
        }
        internal ESupplierCollection(int queueNum, ISupplierCollection supplierCollection, int numberOfExecutors)
        {
            ClearTypeCollections();
            NumberOfExecutors = numberOfExecutors;
            _suppliersCollection = supplierCollection;
            QueueNum = queueNum;
        }
        Exception CheckAndRegisterSupplier(Type implementType, SupplierLifetime lifetime = SupplierLifetime.Scoped)
        {
            var ImplementDescriptor = _suppliersCollection.CheckAndRegisterSupplier(implementType, lifetime, out var exception);
            if (exception != null) return exception;
            if (ImplementDescriptor == null) return null;
            foreach (var iSupplierType in ImplementDescriptor.ISupplierTypes)
                TypeCollections.AddWithAutoCreateList(iSupplierType, ImplementDescriptor);
            return null;
        }
        public IESupplierCollection AddESupplier<TSupplier>(SupplierLifetime lifetime = SupplierLifetime.Scoped)
            where TSupplier : IESupplier
        {
            var exception = CheckAndRegisterSupplier(typeof(TSupplier), lifetime);
            if (exception != null) throw exception;
            return this;
        }
        public IESupplierCollection AddESuppliers(SupplierLifetime supplierLifetime = SupplierLifetime.Scoped)
        => AddESuppliers(Assembly.GetCallingAssembly(), supplierLifetime);
        public IESupplierCollection AddESuppliers(Assembly assembly, SupplierLifetime supplierLifetime = SupplierLifetime.Scoped)
        {
            var implementTypes = assembly.GetTypes();
            foreach (var implementType in implementTypes)
                CheckAndRegisterSupplier(implementType, supplierLifetime);
            return this;
        }
        public IESupplierCollection AddESuppliers(Type[] implementTypes, SupplierLifetime lifetime = SupplierLifetime.Scoped)
        {
            if (implementTypes == null)
                throw new ArgumentNullException($"{nameof(implementTypes)} not allow null");
            if (implementTypes.Length < 1)
                throw new ArgumentException("Must include at least 1 ESupplier type to register");
            foreach (var implementType in implementTypes)
            {
                var exception = CheckAndRegisterSupplier(implementType, lifetime);
                if (exception != null) throw exception;
            }
            return this;
        }

        public bool IsEmpty()
        => TypeCollections.Count == 0;

        public void Dispose()
        {
            TypeCollections = null;
        }
    }
    internal class EInputsQueueCollection : IEInputsQueueCollection
    {
        public Dictionary<Type, List<IInternalESupplierCollection>> ESupplierCollectionDic { get; private set; }
        private int _queueCount;
        public ISupplierCollection SupplierCollection { get; private set; }
        internal EInputsQueueCollection(ISupplierCollection supplierCollection)
        {
            SupplierCollection = supplierCollection;
            ESupplierCollectionDic = new Dictionary<Type, List<IInternalESupplierCollection>>();
        }

        public void Dispose()
        {
            SupplierCollection.Dispose();
            SupplierCollection = null;
            ESupplierCollectionDic = null;
        }

        public IEInputsQueueCollection AddQueue(Action<IESupplierCollection> registerSuppliers, int numberOfExecutors = 1)
        {
            if (numberOfExecutors < 1) return this;
            IInternalESupplierCollection newSupplierCollection = new ESupplierCollection(_queueCount, SupplierCollection, numberOfExecutors);
            registerSuppliers(newSupplierCollection);
            if (newSupplierCollection.IsEmpty()) return this;
            _queueCount++;
            foreach (var pair in newSupplierCollection.TypeCollections)
                ESupplierCollectionDic.AddWithAutoCreateList(pair.Key, newSupplierCollection);
            return this;
        }

        public bool IsEmpty() => ESupplierCollectionDic.Count == 0;
    }
}

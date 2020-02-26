using System;
using System.Reflection;

namespace DDMedi
{
    public static class SuppliersExtensions
    {
        public static DDMediCollection AddSupplier<TSupplier>(this DDMediCollection ddCollection, SupplierLifetime lifetime = SupplierLifetime.Scoped) where TSupplier : ISupplier
        {
            ddCollection.SupplierCollection.AddSupplier(typeof(TSupplier), lifetime);
            return ddCollection;
        }

        public static DDMediCollection AddSuppliers(this DDMediCollection ddCollection, SupplierLifetime lifetime = SupplierLifetime.Scoped)
        {
            ddCollection.SupplierCollection.AddSuppliers(Assembly.GetCallingAssembly(), lifetime);
            return ddCollection;
        }

        public static DDMediCollection AddSuppliers(this DDMediCollection ddCollection, Assembly assembly, SupplierLifetime lifetime = SupplierLifetime.Scoped)
        {
            ddCollection.SupplierCollection.AddSuppliers(assembly, lifetime);
            return ddCollection;
        }

        public static DDMediCollection AddSuppliers(this DDMediCollection ddCollection, Type[] implementTypes, SupplierLifetime lifetime = SupplierLifetime.Scoped)
        {
            ddCollection.SupplierCollection.AddSuppliers(implementTypes, lifetime);
            return ddCollection;
        }

        public static bool AreSuppliers(this Type implementType)
        {
            implementType.GetISupplierTypes(TypeConstant.IGeneralSupplierType, TypeConstant.IGenericSupplierTypeDic[TypeConstant.IGeneralSupplierType], out var exception);
            return exception == null;
        }

    }
}

using System;

namespace DDMedi
{
    public static class ESuppliersExtensions
    {
        public static DDMediCollection AddQueue(this DDMediCollection ddCollection, Action<IESupplierCollection> registerSuppliers, int numberOfExecutors = 1)
        {
            ddCollection.EInputsQueueCollection.AddQueue(registerSuppliers, numberOfExecutors);
            return ddCollection;
        }
        public static bool AreESuppliers(this Type implementType)
        {
            implementType.GetISupplierTypes(TypeConstant.IGeneralESupplierType, TypeConstant.IGenericSupplierTypeDic[TypeConstant.IGeneralESupplierType], out var exception);
            return exception == null;
        }
    }
}

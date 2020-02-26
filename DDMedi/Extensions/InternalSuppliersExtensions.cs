using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DDMedi
{
    internal static class InternalSuppliersExtensions
    {
        internal static Type ConvertToIGeneralSupplierType(this Type iGenericSupplierType)
        {
            foreach (var pair in TypeConstant.IGenericSupplierTypeDic)
                foreach (var iGenericType in pair.Value)
                    if (iGenericType == iGenericSupplierType)
                        return pair.Key;
            throw new ArgumentException($"{iGenericSupplierType.Name} cannot convert into any genernal supplier Interface");
        }
        internal static void CheckDuplicateDecorator(this SupplierDescriptor decoratorDescriptor, Type decoratorImplementType)
        {
            var current = decoratorDescriptor;
            do
            {
                if (current.ImplementDescriptor.ImplementType == decoratorImplementType)
                    if (current.Next != null)
                        throw new NotSupportedException($"{decoratorImplementType.FullName} is registered duplicated with {current.ISupplierType.FullName}!");
                    else
                        throw new NotSupportedException($"{decoratorImplementType.FullName} cannot be the decorator itself!");
                current = current.Next;
            } while (current != null);
        }
        internal static HashSet<Type> GetISupplierTypes(this Type implementType, Type iGeneralSupplierType, IReadOnlyList<Type> iGenericSupplierTypes, out Exception exception)
        {
            HashSet<Type> iSupplierTypes = null;
            if (implementType == null) exception = new ArgumentNullException();
            else if (!implementType.IsClass || implementType.IsAbstract) exception = new NotSupportedException($"{implementType.FullName} must be a class!");
            else if (!iGeneralSupplierType.IsAssignableFrom(implementType)) exception = new NotSupportedException($"{implementType.FullName} must be an {iGeneralSupplierType.Name}!");
            else
            {

                iSupplierTypes = new HashSet<Type>(implementType.GetInterfaces().Where(e => e.IsSupplier(iGenericSupplierTypes)));
                if (iSupplierTypes.Count < 1) exception = new NotSupportedException($"{implementType.FullName} must implement at least one {iGeneralSupplierType.Name}!");
                else
                    exception = null;
            }
            return iSupplierTypes;
        }
        internal static bool IsSupplier(this Type iSupplierType, IReadOnlyList<Type> iGenericSupplierTypes)
        {
            if (iSupplierType.IsGenericType)
            {
                var Supplier = iSupplierType.GetGenericTypeDefinition();
                if (!iGenericSupplierTypes.Contains(Supplier)) return false;
                return !iSupplierType.GetGenericArguments().Any(e => e.FullName == null);
            }
            return false;
        }
        internal static bool IsISupplier(this Type iSupplierType)
        => IsSupplier(iSupplierType, TypeConstant.IGenericSupplierTypes);
        internal static bool IsIESupplier(this Type iESupplierType)
        => IsSupplier(iESupplierType, TypeConstant.IGenericESupplierTypes);
        internal static IEnumerable<Type> GetIESupplierTypes(this Type implementType)
        {
            return implementType.GetInterfaces().Where(e =>
                e.IsGenericType &&
                e.GetGenericTypeDefinition() == TypeConstant.IGenericESupplierType);
        }

        internal static void AddWithAutoCreateList<T>(this Dictionary<Type, List<T>> dic, Type key, T model)
        {
            var models = dic.GetValueOrDefault(key);
            if (models != null) models.Add(model);
            else dic[key] = new List<T> { model };
        }
        internal static V GetValueOrDefault<V>(this IEnumerable<KeyValuePair<Type, V>> pairs, Type type)
        {
            foreach (var pair in pairs)
                if (pair.Key == type)
                    return pair.Value;
            return default;
        }

        internal static void QueueItem(this EInputsQueueDescriptor[] exceptionDescriptors, ExceptionEInputs exceptionEInputs, string correlationId, CancellationToken token = default)
        {
            if (exceptionDescriptors != null)
                foreach (var descriptor in exceptionDescriptors)
                    descriptor.Queue.QueueItem(exceptionEInputs, correlationId, token);
        }
    }
}

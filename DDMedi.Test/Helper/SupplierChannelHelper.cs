using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DDMedi.Test.Helper
{
    public static class SupplierChannelHelper
    {

        public static bool HasSupplier<TSupplier>(this object supplierChannel) where TSupplier : class
        {
            var cachedInstance = supplierChannel.GetHiddenPublicProperty<IBaseContext>();
            if (cachedInstance != null)
                return typeof(TSupplier).IsAssignableFrom(cachedInstance.Instance.GetType());
            return supplierChannel.GetNonePublicField<TSupplier>() != null;
        }
        public static TSupplier GetSupplier<TSupplier>(this object supplierChannel) where TSupplier : class
        => supplierChannel.GetHiddenPublicProperty<IBaseContext>().Instance as TSupplier ??
            supplierChannel.GetNonePublicField<TSupplier>();
        public static bool HasSupplier<TSupplier>(this object supplierChannel, TSupplier supplier)
            where TSupplier: class
        {
            var prop = supplierChannel.GetHiddenPublicProperty<IBaseContext>();
            if(prop != null)
                return prop.Instance == supplier;
            return supplierChannel.GetNonePublicField<TSupplier>() == supplier;
        }
        public static bool HasProperty<TSupplier>(this object supplierChannel) where TSupplier : class
        {
            var decoratorType = typeof(TSupplier);
            var decorator = supplierChannel.GetHiddenPublicProperty<TSupplier>();
            if (decorator == null) return false;
            return decoratorType.IsAssignableFrom(decorator.GetType());
        }
        public static TSupplier GetHiddenPublicProperty<TSupplier>(this object supplierChannel) where TSupplier : class
        {
            var supplierType = typeof(TSupplier);
            var serivceProp = supplierChannel.GetHiddenPublicProperty(supplierType);
            return serivceProp?.GetValue(supplierChannel) as TSupplier;
        }
        public static Dictionary<Type, Dictionary<Type, SupplierDescriptor[]>> GetAllDescriptorDic(this object supplierChannel)
        => supplierChannel.GetNonePublicProperty<Dictionary<Type, Dictionary<Type, SupplierDescriptor[]>>>();
        public static Dictionary<Type, SupplierDescriptor[]> GetDescriptorDic(this object supplierChannel)
        => supplierChannel.GetNonePublicProperty<Dictionary<Type, SupplierDescriptor[]>>();
        public static TSupplier GetNonePublicProperty<TSupplier>(this object supplierChannel) where TSupplier : class
        {
            var supplierType = typeof(TSupplier);
            var serivceProp = supplierChannel.GetNonePublicProperty(supplierType);
            return serivceProp?.GetValue(supplierChannel) as TSupplier;
        }
        public static TSupplier GetHiddenPublicField<TSupplier>(this object supplierChannel) where TSupplier : class
        {
            var supplierType = typeof(TSupplier);
            var serivceProp = supplierChannel.GetHiddenPublicField(supplierType);
            return serivceProp?.GetValue(supplierChannel) as TSupplier;
        }
        public static TSupplier GetNonePublicField<TSupplier>(this object supplierChannel) where TSupplier : class
        {
            var supplierType = typeof(TSupplier);
            var serivceProp = supplierChannel.GetNonePublicField(supplierType);
            return serivceProp?.GetValue(supplierChannel) as TSupplier;
        }
        static PropertyInfo GetHiddenPublicProperty(this object supplierChannel, Type supplierType) =>
            supplierChannel.GetType().GetProperties()
                .FirstOrDefault(e => e.PropertyType.IsAssignableFrom(supplierType) ||
                supplierType.IsAssignableFrom(e.PropertyType));
        static PropertyInfo GetNonePublicProperty(this object supplierChannel, Type supplierType) =>
            supplierChannel.GetType().GetProperties(BindingFlags.Instance|BindingFlags.NonPublic)
                .FirstOrDefault(e => e.PropertyType.IsAssignableFrom(supplierType) ||
                supplierType.IsAssignableFrom(e.PropertyType));
        static FieldInfo GetHiddenPublicField(this object supplierChannel, Type supplierType) =>
            supplierChannel.GetType().GetFields()
                .FirstOrDefault(e => e.FieldType.IsAssignableFrom(supplierType) ||
                supplierType.IsAssignableFrom(e.FieldType));
        static FieldInfo GetNonePublicField(this object supplierChannel, Type supplierType) =>
            supplierChannel.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(e => e.FieldType.IsAssignableFrom(supplierType) ||
                supplierType.IsAssignableFrom(e.FieldType));
        public static TService Get<TService>(this IServiceProvider serviceProvider)
            => (TService)serviceProvider.GetService(typeof(TService));
        public static bool ValidateLifeTime(this ISupplierScopeFactory scopeFactory, Type implementType, SupplierLifetime ExpectedLifeTime)
        {
            var provider1 = scopeFactory.CreateScope().ServiceProvider;
            var obj1 = provider1.GetService(implementType);
            if (ExpectedLifeTime == SupplierLifetime.Scoped)
                return obj1 == provider1.GetService(implementType) &&
                    obj1 != scopeFactory.CreateScope().ServiceProvider.GetService(implementType);
            if (ExpectedLifeTime == SupplierLifetime.Singleton)
                return obj1 == scopeFactory.CreateScope().ServiceProvider.GetService(implementType);
            return obj1 != provider1.GetService(implementType);
        }
    }
}

using Autofac;
using Autofac.Builder;
using System;

namespace DDMedi.AutoFac
{
    public class SupplierScopeFactory : ISupplierScopeFactory
    {
        ILifetimeScope _factory;
        public SupplierScopeFactory(ILifetimeScope factory)
        {
            _factory = factory;
        }
        public ISupplierScope CreateScope() => new SupplierScope(_factory.BeginLifetimeScope());
    }
    public class SupplierScope : ISupplierScope
    {
        ILifetimeScope _scope;
        public SupplierScope(ILifetimeScope scope)
        {
            _scope = scope;
            ServiceProvider = new SupplierProvider(_scope);
        }

        public IServiceProvider ServiceProvider { get; private set; }

        public void Dispose()
        {
            _scope.Dispose();
            _scope = null;
            ServiceProvider = null;
        }
    }
    public class SupplierProvider : IServiceProvider
    {
        IComponentContext _scope;
        public SupplierProvider(IComponentContext scope)
        {
            _scope = scope;
        }

        public object GetService(Type serviceType)
        {
            _scope.TryResolve(serviceType, out var obj);
            return obj;
        }
    }
    public static class AutoFacExtensions
    {
        public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle>
            SetLifetime<TLimit, TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> build, SupplierLifetime supplierLifetime)
        {
            switch (supplierLifetime)
            {
                case SupplierLifetime.Scoped:
                    return build.InstancePerLifetimeScope();
                case SupplierLifetime.Singleton:
                    return build.SingleInstance();
            }
            return build.InstancePerDependency();
        }
        public static void Register<Implement, IInterface>(this ContainerBuilder build, SupplierLifetime supplierLifetime)
        {
            build.RegisterType<Implement>().As<IInterface>().SetLifetime(supplierLifetime);
        }
        public static void RegisterDelegate<IInterface>(this ContainerBuilder build, Func<IServiceProvider,object> GetSupplierInstance, SupplierLifetime supplierLifetime)
            where IInterface : class
        {
            build.Register(ctx => GetSupplierInstance(ctx.Resolve<IServiceProvider>()) as IInterface).SetLifetime(supplierLifetime);
        }
        public static ContainerBuilder AddDDMediFactory(this ContainerBuilder services, Action<DDMediCollection> ddCollectionAction)
        {
            var ddCollection = new DDMediCollection();
            ddCollectionAction(ddCollection);
            return services.AddDDMediFactory(ddCollection.BuildSuppliers());
        }
        public static ContainerBuilder AddDDMediFactory(this ContainerBuilder services, DDMediCollection ddCollection)
        => services.AddDDMediFactory(ddCollection.BuildSuppliers());
        public static ContainerBuilder AddDDMediFactory(this ContainerBuilder services, DDMediFactory ddFactory)
        {
            services.RegisterType<SupplierProvider>().As<IServiceProvider>().SetLifetime(SupplierLifetime.Scoped);
            services.RegisterType<SupplierScopeFactory>().As<ISupplierScopeFactory>().SetLifetime(SupplierLifetime.Singleton);
            var registerMethod = typeof(AutoFacExtensions).GetMethod(nameof(Register));
            var registerDelegateMethod = typeof(AutoFacExtensions).GetMethod(nameof(RegisterDelegate));
            foreach (var descriptor in ddFactory.BaseDescriptorCollection.Descriptors)
            {
                if (descriptor.ImplementType != null)
                {
                    registerMethod.MakeGenericMethod(descriptor.ImplementType, descriptor.RegisterType)
                        .Invoke(null, new object[] { services, descriptor.Lifetime });
                }
                else
                {
                    registerDelegateMethod.MakeGenericMethod(descriptor.RegisterType)
                        .Invoke(null, new object[] { services, descriptor.GetInstance, descriptor.Lifetime });
                }
            }
            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;

namespace DDMedi.DependencyInjection
{
    public class SupplierScopeFactory : ISupplierScopeFactory
    {
        IServiceScopeFactory _factory;
        public SupplierScopeFactory(IServiceScopeFactory factory)
        {
            _factory = factory;
        }
        public ISupplierScope CreateScope() => new SupplierScope(_factory.CreateScope());
    }
    public class SupplierScope : ISupplierScope
    {
        IServiceScope _scope;
        public SupplierScope(IServiceScope scope)
        {
            _scope = scope;
        }

        public IServiceProvider ServiceProvider => _scope.ServiceProvider;

        public void Dispose()
        {
            _scope.Dispose();
            _scope = null;
        }
    }
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDDMediFactory(this IServiceCollection services, Action<DDMediCollection> ddCollectionAction)
        {
            var ddCollection = new DDMediCollection();
            ddCollectionAction(ddCollection);
            return services.AddDDMediFactory(ddCollection.BuildSuppliers());
        }
        public static IServiceCollection AddDDMediFactory(this IServiceCollection services, DDMediCollection ddCollection)
        => services.AddDDMediFactory(ddCollection.BuildSuppliers());
        public static IServiceCollection AddDDMediFactory(this IServiceCollection services, DDMediFactory ddFactory)
        {
            services.AddSingleton<ISupplierScopeFactory, SupplierScopeFactory>();
            foreach (var descriptor in ddFactory.BaseDescriptorCollection.Descriptors)
            {
                if(descriptor.ImplementType != null)
                    services.Add(ServiceDescriptor.Describe(descriptor.RegisterType, descriptor.ImplementType, (ServiceLifetime)descriptor.Lifetime));
                else
                    services.Add(ServiceDescriptor.Describe(descriptor.RegisterType, descriptor.GetInstance, (ServiceLifetime)descriptor.Lifetime));
            }
            return services;
        }
    }
}

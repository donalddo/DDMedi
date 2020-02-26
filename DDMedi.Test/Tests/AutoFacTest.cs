using Autofac;
using DDMedi.AutoFac;
using System;

namespace DDMedi.Test.Tests
{
    public class AutoFacTest : BaseDependencyInjectionTest
    {

        protected override void AddSingleton<TInstance>(object Collection, TInstance instance) where TInstance : class
        => (Collection as ContainerBuilder).RegisterInstance(instance).SingleInstance();

        protected override ISupplierScopeFactory BuildNewScopeFactory(DDMediFactory ddMediFactory)
        => new ContainerBuilder().AddDDMediFactory(ddMediFactory).Build().Resolve<ISupplierScopeFactory>();

        protected override IServiceProvider BuildNewServiceProvider(DDMediFactory ddMediFactory)
        => new ContainerBuilder().AddDDMediFactory(ddMediFactory).Build().Resolve<IServiceProvider>();

        protected override IServiceProvider BuildServiceProvider(object Collection, DDMediFactory ddMediFactory)
        => (Collection as ContainerBuilder).AddDDMediFactory(ddMediFactory).Build().Resolve<IServiceProvider>();

        protected override object CreateCollection()
        => new ContainerBuilder();
    }
}

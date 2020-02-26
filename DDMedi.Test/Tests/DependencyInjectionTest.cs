using DDMedi.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DDMedi.Test.Tests
{
    public class DependencyInjectionTest : BaseDependencyInjectionTest
    {

        protected override void AddSingleton<TInstance>(object Collection, TInstance instance)
        => (Collection as ServiceCollection).AddSingleton(instance);

        protected override ISupplierScopeFactory BuildNewScopeFactory(DDMediFactory ddMediFactory)
        => new ServiceCollection().AddDDMediFactory(ddMediFactory).BuildServiceProvider()
                .GetService<ISupplierScopeFactory>();

        protected override IServiceProvider BuildNewServiceProvider(DDMediFactory ddMediFactory)
        => new ServiceCollection().AddDDMediFactory(ddMediFactory).BuildServiceProvider();

        protected override IServiceProvider BuildServiceProvider(object Collection, DDMediFactory ddMediFactory)
        => (Collection as ServiceCollection).AddDDMediFactory(ddMediFactory).BuildServiceProvider();
        protected override object CreateCollection()
        => new ServiceCollection();
    }
}

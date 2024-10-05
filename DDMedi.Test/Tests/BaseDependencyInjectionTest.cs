using DDMedi.Test.Dummies;
using DDMedi.Test.Helper;
using DDMedi.Test.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi.Test.Tests
{
    public abstract class BaseDependencyInjectionTest
    {
        protected IServiceProvider provider { get; set; }
        protected IDDBroker ddBroker { get; set; }
        protected DDMediCollection ddCollection { get; set; }
        [SetUp]
        public void Setup()
        {
            ddCollection = new DDMediCollection();
        }
        protected abstract object CreateCollection();
        protected abstract void AddSingleton<TInstance>(object Collection, TInstance instance) where TInstance : class;
        protected abstract IServiceProvider BuildServiceProvider(object Collection, DDMediFactory ddMediFactory);
        protected abstract IServiceProvider BuildNewServiceProvider(DDMediFactory ddMediFactory);
        protected abstract ISupplierScopeFactory BuildNewScopeFactory(DDMediFactory ddMediFactory);

        [TestCase(typeof(DummySupplier), new Type[] { typeof(DummySupplier) }, true)]

        [TestCase(typeof(IAsyncSupplier<DummyInputs<DummyOutput>, DummyOutput>), new Type[] { typeof(DummySupplier) }, false)]
        [TestCase(typeof(IAsyncSupplier<DummyInputs>), new Type[] { typeof(DummySupplier) }, false)]
        [TestCase(typeof(ISupplier<DummyInputs<DummyOutput>, DummyOutput>), new Type[] { typeof(DummySupplier) }, false)]
        [TestCase(typeof(ISupplier<DummyInputs>), new Type[] { typeof(DummySupplier) }, false)]
        [TestCase(typeof(IESupplier<DummyEInputs>), new Type[] { typeof(DummySupplier) }, false)]

        [TestCase(typeof(IAsyncSupplier<Dummy2Inputs<DummyOutput>, DummyOutput>), new Type[] { typeof(DummySupplier) }, false)]
        [TestCase(typeof(IAsyncSupplier<Dummy2Inputs>), new Type[] { typeof(DummySupplier) }, false)]
        [TestCase(typeof(ISupplier<Dummy2Inputs<DummyOutput>, DummyOutput>), new Type[] { typeof(DummySupplier) }, false)]
        [TestCase(typeof(ISupplier<Dummy2Inputs>), new Type[] { typeof(DummySupplier) }, false)]
        [TestCase(typeof(IESupplier<Dummy2EInputs>), new Type[] { typeof(DummySupplier) }, false)]
        public void RegisterValidSupplierTypesTest(Type TypeToResolve, Type[] RegisterTypes, bool CanResolveService)
        {
            var provider = BuildNewServiceProvider(ddCollection.AddSuppliers(RegisterTypes).BuildSuppliers());
            var supplier = provider.GetService(TypeToResolve);

            Assert.IsTrue(CanResolveService == (supplier != null));

            provider = BuildNewServiceProvider(ddCollection.AddQueue(eSuppliers => eSuppliers
                .AddESuppliers(RegisterTypes)).BuildSuppliers());
            supplier = provider.GetService(TypeToResolve);

            Assert.IsTrue(CanResolveService == (supplier != null));

        }

        [Test]
        public void RegisterAllSuppliersFromCurrentProjectTest()
        {
            var provider = BuildNewServiceProvider(ddCollection.AddSuppliers().BuildSuppliers());
            Assert.IsTrue(provider.GetService(typeof(DummySupplier)) != null);
            Assert.IsTrue(provider.GetService(typeof(DummySupplier<>)) == null);
            provider = BuildNewServiceProvider(ddCollection.AddQueue( eSuppliers => eSuppliers.AddESuppliers()).BuildSuppliers());
            Assert.IsTrue(provider.GetService(typeof(DummySupplier)) != null);
            Assert.IsTrue(provider.GetService(typeof(DummyESupplier<>)) == null);
        }

        [Test]
        public void RegisterAllSuppliersFromAssemblyTest()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var provider = BuildNewServiceProvider(ddCollection.AddSuppliers(assembly).BuildSuppliers());
            Assert.IsTrue(provider.GetService(typeof(DummySupplier)) != null);
            Assert.IsTrue(provider.GetService(typeof(DummySupplier<>)) == null);
            provider = BuildNewServiceProvider(ddCollection.AddQueue(eSuppliers => eSuppliers.AddESuppliers(assembly)).BuildSuppliers());
            Assert.IsTrue(provider.GetService(typeof(DummySupplier)) != null);
            Assert.IsTrue(provider.GetService(typeof(DummyESupplier<>)) == null);
        }
        [Test]
        public void RegisteredSupplierSameInterfaceTest()
        {
            var provider = BuildNewServiceProvider(
            ddCollection
                .AddSupplier<MockAllSupplier<IInputs>>()
                .AddSupplier<MockSupplier<IInputs>>()
                .BuildSuppliers());
            var ddBroker = provider.Get<IDDBroker>();
            var mockSupplier = provider.Get<MockSupplier<IInputs>>();
            var mockAllSupplier = provider.Get<MockAllSupplier<IInputs>>();

            var supplierChannel = ddBroker.CreateSupplierChannel<IInputs>();
            var asyncSupplierChannel = ddBroker.CreateAsyncSupplierChannel<IInputs>();

            Assert.IsTrue(supplierChannel.HasSupplier(mockSupplier));
            Assert.IsFalse(supplierChannel.HasSupplier(mockAllSupplier));
            Assert.IsTrue(asyncSupplierChannel.HasSupplier(mockAllSupplier));
            Assert.IsFalse(asyncSupplierChannel.HasSupplier(mockSupplier));
        }
        [Test]
        public void RegisteredSupplierRedundantTest()
        {
            var ddCollection = new DDMediCollection()
                .AddSupplier<DummySupplier>(SupplierLifetime.Singleton)
                .AddSupplier<DummySupplier>(SupplierLifetime.Scoped);
            var ddCollection2 = new DDMediCollection()
                .AddSupplier<DummySupplier>(SupplierLifetime.Scoped)
                .AddSupplier<DummySupplier>(SupplierLifetime.Singleton);
            var ddCollection3 = new DDMediCollection()
                .AddSupplier<DummySupplier>(SupplierLifetime.Scoped)
                .AddSuppliers(SupplierLifetime.Singleton);
            var ddCollection4 = new DDMediCollection()
                .AddSuppliers(SupplierLifetime.Singleton)
                .AddSupplier<DummySupplier>(SupplierLifetime.Scoped);
            ValidateRedundantTest(ddCollection, SupplierLifetime.Singleton);
            ValidateRedundantTest(ddCollection2, SupplierLifetime.Scoped);
            ValidateRedundantTest(ddCollection3, SupplierLifetime.Scoped);
            ValidateRedundantTest(ddCollection4, SupplierLifetime.Singleton);
        }
        void ValidateRedundantTest(DDMediCollection ddCollection, SupplierLifetime ExpectedLifeTime)
        {
            var ddMediFactory = ddCollection.BuildSuppliers();
            var scopeFactory = BuildNewScopeFactory(ddMediFactory);
            var supplierFactory = ddMediFactory.GetNonePublicProperty<ISupplierFactory>();
            var realDescriptors = supplierFactory.GetAllDescriptorDic();
            var supplierDescriptors = realDescriptors[typeof(ISupplier<>)][typeof(DummyInputs)];

            Assert.IsTrue(supplierDescriptors.Length == 1);
            Assert.IsTrue(supplierDescriptors.FirstOrDefault().ImplementDescriptor.Lifetime == ExpectedLifeTime);
            Assert.IsTrue(scopeFactory.ValidateLifeTime(typeof(DummySupplier), ExpectedLifeTime));
        }
        

        public ISupplierScopeFactory SetUpRegisteredSuppliers(out ISupplierFactory supplierfactory)
        {
            ddCollection
                .AddSupplier<DummySupplier>(SupplierLifetime.Scoped)
                .AddSupplier<DummySupplier<DummyInputs>>(SupplierLifetime.Singleton);
            Assert.Throws(typeof(NotSupportedException), () => ddCollection.AddSuppliers(new Type[] { typeof(DummyDecorator) }));

            var ddMediFactory = ddCollection.AddDecorator<DummyInputs, DummyDecorator>().BuildSuppliers();
            var scopeFactory = BuildNewScopeFactory(ddMediFactory);
            supplierfactory = ddMediFactory.GetNonePublicProperty<ISupplierFactory>();
            return scopeFactory;
        }
        [Test]
        public void RegisteredSupplierCollectionsTest()
        {
            var scopeFactory = SetUpRegisteredSuppliers(out var supplierFactory);

            var realDescriptors = supplierFactory.GetAllDescriptorDic();
            var supplierDescriptors = realDescriptors[typeof(ISupplier<>)][typeof(DummyInputs)];
            var asyncSupplierDescriptors = realDescriptors[typeof(IAsyncSupplier<>)][typeof(DummyInputs)];

            Assert.IsTrue(supplierDescriptors.Length == 2);
            Assert.IsTrue(asyncSupplierDescriptors.Length == 2);
            Assert.IsTrue(scopeFactory.ValidateLifeTime(typeof(DummyDecorator), SupplierLifetime.Scoped));
        }

        [Test]
        public void RegisterSupplierTest()
        {
            var scopeFactory = SetUpRegisteredSuppliers(out _);
            var provider = scopeFactory.CreateScope().ServiceProvider;
            var ddBroker = provider.Get<IDDBroker>();
            var supplier = provider.Get<DummySupplier>();
            var supplier2 = provider.Get<DummySupplier<DummyInputs>>();
            var dummyDecorator = provider.Get<DummyDecorator>();
            var dummyDecorator2 = provider.Get<DummyDecorator>();
            var supplierChannel1 = ddBroker.CreateSupplierChannel<DummyInputs>();
            var supplierChannel2 = ddBroker.CreateAsyncSupplierChannel<DummyInputs>();
            var supplierChannel3 = ddBroker.CreateSupplierChannel<DummyInputs<DummyOutput>, DummyOutput>();
            var supplierChannel4 = ddBroker.CreateAsyncSupplierChannel<DummyInputs<DummyOutput>, DummyOutput>();

            Assert.IsNotNull(supplier);
            Assert.IsTrue(dummyDecorator == dummyDecorator2);
            Assert.IsTrue(supplierChannel1.HasSupplier(dummyDecorator));
            Assert.IsFalse(supplierChannel1.HasSupplier(supplier));
            Assert.IsTrue(supplierChannel2.HasSupplier(supplier2));
            Assert.IsFalse(supplierChannel2.HasSupplier(supplier));
            Assert.IsTrue(supplierChannel3.HasSupplier(supplier));
            Assert.IsTrue(supplierChannel4.HasSupplier(supplier));
        }
        [Test]
        public void InitDecoratorsTest()
        {
            var provider =  BuildNewServiceProvider(ddCollection
                .AddSupplier<DummySupplier>()
                .AddAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput, DummyAllDecorator>()
                .AddAsyncDecorator<DummyInputs, DummyAllDecorator>()
                .AddDecorator<DummyInputs<DummyOutput>, DummyOutput, DummyAllDecorator>()
                .AddDecorator<DummyInputs, DummyDecorator>()
                .AddDecorator<DummyInputs, DummyAllDecorator>()
                .BuildSuppliers());
            var ddBroker = provider.Get<IDDBroker>();

            var outputAsyncSupplierChannel = ddBroker.CreateAsyncSupplierChannel<DummyInputs<DummyOutput>, DummyOutput>();
            var asyncSupplierChannel = ddBroker.CreateAsyncSupplierChannel<DummyInputs>();
            var outputSupplierChannel = ddBroker.CreateSupplierChannel<DummyInputs<DummyOutput>, DummyOutput>();
            var supplierChannel = ddBroker.CreateSupplierChannel<DummyInputs>();

            outputAsyncSupplierChannel.ProcessAsync();
            asyncSupplierChannel.ProcessAsync();
            outputSupplierChannel.Process();
            supplierChannel.Process();

            var asyncDecoratorOutput = outputAsyncSupplierChannel.GetSupplier<IAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput>>();
            var asyncDecorator = asyncSupplierChannel.GetSupplier<IAsyncDecorator<DummyInputs>>();
            var decoratorOutput = outputSupplierChannel.GetSupplier<IDecorator<DummyInputs<DummyOutput>, DummyOutput>>();
            var decorator = supplierChannel.GetSupplier<IDecorator<DummyInputs>>();

            Assert.NotNull(asyncDecoratorOutput);
            Assert.AreEqual(asyncDecoratorOutput, asyncDecorator);
            Assert.AreEqual(asyncDecoratorOutput, decoratorOutput);
            Assert.AreEqual(asyncDecoratorOutput, decorator);
        }
        [Test]
        public void InitGenericDecoratorsTest()
        {
            var provider = BuildNewServiceProvider(ddCollection
                .AddSupplier<DummySupplier>()
                .AddGenericDecorator(typeof(IAsyncSupplier<,>), typeof(DummyAsyncDecorator<,>))
                .AddGenericDecorator(typeof(ISupplier<>), typeof(DummyDecorator<>))
                .AddGenericDecorator(typeof(IAsyncSupplier<>), typeof(DummyAsyncDecorator<>))
                .AddGenericDecorator(typeof(ISupplier<,>), typeof(DummyDecorator<,>))
                .AddGenericDecorator(typeof(IAsyncSupplier<,>), typeof(DummyAllDecorator<,>))
                .AddGenericDecorator(typeof(ISupplier<>), typeof(DummyAllDecorator<>))
                .AddGenericDecorator(typeof(IAsyncSupplier<>), typeof(DummyAllDecorator<>))
                .AddGenericDecorator(typeof(ISupplier<,>), typeof(DummyAllDecorator<,>))
                .BuildSuppliers());
            var ddBroker = provider.Get<IDDBroker>();

            var outputAsyncSupplierChannel = ddBroker.CreateAsyncSupplierChannel<DummyInputs<DummyOutput>, DummyOutput>();
            var asyncSupplierChannel = ddBroker.CreateAsyncSupplierChannel<DummyInputs>();
            var outputSupplierChannel = ddBroker.CreateSupplierChannel<DummyInputs<DummyOutput>, DummyOutput>();
            var supplierChannel = ddBroker.CreateSupplierChannel<DummyInputs>();

            outputAsyncSupplierChannel.ProcessAsync();
            asyncSupplierChannel.ProcessAsync();
            outputSupplierChannel.Process();
            supplierChannel.Process();

            var asyncDecoratorOutput = outputAsyncSupplierChannel.GetSupplier<IAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput>>();
            var asyncDecorator = asyncSupplierChannel.GetSupplier<IAsyncDecorator<DummyInputs>>();
            var decoratorOutput = outputSupplierChannel.GetSupplier<IDecorator<DummyInputs<DummyOutput>, DummyOutput>>();
            var decorator = supplierChannel.GetSupplier<IDecorator<DummyInputs>>();

            Assert.NotNull(asyncDecoratorOutput);
            Assert.AreEqual(asyncDecoratorOutput, decoratorOutput);
            Assert.NotNull(decoratorOutput);
            Assert.AreEqual(asyncDecorator, decorator);
        }
        [Test]
        public void RegisterDecoratorsWithConditionTest()
        {
            var provider = BuildNewServiceProvider(ddCollection
                .AddSupplier<DummySupplier<Dummy2Inputs>>()
                .AddSupplier<DummySupplier>()
                .AddGenericDecorator(typeof(ISupplier<>), typeof(DummyDecorator<>))
                .AddGenericDecorator(typeof(IAsyncSupplier<>), typeof(DummyAsyncDecorator<>),
                    context => context.ISupplierType != typeof(IAsyncSupplier<DummyInputs>))
                .AddGenericDecorator(typeof(IESupplier<>), typeof(DummyEDecorator<>))
            .BuildSuppliers());
            var ddBroker = provider.Get<IDDBroker>();
            var supplierChannel = ddBroker.CreateSupplierChannel<DummyInputs>();
            var asyncSupplierChannel = ddBroker.CreateAsyncSupplierChannel<DummyInputs>();
            var asyncSupplierChannel2 = ddBroker.CreateAsyncSupplierChannel<Dummy2Inputs>();

            Assert.IsTrue(supplierChannel.HasSupplier<IDecorator<DummyInputs>>());
            Assert.IsFalse(asyncSupplierChannel.HasSupplier<IAsyncDecorator<DummyInputs>>());
            Assert.IsTrue(asyncSupplierChannel2.HasSupplier<IAsyncDecorator<Dummy2Inputs>>());
        }
        [Test]
        public void WrapAllSuppliersTest()
        {
            var provider = BuildNewServiceProvider(ddCollection
                .AddSupplier<DummySupplier<DummyInputs>>(SupplierLifetime.Singleton)
                .AddSupplier<DummySupplier>(SupplierLifetime.Singleton)
                .AddSupplier<DummySupplier<DummyInputs<DummyOutput>, DummyOutput>>()
                .AddDecorator<DummyInputs, DummyDecorator>()
                .AddDecorator<DummyInputs<DummyOutput>, DummyOutput, DummyOutputDecorator>()
                .AddAsyncDecorator<DummyInputs, DummyAsyncDecorator>()
                .AddAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput, DummyAsyncOutputDecorator>()
                .BuildSuppliers());
            var ddBroker = provider.Get<IDDBroker>();

            var outputAsyncSupplierChannel = ddBroker.CreateAsyncSupplierChannel<DummyInputs<DummyOutput>, DummyOutput>();
            var asyncSupplierChannel = ddBroker.CreateAsyncSupplierChannel<DummyInputs>();
            var outputSupplierChannel = ddBroker.CreateSupplierChannel<DummyInputs<DummyOutput>, DummyOutput>();
            var supplierChannel = ddBroker.CreateSupplierChannel<DummyInputs>();

            outputAsyncSupplierChannel.ProcessAsync();
            asyncSupplierChannel.ProcessAsync();
            outputSupplierChannel.Process();
            supplierChannel.Process();

            Assert.IsTrue(outputAsyncSupplierChannel.HasSupplier<IAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput>>());
            Assert.IsTrue(asyncSupplierChannel.HasSupplier<IAsyncDecorator<DummyInputs>>());
            Assert.IsTrue(outputSupplierChannel.HasSupplier<IDecorator<DummyInputs<DummyOutput>, DummyOutput>>());
            Assert.IsTrue(supplierChannel.HasSupplier<IDecorator<DummyInputs>>());

            Assert.IsFalse(outputAsyncSupplierChannel.HasSupplier<IEDecorator<DummyEInputs>>());
            Assert.IsFalse(asyncSupplierChannel.HasSupplier<IEDecorator<DummyEInputs>>());
            Assert.IsFalse(outputSupplierChannel.HasSupplier<IEDecorator<DummyEInputs>>());
            Assert.IsFalse(supplierChannel.HasSupplier<IEDecorator<DummyEInputs>>());
        }
        [Test]
        public void Init2LayersDecoratorsTest()
        {
            var provider = BuildNewServiceProvider(ddCollection
                .AddSupplier<DummySupplier>()
                .AddGenericDecorator(typeof(IAsyncSupplier<>), typeof(DummyAsyncDecorator<>))
                .AddAsyncDecorator<DummyInputs, DummyAsyncDecorator>()
                .AddGenericDecorator(typeof(ISupplier<>), typeof(DummyDecorator<>))
                .AddDecorator<DummyInputs, DummyDecorator>()
                .BuildSuppliers());
            var ddBroker = provider.Get<IDDBroker>();

            var supplierChannel = ddBroker.CreateSupplierChannel<DummyInputs>();
            var asyncSupplierChannel = ddBroker.CreateAsyncSupplierChannel<DummyInputs>();

            ddBroker.Process<DummyInputs>();
            ddBroker.ProcessAsync<DummyInputs>();
            ddBroker.Process<DummyInputs>();
            ddBroker.ProcessAsync<DummyInputs>();
            supplierChannel.Process();
            asyncSupplierChannel.ProcessAsync();
            supplierChannel.Process();
            asyncSupplierChannel.ProcessAsync();

            var supplier = provider.Get<DummySupplier>();
            var asyncDecorator = asyncSupplierChannel.GetSupplier<IAsyncDecorator<DummyInputs>>();
            var decorator = supplierChannel.GetSupplier<IDecorator<DummyInputs>>();
            var decorator2 = provider.Get<DummyDecorator>();
            var asyncDecorator2 = provider.Get<DummyAsyncDecorator>();
            var decorator3 = provider.Get<DummyDecorator<DummyInputs>>();
            var asyncDecorator3 = provider.Get<DummyAsyncDecorator<DummyInputs>>();

            Assert.NotNull(decorator);
            Assert.NotNull(decorator2);
            Assert.NotNull(decorator3);
            Assert.NotNull(asyncDecorator);
            Assert.NotNull(asyncDecorator2);
            Assert.NotNull(asyncDecorator3);
            Assert.AreEqual(decorator, decorator2);
            Assert.AreNotEqual(decorator, decorator3);
            Assert.AreEqual(asyncDecorator, asyncDecorator2);
            Assert.AreNotEqual(asyncDecorator, asyncDecorator3);

            Assert.IsTrue(decorator2.Called);
            Assert.IsTrue(decorator3.Called);
            Assert.IsTrue(supplier.Called);

            Assert.IsTrue(asyncDecorator2.Called);
            Assert.IsTrue(asyncDecorator3.Called);
            Assert.IsTrue(supplier.AsyncCalled);
        }
        [Test]
        public void Init2LayersDecoratorsOutputTest()
        {
            var provider = BuildNewServiceProvider(ddCollection
                .AddSupplier<DummySupplier>()
                .AddGenericDecorator(typeof(IAsyncSupplier<,>), typeof(DummyAsyncDecorator<,>))
                .AddAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput, DummyAsyncOutputDecorator>()
                .AddGenericDecorator(typeof(ISupplier<,>), typeof(DummyDecorator<,>))
                .AddDecorator<DummyInputs<DummyOutput>, DummyOutput, DummyOutputDecorator>()
                .BuildSuppliers());
            var ddBroker = provider.Get<IDDBroker>();

            var supplierChannel = ddBroker.CreateSupplierChannel<DummyInputs<DummyOutput>, DummyOutput>();
            var asyncSupplierChannel = ddBroker.CreateAsyncSupplierChannel<DummyInputs<DummyOutput>, DummyOutput>();

            supplierChannel.Process();
            asyncSupplierChannel.ProcessAsync();
            supplierChannel.Process();
            asyncSupplierChannel.ProcessAsync();

            var supplier = provider.Get<DummySupplier>();
            var asyncDecorator = asyncSupplierChannel.GetSupplier<IAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput>>();
            var decorator = supplierChannel.GetSupplier<IDecorator<DummyInputs<DummyOutput>, DummyOutput>>();
            var decorator2 = provider.Get<DummyOutputDecorator>();
            var asyncDecorator2 = provider.Get<DummyAsyncOutputDecorator>();
            var decorator3 = provider.Get<DummyDecorator<DummyInputs<DummyOutput>, DummyOutput>>();
            var asyncDecorator3 = provider.Get<DummyAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput>>();

            Assert.NotNull(decorator);
            Assert.NotNull(decorator2);
            Assert.NotNull(decorator3);
            Assert.NotNull(asyncDecorator);
            Assert.NotNull(asyncDecorator2);
            Assert.NotNull(asyncDecorator3);
            Assert.AreEqual(decorator, decorator2);
            Assert.AreNotEqual(decorator, decorator3);
            Assert.AreEqual(asyncDecorator, asyncDecorator2);
            Assert.AreNotEqual(asyncDecorator, asyncDecorator3);

            Assert.IsTrue(decorator2.Called);
            Assert.IsTrue(decorator3.Called);
            Assert.IsTrue(supplier.OutputCalled);

            Assert.IsTrue(asyncDecorator2.Called);
            Assert.IsTrue(asyncDecorator3.Called);
            Assert.IsTrue(supplier.AsyncOutputCalled);
        }
        public void CreateAndBuildDDBroker()
        {
            provider = BuildNewServiceProvider(ddCollection.AddSupplier<Dummy2Supplier>()
                .AddSupplier<DummySupplier>().BuildSuppliers());
            ddBroker = provider.Get<IDDBroker>();
        }
        [Test]
        public void ExceptionWhenProcessUnRegisterSupplierTest()
        {
            CreateAndBuildDDBroker();
            var notImplementedExceptionType = typeof(NotImplementedException);

            var exception1 = Assert.Throws(notImplementedExceptionType, () => ddBroker.ProcessAsync<Dummy2Inputs<DummyOutput>, DummyOutput>(new Dummy2Inputs<DummyOutput>()));
            var exception2 = Assert.Throws(notImplementedExceptionType, () => ddBroker.ProcessAsync(new Dummy2Inputs()));
            var exception3 = Assert.Throws(notImplementedExceptionType, () => ddBroker.Process<Dummy2Inputs<DummyOutput>, DummyOutput>(new Dummy2Inputs<DummyOutput>()));
            var exception4 = Assert.Throws(notImplementedExceptionType, () => ddBroker.Process(new Dummy2Inputs()));

            Assert.AreEqual(exception1.Message, $"{nameof(IAsyncSupplier<object, object>)}<{typeof(Dummy2Inputs<DummyOutput>)},{typeof(DummyOutput)}> is not implemented!");
            Assert.AreEqual(exception2.Message, $"{nameof(IAsyncSupplier<object>)}<{typeof(Dummy2Inputs)}> is not implemented!");
            Assert.AreEqual(exception3.Message, $"{nameof(ISupplier<object, object>)}<{typeof(Dummy2Inputs<DummyOutput>)},{typeof(DummyOutput)}> is not implemented!");
            Assert.AreEqual(exception4.Message, $"{nameof(ISupplier<object>)}<{typeof(Dummy2Inputs)}> is not implemented!");
        }

        [Test]
        public void ProcessRegisterSuppliersTest()
        {
            CreateAndBuildDDBroker();
            Assert.DoesNotThrow(() => ddBroker.ProcessAsync<DummyInputs<DummyOutput>, DummyOutput>(new DummyInputs<DummyOutput>()));
            Assert.DoesNotThrow(() => ddBroker.ProcessAsync(new DummyInputs()));
            Assert.DoesNotThrow(() => ddBroker.Process<DummyInputs<DummyOutput>, DummyOutput>(new DummyInputs<DummyOutput>()));
            Assert.DoesNotThrow(() => ddBroker.Process(new DummyInputs()));
        }
        [Test]
        public async Task ProcessInputsTest()
        {
            provider = BuildNewServiceProvider(ddCollection
                .AddSupplier<MockAsyncSupplier<IInputs<object>, object>>()
                .AddSupplier<MockAsyncSupplier<IInputs>>(SupplierLifetime.Singleton)
                .AddSupplier<MockSupplier<IInputs<object>, object>>()
                .AddSupplier<MockSupplier<IInputs>>()
                .BuildSuppliers());
            ddBroker = provider.Get<IDDBroker>();

            IDDBroker ddBroker1 = null;
            IDDBroker ddBroker2 = null;
            IDDBroker ddBroker3 = null;
            IDDBroker ddBroker4 = null;


            var supplier = provider.Get<MockAsyncSupplier<IInputs>>();
            var supplier2 = provider.Get<MockSupplier<IInputs>>();
            var supplier3 = provider.Get<MockAsyncSupplier<IInputs<object>, object>>();
            var supplier4 = provider.Get<MockSupplier<IInputs<object>, object>>();

            var mockSupplier = supplier.Mock;
            var mockSupplier2 = supplier2.Mock;
            var mockSupplier3 = supplier3.Mock;
            var mockSupplier4 = supplier4.Mock;

            mockSupplier.SetUpProcessAsyncAny()
                .Callback<IInputs, ISupplierContext, CancellationToken>((inputs, context, token) => ddBroker1 = context.DDBroker);
            mockSupplier2.SetUpProcessAny()
                .Callback<IInputs, ISupplierContext>((inputs, context) => ddBroker2 = context.DDBroker);
            mockSupplier3.SetUpProcessAsyncAny()
                .Callback< IInputs<object>, ISupplierContext, CancellationToken>((inputs, context, token) => ddBroker3 = context.DDBroker);
            mockSupplier4.SetUpProcessAny()
                .Callback< IInputs<object>, ISupplierContext>((inputs, context) => ddBroker4 = context.DDBroker);

            var supplierChannel = this.ddBroker.CreateSupplierChannel<IInputs>();
            var supplierChannel2 = this.ddBroker.CreateAsyncSupplierChannel<IInputs>();
            supplierChannel.Process();
            this.ddBroker.Process<IInputs<object>, object>();
            await Task.WhenAll(
                supplierChannel2.ProcessAsync(),
                this.ddBroker.ProcessAsync<IInputs<object>, object>());

            mockSupplier.VerifyProcessAsyncAny(Times.Once());
            mockSupplier2.VerifyProcessAny(Times.Once());
            mockSupplier3.VerifyProcessAsyncAny(Times.Once());
            mockSupplier4.VerifyProcessAny(Times.Once());
            Assert.IsTrue(ddBroker1 == this.ddBroker);
            Assert.IsTrue(ddBroker2 == this.ddBroker);
            Assert.IsTrue(ddBroker3 == this.ddBroker);
            Assert.IsTrue(ddBroker4 == this.ddBroker);
            Assert.IsTrue(supplierChannel2.HasSupplier(supplier));
            Assert.IsTrue(supplierChannel.HasSupplier(supplier2));
            Assert.IsFalse(supplierChannel.HasSupplier(supplier3));
            Assert.IsFalse(supplierChannel2.HasSupplier(supplier4));
        }
        [Test]
        public async Task ProcessInputsWithDecoratorTest()
        {
            provider = BuildNewServiceProvider(ddCollection
                .AddSupplier<MockSupplier<IInputs>>()
                .AddSupplier<MockAsyncSupplier<IInputs>>(SupplierLifetime.Singleton)
                .AddAsyncDecorator<IInputs, MockAsyncDecorator<IInputs>>()
                .AddDecorator<IInputs, MockDecorator<IInputs>>()
                .AddAsyncDecorator<IInputs, MockAllDecorator<IInputs>>()
                .AddDecorator<IInputs, MockAllDecorator<IInputs>>()
                .BuildSuppliers());
            this.ddBroker = provider.Get<IDDBroker>();

            IDDBroker ddBroker1 = null;
            IDDBroker ddBroker2 = null;
            IDDBroker ddBroker3 = null;
            IDDBroker ddBroker4 = null;
            IDDBroker ddBroker5 = null;
            IDDBroker ddBroker6 = null;


            var supplierAsync = provider.Get<MockAsyncSupplier<IInputs>>();
            var supplier = provider.Get<MockSupplier<IInputs>>();
            var decoratorAsync = provider.Get<MockAsyncDecorator<IInputs>>();
            var decorator = provider.Get<MockDecorator<IInputs>>();
            var decoratorAll = provider.Get<MockAllDecorator<IInputs>>();

            var mockSupplierAsync = supplierAsync.Mock;
            var mockSupplier = supplier.Mock;
            var mockDecoratorAsync = decoratorAsync.Mock;
            var mockDecorator = decorator.Mock;
            var mockDecoratorAllAsync = decoratorAll.MockAsync;
            var mockDecoratorAll = decoratorAll.Mock;

            mockSupplierAsync.SetUpProcessAsyncAny()
                .Callback<IInputs, ISupplierContext, CancellationToken>((inputs, context, token) => ddBroker1 = context.DDBroker);
            mockSupplier.SetUpProcessAny()
                .Callback<IInputs, ISupplierContext>((inputs, context) => ddBroker2 = context.DDBroker);

            mockDecoratorAsync.SetUpProcessAsyncAny()
                .Callback<IInputs, ISupplierContext, CancellationToken>((inputs, context, token) => ddBroker3 = context.DDBroker);
            mockDecorator.SetUpProcessAny()
                .Callback<IInputs, ISupplierContext>((inputs, context) => ddBroker4 = context.DDBroker);

            mockDecoratorAllAsync.SetUpProcessAsyncAny()
                .Callback<IInputs, ISupplierContext, CancellationToken>((inputs, context, token) => ddBroker5 = context.DDBroker);
            mockDecoratorAll.SetUpProcessAny()
                .Callback<IInputs, ISupplierContext>((inputs, context) => ddBroker6 = context.DDBroker);

            var supplierChannel = this.ddBroker.CreateSupplierChannel<IInputs>();
            supplierChannel.Process();
            var asyncSupplierChannel = this.ddBroker.CreateAsyncSupplierChannel<IInputs>();
            await Task.WhenAll(
                asyncSupplierChannel.ProcessAsync(),
                this.ddBroker.ProcessAsync<IInputs>());

            mockDecoratorAllAsync.VerifyProcessAsyncAny(Times.Exactly(2));
            mockDecoratorAsync.VerifyProcessAsyncAny(Times.Exactly(2));
            mockSupplierAsync.VerifyProcessAsyncAny(Times.Exactly(2));

            mockDecoratorAll.VerifyProcessAny(Times.Once());
            mockDecorator.VerifyProcessAny(Times.Once());
            mockSupplier.VerifyProcessAny(Times.Once());

            Assert.IsTrue(ddBroker1 == this.ddBroker);
            Assert.IsTrue(ddBroker2 == this.ddBroker);
            Assert.IsTrue(ddBroker3 == this.ddBroker);
            Assert.IsTrue(ddBroker4 == this.ddBroker);
            Assert.IsTrue(ddBroker5 == this.ddBroker);
            Assert.IsTrue(ddBroker6 == this.ddBroker);
            Assert.IsTrue(asyncSupplierChannel.HasSupplier(decoratorAll));
            Assert.IsTrue(supplierChannel.HasSupplier(decoratorAll));
            Assert.IsFalse(asyncSupplierChannel.HasSupplier(decoratorAsync));
            Assert.IsFalse(supplierChannel.HasSupplier(decorator));
            Assert.IsFalse(asyncSupplierChannel.HasSupplier(supplierAsync));
            Assert.IsFalse(supplierChannel.HasSupplier(supplier));
        }
        [Test]
        public async Task ProcessOutputtWithDecoratorTest()
        {
            provider = BuildNewServiceProvider(ddCollection
                .AddSupplier<MockSupplier<IInputs<object>, object>>(SupplierLifetime.Singleton)
                .AddSupplier<MockAsyncSupplier<IInputs<object>, object>>()
                .AddAsyncDecorator<IInputs<object>, object, MockAsyncDecorator<IInputs<object>, object>>()
                .AddDecorator<IInputs<object>, object, MockDecorator<IInputs<object>, object>>()
                .AddAsyncDecorator<IInputs<object>, object, MockAllDecorator<IInputs<object>, object>>()
                .AddDecorator<IInputs<object>, object, MockAllDecorator<IInputs<object>, object>>()
                .BuildSuppliers());
            this.ddBroker = provider.Get<IDDBroker>();

            IDDBroker ddBroker1 = null;
            IDDBroker ddBroker2 = null;
            IDDBroker ddBroker3 = null;
            IDDBroker ddBroker4 = null;
            IDDBroker ddBroker5 = null;
            IDDBroker ddBroker6 = null;


            var supplierAsync = provider.Get<MockAsyncSupplier<IInputs<object>, object>>();
            var supplier = provider.Get<MockSupplier<IInputs<object>, object>>();
            var decoratorAsync = provider.Get<MockAsyncDecorator<IInputs<object>, object>>();
            var decorator = provider.Get<MockDecorator<IInputs<object>, object>>();
            var decoratorAll = provider.Get<MockAllDecorator<IInputs<object>, object>>();

            var mockSupplierAsync = supplierAsync.Mock;
            var mockSupplier = supplier.Mock;
            var mockDecoratorAsync = decoratorAsync.Mock;
            var mockDecorator = decorator.Mock;
            var mockDecoratorAllAsync = decoratorAll.MockAsync;
            var mockDecoratorAll = decoratorAll.Mock;

            mockSupplierAsync.SetUpProcessAsyncAny()
                .Callback<IInputs<object>, ISupplierContext, CancellationToken>((inputs, context, token) => ddBroker1 = context.DDBroker);
            mockSupplier.SetUpProcessAny()
                .Callback<IInputs<object>, ISupplierContext>((inputs, context) => ddBroker2 = context.DDBroker);

            mockDecoratorAsync.SetUpProcessAsyncAny()
                .Callback<IInputs<object>, ISupplierContext, CancellationToken>((inputs, context, token) => ddBroker3 = context.DDBroker);
            mockDecorator.SetUpProcessAny()
                .Callback<IInputs<object>, ISupplierContext>((inputs, context) => ddBroker4 = context.DDBroker);

            mockDecoratorAllAsync.SetUpProcessAsyncAny()
                .Callback<IInputs<object>, ISupplierContext, CancellationToken>((inputs, context, token) => ddBroker5 = context.DDBroker);
            mockDecoratorAll.SetUpProcessAny()
                .Callback<IInputs<object>, ISupplierContext>((inputs, context) => ddBroker6 = context.DDBroker);

            var supplierChannel = this.ddBroker.CreateSupplierChannel<IInputs<object>, object>();
            supplierChannel.Process();
            this.ddBroker.Process<IInputs<object>, object>();
            var asyncSupplierChannel = this.ddBroker.CreateAsyncSupplierChannel<IInputs<object>, object>();
            await Task.WhenAll(
                asyncSupplierChannel.ProcessAsync());

            mockDecoratorAllAsync.VerifyProcessAsyncAny(Times.Once());
            mockDecoratorAsync.VerifyProcessAsyncAny(Times.Once());
            mockSupplierAsync.VerifyProcessAsyncAny(Times.Once());

            mockDecoratorAll.VerifyProcessAny(Times.Exactly(2));
            mockDecorator.VerifyProcessAny(Times.Exactly(2));
            mockSupplier.VerifyProcessAny(Times.Exactly(2));

            Assert.IsTrue(ddBroker1 == this.ddBroker);
            Assert.IsTrue(ddBroker2 == this.ddBroker);
            Assert.IsTrue(ddBroker3 == this.ddBroker);
            Assert.IsTrue(ddBroker4 == this.ddBroker);
            Assert.IsTrue(ddBroker5 == this.ddBroker);
            Assert.IsTrue(ddBroker6 == this.ddBroker);
            Assert.IsTrue(asyncSupplierChannel.HasSupplier(decoratorAll));
            Assert.IsTrue(supplierChannel.HasSupplier(decoratorAll));
            Assert.IsFalse(asyncSupplierChannel.HasSupplier(decoratorAsync));
            Assert.IsFalse(supplierChannel.HasSupplier(decorator));
            Assert.IsFalse(asyncSupplierChannel.HasSupplier(supplierAsync));
            Assert.IsFalse(supplierChannel.HasSupplier(supplier));
        }

        private ISupplierScopeFactory SetUpScopeFactory()
        => BuildNewScopeFactory(ddCollection
                .AddSupplier<MockAllSupplier<IInputs<object>, object>>()
                .AddSupplier<MockAllSupplier<IInputs>>()
                .AddAsyncDecorator<IInputs<object>, object, MockAllDecorator<IInputs<object>, object>>()
                .AddDecorator<IInputs<object>, object, MockAllDecorator<IInputs<object>, object>>()
                .AddAsyncDecorator<IInputs, MockAllDecorator<IInputs>>()
                .AddDecorator<IInputs, MockAllDecorator<IInputs>>()
                .BuildSuppliers());

        [Test]
        public async Task DecoratorSameScopesTest()
        {
            var scopeFactory = SetUpScopeFactory();
            var scope1 = scopeFactory.CreateScope();
            var provider1 = scope1.ServiceProvider;

            var broker1 = provider1.Get<IDDBroker>();
            var channel1 = broker1.CreateAsyncSupplierChannel<IInputs<object>, object>();
            var channel2 = broker1.CreateSupplierChannel<IInputs<object>, object>();
            var channel3 = broker1.CreateAsyncSupplierChannel<IInputs>();
            var channel4 = broker1.CreateSupplierChannel<IInputs>();
            var decoratorOutput = provider1.Get<MockAllDecorator<IInputs<object>, object>>();
            var decorator = provider1.Get<MockAllDecorator<IInputs>>();
            var supplierOutput1 = provider1.Get<MockAllSupplier<IInputs<object>, object>>();
            var supplier1 = provider1.Get<MockAllSupplier<IInputs>>();
            ISupplierContext context1 = null;
            ISupplierContext context2 = null;
            ISupplierContext context3 = null;
            ISupplierContext context4 = null;
            ISupplierContext context5 = null;
            ISupplierContext context6 = null;
            ISupplierContext context7 = null;
            ISupplierContext context8 = null;
            int count = 0;
            supplierOutput1.MockAsync.SetUpProcessAsyncAny()
                .Callback<IInputs<object>, ISupplierContext, CancellationToken>((x, y, z) =>
                {
                    if (count++ == 0) context1 = y; context5 = y;
                });
            supplierOutput1.Mock.SetUpProcessAny()
                .Callback<IInputs<object>, ISupplierContext>((x, y) =>
                {
                    if (count++ == 1) context2 = y; context6 = y;
                });
            supplier1.MockAsync.SetUpProcessAsyncAny()
                .Callback<IInputs, ISupplierContext, CancellationToken>((x, y, z) =>
                {
                    if (count++ == 2) context3 = y; context7 = y;
                });
            supplier1.Mock.SetUpProcessAny()
                .Callback<IInputs, ISupplierContext>((x, y) =>
                {
                    if (count++ == 3) context4 = y; context8 = y;
                });

            await channel1.ProcessAsync();
            channel2.Process();
            await channel3.ProcessAsync();
            channel4.Process();
            decoratorOutput.Scope = scope1;
            decorator.Scope = scope1;
            await channel1.ProcessAsync();
            channel2.Process();
            await channel3.ProcessAsync();
            channel4.Process();

            decoratorOutput.MockAsync.VerifyProcessAsyncAny(Times.Exactly(2));
            decoratorOutput.Mock.VerifyProcessAny(Times.Exactly(2));
            decorator.MockAsync.VerifyProcessAsyncAny(Times.Exactly(2));
            decorator.Mock.VerifyProcessAny(Times.Exactly(2));
            supplierOutput1.MockAsync.VerifyProcessAsyncAny(Times.Exactly(2));
            supplierOutput1.Mock.VerifyProcessAny(Times.Exactly(2));
            supplier1.MockAsync.VerifyProcessAsyncAny(Times.Exactly(2));
            supplier1.Mock.VerifyProcessAny(Times.Exactly(2));
            Assert.AreEqual(context1, context5);
            Assert.AreEqual(context2, context6);
            Assert.AreEqual(context3, context7);
            Assert.AreEqual(context4, context8);
        }

        [Test]
        public async Task DecoratorDifferentScopesTest()
        {
            var scopeFactory = SetUpScopeFactory();

            var scope1 = scopeFactory.CreateScope();
            var provider1 = scope1.ServiceProvider;
            var broker1 = provider1.Get<IDDBroker>();
            var scope2 = broker1.CreateScope();
            var provider2 = scope2.ServiceProvider;
            var testCorrelationId = "test id!!!";
            var scope3 = scopeFactory.CreateScope(testCorrelationId);
            var provider3 = scope3.ServiceProvider;

            var channel1 = broker1.CreateAsyncSupplierChannel<IInputs<object>, object>();
            var channel2 = broker1.CreateSupplierChannel<IInputs<object>, object>();
            var channel3 = broker1.CreateAsyncSupplierChannel<IInputs>();
            var channel4 = broker1.CreateSupplierChannel<IInputs>();
            var decoratorOutput = provider1.Get<MockAllDecorator<IInputs<object>, object>>();
            var decorator = provider1.Get<MockAllDecorator<IInputs>>();
            var supplierOutput1 = provider1.Get<MockAllSupplier<IInputs<object>, object>>();
            var supplier1 = provider1.Get<MockAllSupplier<IInputs>>();
            ISupplierContext context1 = null;
            ISupplierContext context2 = null;
            ISupplierContext context3 = null;
            ISupplierContext context4 = null;
            supplierOutput1.MockAsync.SetUpProcessAsyncAny()
                .Callback<IInputs<object>, ISupplierContext, CancellationToken>((x, y, z) => context1 = y);
            supplierOutput1.Mock.SetUpProcessAny()
                .Callback<IInputs<object>, ISupplierContext>((x, y) => context2 = y);
            supplier1.MockAsync.SetUpProcessAsyncAny()
                .Callback<IInputs, ISupplierContext, CancellationToken>((x, y, z) => context3 = y);
            supplier1.Mock.SetUpProcessAny()
                .Callback<IInputs, ISupplierContext>((x, y) => context4 = y);

            var broker2 = provider2.Get<IDDBroker>();
            var supplierOutput2 = provider2.Get<MockAllSupplier<IInputs<object>, object>>();
            var supplier2 = provider2.Get<MockAllSupplier<IInputs>>();
            ISupplierContext context5 = null;
            ISupplierContext context6 = null;
            ISupplierContext context7 = null;
            ISupplierContext context8 = null;
            supplierOutput2.MockAsync.SetUpProcessAsyncAny()
                .Callback<IInputs<object>, ISupplierContext, CancellationToken>((x, y, z) => context5 = y);
            supplierOutput2.Mock.SetUpProcessAny()
                .Callback<IInputs<object>, ISupplierContext>((x, y) => context6 = y);
            supplier2.MockAsync.SetUpProcessAsyncAny()
                .Callback<IInputs, ISupplierContext, CancellationToken>((x, y, z) => context7 = y);
            supplier2.Mock.SetUpProcessAny()
                .Callback<IInputs, ISupplierContext>((x, y) => context8 = y);

            var broker3 = provider3.Get<IDDBroker>();
            var supplierOutput3 = provider3.Get<MockAllSupplier<IInputs<object>, object>>();
            var supplier3 = provider3.Get<MockAllSupplier<IInputs>>();
            ISupplierContext context9 = null;
            ISupplierContext context10 = null;
            ISupplierContext context11 = null;
            ISupplierContext context12 = null;
            supplierOutput3.MockAsync.SetUpProcessAsyncAny()
                .Callback<IInputs<object>, ISupplierContext, CancellationToken>((x, y, z) => context9 = y);
            supplierOutput3.Mock.SetUpProcessAny()
                .Callback<IInputs<object>, ISupplierContext>((x, y) => context10 = y);
            supplier3.MockAsync.SetUpProcessAsyncAny()
                .Callback<IInputs, ISupplierContext, CancellationToken>((x, y, z) => context11 = y);
            supplier3.Mock.SetUpProcessAny()
                .Callback<IInputs, ISupplierContext>((x, y) => context12 = y);

            await channel1.ProcessAsync();
            channel2.Process();
            await channel3.ProcessAsync();
            channel4.Process();
            decoratorOutput.Scope = scope2;
            decorator.Scope = scope2;
            await channel1.ProcessAsync();
            channel2.Process();
            await channel3.ProcessAsync();
            channel4.Process();
            decoratorOutput.Scope = scope3;
            decorator.Scope = scope3;
            await channel1.ProcessAsync();
            channel2.Process();
            await channel3.ProcessAsync();
            channel4.Process();

            decoratorOutput.MockAsync.VerifyProcessAsyncAny(Times.Exactly(3));
            decoratorOutput.Mock.VerifyProcessAny(Times.Exactly(3));
            decorator.MockAsync.VerifyProcessAsyncAny(Times.Exactly(3));
            decorator.Mock.VerifyProcessAny(Times.Exactly(3));
            supplierOutput1.MockAsync.VerifyProcessAsyncAny(Times.Once());
            supplierOutput1.Mock.VerifyProcessAny(Times.Once());
            supplier1.MockAsync.VerifyProcessAsyncAny(Times.Once());
            supplier1.Mock.VerifyProcessAny(Times.Once());
            supplierOutput2.MockAsync.VerifyProcessAsyncAny(Times.Once());
            supplierOutput2.Mock.VerifyProcessAny(Times.Once());
            supplier2.MockAsync.VerifyProcessAsyncAny(Times.Once());
            supplier2.Mock.VerifyProcessAny(Times.Once());
            supplierOutput3.MockAsync.VerifyProcessAsyncAny(Times.Once());
            supplierOutput3.Mock.VerifyProcessAny(Times.Once());
            supplier3.MockAsync.VerifyProcessAsyncAny(Times.Once());
            supplier3.Mock.VerifyProcessAny(Times.Once());
            Assert.AreNotEqual(context1, context5);
            Assert.AreNotEqual(context2, context6);
            Assert.AreNotEqual(context3, context7);
            Assert.AreNotEqual(context4, context8);
            Assert.AreNotEqual(supplierOutput1, supplierOutput2);
            Assert.AreNotEqual(supplier2, supplier1);
            Assert.AreNotEqual(supplierOutput3, supplierOutput2);
            Assert.AreNotEqual(supplier2, supplier3);
            Assert.AreNotEqual(broker1, broker2);
            Assert.AreNotEqual(broker1, broker3);
            Assert.AreEqual(broker1.CorrelationId, broker2.CorrelationId);
            Assert.AreEqual(broker3.CorrelationId, testCorrelationId);
            Assert.AreNotEqual(broker1.CorrelationId, broker3.CorrelationId);
            Assert.AreEqual(context1.CorrelationId, context5.CorrelationId);
            Assert.AreEqual(context2.CorrelationId, context6.CorrelationId);
            Assert.AreEqual(context3.CorrelationId, context7.CorrelationId);
            Assert.AreEqual(context4.CorrelationId, context8.CorrelationId);
            Assert.AreNotEqual(context1.CorrelationId, context9.CorrelationId);
            Assert.AreNotEqual(context2.CorrelationId, context10.CorrelationId);
            Assert.AreNotEqual(context3.CorrelationId, context11.CorrelationId);
            Assert.AreNotEqual(context4.CorrelationId, context12.CorrelationId);
        }

        [Test]
        public async Task EDecoratorScopesTest()
        {
            var testCorrelationId = "test id!!!";
            var mockSupplier = new Mock<IESupplier<IEInputs>>();
            var mockDecorator = new Mock<IEDecorator<IEInputs>>();
            var collection = CreateCollection();
            AddSingleton(collection, mockSupplier);
            AddSingleton(collection, mockDecorator);
            provider = BuildServiceProvider(collection, ddCollection
                .AddQueue(suppliers => suppliers
                   .AddESupplier<MockESupplier<IEInputs>>())
                .AddEDecorator<IEInputs, MockEDecorator<IEInputs>>()
                .BuildSuppliers());
            var scopeFactory = provider.Get<ISupplierScopeFactory>();
            this.ddBroker = provider.Get<IDDBroker>();
            MockEDecorator<IEInputs> decorator1 = null;
            MockEDecorator<IEInputs> decorator2 = null;
            MockEDecorator<IEInputs> decorator3 = null;
            IDDBroker broker1 = null;
            IDDBroker broker2 = null;
            IDDBroker broker3 = null;
            IDDBroker broker4 = null;
            IDDBroker broker5 = null;
            IDDBroker broker6 = null;
            IDDBroker broker7 = null;
            IDDBroker broker8 = null;
            int i = 0;
            var task = new Task(() => { });
            mockDecorator.SetUpProcessAsyncAny()
                .Callback<IEInputs, IEDecoratorContext, CancellationToken>((inputs, context, token) =>
                {
                    if (i == 0)
                    {
                        broker1 = context.DDBroker;
                        i++;
                        return;
                    }
                    if (i == 2)
                    {
                        i++;
                        decorator1 = context.GetNonePublicField<IEDecorator<IEInputs>>() as MockEDecorator<IEInputs>;
                        decorator1.Scope = context.DDBroker.GetHiddenPublicProperty<ISupplierScope>();
                        broker2 = context.DDBroker;
                    }
                    if (i == 4)
                    {
                        i++;
                        decorator2 = context.GetNonePublicField<IEDecorator<IEInputs>>() as MockEDecorator<IEInputs>;
                        decorator2.Scope = context.DDBroker.CreateScope();
                        broker3 = context.DDBroker;
                    }
                    if (i == 6)
                    {
                        i++;
                        decorator3 = context.GetNonePublicField<IEDecorator<IEInputs>>() as MockEDecorator<IEInputs>;
                        decorator3.Scope = scopeFactory.CreateScope(testCorrelationId);
                        broker7 = context.DDBroker;
                    }
                });
            mockSupplier.SetUpProcessAsyncAny()
                .Callback<IEInputs, ISupplierContext, CancellationToken>((inputs, context, token) =>
                {
                    if (i == 1)
                    {
                        i++;
                        broker4 = context.DDBroker;
                        return;
                    }
                    if (i == 3)
                    {
                        broker5 = context.DDBroker;
                        i++;
                    }
                    if (i == 5)
                    {
                        broker6 = context.DDBroker;
                        i++;
                    }
                    if (i == 7)
                    {
                        broker8 = context.DDBroker;
                        task.Start();
                    }
                });

            await this.ddBroker.Publish<IEInputs>();
            await this.ddBroker.Publish<IEInputs>();
            await this.ddBroker.Publish<IEInputs>();
            await this.ddBroker.Publish<IEInputs>();
            await task;

            mockDecorator.VerifyProcessAsyncAny(Times.Exactly(4));
            mockSupplier.VerifyProcessAsyncAny(Times.Exactly(4));
            Assert.IsNotNull(decorator1);
            Assert.AreNotEqual(decorator1, decorator2);
            Assert.AreEqual(broker1, broker4);
            Assert.AreEqual(broker1.CorrelationId, broker4.CorrelationId);
            Assert.AreEqual(broker2, broker5);
            Assert.AreEqual(broker2.CorrelationId, broker5.CorrelationId);
            Assert.AreNotEqual(broker1, broker5);
            Assert.AreNotEqual(broker3, broker6);
            Assert.AreEqual(broker3.CorrelationId, broker6.CorrelationId);
            Assert.AreEqual(broker1.CorrelationId, broker6.CorrelationId);
            Assert.AreNotEqual(broker1, broker7);
            Assert.AreNotEqual(broker3, broker8);
            Assert.AreEqual(broker1.CorrelationId, broker7.CorrelationId);
            Assert.AreNotEqual(broker7.CorrelationId, broker8.CorrelationId);
            Assert.AreEqual(broker8.CorrelationId, testCorrelationId);
        }
        [Test]
        public async Task PublishTest()
        {
            var mockSupplier = new Mock<IESupplier<IEInputs>>();
            var mockSupplier2 = new Mock<IESupplier<DummyEInputs>>();
            var collection = CreateCollection();
            AddSingleton(collection, mockSupplier);
            AddSingleton(collection, mockSupplier2);
            provider = BuildServiceProvider(collection, ddCollection
                .AddQueue(suppliers => suppliers
                   .AddESupplier<MockESupplier<IEInputs>>(SupplierLifetime.Singleton)
                   .AddESupplier<MockESupplier<DummyEInputs>>())
                .BuildSuppliers());
            this.ddBroker = provider.Get<IDDBroker>();
            IDDBroker ddBroker1 = null;
            var task = new Task(() => { });
            mockSupplier.SetUpProcessAsyncAny()
                .Callback<IEInputs, ISupplierContext, CancellationToken>((inputs, context, token) =>
                {
                    ddBroker1 = context.DDBroker;
                    task.Start();
                });

            await this.ddBroker.Publish<IEInputs>(new DummyEInputs());
            await task;

            mockSupplier.VerifyProcessAsyncAny(Times.Once());
            mockSupplier2.VerifyProcessAsyncAny(Times.Never());
            Assert.IsFalse(ddBroker1 == this.ddBroker);
            Assert.IsNotNull(ddBroker1);
        }
        [Test]
        public async Task PublishWithAutoGetRealTypeTest()
        {
            var mockSupplier = new Mock<IESupplier<DummyEInputs>>();
            var mockSupplier2 = new Mock<IESupplier<Dummy2EInputs>>();
            var mockSupplier3 = new Mock<IESupplier<ExceptionEInputs>>();
            var collection = CreateCollection();
            AddSingleton(collection, mockSupplier);
            AddSingleton(collection, mockSupplier2);
            AddSingleton(collection, mockSupplier3);
            var dummyEInputs = new DummyEInputs();
            var inputss = new List<IEInputs>
            {
                dummyEInputs,
                new Dummy2EInputs()
            };
            var ddFactory = ddCollection
                .AddQueue(suppliers => suppliers
                    .AddESupplier<DummyESupplier<DummyEInputs>>() // will cause cannot resolve service exception
                    .AddESupplier<MockESupplier<DummyEInputs>>(SupplierLifetime.Singleton)
                    .AddESupplier<MockESupplier<Dummy2EInputs>>(), 3)
                .AddQueue(suppliers => suppliers
                    .AddESupplier<MockESupplier<ExceptionEInputs>>(), 2)
                .BuildSuppliers();
            provider = BuildServiceProvider(collection, ddFactory);
            ddBroker = provider.Get<IDDBroker>();
            var task1 = new Task(() => { });
            var task2 = new Task(() => { });
            var task3 = new Task(() => { });
            IDDBroker ddBroker1 = null;
            IDDBroker ddBroker2 = null;
            IDDBroker ddBroker3 = null;
            ExceptionEInputs exceptionEInputs = null;
            ExceptionEInputs exceptionEInputs2 = null;
            mockSupplier.SetUpProcessAsyncAny()
                .Callback<DummyEInputs, ISupplierContext, CancellationToken>((inputs, context, token) =>
                {
                    ddBroker1 = context.DDBroker;
                    exceptionEInputs2 = new ExceptionEInputs(new Exception());
                    task1.Start();
                });
            mockSupplier2.SetUpProcessAsyncAny()
                .Callback<Dummy2EInputs, ISupplierContext, CancellationToken>((inputs, context, token) =>
                {
                    ddBroker2 = context.DDBroker;
                    task2.Start();
                });
            mockSupplier3.SetUpProcessAsyncAny()
                .Callback<ExceptionEInputs, ISupplierContext, CancellationToken>((inputs, context, token) =>
                {
                    ddBroker3 = context.DDBroker;
                    exceptionEInputs = inputs;
                    task3.Start();
                });

            foreach (var item in inputss)
            {
                await this.ddBroker.Publish(item);
            }
            await Task.WhenAll(task1, task2, task3);
            var queueFactory = ddFactory.GetNonePublicProperty<IEInputsQueueFactory>();
            var executeTasks = queueFactory.GetNonePublicField<List<Task>>();
            Assert.IsTrue(executeTasks.Count == 5);
            mockSupplier.VerifyProcessAsyncAny(Times.Once());
            mockSupplier2.VerifyProcessAsyncAny(Times.Once());
            mockSupplier3.VerifyProcessAsyncAny(Times.Once());
            Assert.IsFalse(ddBroker1 == this.ddBroker);
            Assert.IsFalse(ddBroker2 == this.ddBroker);
            Assert.IsFalse(ddBroker3 == this.ddBroker);
            Assert.IsFalse(ddBroker1 == ddBroker2);
            Assert.IsFalse(ddBroker1 == ddBroker3);
            Assert.IsNotNull(ddBroker1);
            Assert.IsNotNull(ddBroker2);
            Assert.IsNotNull(ddBroker3);
            Assert.IsTrue(exceptionEInputs.Inputs.Contains(dummyEInputs));
            Assert.IsFalse(exceptionEInputs2.Inputs.Contains(dummyEInputs));
        }
        [Test]
        public async Task PublishWithDecoratorTest()
        {
            var mockSupplier = new Mock<IESupplier<IEInputs>>();
            var mockSupplier2 = new Mock<IESupplier<DummyEInputs>>();
            var mockDecorator = new Mock<IEDecorator<IEInputs>>();
            var mockDecorator2 = new Mock<IEDecorator<DummyEInputs>>();
            var collection = CreateCollection();
            AddSingleton(collection, mockSupplier);
            AddSingleton(collection, mockSupplier2);
            AddSingleton(collection, mockDecorator);
            AddSingleton(collection, mockDecorator2);
            provider = BuildServiceProvider(collection, ddCollection
                    .AddQueue(suppliers => suppliers
                        .AddESupplier<MockESupplier<IEInputs>>(SupplierLifetime.Singleton)
                        .AddESupplier<MockESupplier<DummyEInputs>>())
                    .AddEDecorator<IEInputs, MockEDecorator<IEInputs>>()
                    .AddEDecorator<IEInputs, Mock2EDecorator<IEInputs>>()
                    .AddEDecorator<DummyEInputs, Mock2EDecorator<DummyEInputs>>()
                    .AddEDecorator<DummyEInputs, MockEDecorator<DummyEInputs>>()
                    .BuildSuppliers());
            this.ddBroker = provider.Get<IDDBroker>();
            IDDBroker ddBroker1 = null;
            IDDBroker ddBroker2 = null;
            IDDBroker ddBroker3 = null;
            IDDBroker ddBroker4 = null;
            var task = new Task(() => { });
            var task2 = new Task(() => { });
            var decCounter = 0;
            var task3 = new Task(() => { });
            var task4 = new Task(() => { });
            var decCounter2 = 0;
            var task5 = new Task(() => { });
            var task6 = new Task(() => { });
            mockSupplier.SetUpProcessAsyncAny()
                .Callback<IEInputs, ISupplierContext, CancellationToken>((inputs, context, token) =>
                {
                    ddBroker1 = context.DDBroker;
                    task.Start();
                });
            mockSupplier2.SetUpProcessAsyncAny()
                .Callback<DummyEInputs, ISupplierContext, CancellationToken>((inputs, context, token) =>
                {
                    ddBroker2 = context.DDBroker;
                    task2.Start();
                });
            mockDecorator.SetUpProcessAsyncAny()
                .Callback<IEInputs, ISupplierContext, CancellationToken>((inputs, context, token) =>
                {
                    ddBroker3 = context.DDBroker;
                    if (decCounter++ == 0)
                        task3.Start();
                    else
                        task4.Start();
                });
            mockDecorator2.SetUpProcessAsyncAny()
                .Callback<DummyEInputs, ISupplierContext, CancellationToken>((inputs, context, token) =>
                {
                    ddBroker4 = context.DDBroker;
                    if (decCounter2++ == 0)
                        task5.Start();
                    else
                        task6.Start();
                });

            await this.ddBroker.Publish<IEInputs>(new DummyEInputs());
            await this.ddBroker.Publish(new DummyEInputs() as IEInputs);
            await Task.WhenAll(task, task2, task3, task4, task5, task6);

            mockDecorator.VerifyProcessAsyncAny(Times.Exactly(2));
            mockDecorator2.VerifyProcessAsyncAny(Times.Exactly(2));
            mockSupplier.VerifyProcessAsyncAny(Times.Once());
            mockSupplier2.VerifyProcessAsyncAny(Times.Once());

            Assert.IsTrue(ddBroker1 == ddBroker3);
            Assert.IsTrue(ddBroker2 == ddBroker4);
            Assert.IsFalse(ddBroker1 == this.ddBroker);
            Assert.IsFalse(ddBroker2 == this.ddBroker);
            Assert.IsFalse(ddBroker3 == this.ddBroker);
            Assert.IsFalse(ddBroker4 == this.ddBroker);
            Assert.IsNotNull(ddBroker1);
            Assert.IsNotNull(ddBroker2);
            Assert.IsNotNull(ddBroker3);
            Assert.IsNotNull(ddBroker4);
        }
        [Test]
        public async Task SameCorrelationTest()
        {
            var mockESupplier1 = new Mock<IESupplier<IEInputs>>();
            var mockESupplier2 = new Mock<IESupplier<DummyEInputs>>();
            var collection = CreateCollection();
            AddSingleton(collection, mockESupplier1);
            AddSingleton(collection, mockESupplier2);
            provider = BuildServiceProvider(collection, ddCollection
                .AddSupplier<MockAllSupplier<IInputs<object>, object>>()
                .AddSupplier<MockAllSupplier<IInputs>>()
                .AddQueue(suppliers => suppliers
                    .AddESupplier<MockESupplier<IEInputs>>(SupplierLifetime.Singleton)
                    .AddESupplier<MockESupplier<DummyEInputs>>(SupplierLifetime.Singleton))
                .BuildSuppliers());
            ddBroker = provider.Get<IDDBroker>();
            var mockAllOutputSupplier = provider.Get<MockAllSupplier<IInputs<object>, object>>();
            var mockAllSupplier = provider.Get<MockAllSupplier<IInputs>>();
            var mockAsyncOutputSupplier = mockAllOutputSupplier.MockAsync;
            var mockAsyncSupplier = mockAllSupplier.MockAsync;
            var mockOutputSupplier = mockAllOutputSupplier.Mock;
            var mockSupplier = mockAllSupplier.Mock;
            var task1 = new Task(() => { });
            var task2 = new Task(() => { });
            ISupplierContext AsyncOutputContext = null;
            ISupplierContext AsyncContext = null;
            ISupplierContext OutputContext = null;
            ISupplierContext Context = null;
            ISupplierContext EContext1 = null;
            ISupplierContext EContext2 = null;
            mockAsyncOutputSupplier.SetUpProcessAsyncAny()
                .Callback< IInputs<object>, ISupplierContext, CancellationToken>((inputs, context, token) => AsyncOutputContext = context);
            mockAsyncSupplier.SetUpProcessAsyncAny()
                .Callback<IInputs, ISupplierContext, CancellationToken>((inputs, context, token) => AsyncContext = context);
            mockOutputSupplier.SetUpProcessAny()
                .Callback<IInputs<object>, ISupplierContext >((inputs, context) => OutputContext = context);
            mockSupplier.SetUpProcessAny()
                .Callback<IInputs,ISupplierContext>((inputs, context) => Context = context);
            mockESupplier1.SetUpProcessAsyncAny()
                .Callback<IEInputs, ISupplierContext, CancellationToken>((inputs, context, token) =>
                {
                    EContext1 = context;
                    context.DDBroker.Publish<DummyEInputs>();
                    task1.Start();
                });
            mockESupplier2.SetUpProcessAsyncAny()
                .Callback<DummyEInputs, ISupplierContext, CancellationToken>((inputs, context, token) =>
                {
                    EContext2 = context;
                    task2.Start();
                });

            await ddBroker.Publish<IEInputs>(new DummyEInputs());
            await ddBroker.ProcessAsync<IInputs<object>, object>();
            await ddBroker.ProcessAsync<IInputs>();
            ddBroker.Process<IInputs<object>, object>();
            ddBroker.Process<IInputs>();
            await task1;
            await task2;

            Assert.AreEqual(ddBroker, AsyncOutputContext.DDBroker);
            Assert.AreEqual(ddBroker, AsyncContext.DDBroker);
            Assert.AreEqual(ddBroker, OutputContext.DDBroker);
            Assert.AreEqual(ddBroker, Context.DDBroker);
            Assert.AreNotEqual(ddBroker, EContext1.DDBroker);
            Assert.AreNotEqual(ddBroker, EContext2.DDBroker);
            Assert.AreNotEqual(EContext1.DDBroker, EContext2.DDBroker);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(ddBroker.CorrelationId));
            Assert.AreEqual(ddBroker.CorrelationId, AsyncOutputContext.CorrelationId);
            Assert.AreEqual(ddBroker.CorrelationId, AsyncContext.CorrelationId);
            Assert.AreEqual(ddBroker.CorrelationId, OutputContext.CorrelationId);
            Assert.AreEqual(ddBroker.CorrelationId, Context.CorrelationId);
            Assert.AreEqual(ddBroker.CorrelationId, EContext1.CorrelationId);
            Assert.AreEqual(ddBroker.CorrelationId, EContext2.CorrelationId);
        }

        [Test]
        public async Task ScopeDifferentCorrelationTest()
        {
            var mockESupplier1 = new Mock<IESupplier<IEInputs>>();
            var mockESupplier2 = new Mock<IESupplier<IEInputs>>();
            var collection1 = CreateCollection();
            var collection2 = CreateCollection();
            AddSingleton(collection1, mockESupplier1);
            var provider1 = BuildServiceProvider(collection1, new DDMediCollection()
                    .AddSupplier<MockAllSupplier<IInputs<object>, object>>()
                    .AddQueue(suppliers => suppliers.AddESupplier<MockESupplier<IEInputs>>(SupplierLifetime.Singleton))
                    .BuildSuppliers());
            AddSingleton(collection2, mockESupplier2);
            var provider2 = BuildServiceProvider(collection2, new DDMediCollection()
                    .AddSupplier<MockAllSupplier<IInputs>>()
                    .AddQueue(suppliers => suppliers.AddESupplier<MockESupplier<IEInputs>>(SupplierLifetime.Singleton))
                    .BuildSuppliers());
            var ddBroker1 = provider1.Get<IDDBroker>();
            var ddBroker2 = provider2.Get<IDDBroker>();
            var mockAllOutputSupplier = provider1.Get<MockAllSupplier<IInputs<object>, object>>();
            var mockAllSupplier = provider2.Get<MockAllSupplier<IInputs>>();
            var mockAsyncOutputSupplier = mockAllOutputSupplier.MockAsync;
            var mockAsyncSupplier = mockAllSupplier.MockAsync;
            var mockOutputSupplier = mockAllOutputSupplier.Mock;
            var mockSupplier = mockAllSupplier.Mock;
            var task1 = new Task(() => { });
            var task2 = new Task(() => { });
            ISupplierContext AsyncOutputContext = null;
            ISupplierContext AsyncContext = null;
            ISupplierContext OutputContext = null;
            ISupplierContext Context = null;
            ISupplierContext EContext1 = null;
            ISupplierContext EContext2 = null;
            mockAsyncOutputSupplier.SetUpProcessAsyncAny()
                .Callback< IInputs<object>, ISupplierContext, CancellationToken>((inputs, context, token) => AsyncOutputContext = context);
            mockOutputSupplier.SetUpProcessAny()
                .Callback< IInputs<object>, ISupplierContext >((inputs, context) => OutputContext = context);
            mockAsyncSupplier.SetUpProcessAsyncAny()
                .Callback<IInputs, ISupplierContext, CancellationToken>((inputs, context, token) => AsyncContext = context);
            mockSupplier.SetUpProcessAny()
                .Callback<IInputs, ISupplierContext>((inputs, context) => Context = context);
            mockESupplier1.SetUpProcessAsyncAny()
                .Callback<IEInputs, ISupplierContext, CancellationToken>((inputs, context, token) =>
                {
                    EContext1 = context;
                    task1.Start();
                });
            mockESupplier2.SetUpProcessAsyncAny()
                .Callback<IEInputs, ISupplierContext, CancellationToken>((inputs, context, token) =>
                {
                    EContext2 = context;
                    task2.Start();
                });
            var eventD = new DummyEInputs();
            await ddBroker1.Publish<IEInputs>(eventD);
            await ddBroker1.ProcessAsync<IInputs<object>, object>();
            ddBroker1.Process<IInputs<object>, object>();
            await ddBroker2.Publish<IEInputs>(eventD);
            await ddBroker2.ProcessAsync<IInputs>();
            ddBroker2.Process<IInputs>();
            await task1;
            await task2;

            Assert.AreEqual(ddBroker1, AsyncOutputContext.DDBroker);
            Assert.AreEqual(ddBroker1, OutputContext.DDBroker);
            Assert.AreEqual(ddBroker2, AsyncContext.DDBroker);
            Assert.AreEqual(ddBroker2, Context.DDBroker);
            Assert.AreNotEqual(ddBroker1, ddBroker2);
            Assert.AreNotEqual(ddBroker1, EContext1.DDBroker);
            Assert.AreNotEqual(ddBroker2, EContext2.DDBroker);
            Assert.AreNotEqual(EContext1.DDBroker, EContext2.DDBroker);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(ddBroker1.CorrelationId));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(ddBroker2.CorrelationId));
            Assert.AreEqual(ddBroker1.CorrelationId, AsyncOutputContext.CorrelationId);
            Assert.AreEqual(ddBroker1.CorrelationId, OutputContext.CorrelationId);
            Assert.AreEqual(ddBroker1.CorrelationId, EContext1.CorrelationId);
            Assert.AreEqual(ddBroker2.CorrelationId, AsyncContext.CorrelationId);
            Assert.AreEqual(ddBroker2.CorrelationId, Context.CorrelationId);
            Assert.AreEqual(ddBroker2.CorrelationId, EContext2.CorrelationId);
            Assert.AreNotEqual(ddBroker1.CorrelationId, ddBroker2.CorrelationId);
        }
    }
}

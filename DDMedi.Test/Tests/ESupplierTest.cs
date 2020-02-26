using DDMedi.Test.Dummies;
using DDMedi.Test.Helper;
using DDMedi.Test.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DDMedi.Test.Tests
{
    public class ESupplierTest
    {
        DDMediCollection ddCollection { get; set; }
        [SetUp]
        public void Setup()
        {
            ddCollection = new DDMediCollection();
        }
        [TestCase(typeof(ArgumentNullException), null)]
        [TestCase(typeof(ArgumentException), new Type[0])]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(ESupplierTest) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(DummySupplier), typeof(SupplierTest) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(InvalidSupplier) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(InvalidSupplier2) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(InvalidESupplier) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(InvalidESupplier2) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(DummySupplier<IInputs>) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(DummyDecorator) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(DummyDecorator<IInputs>) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(IAsyncSupplier<IInputs>) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(ISupplier<IInputs>) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(IAsyncSupplier<IInputs<string>, string>) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(ISupplier<IInputs<string>, string>) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(IESupplier<IEInputs>) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(DummyESupplier<>) })]
        public void ExceptionRegisterInvalidSupplierTypesTest(Type exceptionType, params Type[] RegisterTypes)
        {
            Assert.Throws(exceptionType, () => ddCollection.AddQueue(eSuppliers => eSuppliers.AddESuppliers(RegisterTypes)));
        }
        
        [TestCase(typeof(DummyESupplier<Dummy2EInputs>), true)]
        [TestCase(typeof(DummySupplier), true)]

        [TestCase(typeof(DummyESupplier<>), false)]
        [TestCase(typeof(DummySupplier<Dummy2Inputs<DummyOutput>, DummyOutput>), false)]
        [TestCase(typeof(DummySupplier<Dummy2Inputs>), false)]
        [TestCase(typeof(InvalidSupplier), false)]
        [TestCase(typeof(InvalidSupplier2), false)]
        [TestCase(typeof(InvalidESupplier), false)]
        [TestCase(typeof(InvalidESupplier2), false)]
        [TestCase(typeof(DummyDecorator), false)]
        [TestCase(typeof(ArgumentNullException), false)]

        [TestCase(typeof(IAsyncSupplier<Dummy2Inputs<DummyOutput>, DummyOutput>), false)]
        [TestCase(typeof(IAsyncSupplier<Dummy2Inputs>), false)]
        [TestCase(typeof(ISupplier<Dummy2Inputs<DummyOutput>, DummyOutput>), false)]
        [TestCase(typeof(ISupplier<Dummy2Inputs>), false)]
        [TestCase(typeof(IESupplier<Dummy2EInputs>), false)]
        public void CheckTypeAreESuppliers(Type Type, bool AreESuppliers)
        {
            Assert.AreEqual(Type.AreESuppliers(), AreESuppliers);
        }
        [Test]
        public void RegisterAllESuppliersFromCurrentProjectTest()
        {
            var factory = ddCollection.AddQueue(eSuppliers => eSuppliers.AddESuppliers()).BuildSuppliers();
            ValidateAssemblyRegister(factory);
        }

        [Test]
        public void RegisterAllESuppliersFromAssemblyTest()
        {
            var factory = ddCollection.AddQueue(eSuppliers => eSuppliers.AddESuppliers(Assembly.GetExecutingAssembly())).BuildSuppliers();
            ValidateAssemblyRegister(factory);
        }
        void ValidateAssemblyRegister(DDMediFactory ddfactory)
        {
            Assert.IsTrue(ddfactory.BaseDescriptorCollection.Descriptors.Any(e => e.ImplementType == typeof(DummySupplier)));
            Assert.IsTrue(!ddfactory.BaseDescriptorCollection.Descriptors.Any(e => e.ImplementType == typeof(MockESupplier<>)));
        }

        [Test]
        public void ExceptionRegisterESupplierAsInterfaceOrAbstractClass()
        {
            ddCollection.AddQueue(eSuppliers =>
            {
                Assert.Throws(typeof(NotSupportedException), () => eSuppliers.AddESupplier<IESupplier<IEInputs>>());
                Assert.Throws(typeof(NotSupportedException), () => eSuppliers.AddESupplier<InvalidESupplier>());
                Assert.Throws(typeof(NotSupportedException), () => eSuppliers.AddESupplier<InvalidESupplier2>());
            });
        }
        [Test]
        public void RegisteredESupplierSameInterfaceTest()
        {
            var factory = new DDMediCollection().AddQueue(eSuppliers => eSuppliers
                .AddESupplier<MockESupplier<IEInputs>>()
                .AddESupplier<DummyESupplier<IEInputs>>()
                .AddESupplier<MockESupplier<IEInputs>>())
                .BuildSuppliers();

            var factory2 = new DDMediCollection()
                .AddQueue(eSuppliers => eSuppliers
                    .AddESupplier<MockESupplier<IEInputs>>()
                    .AddESupplier<DummyESupplier<IEInputs>>())
                .AddQueue(eSuppliers => eSuppliers
                    .AddESupplier<MockESupplier<IEInputs>>())
                .BuildSuppliers();

            var factory3 = new DDMediCollection()
                .AddQueue(eSuppliers => eSuppliers
                    .AddESupplier<MockESupplier<IEInputs>>())
                .AddQueue(eSuppliers => eSuppliers
                    .AddESupplier<DummyESupplier<IEInputs>>()
                    .AddESupplier<MockESupplier<IEInputs>>())
                .BuildSuppliers();

            ValidateESupplierSameInterfaceTest(factory, 1);
            ValidateESupplierSameInterfaceTest(factory2, 1);
            ValidateESupplierSameInterfaceTest(factory3, 2);
        }
        void ValidateESupplierSameInterfaceTest(DDMediFactory factory, int expectedQueues)
        {
            var queueFactory = factory.GetNonePublicProperty<IEInputsQueueFactory>();
            var realDescriptors = queueFactory.GetAllDescriptorDic();
            var supplierDescriptors = realDescriptors[typeof(IESupplier<>)][typeof(IEInputs)];
            var iSupplierHandler = supplierDescriptors[0];
            var queueDescriptors = queueFactory.GetNonePublicField<List<EInputsQueueDescriptor>>();
            Assert.IsTrue(factory.BaseDescriptorCollection.Descriptors.Count(e => e.RegisterType == typeof(MockESupplier<IEInputs>)) == 1);
            Assert.IsTrue(factory.BaseDescriptorCollection.Descriptors.Count(e => e.RegisterType == typeof(DummyESupplier<IEInputs>)) == 1);
            Assert.IsTrue(supplierDescriptors.Length == 2);
            Assert.IsTrue(supplierDescriptors.Count(e => e.ImplementDescriptor.RegisterType == typeof(MockESupplier<IEInputs>)) == 1);
            Assert.IsTrue(supplierDescriptors.Count(e => e.ImplementDescriptor.RegisterType == typeof(DummyESupplier<IEInputs>)) == 1);
            Assert.IsTrue(iSupplierHandler.ImplementDescriptor.RegisterType == typeof(DummyESupplier<IEInputs>));
            Assert.IsTrue(queueDescriptors.Count == expectedQueues);
            if (expectedQueues == 1) // 1 queue will contains both implement types
            {
                var eSupplierDescriptors = queueDescriptors[0].GetDescriptorDic()[typeof(IEInputs)];
                Assert.IsTrue(eSupplierDescriptors.Any(e => e.ImplementDescriptor.RegisterType == typeof(MockESupplier<IEInputs>)));
                Assert.IsTrue(eSupplierDescriptors.Any(e => e.ImplementDescriptor.RegisterType == typeof(DummyESupplier<IEInputs>)));
            }
            else if (expectedQueues == 2) // make sure that 1 esupplier only in 1 queue
            {
                var eSupplierDescriptors = queueDescriptors[0].GetDescriptorDic()[typeof(IEInputs)];
                Assert.IsTrue(eSupplierDescriptors.Any(e => e.ImplementDescriptor.RegisterType == typeof(MockESupplier<IEInputs>)));
                Assert.IsTrue(!eSupplierDescriptors.Any(e => e.ImplementDescriptor.RegisterType == typeof(DummyESupplier<IEInputs>)));
                eSupplierDescriptors = queueDescriptors[1].GetDescriptorDic()[typeof(IEInputs)];
                Assert.IsTrue(!eSupplierDescriptors.Any(e => e.ImplementDescriptor.RegisterType == typeof(MockESupplier<IEInputs>)));
                Assert.IsTrue(eSupplierDescriptors.Any(e => e.ImplementDescriptor.RegisterType == typeof(DummyESupplier<IEInputs>)));
            }
        }
        [Test]
        public void RegisteredESupplierRedundantTest()
        {
            var ddCollection = new DDMediCollection().AddQueue(eSuppliers => eSuppliers
                .AddESupplier<DummySupplier>(SupplierLifetime.Singleton)
                .AddESupplier<DummySupplier>(SupplierLifetime.Scoped));
            var ddCollection2 = new DDMediCollection().AddQueue(eSuppliers => eSuppliers
                .AddESupplier<DummySupplier>(SupplierLifetime.Scoped)
                .AddESupplier<DummySupplier>(SupplierLifetime.Singleton));
            var ddCollection3 = new DDMediCollection().AddQueue(eSuppliers => eSuppliers
                .AddESupplier<DummySupplier>(SupplierLifetime.Scoped)
                .AddESuppliers(SupplierLifetime.Singleton));
            var ddCollection4 = new DDMediCollection().AddQueue(eSuppliers => eSuppliers
                .AddESuppliers(SupplierLifetime.Singleton)
                .AddESupplier<DummySupplier>(SupplierLifetime.Scoped));
            ValidateRedundantTest(ddCollection, SupplierLifetime.Singleton);
            ValidateRedundantTest(ddCollection2, SupplierLifetime.Scoped);
            ValidateRedundantTest(ddCollection3, SupplierLifetime.Scoped);
            ValidateRedundantTest(ddCollection4, SupplierLifetime.Singleton);
        }
        void ValidateRedundantTest(DDMediCollection ddCollection, SupplierLifetime ExpectedLifeTime)
        {
            var ddMediFactory = ddCollection.BuildSuppliers();
            var queueFactory = ddMediFactory.GetNonePublicProperty<IEInputsQueueFactory>();
            var realDescriptors = queueFactory.GetAllDescriptorDic();
            var supplierDescriptors = realDescriptors[typeof(IESupplier<>)][typeof(DummyEInputs)];

            Assert.IsTrue(supplierDescriptors.Length == 1);
            Assert.IsTrue(supplierDescriptors[0].ImplementDescriptor.Lifetime == ExpectedLifeTime);

        }
    }
}

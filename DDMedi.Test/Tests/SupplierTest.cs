using DDMedi.Test.Dummies;
using DDMedi.Test.Helper;
using DDMedi.Test.Mocks;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace DDMedi.Test.Tests
{
    public class SupplierTest
    {
        DDMediCollection ddCollection { get; set; }
        [SetUp]
        public void Setup()
        {
            ddCollection = new DDMediCollection();
        }

        [TestCase(typeof(ArgumentNullException), null)]
        [TestCase(typeof(ArgumentException), new Type[0])]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(SupplierTest) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(DummySupplier), typeof(SupplierTest) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(InvalidSupplier) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(InvalidSupplier2) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(InvalidESupplier) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(InvalidESupplier2) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(DummyESupplier<IEInputs>) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(DummyDecorator) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(DummyDecorator<IInputs>) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(IAsyncSupplier<IInputs>) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(ISupplier<IInputs>) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(IAsyncSupplier<IInputs<string>, string>) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(ISupplier<IInputs<string>, string>) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(IESupplier<IEInputs>) })]
        [TestCase(typeof(NotSupportedException), new Type[] { typeof(DummySupplier<>) })]
        public void ExceptionRegisterInvalidSupplierTypesTest(Type exceptionType, params Type[] RegisterTypes)
        {
            Assert.Throws(exceptionType, () => ddCollection.AddSuppliers(RegisterTypes));
        }

        [TestCase(typeof(DummySupplier), true)]
        [TestCase(typeof(DummySupplier<Dummy2Inputs<DummyOutput>, DummyOutput>), true)]
        [TestCase(typeof(DummySupplier<Dummy2Inputs>), true)]

        [TestCase(typeof(DummySupplier<>), false)]
        [TestCase(typeof(DummyESupplier<Dummy2EInputs>), false)]
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
        public void CheckTypeAreSuppliers(Type Type, bool AreSuppliers)
        {
            Assert.AreEqual(Type.AreSuppliers(), AreSuppliers);
        }
        [Test]
        public void RegisterAllSuppliersFromCurrentProjectTest()
        {
            var factory = ddCollection.AddSuppliers().BuildSuppliers();
            ValidateAssemblyRegister(factory);
        }

        [Test]
        public void RegisterAllSuppliersFromAssemblyTest()
        {
            var factory = ddCollection.AddSuppliers(Assembly.GetExecutingAssembly()).BuildSuppliers();
            ValidateAssemblyRegister(factory);
        }
        void ValidateAssemblyRegister(DDMediFactory ddfactory)
        {
            Assert.IsTrue(ddfactory.BaseDescriptorCollection.Descriptors.Any(e => e.ImplementType == typeof(DummySupplier)));
            Assert.IsTrue(!ddfactory.BaseDescriptorCollection.Descriptors.Any(e => e.ImplementType == typeof(MockSupplier<>)));
        }
        [Test]
        public void ExceptionRegisterSupplierAsInterfaceOrAbstractClass()
        {
            Assert.Throws(typeof(NotSupportedException), () => ddCollection.AddSupplier<ISupplier<IInputs>>());
            Assert.Throws(typeof(NotSupportedException), () => ddCollection.AddSupplier<IAsyncSupplier<IInputs>>());
            Assert.Throws(typeof(NotSupportedException), () => ddCollection.AddSupplier<ISupplier<IInputs<string>, string>>());
            Assert.Throws(typeof(NotSupportedException), () => ddCollection.AddSupplier<IAsyncSupplier<IInputs<string>, string>>());
            Assert.Throws(typeof(NotSupportedException), () => ddCollection.AddSupplier<InvalidSupplier>());
            Assert.Throws(typeof(NotSupportedException), () => ddCollection.AddSupplier<InvalidSupplier2>());
        }
        [Test]
        public void RegisteredSupplierSameInterfaceTest()
        {
            var factory = ddCollection
                .AddSupplier<MockAllSupplier<IInputs>>()
                .AddSupplier<MockSupplier<IInputs>>()
                .AddSupplier<MockAllSupplier<IInputs>>()
                .BuildSuppliers();

            var supplierFactory = factory.GetNonePublicProperty<ISupplierFactory>();
            var realDescriptors = supplierFactory.GetAllDescriptorDic();
            var supplierDescriptors = realDescriptors[typeof(ISupplier<>)][typeof(IInputs)];
            var asyncSupplierDescriptors = realDescriptors[typeof(IAsyncSupplier<>)][typeof(IInputs)];
            var iSupplierHandler = supplierDescriptors[0];

            Assert.IsTrue(factory.BaseDescriptorCollection.Descriptors.Count(e => e.RegisterType == typeof(MockAllSupplier<IInputs>)) == 1);
            Assert.IsTrue(factory.BaseDescriptorCollection.Descriptors.Count(e => e.RegisterType == typeof(MockSupplier<IInputs>)) == 1);
            Assert.IsTrue(asyncSupplierDescriptors.Length == 1);
            Assert.IsTrue(supplierDescriptors.Length == 2);
            Assert.IsTrue(supplierDescriptors.Count(e => e.ImplementDescriptor.RegisterType == typeof(MockAllSupplier<IInputs>)) == 1);
            Assert.IsTrue(supplierDescriptors.Count(e => e.ImplementDescriptor.RegisterType == typeof(MockSupplier<IInputs>)) == 1);
            Assert.IsTrue(iSupplierHandler.ImplementDescriptor.RegisterType == typeof(MockSupplier<IInputs>));

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
            var supplierFactory = ddMediFactory.GetNonePublicProperty<ISupplierFactory>();
            var realDescriptors = supplierFactory.GetAllDescriptorDic();
            var supplierDescriptors = realDescriptors[typeof(ISupplier<>)][typeof(DummyInputs)];

            Assert.IsTrue(supplierDescriptors.Length == 1);
            Assert.IsTrue(supplierDescriptors.FirstOrDefault().ImplementDescriptor.Lifetime == ExpectedLifeTime);

        }
    }
}

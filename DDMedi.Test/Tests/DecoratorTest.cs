using DDMedi.Test.Dummies;
using DDMedi.Test.Helper;
using DDMedi.Test.Mocks;
using DDMedi.UnitTest.Helper;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DDMedi.Test.Tests
{
    public class DecoratorTest
    {
        DDMediCollection ddCollection { get; set; }
        [SetUp]
        public void SetUp()
        {
            ddCollection = new DDMediCollection();
        }
        [Test]
        public async Task SingleDecoratorTest()
        {
            var asyncOutputDecorator = new MockAsyncDecorator<IInputs<object>, object>();
            var mockAsyncOutputContext = new Mock<IAsyncDecoratorContext<object>>();

            var asyncDecorator = new MockAsyncDecorator<IInputs>();
            var mockAsyncContext = new Mock<IAsyncDecoratorContext>();

            var outputDecorator = new MockDecorator<IInputs<object>, object>();
            var mockOutputContext = new Mock<IDecoratorContext<object>>();

            var decorator = new MockDecorator<IInputs>();
            var mockContext = new Mock<IDecoratorContext>();

            var eDecorator = new MockEDecorator<IEInputs>(new Mock<IEDecorator<IEInputs>>());
            var mockEInputsContext = new Mock<IEDecoratorContext>();

            await asyncOutputDecorator.ProcessAsync(It.IsAny<IInputs<object>>(), mockAsyncOutputContext.Object);
            await asyncDecorator.ProcessAsync(It.IsAny<IInputs>(), mockAsyncContext.Object);
            outputDecorator.Process(It.IsAny<IInputs<object>>(), mockOutputContext.Object);
            decorator.Process(It.IsAny<IInputs>(), mockContext.Object);
            await eDecorator.ProcessAsync(It.IsAny<IEInputs>(), mockEInputsContext.Object);

            asyncOutputDecorator.VerifyNextAny(mockAsyncOutputContext, Times.Once());
            asyncDecorator.VerifyNextAny(mockAsyncContext, Times.Once());
            outputDecorator.VerifyNextAny(mockOutputContext, Times.Once());
            decorator.VerifyNextAny(mockContext, Times.Once());
            eDecorator.VerifyNextAny(mockEInputsContext, Times.Once());
        }
        
        [TestCase(typeof(IAsyncSupplier<,>), typeof(DummyDecorator), true)]
        [TestCase(typeof(IAsyncSupplier<>), typeof(DummyDecorator), true)]
        [TestCase(typeof(ISupplier<,>), typeof(DummyDecorator), true)]
        [TestCase(typeof(ISupplier<>), typeof(DummyDecorator), true)]
        [TestCase(typeof(IESupplier<>), typeof(DummyDecorator), true)]

        [TestCase(typeof(IAsyncSupplier<,>), typeof(InvalidDecorator2<>), true)]
        [TestCase(typeof(DummySupplier), typeof(DummyDecorator<>), true)]
        [TestCase(typeof(DummySupplier), typeof(DummyEDecorator<>), true)]

        [TestCase(typeof(IAsyncSupplier<,>), typeof(IAsyncDecorator<,>), true)]
        [TestCase(typeof(IAsyncSupplier<>), typeof(IAsyncDecorator<>), true)]
        [TestCase(typeof(ISupplier<,>), typeof(IDecorator<,>), true)]
        [TestCase(typeof(ISupplier<>), typeof(IDecorator<>), true)]
        [TestCase(typeof(IESupplier<>), typeof(IEDecorator<>), true)]

        [TestCase(typeof(IAsyncSupplier<,>), typeof(DummyDecorator<>), true)]
        [TestCase(typeof(IAsyncSupplier<,>), typeof(DummyEDecorator<>), true)]
        [TestCase(typeof(IAsyncSupplier<>), typeof(DummyDecorator<,>), true)]
        [TestCase(typeof(IAsyncSupplier<>), typeof(DummyEDecorator<>), true)]
        [TestCase(typeof(ISupplier<,>), typeof(DummyDecorator<>), true)]
        [TestCase(typeof(ISupplier<,>), typeof(DummyEDecorator<>), true)]
        [TestCase(typeof(ISupplier<>), typeof(DummyDecorator<,>), true)]
        [TestCase(typeof(ISupplier<>), typeof(DummyEDecorator<>), true)]
        [TestCase(typeof(IESupplier<>), typeof(DummyDecorator<>), true)]
        [TestCase(typeof(IESupplier<>), typeof(DummyDecorator<,>), true)]

        [TestCase(typeof(IAsyncDecorator<,>), typeof(DummyDecorator<>), true)]
        [TestCase(typeof(IAsyncDecorator<,>), typeof(DummyDecorator<,>), true)]
        [TestCase(typeof(IAsyncDecorator<,>), typeof(DummyEDecorator<>), true)]
        [TestCase(typeof(IAsyncDecorator<>), typeof(DummyDecorator<>), true)]
        [TestCase(typeof(IAsyncDecorator<>), typeof(DummyDecorator<,>), true)]
        [TestCase(typeof(IAsyncDecorator<>), typeof(DummyEDecorator<>), true)]
        [TestCase(typeof(IDecorator<,>), typeof(DummyDecorator<>), true)]
        [TestCase(typeof(IDecorator<,>), typeof(DummyDecorator<,>), true)]
        [TestCase(typeof(IDecorator<,>), typeof(DummyEDecorator<>), true)]
        [TestCase(typeof(IDecorator<>), typeof(DummyDecorator<>), true)]
        [TestCase(typeof(IDecorator<>), typeof(DummyDecorator<,>), true)]
        [TestCase(typeof(IDecorator<>), typeof(DummyEDecorator<>), true)]
        [TestCase(typeof(IEDecorator<>), typeof(DummyDecorator<>), true)]
        [TestCase(typeof(IEDecorator<>), typeof(DummyDecorator<,>), true)]
        [TestCase(typeof(IEDecorator<>), typeof(DummyEDecorator<>), true)]

        [TestCase(typeof(IAsyncSupplier<,>), typeof(DummySupplier<>), true)]
        [TestCase(typeof(IAsyncSupplier<,>), typeof(DummySupplier<,>), true)]
        [TestCase(typeof(IAsyncSupplier<,>), typeof(DummyESupplier<>), true)]
        [TestCase(typeof(IAsyncSupplier<>), typeof(DummySupplier<>), true)]
        [TestCase(typeof(IAsyncSupplier<>), typeof(DummySupplier<,>), true)]
        [TestCase(typeof(IAsyncSupplier<>), typeof(DummyESupplier<>), true)]
        [TestCase(typeof(ISupplier<,>), typeof(DummySupplier<>), true)]
        [TestCase(typeof(ISupplier<,>), typeof(DummySupplier<,>), true)]
        [TestCase(typeof(ISupplier<,>), typeof(DummyESupplier<>), true)]
        [TestCase(typeof(ISupplier<>), typeof(DummySupplier<>), true)]
        [TestCase(typeof(ISupplier<>), typeof(DummySupplier<,>), true)]
        [TestCase(typeof(ISupplier<>), typeof(DummyESupplier<>), true)]
        [TestCase(typeof(IESupplier<>), typeof(DummySupplier<>), true)]
        [TestCase(typeof(IESupplier<>), typeof(DummySupplier<,>), true)]
        [TestCase(typeof(IESupplier<>), typeof(DummyESupplier<>), true)]

        [TestCase(typeof(IAsyncSupplier<,>), typeof(DummyAsyncDecorator<,>), false)]
        [TestCase(typeof(IAsyncSupplier<>), typeof(DummyAsyncDecorator<>), false)]
        [TestCase(typeof(ISupplier<,>), typeof(DummyDecorator<,>), false)]
        [TestCase(typeof(ISupplier<>), typeof(DummyDecorator<>), false)]
        [TestCase(typeof(IESupplier<>), typeof(DummyEDecorator<>), false)]
        public void ExceptionOnArgumentOfAddGenericDecoratorTest(Type ServiceType, Type DecoratorType, bool WillThrow)
        {
            var exceptionType = typeof(ArgumentException);
            ddCollection.AddSupplier<DummySupplier>();
            if (WillThrow)
                Assert.Throws(exceptionType, () => ddCollection.AddGenericDecorator(ServiceType, DecoratorType));

            else
                Assert.DoesNotThrow(() => ddCollection.AddGenericDecorator(ServiceType, DecoratorType));

        }

        [Test]
        public void ExceptionDecorateUnregisteredServiceTest()
        {
            var exceptionType = typeof(InvalidOperationException);
            ddCollection.AddSupplier<Dummy3Supplier>().AddSupplier<DummySupplier<Dummy2Inputs>>();
            //register decorator for none real suppliers
            Assert.DoesNotThrow(() => ddCollection.AddDecorator<DummyInputs<DummyOutput>, DummyOutput, DummyOutputDecorator>());
            Assert.DoesNotThrow(() => ddCollection.AddDecorator<DummyInputs, DummyDecorator>());
            Assert.DoesNotThrow(() => ddCollection.AddAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput, DummyAsyncOutputDecorator>());
            Assert.DoesNotThrow(() => ddCollection.AddAsyncDecorator<DummyInputs, DummyAsyncDecorator>());
            Assert.DoesNotThrow(() => ddCollection.AddEDecorator<DummyEInputs, DummyEInputsDecorator>());

            //register decorator for any real suppliers
            Assert.DoesNotThrow(() => ddCollection.AddGenericDecorator(typeof(IAsyncSupplier<,>), typeof(DummyAsyncDecorator<,>)));
            Assert.DoesNotThrow(() => ddCollection.AddGenericDecorator(typeof(IAsyncSupplier<>), typeof(DummyAsyncDecorator<>)));
            Assert.DoesNotThrow(() => ddCollection.AddGenericDecorator(typeof(ISupplier<,>), typeof(DummyDecorator<,>)));
            Assert.DoesNotThrow(() => ddCollection.AddGenericDecorator(typeof(ISupplier<>), typeof(DummyDecorator<>)));
            Assert.DoesNotThrow(() => ddCollection.AddGenericDecorator(typeof(IESupplier<>), typeof(DummyEDecorator<>)));
            //will throw exception because there are decorators with none real suppliers
            Assert.Throws(exceptionType, () => ddCollection.BuildSuppliers());
        }
        [Test]
        public void ExceptionRegisterDecoratorAsInterfaceOrAbstractClassTest()
        {
            var exceptionType = typeof(ArgumentException);
            ddCollection.AddSupplier<Dummy2Supplier>();
            Assert.Throws(exceptionType, () => ddCollection.AddDecorator<DummyInputs<DummyOutput>, DummyOutput, IDecorator<DummyInputs<DummyOutput>, DummyOutput>>());
            Assert.Throws(exceptionType, () => ddCollection.AddDecorator<DummyInputs, IDecorator<DummyInputs>>());
            Assert.Throws(exceptionType, () => ddCollection.AddAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput, IAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput>>());
            Assert.Throws(exceptionType, () => ddCollection.AddAsyncDecorator<DummyInputs, IAsyncDecorator<DummyInputs>>());
            Assert.Throws(exceptionType, () => ddCollection.AddEDecorator<DummyEInputs, IEDecorator<DummyEInputs>>());
            Assert.Throws(exceptionType, () => ddCollection.AddDecorator<DummyInputs, InvalidDecorator2<DummyInputs>>());
            Assert.Throws(exceptionType, () => ddCollection.AddDecorator<DummyInputs, InvalidDecorator>());
        }
        [Test]
        public void InitDecoratorsTest()
        {
            var ddFactory = ddCollection
                .AddSupplier<DummySupplier>()
                .AddQueue(eSuppliers => eSuppliers.AddESupplier<DummySupplier>())
                .AddAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput, DummyAllDecorator>()
                .AddAsyncDecorator<DummyInputs, DummyAllDecorator>()
                .AddDecorator<DummyInputs<DummyOutput>, DummyOutput, DummyAllDecorator>()
                .AddDecorator<DummyInputs, DummyDecorator>()
                .AddDecorator<DummyInputs, DummyAllDecorator>()
                .AddEDecorator<DummyEInputs, DummyAllDecorator>()
                .BuildSuppliers();
            var supplierFactory = ddFactory.GetNonePublicProperty<ISupplierFactory>();
            var descriptors = supplierFactory.GetAllDescriptorDic();
            var queueFactory = ddFactory.GetNonePublicProperty<IEInputsQueueFactory>();
            var eDescriptors = queueFactory.GetAllDescriptorDic();

            Assert.IsTrue(ValidateLayers(descriptors[typeof(IAsyncSupplier<,>)][typeof(DummyInputs<DummyOutput>)][0],
                typeof(DummyAllDecorator), typeof(DummySupplier)));
            Assert.IsTrue(ValidateLayers(descriptors[typeof(IAsyncSupplier<>)][typeof(DummyInputs)][0],
                typeof(DummyAllDecorator), typeof(DummySupplier)));
            Assert.IsTrue(ValidateLayers(descriptors[typeof(ISupplier<,>)][typeof(DummyInputs<DummyOutput>)][0],
                typeof(DummyAllDecorator), typeof(DummySupplier)));
            Assert.IsTrue(ValidateLayers(descriptors[typeof(ISupplier<>)][typeof(DummyInputs)][0],
                typeof(DummyAllDecorator), typeof(DummyDecorator), typeof(DummySupplier)));
            Assert.IsTrue(ValidateLayers(eDescriptors[typeof(IESupplier<>)][typeof(DummyEInputs)][0],
                typeof(DummyAllDecorator), typeof(DummySupplier)));
        }
        bool ValidateLayers(SupplierDescriptor descriptor, params Type[] layers)
        {
            int i = 0;
            do
            {
                if (descriptor.ImplementDescriptor.RegisterType != layers[i++])
                    return false;
                descriptor = descriptor.Next;
            }
            while (descriptor != null);
            return true;
        }
        [Test]
        public void InitGenericDecoratorsTest()
        {
            var ddFactory = ddCollection
                .AddSupplier<DummySupplier>()
                .AddQueue(eSuppliers => eSuppliers.AddESupplier<DummySupplier>())
                .AddGenericDecorator(typeof(IAsyncSupplier<,>), typeof(DummyAsyncDecorator<,>))
                .AddGenericDecorator(typeof(ISupplier<>), typeof(DummyDecorator<>))
                .AddGenericDecorator(typeof(IAsyncSupplier<>), typeof(DummyAsyncDecorator<>))
                .AddGenericDecorator(typeof(ISupplier<,>), typeof(DummyDecorator<,>))
                .AddGenericDecorator(typeof(IAsyncSupplier<,>), typeof(DummyAllDecorator<,>))
                .AddGenericDecorator(typeof(ISupplier<>), typeof(DummyAllDecorator<>))
                .AddGenericDecorator(typeof(IAsyncSupplier<>), typeof(DummyAllDecorator<>))
                .AddGenericDecorator(typeof(ISupplier<,>), typeof(DummyAllDecorator<,>))
                .AddGenericDecorator(typeof(IESupplier<>), typeof(DummyEDecorator<>))
                .BuildSuppliers();
            var supplierFactory = ddFactory.GetNonePublicProperty<ISupplierFactory>();
            var descriptors = supplierFactory.GetAllDescriptorDic();
            var queueFactory = ddFactory.GetNonePublicProperty<IEInputsQueueFactory>();
            var eDescriptors = queueFactory.GetAllDescriptorDic();


            Assert.IsTrue(ValidateLayers(descriptors[typeof(IAsyncSupplier<,>)][typeof(DummyInputs<DummyOutput>)][0],
                typeof(DummyAllDecorator<DummyInputs<DummyOutput>, DummyOutput>),
                typeof(DummyAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput>),
                typeof(DummySupplier)));
            Assert.IsTrue(ValidateLayers(descriptors[typeof(IAsyncSupplier<>)][typeof(DummyInputs)][0],
                typeof(DummyAllDecorator<DummyInputs>),
                typeof(DummyAsyncDecorator<DummyInputs>),
                typeof(DummySupplier)));
            Assert.IsTrue(ValidateLayers(descriptors[typeof(ISupplier<,>)][typeof(DummyInputs<DummyOutput>)][0],
                typeof(DummyAllDecorator<DummyInputs<DummyOutput>, DummyOutput>),
                typeof(DummyDecorator<DummyInputs<DummyOutput>, DummyOutput>),
                typeof(DummySupplier)));
            Assert.IsTrue(ValidateLayers(descriptors[typeof(ISupplier<>)][typeof(DummyInputs)][0],
                typeof(DummyAllDecorator<DummyInputs>),
                typeof(DummyDecorator<DummyInputs>),
                typeof(DummySupplier)));
            Assert.IsTrue(ValidateLayers(eDescriptors[typeof(IESupplier<>)][typeof(DummyEInputs)][0],
                typeof(DummyEDecorator<DummyEInputs>),
                typeof(DummySupplier)));
        }
        [Test]
        public void RegisterDecoratorsWithConditionTest()
        {
            var ddFactory = ddCollection
                .AddSupplier<DummySupplier<Dummy2Inputs>>()
                .AddSupplier<DummySupplier>()
                .AddGenericDecorator(typeof(ISupplier<>), typeof(DummyDecorator<>))
                .AddGenericDecorator(typeof(IAsyncSupplier<>), typeof(DummyAsyncDecorator<>),
                    info =>
                    {
                        Assert.IsTrue(
                            info.DecoratorImplementType == typeof(DummyAsyncDecorator<Dummy2Inputs>)||
                            info.DecoratorImplementType == typeof(DummyAsyncDecorator<DummyInputs>));
                        Assert.IsTrue(info.AppliedLayers.Count == 1);
                        Assert.IsTrue(
                            info.AppliedLayers.Contains(typeof(DummySupplier<Dummy2Inputs>)) ||
                            info.AppliedLayers.Contains(typeof(DummySupplier)));
                        return info.ISupplierType != typeof(IAsyncSupplier<DummyInputs>);
                    })
                .AddGenericDecorator(typeof(IESupplier<>), typeof(DummyEDecorator<>))
            .BuildSuppliers();
            var supplierFactory = ddFactory.GetNonePublicProperty<ISupplierFactory>();
            var descriptors = supplierFactory.GetAllDescriptorDic();
            Assert.IsTrue(descriptors[typeof(ISupplier<>)][typeof(DummyInputs)][0]
                .ImplementDescriptor.RegisterType == typeof(DummyDecorator<DummyInputs>));
            Assert.IsFalse(descriptors[typeof(IAsyncSupplier<>)][typeof(DummyInputs)][0]
                .ImplementDescriptor.RegisterType == typeof(DummyAsyncDecorator<DummyInputs>));
            Assert.IsTrue(descriptors[typeof(IAsyncSupplier<>)][typeof(Dummy2Inputs)][0]
                .ImplementDescriptor.RegisterType == typeof(DummyAsyncDecorator<Dummy2Inputs>));
        }
        [Test]
        public void WrapAllSuppliersTest()
        {
            var ddFactory = ddCollection
                .AddSupplier<DummySupplier<DummyInputs>>(SupplierLifetime.Singleton)
                .AddSupplier<DummySupplier>(SupplierLifetime.Singleton)
                .AddSupplier<DummySupplier<DummyInputs<DummyOutput>, DummyOutput>>()
                .AddQueue(eSuppliers => eSuppliers.AddESupplier<DummySupplier>())
                .AddDecorator<DummyInputs, DummyDecorator>()
                .AddDecorator<DummyInputs<DummyOutput>, DummyOutput, DummyOutputDecorator>()
                .AddAsyncDecorator<DummyInputs, DummyAsyncDecorator>()
                .AddAsyncDecorator<DummyInputs<DummyOutput>, DummyOutput, DummyAsyncOutputDecorator>()
                .AddEDecorator<DummyEInputs, DummyEDecorator<DummyEInputs>>()
                .BuildSuppliers();
            var supplierFactory = ddFactory.GetNonePublicProperty<ISupplierFactory>();
            var descriptors = supplierFactory.GetAllDescriptorDic();
            var queueFactory = ddFactory.GetNonePublicProperty<IEInputsQueueFactory>();
            var eDescriptors = queueFactory.GetAllDescriptorDic();

            Assert.IsTrue(descriptors[typeof(IAsyncSupplier<,>)][typeof(DummyInputs<DummyOutput>)][0]
                .ImplementDescriptor.RegisterType == typeof(DummyAsyncOutputDecorator));
            Assert.IsTrue(descriptors[typeof(IAsyncSupplier<>)][typeof(DummyInputs)][0]
                .ImplementDescriptor.RegisterType == typeof(DummyAsyncDecorator));
            Assert.IsTrue(descriptors[typeof(ISupplier<,>)][typeof(DummyInputs<DummyOutput>)][0]
                .ImplementDescriptor.RegisterType == typeof(DummyOutputDecorator));
            Assert.IsTrue(descriptors[typeof(ISupplier<>)][typeof(DummyInputs)][0]
                .ImplementDescriptor.RegisterType == typeof(DummyDecorator));
            Assert.IsTrue(eDescriptors[typeof(IESupplier<>)][typeof(DummyEInputs)][0]
                .ImplementDescriptor.RegisterType == typeof(DummyEDecorator<DummyEInputs>));
        }
        [Test]
        public void TestMixedSupplierInstanceScope()
        {
            var expectedLifetime = SupplierLifetime.Scoped;
            var ddFactory = ddCollection
                .AddSupplier<SuperDummySupplier>(expectedLifetime)
                .AddQueue(eSuppliers => eSuppliers.AddESupplier<SuperDummySupplier>(SupplierLifetime.Singleton))
                .AddAsyncDecorator<IInputs, MockAsyncDecorator<IInputs>>()
                .AddEDecorator<IEInputs, MockEDecorator<IEInputs>>()
                .BuildSuppliers();

            Assert.IsTrue(ddFactory.BaseDescriptorCollection.Descriptors.Count == 6);
            var descriptor1 = ddFactory.BaseDescriptorCollection.Descriptors[3];
            var descriptor2 = ddFactory.BaseDescriptorCollection.Descriptors[4];
            var descriptor3 = ddFactory.BaseDescriptorCollection.Descriptors[5];
            Assert.IsTrue(descriptor1.RegisterType == typeof(SuperDummySupplier));
            Assert.IsTrue(descriptor1.Lifetime == expectedLifetime);
            Assert.IsTrue(descriptor2.RegisterType == typeof(MockAsyncDecorator<IInputs>));
            Assert.IsTrue(descriptor2.Lifetime == expectedLifetime);
            Assert.IsTrue(descriptor3.RegisterType == typeof(MockEDecorator<IEInputs>));
            Assert.IsTrue(descriptor3.Lifetime == expectedLifetime);
        }
    }
}

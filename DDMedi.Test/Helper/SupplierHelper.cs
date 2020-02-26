using Moq;
using Moq.Language.Flow;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi.Test.Helper
{
    public static class SupplierHelper
    {
        public static ISetup<IAsyncSupplier<TInputs, TOutput>, Task<TOutput>> SetUpProcessAsyncAny<TInputs, TOutput>(this Mock<IAsyncSupplier<TInputs, TOutput>> supplier) where TInputs : IInputs<TOutput>
        => supplier.Setup(item => item.ProcessAsync(It.IsAny<ISupplierContext<TInputs>>(), It.IsAny<CancellationToken>()));
        public static ISetup<IAsyncSupplier<TInputs>, Task> SetUpProcessAsyncAny<TInputs>(this Mock<IAsyncSupplier<TInputs>> supplier) where TInputs : IInputs
        => supplier.Setup(item => item.ProcessAsync(It.IsAny<ISupplierContext<TInputs>>(), It.IsAny<CancellationToken>()));
        public static ISetup<ISupplier<TInputs,TOutput>, TOutput> SetUpProcessAny<TInputs, TOutput>(this Mock<ISupplier<TInputs, TOutput>> supplier) where TInputs : IInputs<TOutput>
        => supplier.Setup(item => item.Process(It.IsAny<ISupplierContext<TInputs>>()));
        public static ISetup<ISupplier<TInputs>> SetUpProcessAny<TInputs>(this Mock<ISupplier<TInputs>> supplier) where TInputs : IInputs
        => supplier.Setup(item => item.Process(It.IsAny<ISupplierContext<TInputs>>()));
        public static ISetup<IESupplier<TEInputs>, Task> SetUpProcessAsyncAny<TEInputs>(this Mock<IESupplier<TEInputs>> supplier) where TEInputs : IEInputs
        => supplier.Setup(item => item.ProcessAsync(It.IsAny<ISupplierContext<TEInputs>>(), It.IsAny<CancellationToken>()));

        public static void VerifyProcessAsyncAny<TInputs, TOutput>(this Mock<IAsyncSupplier<TInputs, TOutput>> supplier, Times times) where TInputs : IInputs<TOutput>
        => supplier.Verify(item => item.ProcessAsync(It.IsAny<ISupplierContext<TInputs>>(), It.IsAny<CancellationToken>()), times);
        public static void VerifyProcessAsyncAny<TInputs>(this Mock<IAsyncSupplier<TInputs>> supplier, Times times) where TInputs : IInputs
        => supplier.Verify(item => item.ProcessAsync(It.IsAny<ISupplierContext<TInputs>>(), It.IsAny<CancellationToken>()), times);
        public static void VerifyProcessAny<TInputs>(this Mock<ISupplier<TInputs>> supplier, Times times) where TInputs : IInputs
        => supplier.Verify(item => item.Process(It.IsAny< ISupplierContext<TInputs>>()), times);
        public static void VerifyProcessAny<TInputs, TOutput>(this Mock<ISupplier<TInputs, TOutput>> supplier, Times times) where TInputs : IInputs<TOutput>
        => supplier.Verify(item => item.Process(It.IsAny< ISupplierContext<TInputs>>()), times);
        public static void VerifyProcessAsyncAny<TEInputs>(this Mock<IESupplier<TEInputs>> supplier, Times times) where TEInputs : IEInputs
        => supplier.Verify(item => item.ProcessAsync(It.IsAny<ISupplierContext<TEInputs>>(), It.IsAny<CancellationToken>()), times);
    }
}

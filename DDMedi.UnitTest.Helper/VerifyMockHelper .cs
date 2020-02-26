using Moq;
using System.Threading;

namespace DDMedi.UnitTest.Helper
{
    public static class VerifyMockHelper
    {
        public static void VerifyProcessAsyncAny<TInputs, TOutput>(this Mock<IDDBroker> ddBroker, Times times) where TInputs : IInputs<TOutput>
        => ddBroker.Verify(item => item.ProcessAsync<TInputs, TOutput>(It.IsAny<TInputs>(), It.IsAny<CancellationToken>()), times);
        public static void VerifyProcessAsyncAny<TInputs>(this Mock<IDDBroker> ddBroker, Times times) where TInputs : IInputs
        => ddBroker.Verify(item => item.ProcessAsync(It.IsAny<TInputs>(), It.IsAny<CancellationToken>()), times);
        public static void VerifyProcessAny<TInputs, TOutput>(this Mock<IDDBroker> ddBroker, Times times) where TInputs : IInputs<TOutput>
        => ddBroker.Verify(item => item.Process<TInputs, TOutput>(It.IsAny<TInputs>()), times);
        public static void VerifyProcessAny<TInputs>(this Mock<IDDBroker> ddBroker, Times times) where TInputs : IInputs
        => ddBroker.Verify(item => item.Process(It.IsAny<TInputs>()), times);

        public static void VerifyCreateAsyncSupplierChannel<TInputs, TOutput>(this Mock<IDDBroker> ddBroker, Times times) where TInputs : IInputs<TOutput>
        => ddBroker.Verify(item => item.CreateAsyncSupplierChannel<TInputs, TOutput>(), times);
        public static void VerifyCreateAsyncSupplierChannel<TInputs>(this Mock<IDDBroker> ddBroker, Times times) where TInputs : IInputs
        => ddBroker.Verify(item => item.CreateAsyncSupplierChannel<TInputs>(), times);
        public static void VerifyCreateSupplierChannel<TInputs, TOutput>(this Mock<IDDBroker> ddBroker, Times times) where TInputs : IInputs<TOutput>
        => ddBroker.Verify(item => item.CreateSupplierChannel<TInputs, TOutput>(), times);
        public static void VerifyCreateSupplierChannel<TInputs>(this Mock<IDDBroker> ddBroker, Times times) where TInputs : IInputs
        => ddBroker.Verify(item => item.CreateSupplierChannel<TInputs>(), times);

        public static void VerifyAsyncProcessAny<TInputs, TOutput>(this Mock<IAsyncSupplierChannel<TInputs, TOutput>> channel, Times times) where TInputs : IInputs<TOutput>
        => channel.Verify(item => item.ProcessAsync(It.IsAny<TInputs>(), It.IsAny<CancellationToken>()), times);
        public static void VerifyAsyncProcessAny<TInputs>(this Mock<IAsyncSupplierChannel<TInputs>> channel, Times times) where TInputs : IInputs
        => channel.Verify(item => item.ProcessAsync(It.IsAny<TInputs>(), It.IsAny<CancellationToken>()), times);
        public static void VerifyProcessAny<TInputs, TOutput>(this Mock<ISupplierChannel<TInputs, TOutput>> channel, Times times) where TInputs : IInputs<TOutput>
        => channel.Verify(item => item.Process(It.IsAny<TInputs>()), times);
        public static void VerifyProcessAny<TInputs>(this Mock<ISupplierChannel<TInputs>> channel, Times times) where TInputs : IInputs
        => channel.Verify(item => item.Process(It.IsAny<TInputs>()), times);

        public static void VerifyNextAny<TInputs, TOutput>(this IAsyncDecorator<TInputs, TOutput> decorator, Mock<IAsyncDecoratorContext<TInputs, TOutput>> context, Times times) where TInputs : IInputs<TOutput>
        => context.Verify(item => item.Next(It.IsAny<CancellationToken>()), times);
        public static void VerifyNextAny<TInputs>(this IAsyncDecorator<TInputs> decorator, Mock<IAsyncDecoratorContext<TInputs>> context, Times times) where TInputs : IInputs
        => context.Verify(item => item.Next(It.IsAny<CancellationToken>()), times);
        public static void VerifyNextAny<TInputs, TOutput>(this IDecorator<TInputs, TOutput> decorator, Mock<IDecoratorContext<TInputs, TOutput>> context, Times times) where TInputs : IInputs<TOutput>
        => context.Verify(item => item.Next(), times);
        public static void VerifyNextAny<TInputs>(this IDecorator<TInputs> decorator, Mock<IDecoratorContext<TInputs>> context, Times times) where TInputs : IInputs
        => context.Verify(item => item.Next(), times);
        public static void VerifyNextAny<TEInputs>(this IEDecorator<TEInputs> decorator, Mock<IEDecoratorContext<TEInputs>> context, Times times) where TEInputs : IEInputs
        => context.Verify(item => item.Next(It.IsAny<CancellationToken>()), times);
    }
}

using Moq;
using Moq.Language.Flow;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi.Test.Helper
{
    public static class DecoratorHelper
    {

        public static ISetup<IAsyncDecorator<TInputs, TOutput>, Task<TOutput>> SetUpProcessAsyncAny<TInputs, TOutput>(this Mock<IAsyncDecorator<TInputs, TOutput>> Decorator) where TInputs : IInputs<TOutput>
        => Decorator.Setup(item => item.ProcessAsync(It.IsAny<TInputs>(), It.IsAny<IAsyncDecoratorContext<TOutput>>(), It.IsAny<CancellationToken>()));
        public static ISetup<IAsyncDecorator<TInputs>, Task> SetUpProcessAsyncAny<TInputs>(this Mock<IAsyncDecorator<TInputs>> Decorator) where TInputs : IInputs
        => Decorator.Setup(item => item.ProcessAsync(It.IsAny<TInputs>(), It.IsAny<IAsyncDecoratorContext>(), It.IsAny<CancellationToken>()));
        public static ISetup<IDecorator<TInputs, TOutput>, TOutput> SetUpProcessAny<TInputs, TOutput>(this Mock<IDecorator<TInputs, TOutput>> Decorator) where TInputs : IInputs<TOutput>
        => Decorator.Setup(item => item.Process(It.IsAny<TInputs>(), It.IsAny<IDecoratorContext<TOutput>>()));
        public static ISetup<IDecorator<TInputs>> SetUpProcessAny<TInputs>(this Mock<IDecorator<TInputs>> Decorator) where TInputs : IInputs
        => Decorator.Setup(item => item.Process(It.IsAny<TInputs>(), It.IsAny<IDecoratorContext>()));
        public static ISetup<IEDecorator<TEInputs>, Task> SetUpProcessAsyncAny<TEInputs>(this Mock<IEDecorator<TEInputs>> Decorator) where TEInputs : IEInputs
                => Decorator.Setup(item => item.ProcessAsync(It.IsAny<TEInputs>(), It.IsAny<IEDecoratorContext>(), It.IsAny<CancellationToken>()));

        public static void VerifyProcessAsyncAny<TInputs, TOutput>(this Mock<IAsyncDecorator<TInputs, TOutput>> decorator, Times times) where TInputs : IInputs<TOutput>
        => decorator.Verify(item => item.ProcessAsync(It.IsAny<TInputs>(), It.IsAny<IAsyncDecoratorContext<TOutput>>(), It.IsAny<CancellationToken>()), times);
        public static void VerifyProcessAsyncAny<TInputs>(this Mock<IAsyncDecorator<TInputs>> decorator, Times times) where TInputs : IInputs
        => decorator.Verify(item => item.ProcessAsync(It.IsAny<TInputs>(), It.IsAny<IAsyncDecoratorContext>(), It.IsAny<CancellationToken>()), times);
        public static void VerifyProcessAny<TInputs>(this Mock<IDecorator<TInputs>> decorator, Times times) where TInputs : IInputs
        => decorator.Verify(item => item.Process(It.IsAny<TInputs>(), It.IsAny< IDecoratorContext>()), times);
        public static void VerifyProcessAny<TInputs, TOutput>(this Mock<IDecorator<TInputs, TOutput>> decorator, Times times) where TInputs : IInputs<TOutput>
        => decorator.Verify(item => item.Process(It.IsAny<TInputs>(), It.IsAny< IDecoratorContext<TOutput>>()), times);
        public static void VerifyProcessAsyncAny<TEInputs>(this Mock<IEDecorator<TEInputs>> decorator, Times times) where TEInputs : IEInputs
            => decorator.Verify(item => item.ProcessAsync(It.IsAny<TEInputs>(), It.IsAny<IEDecoratorContext>(), It.IsAny<CancellationToken>()), times);
    }
}

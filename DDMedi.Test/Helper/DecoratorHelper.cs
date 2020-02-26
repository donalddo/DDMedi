using Moq;
using Moq.Language.Flow;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi.Test.Helper
{
    public static class DecoratorHelper
    {

        public static ISetup<IAsyncDecorator<TInputs, TOutput>, Task<TOutput>> SetUpProcessAsyncAny<TInputs, TOutput>(this Mock<IAsyncDecorator<TInputs, TOutput>> Decorator) where TInputs : IInputs<TOutput>
        => Decorator.Setup(item => item.ProcessAsync(It.IsAny<IAsyncDecoratorContext<TInputs, TOutput>>(), It.IsAny<CancellationToken>()));
        public static ISetup<IAsyncDecorator<TInputs>, Task> SetUpProcessAsyncAny<TInputs>(this Mock<IAsyncDecorator<TInputs>> Decorator) where TInputs : IInputs
        => Decorator.Setup(item => item.ProcessAsync(It.IsAny<IAsyncDecoratorContext<TInputs>>(), It.IsAny<CancellationToken>()));
        public static ISetup<IDecorator<TInputs, TOutput>, TOutput> SetUpProcessAny<TInputs, TOutput>(this Mock<IDecorator<TInputs, TOutput>> Decorator) where TInputs : IInputs<TOutput>
        => Decorator.Setup(item => item.Process(It.IsAny<IDecoratorContext<TInputs, TOutput>>()));
        public static ISetup<IDecorator<TInputs>> SetUpProcessAny<TInputs>(this Mock<IDecorator<TInputs>> Decorator) where TInputs : IInputs
        => Decorator.Setup(item => item.Process(It.IsAny<IDecoratorContext<TInputs>>()));
        public static ISetup<IEDecorator<TEInputs>, Task> SetUpProcessAsyncAny<TEInputs>(this Mock<IEDecorator<TEInputs>> Decorator) where TEInputs : IEInputs
                => Decorator.Setup(item => item.ProcessAsync(It.IsAny<IEDecoratorContext<TEInputs>>(), It.IsAny<CancellationToken>()));

        public static void VerifyProcessAsyncAny<TInputs, TOutput>(this Mock<IAsyncDecorator<TInputs, TOutput>> decorator, Times times) where TInputs : IInputs<TOutput>
        => decorator.Verify(item => item.ProcessAsync(It.IsAny<IAsyncDecoratorContext<TInputs, TOutput>>(), It.IsAny<CancellationToken>()), times);
        public static void VerifyProcessAsyncAny<TInputs>(this Mock<IAsyncDecorator<TInputs>> decorator, Times times) where TInputs : IInputs
        => decorator.Verify(item => item.ProcessAsync(It.IsAny<IAsyncDecoratorContext<TInputs>>(), It.IsAny<CancellationToken>()), times);
        public static void VerifyProcessAny<TInputs>(this Mock<IDecorator<TInputs>> decorator, Times times) where TInputs : IInputs
        => decorator.Verify(item => item.Process(It.IsAny< IDecoratorContext<TInputs>>()), times);
        public static void VerifyProcessAny<TInputs, TOutput>(this Mock<IDecorator<TInputs, TOutput>> decorator, Times times) where TInputs : IInputs<TOutput>
        => decorator.Verify(item => item.Process(It.IsAny< IDecoratorContext<TInputs, TOutput>>()), times);
        public static void VerifyProcessAsyncAny<TEInputs>(this Mock<IEDecorator<TEInputs>> decorator, Times times) where TEInputs : IEInputs
            => decorator.Verify(item => item.ProcessAsync(It.IsAny<IEDecoratorContext<TEInputs>>(), It.IsAny<CancellationToken>()), times);
    }
}

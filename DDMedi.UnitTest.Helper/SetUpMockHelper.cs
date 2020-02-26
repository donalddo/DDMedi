using Moq;
using Moq.Language.Flow;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi.UnitTest.Helper
{
    public static class SetUpMockHelper
    {
        public static ISetup<IDDBroker, Task<TOutput>> SetUpProcessAsyncAny<TInputs, TOutput>(this Mock<IDDBroker> ddBroker) where TInputs : IInputs<TOutput>
        => ddBroker.Setup(item => item.ProcessAsync<TInputs, TOutput>(It.IsAny<TInputs>(), It.IsAny<CancellationToken>()));
        public static ISetup<IDDBroker, Task> SetUpProcessAsyncAny<TInputs>(this Mock<IDDBroker> ddBroker) where TInputs : IInputs
        => ddBroker.Setup(item => item.ProcessAsync(It.IsAny<TInputs>(), It.IsAny<CancellationToken>()));
        public static ISetup<IDDBroker, TOutput> SetUpProcessAny<TInputs, TOutput>(this Mock<IDDBroker> ddBroker) where TInputs : IInputs<TOutput>
        => ddBroker.Setup(item => item.Process<TInputs, TOutput>(It.IsAny<TInputs>()));
        public static ISetup<IDDBroker> SetUpProcessAny<TInputs>(this Mock<IDDBroker> ddBroker) where TInputs : IInputs
        => ddBroker.Setup(item => item.Process(It.IsAny<TInputs>()));

        public static void SetUpDDBrokerAndContext<TInputs>(out Mock<IDDBroker> ddBroker, out Mock<ISupplierContext<TInputs>> context)
        {
            context = new Mock<ISupplierContext<TInputs>>();
            ddBroker = new Mock<IDDBroker>();
            context.Setup(e => e.DDBroker).Returns(ddBroker.Object);
        }
        public static void SetUpDDBrokerAndContext<TInputs>(this TInputs inputs, out Mock<IDDBroker> ddBroker, out Mock<ISupplierContext<TInputs>> context)
        {
            SetUpDDBrokerAndContext(out ddBroker, out context);
            context.Setup(e => e.Inputs).Returns(inputs);
        }
        
        public static void SetUpDDBrokerAndContext<TInputs, TOutput>(this TInputs inputs, out Mock<IDDBroker> ddBroker, out Mock<IAsyncDecoratorContext<TInputs, TOutput>> context)
            where TInputs : IInputs<TOutput>
        {
            context = new Mock<IAsyncDecoratorContext<TInputs, TOutput>>();
            ddBroker = new Mock<IDDBroker>();
            context.Setup(e => e.DDBroker).Returns(ddBroker.Object);
            context.Setup(e => e.Inputs).Returns(inputs);
        }
        public static void SetUpDDBrokerAndContext<TInputs>(this TInputs inputs, out Mock<IDDBroker> ddBroker, out Mock<IAsyncDecoratorContext<TInputs>> context)
            where TInputs : IInputs
        {
            context = new Mock<IAsyncDecoratorContext<TInputs>>();
            ddBroker = new Mock<IDDBroker>();
            context.Setup(e => e.DDBroker).Returns(ddBroker.Object);
            context.Setup(e => e.Inputs).Returns(inputs);
        }
        public static void SetUpDDBrokerAndContext<TInputs, TOutput>(this TInputs inputs, out Mock<IDDBroker> ddBroker, out Mock<IDecoratorContext<TInputs, TOutput>> context)
            where TInputs : IInputs<TOutput>
        {
            context = new Mock<IDecoratorContext<TInputs, TOutput>>();
            ddBroker = new Mock<IDDBroker>();
            context.Setup(e => e.DDBroker).Returns(ddBroker.Object);
            context.Setup(e => e.Inputs).Returns(inputs);
        }
        public static void SetUpDDBrokerAndContext<TInputs>(this TInputs inputs, out Mock<IDDBroker> ddBroker, out Mock<IDecoratorContext<TInputs>> context)
            where TInputs : IInputs
        {
            context = new Mock<IDecoratorContext<TInputs>>();
            ddBroker = new Mock<IDDBroker>();
            context.Setup(e => e.DDBroker).Returns(ddBroker.Object);
            context.Setup(e => e.Inputs).Returns(inputs);
        }
        public static void SetUpDDBrokerAndContext<TEInputs>(this TEInputs inputs, out Mock<IDDBroker> ddBroker, out Mock<IEDecoratorContext<TEInputs>> context)
            where TEInputs : IEInputs
        {
            context = new Mock<IEDecoratorContext<TEInputs>>();
            ddBroker = new Mock<IDDBroker>();
            context.Setup(e => e.DDBroker).Returns(ddBroker.Object);
            context.Setup(e => e.Inputs).Returns(inputs);
        }

        public static Mock<IAsyncSupplierChannel<TInputs, TOutput>> SetUpAsyncSupplierChannelAny<TInputs, TOutput>(this Mock<IDDBroker> ddBroker, TOutput output) where TInputs : IInputs<TOutput>
        {
            var mockSupplierChannel = new Mock<IAsyncSupplierChannel<TInputs, TOutput>>();
            mockSupplierChannel.Setup(e => e.ProcessAsync(It.IsAny<TInputs>(), It.IsAny<CancellationToken>())).ReturnsAsync(output);
            ddBroker.Setup(item => item.CreateAsyncSupplierChannel<TInputs, TOutput>()).Returns(mockSupplierChannel.Object);
            return mockSupplierChannel;
        }
        public static Mock<IAsyncSupplierChannel<TInputs>> SetUpAsyncSupplierChannelAny<TInputs>(this Mock<IDDBroker> ddBroker) where TInputs : IInputs
        {
            var mockSupplierChannel = new Mock<IAsyncSupplierChannel<TInputs>>();
            mockSupplierChannel.Setup(e => e.ProcessAsync(It.IsAny<TInputs>(), It.IsAny<CancellationToken>()));
            ddBroker.Setup(item => item.CreateAsyncSupplierChannel<TInputs>()).Returns(mockSupplierChannel.Object);
            return mockSupplierChannel;
        }
        public static Mock<ISupplierChannel<TInputs, TOutput>> SetUpSupplierChannelAny<TInputs, TOutput>(this Mock<IDDBroker> ddBroker, TOutput output) where TInputs : IInputs<TOutput>
        {
            var mockSupplierChannel = new Mock<ISupplierChannel<TInputs, TOutput>>();
            mockSupplierChannel.Setup(e => e.Process(It.IsAny<TInputs>())).Returns(output);
            ddBroker.Setup(item => item.CreateSupplierChannel<TInputs, TOutput>()).Returns(mockSupplierChannel.Object);
            return mockSupplierChannel;
        }
        public static Mock<ISupplierChannel<TInputs>> SetUpSupplierChannelAny<TInputs>(this Mock<IDDBroker> ddBroker) where TInputs : IInputs
        {
            var mockSupplierChannel = new Mock<ISupplierChannel<TInputs>>();
            mockSupplierChannel.Setup(e => e.Process(It.IsAny<TInputs>()));
            ddBroker.Setup(item => item.CreateSupplierChannel<TInputs>()).Returns(mockSupplierChannel.Object);
            return mockSupplierChannel;
        }

        public static ISetup<IAsyncDecoratorContext<TInputs,TOutput>,Task<TOutput>> SetUpNextAny<TInputs, TOutput>(this IAsyncDecorator<TInputs, TOutput> decorator, Mock<IAsyncDecoratorContext<TInputs, TOutput>> context) where TInputs : IInputs<TOutput>
        => context.Setup(item => item.Next( It.IsAny<CancellationToken>()));
        public static ISetup<IAsyncDecoratorContext<TInputs>, Task> SetUpNextAny<TInputs>(this IAsyncDecorator<TInputs> decorator, Mock<IAsyncDecoratorContext<TInputs>> context) where TInputs : IInputs
        => context.Setup(item => item.Next(It.IsAny<CancellationToken>()));
        public static ISetup<IDecoratorContext<TInputs, TOutput>, TOutput> SetUpNextAny<TInputs, TOutput>(this IDecorator<TInputs, TOutput> decorator, Mock<IDecoratorContext<TInputs, TOutput>> context) where TInputs : IInputs<TOutput>
        => context.Setup(item => item.Next());
        public static ISetup<IDecoratorContext<TInputs>> SetUpNextAny<TInputs>(this IDecorator<TInputs> decorator, Mock<IDecoratorContext<TInputs>> context) where TInputs : IInputs
        => context.Setup(item => item.Next());
        public static ISetup<IEDecoratorContext<TEInputs>, Task> SetUpNextAny<TEInputs>(this IEDecorator<TEInputs> decorator, Mock<IEDecoratorContext<TEInputs>> context) where TEInputs : IEInputs
        => context.Setup(item => item.Next(It.IsAny<CancellationToken>()));
    }
}

using Moq;
using Moq.Language.Flow;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi.UnitTest.Helper
{
    public static class SetUpMockHelper
    {
        public static ISetup<IDDBroker, Task<TOutput>> SetUpProcessAsyncAny<TInputs, TOutput>(this Mock<IDDBroker> ddBroker) where TInputs : class
        => ddBroker.Setup(item => item.ProcessAsync<TInputs, TOutput>(It.IsAny<TInputs>(), It.IsAny<CancellationToken>()));
        public static ISetup<IDDBroker, Task> SetUpProcessAsyncAny<TInputs>(this Mock<IDDBroker> ddBroker) where TInputs : class
        => ddBroker.Setup(item => item.ProcessAsync(It.IsAny<TInputs>(), It.IsAny<CancellationToken>()));
        public static ISetup<IDDBroker, TOutput> SetUpProcessAny<TInputs, TOutput>(this Mock<IDDBroker> ddBroker) where TInputs : class
        => ddBroker.Setup(item => item.Process<TInputs, TOutput>(It.IsAny<TInputs>()));
        public static ISetup<IDDBroker> SetUpProcessAny<TInputs>(this Mock<IDDBroker> ddBroker) where TInputs : class
        => ddBroker.Setup(item => item.Process(It.IsAny<TInputs>()));

        public static void SetUpDDBrokerAndContext(out Mock<IDDBroker> ddBroker, out Mock<ISupplierContext> context)
        {
            context = new Mock<ISupplierContext>();
            ddBroker = new Mock<IDDBroker>();
            context.Setup(e => e.DDBroker).Returns(ddBroker.Object);
        }
        public static void SetUpDDBrokerAndContext<TInputs>(this TInputs inputs, out Mock<IDDBroker> ddBroker, out Mock<ISupplierContext> context)
        {
            SetUpDDBrokerAndContext(out ddBroker, out context);
        }
        
        public static void SetUpDDBrokerAndContext<TInputs, TOutput>(this TInputs inputs, out Mock<IDDBroker> ddBroker, out Mock<IAsyncDecoratorContext<TOutput>> context)
            where TInputs : class
        {
            context = new Mock<IAsyncDecoratorContext<TOutput>>();
            ddBroker = new Mock<IDDBroker>();
            context.Setup(e => e.DDBroker).Returns(ddBroker.Object);
        }
        public static void SetUpDDBrokerAndContext<TInputs>(this TInputs inputs, out Mock<IDDBroker> ddBroker, out Mock<IAsyncDecoratorContext> context)
            where TInputs : class
        {
            context = new Mock<IAsyncDecoratorContext>();
            ddBroker = new Mock<IDDBroker>();
            context.Setup(e => e.DDBroker).Returns(ddBroker.Object);
        }
        public static void SetUpDDBrokerAndContext<TInputs, TOutput>(this TInputs inputs, out Mock<IDDBroker> ddBroker, out Mock<IDecoratorContext<TOutput>> context)
            where TInputs : class
        {
            context = new Mock<IDecoratorContext<TOutput>>();
            ddBroker = new Mock<IDDBroker>();
            context.Setup(e => e.DDBroker).Returns(ddBroker.Object);
        }
        public static void SetUpDDBrokerAndContext<TInputs>(this TInputs inputs, out Mock<IDDBroker> ddBroker, out Mock<IDecoratorContext> context)
            where TInputs : class
        {
            context = new Mock<IDecoratorContext>();
            ddBroker = new Mock<IDDBroker>();
            context.Setup(e => e.DDBroker).Returns(ddBroker.Object);
        }
        public static void SetUpDDBrokerAndContext<TEInputs>(this TEInputs inputs, out Mock<IDDBroker> ddBroker, out Mock<IEDecoratorContext> context)
            where TEInputs : IEInputs
        {
            context = new Mock<IEDecoratorContext>();
            ddBroker = new Mock<IDDBroker>();
            context.Setup(e => e.DDBroker).Returns(ddBroker.Object);
        }

        public static Mock<IAsyncSupplierChannel<TInputs, TOutput>> SetUpAsyncSupplierChannelAny<TInputs, TOutput>(this Mock<IDDBroker> ddBroker, TOutput output) where TInputs : class
        {
            var mockSupplierChannel = new Mock<IAsyncSupplierChannel<TInputs, TOutput>>();
            mockSupplierChannel.Setup(e => e.ProcessAsync(It.IsAny<TInputs>(), It.IsAny<CancellationToken>())).ReturnsAsync(output);
            ddBroker.Setup(item => item.CreateAsyncSupplierChannel<TInputs, TOutput>()).Returns(mockSupplierChannel.Object);
            return mockSupplierChannel;
        }
        public static Mock<IAsyncSupplierChannel<TInputs>> SetUpAsyncSupplierChannelAny<TInputs>(this Mock<IDDBroker> ddBroker) where TInputs : class
        {
            var mockSupplierChannel = new Mock<IAsyncSupplierChannel<TInputs>>();
            mockSupplierChannel.Setup(e => e.ProcessAsync(It.IsAny<TInputs>(), It.IsAny<CancellationToken>()));
            ddBroker.Setup(item => item.CreateAsyncSupplierChannel<TInputs>()).Returns(mockSupplierChannel.Object);
            return mockSupplierChannel;
        }
        public static Mock<ISupplierChannel<TInputs, TOutput>> SetUpSupplierChannelAny<TInputs, TOutput>(this Mock<IDDBroker> ddBroker, TOutput output) where TInputs : class
        {
            var mockSupplierChannel = new Mock<ISupplierChannel<TInputs, TOutput>>();
            mockSupplierChannel.Setup(e => e.Process(It.IsAny<TInputs>())).Returns(output);
            ddBroker.Setup(item => item.CreateSupplierChannel<TInputs, TOutput>()).Returns(mockSupplierChannel.Object);
            return mockSupplierChannel;
        }
        public static Mock<ISupplierChannel<TInputs>> SetUpSupplierChannelAny<TInputs>(this Mock<IDDBroker> ddBroker) where TInputs : class
        {
            var mockSupplierChannel = new Mock<ISupplierChannel<TInputs>>();
            mockSupplierChannel.Setup(e => e.Process(It.IsAny<TInputs>()));
            ddBroker.Setup(item => item.CreateSupplierChannel<TInputs>()).Returns(mockSupplierChannel.Object);
            return mockSupplierChannel;
        }

        public static ISetup<IAsyncDecoratorContext<TOutput>,Task<TOutput>> SetUpNextAny<TInputs, TOutput>(this IAsyncDecorator<TInputs, TOutput> decorator, Mock<IAsyncDecoratorContext<TOutput>> context) where TInputs : class
        => context.Setup(item => item.Next(It.IsAny<TInputs>(), It.IsAny<CancellationToken>()));
        public static ISetup<IAsyncDecoratorContext, Task> SetUpNextAny<TInputs>(this IAsyncDecorator<TInputs> decorator, Mock<IAsyncDecoratorContext> context) where TInputs : class
        => context.Setup(item => item.Next(It.IsAny<TInputs>(), It.IsAny<CancellationToken>()));
        public static ISetup<IDecoratorContext<TOutput>, TOutput> SetUpNextAny<TInputs, TOutput>(this IDecorator<TInputs, TOutput> decorator, Mock<IDecoratorContext<TOutput>> context) where TInputs : class
        => context.Setup(item => item.Next(It.IsAny<TInputs>()));
        public static ISetup<IDecoratorContext> SetUpNextAny<TInputs>(this IDecorator<TInputs> decorator, Mock<IDecoratorContext> context) where TInputs : class
        => context.Setup(item => item.Next(It.IsAny<TInputs>()));
        public static ISetup<IEDecoratorContext, Task> SetUpNextAny<TEInputs>(this IEDecorator<TEInputs> decorator, Mock<IEDecoratorContext> context) where TEInputs : IEInputs
        => context.Setup(item => item.Next(It.IsAny<IEInputs>(), It.IsAny<CancellationToken>()));
    }
}

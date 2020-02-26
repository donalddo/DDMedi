using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public static class DecoratorContextExtensions
    {
        public static Task<R> Next<T, R>(this IAsyncDecorator<T, R> currentDecorator, IAsyncDecoratorContext<T, R> context, CancellationToken token = default)
            where T : IInputs<R> =>
            context.Next(token);
        public static Task Next<T>(this IAsyncDecorator<T> currentDecorator, IAsyncDecoratorContext<T> context, CancellationToken token = default)
            where T : IInputs =>
            context.Next(token);
        public static R Next<T, R>(this IDecorator<T, R> currentDecorator, IDecoratorContext<T, R> context)
            where T : IInputs<R> =>
            context.Next();
        public static void Next<T>(this IDecorator<T> currentDecorator, IDecoratorContext<T> context)
            where T : IInputs =>
            context.Next();
        public static Task Next<T>(this IEDecorator<T> currentDecorator, IEDecoratorContext<T> context, CancellationToken token = default)
            where T : IEInputs =>
            context.Next(token);
    }
}

using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public interface IDecorator { }
    public interface IAsyncDecorator<TInputs, TOutput> : IDecorator
        where TInputs : IInputs<TOutput>
    {
        Task<TOutput> ProcessAsync(IAsyncDecoratorContext<TInputs, TOutput> context, CancellationToken token = default);
    }
    public interface IAsyncDecorator<TInputs> : IDecorator
        where TInputs : IInputs
    {
        Task ProcessAsync(IAsyncDecoratorContext<TInputs> context, CancellationToken token = default);
    }
    public interface IDecorator<TInputs, TOutput> : IDecorator
        where TInputs : IInputs<TOutput>
    {
        TOutput Process(IDecoratorContext<TInputs, TOutput> context);
    }
    public interface IDecorator<TInputs> : IDecorator
        where TInputs : IInputs
    {
        void Process(IDecoratorContext<TInputs> context);
    }
    public interface IEDecorator<TEInputs> : IDecorator
        where TEInputs : IEInputs
    {
        Task ProcessAsync(IEDecoratorContext<TEInputs> context, CancellationToken token = default);
    }
}

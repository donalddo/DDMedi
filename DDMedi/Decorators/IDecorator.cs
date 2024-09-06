using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public interface IDecorator { }
    public interface IAsyncDecorator<TInputs, TOutput> : IDecorator
        where TInputs : class
    {
        Task<TOutput> ProcessAsync(TInputs inputs, IAsyncDecoratorContext<TOutput> context, CancellationToken token = default);
    }
    public interface IAsyncDecorator<TInputs> : IDecorator
        where TInputs : class
    {
        Task ProcessAsync(TInputs inputs, IAsyncDecoratorContext context, CancellationToken token = default);
    }
    public interface IDecorator<TInputs, TOutput> : IDecorator
        where TInputs : class
    {
        TOutput Process(TInputs inputs, IDecoratorContext<TOutput> context);
    }
    public interface IDecorator<TInputs> : IDecorator
        where TInputs : class
    {
        void Process(TInputs inputs, IDecoratorContext context);
    }
    public interface IEDecorator<TEInputs> : IDecorator
        where TEInputs : IEInputs
    {
        Task ProcessAsync(TEInputs inputs, IEDecoratorContext context, CancellationToken token = default);
    }
}

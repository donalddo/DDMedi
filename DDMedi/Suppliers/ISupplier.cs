using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public interface IInputs { }
    public interface IInputs<out Output> { }
    
    public interface ISupplier { };
    
    public interface IAsyncSupplier<TInputs, TOutput> : ISupplier where TInputs : IInputs<TOutput>
    {
        Task<TOutput> ProcessAsync(ISupplierContext<TInputs> context, CancellationToken token = default);
    }
    public interface IAsyncSupplier<TInputs> : ISupplier where TInputs : IInputs
    {
        Task ProcessAsync(ISupplierContext<TInputs> context, CancellationToken token = default);
    }
    public interface ISupplier<TInputs, TOutput> : ISupplier where TInputs : IInputs<TOutput>
    {
        TOutput Process(ISupplierContext<TInputs> context);
    }
    public interface ISupplier<TInputs> : ISupplier where TInputs : IInputs
    {
        void Process(ISupplierContext<TInputs> context);
    }
}

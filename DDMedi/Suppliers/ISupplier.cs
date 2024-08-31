using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public interface IInputs { }
    public interface IInputs<out Output> { }
    
    public interface ISupplier { };
    
    public interface IAsyncSupplier<TInputs, TOutput> : ISupplier where TInputs : IInputs<TOutput>
    {
        Task<TOutput> ProcessAsync(TInputs inputs, ISupplierContext context, CancellationToken token = default);
    }
    public interface IAsyncSupplier<TInputs> : ISupplier where TInputs : IInputs
    {
        Task ProcessAsync(TInputs inputs, ISupplierContext context, CancellationToken token = default);
    }
    public interface ISupplier<TInputs, TOutput> : ISupplier where TInputs : IInputs<TOutput>
    {
        TOutput Process(TInputs inputs, ISupplierContext context);
    }
    public interface ISupplier<TInputs> : ISupplier where TInputs : IInputs
    {
        void Process(TInputs inputs, ISupplierContext context);
    }
}

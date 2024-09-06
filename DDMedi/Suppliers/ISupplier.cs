using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{ 
    public interface ISupplier { };
    
    public interface IAsyncSupplier<TInputs, TOutput> : ISupplier where TInputs : class
    {
        Task<TOutput> ProcessAsync(TInputs inputs, ISupplierContext context, CancellationToken token = default);
    }
    public interface IAsyncSupplier<TInputs> : ISupplier where TInputs : class
    {
        Task ProcessAsync(TInputs inputs, ISupplierContext context, CancellationToken token = default);
    }
    public interface ISupplier<TInputs, TOutput> : ISupplier where TInputs : class
    {
        TOutput Process(TInputs inputs, ISupplierContext context);
    }
    public interface ISupplier<TInputs> : ISupplier where TInputs : class
    {
        void Process(TInputs inputs, ISupplierContext context);
    }
}

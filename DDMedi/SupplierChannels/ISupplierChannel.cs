using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public interface IBaseSupplierChannel
    {
        IDDBroker DDBroker { get; }
    }
    public interface IAsyncSupplierChannel<TInputs, TOutput> : IBaseSupplierChannel where TInputs : IInputs<TOutput>
    {
        Task<TOutput> ProcessAsync(TInputs inputs = default, CancellationToken token = default);
    }
    public interface IAsyncSupplierChannel<TInputs> : IBaseSupplierChannel where TInputs : IInputs
    {
        Task ProcessAsync(TInputs inputs = default, CancellationToken token = default);
    }
    public interface ISupplierChannel<TInputs, TOutput> : IBaseSupplierChannel where TInputs : IInputs<TOutput>
    {
        TOutput Process(TInputs inputs = default);
    }
    public interface ISupplierChannel<TInputs> : IBaseSupplierChannel where TInputs : IInputs
    {
        void Process(TInputs inputs = default);
    }
    public interface IESupplierChannel<TInputs> : IBaseSupplierChannel where TInputs : IEInputs
    {
        Task ProcessAsync(TInputs inputs = default, CancellationToken token = default);
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public interface IEInputs { }
    public sealed class ExceptionEInputs : IEInputs
    {
        public string Message { get; }
        public Exception Exception { get; }
        public IReadOnlyList<object> Inputs { get; }
        public ExceptionEInputs(Exception exception, string message, params object[] inputs) :
            this(exception, inputs)
        {
            Message = message;
        }
        public ExceptionEInputs(Exception exception, params object[] inputs)
        {
            Exception = exception;
            Inputs = inputs;
        }
    }
    
    public interface IESupplier { };
    
    public interface IESupplier<TEInputs> : IESupplier where TEInputs : IEInputs
    {
        Task ProcessAsync(TEInputs inputs, ISupplierContext context, CancellationToken token = default);
    }
}

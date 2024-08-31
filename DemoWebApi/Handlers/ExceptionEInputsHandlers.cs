using DDMedi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DemoWebApi.Handlers
{
    public class ExceptionEInputsHandlers :
        IESupplier<ExceptionEInputs>
    {
        public async Task ProcessAsync(ExceptionEInputs inputs, ISupplierContext context, CancellationToken token = default)
        {
            await Task.Delay(20000);
            await Console.Out.WriteLineAsync($"Logged exception {context.CorrelationId} {inputs.Exception.Message}");
        }
    }
}

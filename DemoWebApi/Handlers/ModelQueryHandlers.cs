using DDMedi;
using DemoWebApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoWebApi.Handlers
{
    public class GetModelQuery
    {
        public GetModelQuery(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
    public class ModelQueryHandlers :
        IAsyncSupplier<GetModelQuery, DemoModel>,
        ISupplier<GetModelQuery, DemoModel>
    {
        private readonly List<DemoModel> _models;
        public ModelQueryHandlers(List<DemoModel> models)
        {
            _models = models;
        }
        public DemoModel Process(GetModelQuery inputs, ISupplierContext context)
        {
            return _models.FirstOrDefault(e => e.Id == inputs.Id) ??
                throw new KeyNotFoundException();
        }

        public Task<DemoModel> ProcessAsync(GetModelQuery inputs, ISupplierContext context, CancellationToken token = default)
        {
            var model = _models.FirstOrDefault(e => e.Id == inputs.Id) ??
                throw new KeyNotFoundException();
            return Task.FromResult(model);
        }
    }
}

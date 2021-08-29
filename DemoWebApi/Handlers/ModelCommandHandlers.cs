using DDMedi;
using DemoWebApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoWebApi.Handlers
{
    public class InsertModelCommand : IInputs<int>
    {
    }
    public class UpdateModelCommand : IInputs
    {
        public UpdateModelCommand(int id, int count)
        {
            Id = id;
            Count = count;
        }

        public int Id { get; }
        public int Count { get; }
    }
    public class DeleteModelCommand : IInputs
    {
        public DeleteModelCommand(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
    public class ModelCommandHandlers :
        IAsyncSupplier<InsertModelCommand, int>,
        IAsyncSupplier<UpdateModelCommand>,
        IAsyncSupplier<DeleteModelCommand>
    {
        private readonly List<DemoModel> _models;
        private readonly SemaphoreSlim _slim;
        public ModelCommandHandlers(List<DemoModel> models)
        {
            _models = models;
            _slim = new SemaphoreSlim(1);
        }

        public async Task<int> ProcessAsync(ISupplierContext<InsertModelCommand> context, CancellationToken token = default)
        {
            int id = 0;
            try
            {
                await _slim.WaitAsync();
                id = _models.LastOrDefault()?.Id + 1 ?? 1;
                _models.Add(new DemoModel { Id = id });
            }
            finally
            {
                _slim.Release();
            }
            await context.DDBroker.Publish(new ModelAddedEvent(id, 0));
            return id;
        }

        public async Task ProcessAsync(ISupplierContext<UpdateModelCommand> context, CancellationToken token = default)
        {
            DemoModel model = null;
            try
            {
                await _slim.WaitAsync();
                model = _models.FirstOrDefault(e => e.Id == context.Inputs.Id) ??
                    throw new KeyNotFoundException();
                model.Count = context.Inputs.Count;
            }
            finally
            {
                _slim.Release();
            }
            await context.DDBroker.Publish(new ModelUpdatedEvent(model.Id, model.Count));
        }

        public async Task ProcessAsync(ISupplierContext<DeleteModelCommand> context, CancellationToken token = default)
        {
            try
            {
                await _slim.WaitAsync();
                var model = _models.FirstOrDefault(e => e.Id == context.Inputs.Id) ??
                    throw new KeyNotFoundException();
                _models.Remove(model);
            }
            finally
            {
                _slim.Release();
            }
            await context.DDBroker.Publish(new ModelDeletedEvent(context.Inputs.Id));
        }
    }
}

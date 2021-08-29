using DDMedi;
using DemoWebApi.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DemoWebApi.Handlers
{
    public class ModelAddedEvent : IEInputs
    {
        public ModelAddedEvent(int id, int count)
        {
            Id = id;
            Count = count;
        }

        public int Id { get; }
        public int Count { get; }
    }
    public class ModelUpdatedEvent : IEInputs
    {
        public ModelUpdatedEvent(int id, int count)
        {
            Id = id;
            Count = count;
        }

        public int Id { get; }
        public int Count { get; }
    }
    public class ModelDeletedEvent : IEInputs
    {
        public ModelDeletedEvent(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
    public class PurgeCachedModelsCommand : IInputs { }
    public class CachedModelHandlers :
        IAsyncDecorator<GetModelQuery, DemoModel>,
        IDecorator<GetModelQuery, DemoModel>,
        IESupplier<ModelAddedEvent>,
        IESupplier<ModelUpdatedEvent>,
        IESupplier<ModelDeletedEvent>,
        ISupplier<PurgeCachedModelsCommand>
    {
        private readonly ConcurrentDictionary<int, DemoModel> _cachedModels;
        public CachedModelHandlers(ConcurrentDictionary<int, DemoModel> cachedModels)
        {
            _cachedModels = cachedModels;
        }
        public DemoModel Process(IDecoratorContext<GetModelQuery, DemoModel> context)
        {
            if (_cachedModels.TryGetValue(context.Inputs.Id, out var model))
                return model;
            model = this.Next(context);
            _cachedModels.TryAdd(context.Inputs.Id, model);
            return model;
        }

        public void Process(ISupplierContext<PurgeCachedModelsCommand> context)
        {
            _cachedModels.Clear();
        }

        public async Task<DemoModel> ProcessAsync(IAsyncDecoratorContext<GetModelQuery, DemoModel> context, CancellationToken token = default)
        {
            if (_cachedModels.TryGetValue(context.Inputs.Id, out var model))
                return model;
            model = await this.Next(context);
            _cachedModels.TryAdd(context.Inputs.Id, model);
            return model;
        }
        public Task ProcessAsync(ISupplierContext<ModelAddedEvent> context, CancellationToken token = default)
        {
            if (!_cachedModels.TryAdd(context.Inputs.Id, new DemoModel { Id = context.Inputs.Id, Count = context.Inputs.Count }))
                throw new NotSupportedException();
            return Task.CompletedTask;
        }
        public Task ProcessAsync(ISupplierContext<ModelUpdatedEvent> context, CancellationToken token = default)
        {
            if (!_cachedModels.TryGetValue(context.Inputs.Id, out var model))
                throw new KeyNotFoundException();
            model.Count = context.Inputs.Count;
            return Task.CompletedTask;
        }

        public Task ProcessAsync(ISupplierContext<ModelDeletedEvent> context, CancellationToken token = default)
        {
            if (!_cachedModels.TryRemove(context.Inputs.Id, out _))
                throw new KeyNotFoundException();
            return Task.CompletedTask;
        }
    }
}

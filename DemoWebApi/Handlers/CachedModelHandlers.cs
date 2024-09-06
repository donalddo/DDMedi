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
    public class PurgeCachedModelsCommand { }
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
        public DemoModel Process(GetModelQuery inputs, IDecoratorContext<DemoModel> context)
        {
            if (_cachedModels.TryGetValue(inputs.Id, out var model))
                return model;
            model = this.Next(inputs, context);
            _cachedModels.TryAdd(inputs.Id, model);
            return model;
        }

        public void Process(PurgeCachedModelsCommand inputs, ISupplierContext context)
        {
            _cachedModels.Clear();
        }

        public async Task<DemoModel> ProcessAsync(GetModelQuery inputs, IAsyncDecoratorContext<DemoModel> context, CancellationToken token = default)
        {
            if (_cachedModels.TryGetValue(inputs.Id, out var model))
                return model;
            model = await this.Next(inputs, context);
            _cachedModels.TryAdd(inputs.Id, model);
            return model;
        }
        public Task ProcessAsync(ModelAddedEvent inputs, ISupplierContext context, CancellationToken token = default)
        {
            if (!_cachedModels.TryAdd(inputs.Id, new DemoModel { Id = inputs.Id, Count = inputs.Count }))
                throw new NotSupportedException();
            return Task.CompletedTask;
        }
        public Task ProcessAsync(ModelUpdatedEvent inputs, ISupplierContext context, CancellationToken token = default)
        {
            if (!_cachedModels.TryGetValue(inputs.Id, out var model))
                throw new KeyNotFoundException();
            model.Count = inputs.Count;
            return Task.CompletedTask;
        }

        public Task ProcessAsync(ModelDeletedEvent inputs, ISupplierContext context, CancellationToken token = default)
        {
            if (!_cachedModels.TryRemove(inputs.Id, out _))
                throw new KeyNotFoundException();
            return Task.CompletedTask;
        }
    }
}

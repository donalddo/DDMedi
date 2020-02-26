using Autofac;
using DDMedi;
using DDMedi.AutoFac;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleApp
{
    public class DoneEvent : IEInputs
    {
        public int Counted { get; }
        public DoneEvent(int Counted)
        {
            this.Counted = Counted;
        }
    }
    public class DoneEvent2 : IEInputs
    {
        public DoneEvent2(string correlationId)
        {
        }
        public int Counted { get; }
    }
    public class DoCommandObj : IInputs<object>
    {
        public int Count { get; set; }
    }
    public class DoCommand : IInputs
    {
        public int Count { get; set; }
    }
    public class WhateverESuppliers
        : IESupplier<DoneEvent>
    {
        public async Task ProcessAsync(ISupplierContext<DoneEvent> Context, CancellationToken Token = default)
        {
            await Task.Delay(1000);
            await Context.DDBroker.Publish(new DoneEvent2(Context.CorrelationId));
            await Console.Out.WriteLineAsync($"this is {nameof(IESupplier<DoneEvent>)} Count: {Context.Inputs.Counted}");
            await Task.Delay(2000);
        }
    }
    public class WhateverESuppliers2
        : IESupplier<DoneEvent>
    {

        public async Task ProcessAsync(ISupplierContext<DoneEvent> Context, CancellationToken Token = default)
        {
            await Task.Delay(800);
            await Console.Out.WriteLineAsync($"this is {nameof(IESupplier<DoneEvent>)}2 Count: {Context.Inputs.Counted}");
            await Task.Delay(1000);
        }
    }
    public class WhateverEDecorators
        : IEDecorator<DoneEvent>
    {

        public async Task ProcessAsync(IEDecoratorContext<DoneEvent> Context, CancellationToken Token = default)
        {
            await Console.Out.WriteLineAsync($"this is {nameof(IEDecorator<DoneEvent>)} Count: {Context.Inputs.Counted}");
            await this.Next(Context);
            await Console.Out.WriteLineAsync($"this is {nameof(IEDecorator<DoneEvent>)} Done: {Context.Inputs.Counted}");
        }
    }
    public class WhateverSuppliers :
        IAsyncSupplier<DoCommand>,
        ISupplier<DoCommand>,
        ISupplier<DoCommandObj, object>
    {
        public void Process(ISupplierContext<DoCommand> Context)
        {
            Console.Out.WriteLine($"this is {nameof(ISupplier<DoCommand>)} Count: {Context.Inputs.Count}");
        }

        public object Process(ISupplierContext<DoCommandObj> context)
        {
            return 5;
        }

        public Task ProcessAsync(ISupplierContext<DoCommand> Context, CancellationToken Token = default)
        {
            return Console.Out.WriteLineAsync($"this is {nameof(IAsyncSupplier<DoCommand>)} Count: {Context.Inputs.Count}");
        }
    }
    public class WhateverDecorators :
        IAsyncDecorator<DoCommand>,
        IDecorator<DoCommand>,
        IDecorator<DoCommandObj, object>
    {

        public void Process(IDecoratorContext<DoCommand> Context)
        {
            Console.Out.WriteLine($"this is {nameof(IDecorator<DoCommand>)} Count: {Context.Inputs.Count}");
            this.Next(Context);
            Console.Out.WriteLine($"this is {nameof(IDecorator<DoCommand>)} Done: {Context.Inputs.Count}");
        }

        public object Process(IDecoratorContext<DoCommandObj, object> context)
        {
            return this.Next(context);
        }

        public async Task ProcessAsync(IAsyncDecoratorContext<DoCommand> Context, CancellationToken Token = default)
        {
            await Console.Out.WriteLineAsync($"this is {nameof(IAsyncDecorator<DoCommand>)} Count: {Context.Inputs.Count}");
            await this.Next(Context);
            await Console.Out.WriteLineAsync($"this is {nameof(IAsyncDecorator<DoCommand>)} Done: {Context.Inputs.Count}");
        }
    }
    class Program
    {
        static async Task Main(string[] args)
        {
            var ddCollection = new DDMediCollection()
                .AddSupplier<WhateverSuppliers>(SupplierLifetime.Singleton)
                .AddSuppliers()
                .AddQueue(suppliers => suppliers.AddESupplier<WhateverESuppliers>())
                .AddQueue(suppliers => suppliers.AddESuppliers())
                .AddAsyncDecorator<DoCommand, WhateverDecorators>()
                .AddDecorator<DoCommand, WhateverDecorators>()
                .AddDecorator<DoCommandObj, object, WhateverDecorators>()
                .AddEDecorator<DoneEvent, WhateverEDecorators>();
            var builder = new ContainerBuilder();
            builder.AddDDMediFactory(ddCollection.BuildSuppliers());
            var container = builder.Build();
            var broker = container.Resolve<IDDBroker>();
            var ev = new DoneEvent(40);
            await broker.Publish(ev);
            await broker.ProcessAsync(new DoCommand { Count = 10 });

            var channel = broker.CreateAsyncSupplierChannel<DoCommand>();
            await channel.ProcessAsync(new DoCommand { Count = 30 });
            await channel.ProcessAsync(new DoCommand { Count = 50 });
            await channel.ProcessAsync(new DoCommand { Count = 60 });
            var d = broker.Process<DoCommandObj, object>();
            Console.ReadLine();

        }
    }
}

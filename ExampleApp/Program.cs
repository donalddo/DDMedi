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
    public class DoCommandObj
    {
        public int Count { get; set; }
    }
    public class DoCommand
    {
        public int Count { get; set; }
    }
    public class WhateverESuppliers
        : IESupplier<DoneEvent>
    {
        public async Task ProcessAsync(DoneEvent inputs, ISupplierContext Context, CancellationToken Token = default)
        {
            await Task.Delay(1000);
            await Context.DDBroker.Publish(new DoneEvent2(Context.CorrelationId)); // DoneEvent2 is fire and forget
            await Console.Out.WriteLineAsync($"this is {nameof(IESupplier<DoneEvent>)} Count: {inputs.Counted}");
            await Task.Delay(2000);
        }
    }
    public class WhateverESuppliers2
        : IESupplier<DoneEvent>
    {

        public async Task ProcessAsync(DoneEvent inputs, ISupplierContext Context, CancellationToken Token = default)
        {
            await Task.Delay(800);
            await Console.Out.WriteLineAsync($"this is {nameof(IESupplier<DoneEvent>)}2 Count: {inputs.Counted}");
            await Task.Delay(1000);
        }
    }
    public class WhateverEDecorators
        : IEDecorator<DoneEvent>
    {

        public async Task ProcessAsync(DoneEvent inputs, IEDecoratorContext Context, CancellationToken Token = default)
        {
            await Console.Out.WriteLineAsync($"this is {nameof(IEDecorator<DoneEvent>)} Count: {inputs.Counted}");
            await this.Next(inputs, Context);
            await Console.Out.WriteLineAsync($"this is {nameof(IEDecorator<DoneEvent>)} Done: {inputs.Counted}");
        }
    }
    public class WhateverSuppliers :
        IAsyncSupplier<DoCommand>,
        ISupplier<DoCommand>,
        ISupplier<DoCommandObj, object>
    {
        public void Process(DoCommand inputs, ISupplierContext Context)
        {
            Console.Out.WriteLine($"this is {nameof(ISupplier<DoCommand>)} Count: {inputs.Count}");
        }

        public object Process(DoCommandObj inputs, ISupplierContext context)
        {
            return 5;
        }

        public Task ProcessAsync(DoCommand inputs, ISupplierContext Context, CancellationToken Token = default)
        {
            return Console.Out.WriteLineAsync($"this is {nameof(IAsyncSupplier<DoCommand>)} Count: {inputs.Count}");
        }
    }
    public class WhateverDecorators :
        IAsyncDecorator<DoCommand>,
        IDecorator<DoCommand>,
        IDecorator<DoCommandObj, object>
    {

        public void Process(DoCommand inputs, IDecoratorContext Context)
        {
            Console.Out.WriteLine($"this is {nameof(IDecorator<DoCommand>)} Count: {inputs.Count}");
            this.Next(inputs, Context);
            Console.Out.WriteLine($"this is {nameof(IDecorator<DoCommand>)} Done: {inputs.Count}");
        }

        public object Process(DoCommandObj inputs, IDecoratorContext<object> context)
        {
            return this.Next(inputs, context);
        }

        public async Task ProcessAsync(DoCommand inputs, IAsyncDecoratorContext Context, CancellationToken Token = default)
        {
            await Console.Out.WriteLineAsync($"this is {nameof(IAsyncDecorator<DoCommand>)} Count: {inputs.Count}");
            await this.Next(inputs, Context);
            await Console.Out.WriteLineAsync($"this is {nameof(IAsyncDecorator<DoCommand>)} Done: {inputs.Count}");
        }
    }
    class Program
    {
        static async Task Main(string[] args)
        {
            var ddCollection = new DDMediCollection()
                .AddSupplier<WhateverSuppliers>(SupplierLifetime.Singleton) // register WhateverSuppliers as singleton
                .AddSuppliers() // register all other Suppliers as scope in this Example App project, WhateverSuppliers' lifetime is still singleton
                .AddQueue(suppliers => suppliers.AddESupplier<WhateverESuppliers>()) // register WhateverESuppliers in a dedicated queue
                .AddQueue(suppliers => suppliers.AddESuppliers()) // register all other ESuppliers in another new queue
                .AddAsyncDecorator<DoCommand, WhateverDecorators>()
                .AddDecorator<DoCommand, WhateverDecorators>() 
                .AddDecorator<DoCommandObj, object, WhateverDecorators>()
                .AddEDecorator<DoneEvent, WhateverEDecorators>(); // decorator for all ESuppliers handling DoneEvent
            var builder = new ContainerBuilder();
            builder.AddDDMediFactory(ddCollection.BuildSuppliers());
            var container = builder.Build();
            var broker = container.Resolve<IDDBroker>();
            var ev = new DoneEvent(40);
            await broker.Publish(ev); // long running for handling DoneEvent won't effect on next line of code
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

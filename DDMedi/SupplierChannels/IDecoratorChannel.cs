using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    internal interface IBaseDecoratorChannel<TInputs> : IBaseSupplierChannel
    {
        TInputs Inputs { get; }
        IServiceProvider Provider { get; }
    }
    internal abstract class BasicDecoratorChannel<TInputs, TSupplierContext> :
        IBaseDecoratorChannel<TInputs>
        
    {
        protected BasicDecoratorChannel(IInternalDDBroker ddBroker)
        {
            DDBroker = ddBroker;
            Provider = ddBroker.Provider;
        }
        public TSupplierContext Supplier { get; protected set; }
        public TInputs Inputs { get; protected set; }
        public IDDBroker DDBroker { get; }
        public IServiceProvider Provider { get; }
    }
    internal sealed class AsyncDecoratorChannel<TInputs, TOutput> :
        BasicDecoratorChannel<TInputs, IInternalAsyncSupplierContext<TInputs, TOutput>>,
        IAsyncSupplierChannel<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        internal AsyncDecoratorChannel(IInternalDDBroker ddBroker, SupplierDescriptor descriptor) : base(ddBroker)
        => Supplier = new AsyncDecoratorContext<TInputs, TOutput>(this, descriptor,
                    Provider.GetService(descriptor.ImplementDescriptor.RegisterType), CreateSupplier(descriptor.Next));
        private IInternalAsyncSupplierContext<TInputs, TOutput> CreateSupplier(SupplierDescriptor descriptor)
        {
            if (descriptor.Next == null)
                return new AsyncSupplierContext<TInputs, TOutput>(this, descriptor);
            return new AsyncDecoratorContext<TInputs, TOutput>(this, descriptor, CreateSupplier(descriptor.Next));
        }
        public Task<TOutput> ProcessAsync(TInputs inputs = default, CancellationToken token = default)
        {
            Inputs = inputs;
            return Supplier.ProcessAsync(token);
        }

    }
    internal sealed class AsyncDecoratorChannel<TInputs> :
        BasicDecoratorChannel<TInputs, IInternalAsyncSupplierContext<TInputs>>,
        IAsyncSupplierChannel<TInputs>
        where TInputs : IInputs
    {
        internal AsyncDecoratorChannel(IInternalDDBroker ddBroker, SupplierDescriptor descriptor) : base(ddBroker)
        => Supplier = new AsyncDecoratorContext<TInputs>(this, descriptor,
                    Provider.GetService(descriptor.ImplementDescriptor.RegisterType), CreateSupplier(descriptor.Next));
        private IInternalAsyncSupplierContext<TInputs> CreateSupplier(SupplierDescriptor descriptor)
        {
            if(descriptor.Next == null)
               return new AsyncSupplierContext<TInputs>(this, descriptor);
            return new AsyncDecoratorContext<TInputs>(this, descriptor, CreateSupplier(descriptor.Next));
        }
        public Task ProcessAsync(TInputs inputs = default, CancellationToken token = default)
        {
            Inputs = inputs;
            return Supplier.ProcessAsync(token);
        }

    }
    internal sealed class DecoratorChannel<TInputs, TOutput> :
        BasicDecoratorChannel<TInputs, IInternalSupplierContext<TInputs, TOutput>>,
        ISupplierChannel<TInputs, TOutput>
        where TInputs : IInputs<TOutput>
    {
        internal DecoratorChannel(IInternalDDBroker ddBroker, SupplierDescriptor descriptor) : base(ddBroker)
        => Supplier = new DecoratorContext<TInputs, TOutput>(this, descriptor,
                    Provider.GetService(descriptor.ImplementDescriptor.RegisterType), CreateSupplier(descriptor.Next));
        private IInternalSupplierContext<TInputs, TOutput> CreateSupplier(SupplierDescriptor descriptor)
        {
            if (descriptor.Next == null)
                return new SupplierContext<TInputs, TOutput>(this, descriptor);
            return new DecoratorContext<TInputs, TOutput>(this, descriptor, CreateSupplier(descriptor.Next));
        }
        public TOutput Process(TInputs inputs = default)
        {
            Inputs = inputs;
            return Supplier.Process();
        }

    }
    internal sealed class DecoratorChannel<TInputs> :
        BasicDecoratorChannel<TInputs, IInternalSupplierContext<TInputs>>,
        ISupplierChannel<TInputs>
        where TInputs : IInputs
    {
        internal DecoratorChannel(IInternalDDBroker ddBroker, SupplierDescriptor descriptor) : base(ddBroker)
        => Supplier = new DecoratorContext<TInputs>(this, descriptor,
                    Provider.GetService(descriptor.ImplementDescriptor.RegisterType), CreateSupplier(descriptor.Next));
        private IInternalSupplierContext<TInputs> CreateSupplier(SupplierDescriptor descriptor)
        {
            if (descriptor.Next == null)
                return new SupplierContext<TInputs>(this, descriptor);
            return new DecoratorContext<TInputs>(this, descriptor, CreateSupplier(descriptor.Next));
        }
        public void Process(TInputs inputs = default)
        {
            Inputs = inputs;
            Supplier.Process();
        }

    }
    internal sealed class EDecoratorChannel<TEInputs> :
        BasicDecoratorChannel<TEInputs, IInternalESupplierContext<TEInputs>>
        where TEInputs : IEInputs
    {
        internal EDecoratorChannel(IInternalDDBroker ddBroker, SupplierDescriptor descriptor) : base(ddBroker)
        => Supplier = new EDecoratorContext<TEInputs>(this, descriptor,
                    Provider.GetService(descriptor.ImplementDescriptor.RegisterType), CreateSupplier(descriptor.Next));
        private IInternalESupplierContext<TEInputs> CreateSupplier(SupplierDescriptor descriptor)
        {
            if (descriptor.Next == null)
                return new ESupplierContext<TEInputs>(this, descriptor);
            return new  EDecoratorContext<TEInputs>(this, descriptor, CreateSupplier(descriptor.Next));
        }
        public Task ProcessAsync(TEInputs inputs = default, CancellationToken token = default)
        {
            Inputs = inputs;
            return Supplier.ProcessAsync(token);
        }

    }
}

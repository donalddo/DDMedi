using DDMedi;
using DDMedi.DependencyInjection;
using DemoWebApi.Handlers;
using DemoWebApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DemoWebApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var ddFactory = new DDMediCollection()
                    //ModelCommandHandlers is dedicated as singleton lifetime
                    .AddSupplier<ModelCommandHandlers>(SupplierLifetime.Singleton)
                    // other handlers are scope lifetime, ModelCommandHandlers won't be added here
                    .AddSuppliers()
                    // Add a dedicate queue with 3 executors to process ExceptionEInputs becasue of long delay
                    .AddQueue(eSuppliers => eSuppliers.AddESupplier<ExceptionEInputsHandlers>(SupplierLifetime.Singleton), 3)
                    // any exception occur in esuppliers won't effect to main task, ExceptionEInputsHandlers won't be added here
                    .AddQueue(eSuppliers => eSuppliers.AddESuppliers())
                    //Decorator need to be defined separately for any specific message
                    .AddAsyncDecorator<GetModelQuery, DemoModel, CachedModelHandlers>()
                    .AddDecorator<GetModelQuery, DemoModel, CachedModelHandlers>()
                    // build DDMediFactory when finished register handlers
                    .BuildSuppliers();

            services.AddDDMediFactory(ddFactory); // support IOC

            services.AddControllers();
            services.AddSingleton(new ConcurrentDictionary<int, DemoModel>());
            services.AddSingleton(new List<DemoModel>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

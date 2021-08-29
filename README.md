[DDMedi](https://www.nuget.org/packages/DDMedi/) 
=======

A library with mediator design in .NET

With simple supports CQRS, Decorator, Mediator design pattern on both sync and async methods for loosely coupling via C# generic variance.

### Installing DDMedi

Install [Nuget Package](https://www.nuget.org/packages/DDMedi/):

    Install-Package DDMedi
    
Or with the .NET Core command line: 

    dotnet add package DDMedi

From commands, Package Manager Console or .NET Core CLI, will download and install DDMedi and all required dependencies.

### Example Code

Please go to [ExampleApp project](https://github.com/donalddo/DDMedi/tree/master/ExampleApp) or [DemoWebApi project](https://github.com/donalddo/DDMedi/tree/master/DemoWebApi) for the simple demo

### IOC

There is no default CONTAINER in DDMedi, you should do it yourselves. There are 2 examples with .Net Core [Dependency Injection](https://github.com/donalddo/DDMedi/blob/master/DDMedi.DependencyInjection/ServiceCollectionExtensions.cs) in [DemoWebApi](https://github.com/donalddo/DDMedi/tree/master/DemoWebApi/Startup.cs) and [AutoFac](https://github.com/donalddo/DDMedi/blob/master/DDMedi.AutoFac/AutoFacExtensions.cs) in [ExampleApp](https://github.com/donalddo/DDMedi/tree/master/ExampleApp/Program.cs)

For other CONTAINER libraries, after registered to `DDMediCollection` and builded into `DDMediFactory` there are 4 aspects to be resolved:
- Convert 3 `SupplierLifetime` enum values into CONTAINER lifetime enum values
- Implement and register interface `ISupplierScopeFactory` to CONTAINER as SINGLETON lifetime for `IEInputsQueueFactory` creating new scope in every new event dequeue
- Implement and register interface `IServiceProvider` to CONTAINER as SCOPE lifetime for every `IDDBroker`created from CONTAINER
- In `IBaseDescriptorCollection` of `DDMediFactory`, register to CONTAINER all `BaseDescriptor` with properties:
  - `RegisterType` is the type to be resolved from CONTAINER
  - if `ImplementType` is not null, it is the implementation of `RegisterType` 
  - `GetInstance` is the Delegate to init instance with `IServiceProvider` as param if `ImplementType` is null
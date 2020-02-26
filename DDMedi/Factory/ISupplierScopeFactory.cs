using System;

namespace DDMedi
{
    public enum SupplierLifetime
    {
        Singleton = 0,
        Scoped, Transient
    }
    public interface ISupplierScopeFactory
    {
        ISupplierScope CreateScope();
    }
    public interface ISupplierScope : IDisposable
    {
        IServiceProvider ServiceProvider { get; }
    }
}

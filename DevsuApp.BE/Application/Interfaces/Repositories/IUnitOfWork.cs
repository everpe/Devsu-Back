namespace DevsuApp.BE.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IClienteRepository Clientes { get; }
    ICuentaRepository Cuentas { get; }
    IMovimientoRepository Movimientos { get; }
        
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
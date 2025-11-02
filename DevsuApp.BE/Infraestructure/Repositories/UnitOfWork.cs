using DevsuApp.BE.Application.Interfaces.Repositories;
using DevsuApp.BE.Infraestructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevsuApp.BE.Infraestructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public IClienteRepository Clientes { get; }
    public ICuentaRepository Cuentas { get; }
    public IMovimientoRepository Movimientos { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IClienteRepository clienteRepository,
        ICuentaRepository cuentaRepository,
        IMovimientoRepository movimientoRepository)
    {
        _context = context;
        Clientes = clienteRepository;
        Cuentas = cuentaRepository;
        Movimientos = movimientoRepository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
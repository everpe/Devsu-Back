using DevsuApp.BE.Application.Interfaces.Repositories;
using DevsuApp.BE.Domain.Entities;
using DevsuApp.BE.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevsuApp.BE.Infraestructure.Repositories;

public class CuentaRepository : Repository<Cuenta>, ICuentaRepository
{
    public CuentaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta)
    {
        return await _dbSet
            .Include(c => c.Cliente)
            .FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);
    }

    public async Task<Cuenta?> GetByIdWithMovimientosAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Movimientos.OrderByDescending(m => m.Fecha))
            .Include(c => c.Cliente)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Cuenta?> GetByIdWithClienteAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Cliente)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Cuenta>> GetByClienteIdAsync(int clienteId)
    {
        return await _dbSet
            .Where(c => c.ClienteId == clienteId)
            .Include(c => c.Cliente)
            .ToListAsync();
    }

    public async Task<bool> ExistsByNumeroCuentaAsync(string numeroCuenta, int? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await _dbSet
                .AnyAsync(c => c.NumeroCuenta == numeroCuenta && c.Id != excludeId.Value);
        }
            
        return await _dbSet
            .AnyAsync(c => c.NumeroCuenta == numeroCuenta);
    }
}
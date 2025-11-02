using DevsuApp.BE.Application.Interfaces.Repositories;
using DevsuApp.BE.Domain.Entities;
using DevsuApp.BE.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevsuApp.BE.Infraestructure.Repositories;

public class ClienteRepository : Repository<Cliente>, IClienteRepository
{
    public ClienteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Cliente?> GetByIdWithCuentasAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Cuentas)
            .FirstOrDefaultAsync(c => c.ClienteId == id);
    }

    public async Task<Cliente?> GetByIdentificacionAsync(string identificacion)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Identificacion == identificacion);
    }

    public async Task<bool> ExistsByIdentificacionAsync(string identificacion, int? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await _dbSet
                .AnyAsync(c => c.Identificacion == identificacion && c.ClienteId != excludeId.Value);
        }
            
        return await _dbSet
            .AnyAsync(c => c.Identificacion == identificacion);
    }

    public override async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.ClienteId == id);
    }
}
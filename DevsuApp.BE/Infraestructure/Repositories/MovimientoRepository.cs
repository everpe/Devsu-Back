using DevsuApp.BE.Application.Interfaces.Repositories;
using DevsuApp.BE.Domain.Entities;
using DevsuApp.BE.Domain.Enums;
using DevsuApp.BE.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DevsuApp.BE.Infraestructure.Repositories;

public class MovimientoRepository : Repository<Movimiento>, IMovimientoRepository
{
    public MovimientoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Movimiento>> GetByCuentaIdAsync(int cuentaId)
    {
        return await _dbSet
            .Where(m => m.CuentaId == cuentaId)
            .Include(m => m.Cuenta)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    public async Task<IEnumerable<Movimiento>> GetByClienteIdAndFechaRangoAsync(
        int clienteId, 
        DateTime fechaInicio, 
        DateTime fechaFin)
    {
        return await _dbSet
            .Include(m => m.Cuenta)
            .ThenInclude(c => c.Cliente)
            .Where(m => m.Cuenta.ClienteId == clienteId 
                        && m.Fecha >= fechaInicio 
                        && m.Fecha <= fechaFin)
            .OrderBy(m => m.Fecha)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalDebitosDelDiaAsync(int cuentaId, DateTime fecha)
    {
        var inicioDelDia = fecha.Date;
        var finDelDia = inicioDelDia.AddDays(1).AddTicks(-1);

        var totalDebitos = await _dbSet
            .Where(m => m.CuentaId == cuentaId 
                        && m.TipoMovimiento == TipoMovimiento.Debito
                        && m.Fecha >= inicioDelDia 
                        && m.Fecha <= finDelDia)
            .SumAsync(m => Math.Abs(m.Valor));

        return totalDebitos;
    }

    public async Task<Movimiento?> GetUltimoMovimientoByCuentaAsync(int cuentaId)
    {
        return await _dbSet
            .Where(m => m.CuentaId == cuentaId)
            .OrderByDescending(m => m.Fecha)
            .ThenByDescending(m => m.Id)
            .FirstOrDefaultAsync();
    }
}
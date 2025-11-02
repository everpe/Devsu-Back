using DevsuApp.BE.Domain.Entities;

namespace DevsuApp.BE.Application.Interfaces.Repositories;

public interface IMovimientoRepository : IRepository<Movimiento>
{
    Task<IEnumerable<Movimiento>> GetByCuentaIdAsync(int cuentaId);
    Task<IEnumerable<Movimiento>> GetByClienteIdAndFechaRangoAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin);
    Task<decimal> GetTotalDebitosDelDiaAsync(int cuentaId, DateTime fecha);
    Task<Movimiento?> GetUltimoMovimientoByCuentaAsync(int cuentaId);
}
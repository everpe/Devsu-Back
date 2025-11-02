using DevsuApp.BE.Domain.Entities;

namespace DevsuApp.BE.Application.Interfaces.Repositories;

public interface ICuentaRepository : IRepository<Cuenta>
{
    Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta);
    Task<Cuenta?> GetByIdWithMovimientosAsync(int id);
    Task<Cuenta?> GetByIdWithClienteAsync(int id);
    Task<IEnumerable<Cuenta>> GetByClienteIdAsync(int clienteId);
    Task<bool> ExistsByNumeroCuentaAsync(string numeroCuenta, int? excludeId = null);
}
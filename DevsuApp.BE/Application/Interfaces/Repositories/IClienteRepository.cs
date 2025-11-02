using DevsuApp.BE.Domain.Entities;

namespace DevsuApp.BE.Application.Interfaces.Repositories;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> GetByIdWithCuentasAsync(int id);
    Task<Cliente?> GetByIdentificacionAsync(string identificacion);
    Task<bool> ExistsByIdentificacionAsync(string identificacion, int? excludeId = null);
}
using DevsuApp.BE.Application.DTOs;

namespace DevsuApp.BE.Application.Interfaces.Services;

public interface IMovimientoService
{
    Task<IEnumerable<MovimientoDto>> GetAllAsync();
    Task<MovimientoDto?> GetByIdAsync(int id);
    Task<IEnumerable<MovimientoDto>> GetByCuentaIdAsync(int cuentaId);
    Task<MovimientoDto> CreateAsync(CreateMovimientoDto dto);
    Task<bool> DeleteAsync(int id);
}
using DevsuApp.BE.Application.DTOs;

namespace DevsuApp.BE.Application.Interfaces.Services;

public interface ICuentaService
{
    Task<IEnumerable<CuentaDto>> GetAllAsync();
    Task<CuentaDto?> GetByIdAsync(int id);
    Task<IEnumerable<CuentaDto>> GetByClienteIdAsync(int clienteId);
    Task<CuentaDto> CreateAsync(CreateCuentaDto dto);
    Task<CuentaDto> UpdateAsync(int id, UpdateCuentaDto dto);
    Task<bool> DeleteAsync(int id);
}
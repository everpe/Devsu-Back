using DevsuApp.BE.Application.DTOs;

namespace DevsuApp.BE.Application.Interfaces.Services;

public interface IReporteService
{
    Task<EstadoCuentaDto> GetEstadoCuentaAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin);
    Task<byte[]> GetEstadoCuentaPdfAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin);
}
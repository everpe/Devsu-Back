using DevsuApp.BE.Application.DTOs;

namespace DevsuApp.BE.Application.Interfaces.Services;

public interface IReporteService
{
    Task<EstadoCuentaDto> GenerarEstadoCuentaAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin);
    Task<byte[]> GenerarEstadoCuentaPdfAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin);
}
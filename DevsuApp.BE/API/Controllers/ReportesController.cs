using DevsuApp.BE.Application.DTOs;
using DevsuApp.BE.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevsuApp.BE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _reporteService;
    private readonly ILogger<ReportesController> _logger;

    public ReportesController(
        IReporteService reporteService,
        ILogger<ReportesController> logger)
    {
        _reporteService = reporteService;
        _logger = logger;
    }

    /// <summary>
    /// Genera un reporte de estado de cuenta por rango de fechas
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// 
    ///     GET /api/reportes?clienteId=1&amp;fechaInicio=2024-02-01&amp;fechaFin=2024-02-28
    ///     
    /// El reporte incluye:
    /// - Información del cliente
    /// - Cuentas asociadas con sus saldos
    /// - Movimientos realizados en el rango de fechas
    /// - Total de créditos y débitos
    /// </remarks>
    /// <param name="clienteId">ID del cliente</param>
    /// <param name="fechaInicio">Fecha de inicio del reporte (formato: yyyy-MM-dd)</param>
    /// <param name="fechaFin">Fecha de fin del reporte (formato: yyyy-MM-dd)</param>
    /// <returns>Estado de cuenta en formato JSON</returns>
    [HttpGet]
    [ProducesResponseType(typeof(EstadoCuentaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstadoCuentaDto>> GetEstadoCuenta(
        [FromQuery] int clienteId,
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin)
    {
        _logger.LogInformation(
            "Generando reporte para cliente {ClienteId} desde {FechaInicio} hasta {FechaFin}",
            clienteId, fechaInicio, fechaFin);

        if (fechaInicio > fechaFin)
        {
            return BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha fin" });
        }

        var reporte = await _reporteService.GetEstadoCuentaAsync(clienteId, fechaInicio, fechaFin);
        
        _logger.LogInformation(
            "Reporte generado exitosamente - Total Créditos: {TotalCreditos}, Total Débitos: {TotalDebitos}",
            reporte.TotalCreditos, reporte.TotalDebitos);

        return Ok(reporte);
    }

    /// <summary>
    /// Genera un reporte de estado de cuenta en formato PDF (Base64)
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// 
    ///     GET /api/reportes/pdf?clienteId=1&amp;fechaInicio=2024-02-01&amp;fechaFin=2024-02-28
    ///     
    /// Retorna el PDF codificado en Base64 que puede ser:
    /// - Descargado por el frontend
    /// - Convertido a archivo PDF
    /// - Enviado por email
    /// </remarks>
    /// <param name="clienteId">ID del cliente</param>
    /// <param name="fechaInicio">Fecha de inicio del reporte</param>
    /// <param name="fechaFin">Fecha de fin del reporte</param>
    /// <returns>PDF en formato Base64</returns>
    [HttpGet("pdf")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetEstadoCuentaPdf(
        [FromQuery] int clienteId,
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin)
    {
        _logger.LogInformation(
            "Generando reporte PDF para cliente {ClienteId} desde {FechaInicio} hasta {FechaFin}",
            clienteId, fechaInicio, fechaFin);

        if (fechaInicio > fechaFin)
        {
            return BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha fin" });
        }

        var pdfBytes = await _reporteService.GetEstadoCuentaPdfAsync(clienteId, fechaInicio, fechaFin);
        var base64Pdf = Convert.ToBase64String(pdfBytes);

        _logger.LogInformation("Reporte PDF generado exitosamente para cliente {ClienteId}", clienteId);

        return Ok(new
        {
            clienteId,
            fechaInicio,
            fechaFin,
            formato = "PDF",
            contenidoBase64 = base64Pdf,
            mensaje = "Para descargar el PDF, decodifica el contenidoBase64"
        });
    }

    /// <summary>
    /// Descarga directa del PDF (alternativa)
    /// </summary>
    /// <param name="clienteId">ID del cliente</param>
    /// <param name="fechaInicio">Fecha de inicio del reporte</param>
    /// <param name="fechaFin">Fecha de fin del reporte</param>
    /// <returns>Archivo PDF para descarga</returns>
    [HttpGet("pdf/download")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DownloadEstadoCuentaPdf(
        [FromQuery] int clienteId,
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin)
    {
        _logger.LogInformation(
            "Descargando reporte PDF para cliente {ClienteId}",
            clienteId);

        if (fechaInicio > fechaFin)
        {
            return BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha fin" });
        }

        var pdfBytes = await _reporteService.GetEstadoCuentaPdfAsync(clienteId, fechaInicio, fechaFin);
        
        var fileName = $"EstadoCuenta_Cliente{clienteId}_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }
}
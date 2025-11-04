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
    /// Genera el estado de cuenta en formato JSON
    /// </summary>
    /// <param name="clienteId">ID del cliente</param>
    /// <param name="fechaInicio">Fecha inicio del reporte</param>
    /// <param name="fechaFin">Fecha fin del reporte</param>
    [HttpGet]
    public async Task<IActionResult> GenerarEstadoCuenta(
        [FromQuery] int clienteId,
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin)
    {
        try
        {
            var reporte = await _reporteService.GenerarEstadoCuentaAsync(clienteId, fechaInicio, fechaFin);
            return Ok(reporte);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar estado de cuenta");
            return StatusCode(500, new { message = "Error al generar el reporte" });
        }
    }

    /// <summary>
    /// Genera el estado de cuenta en formato PDF (Base64)
    /// </summary>
    [HttpGet("pdf")]
    public async Task<IActionResult> GenerarEstadoCuentaPdf(
        [FromQuery] int clienteId,
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin)
    {
        try
        {
            var pdfBytes = await _reporteService.GenerarEstadoCuentaPdfAsync(clienteId, fechaInicio, fechaFin);
            var base64 = Convert.ToBase64String(pdfBytes);

            return Ok(new
            {
                pdf = base64,
                filename = $"EstadoCuenta_{clienteId}_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.pdf"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar PDF");
            return StatusCode(500, new { message = "Error al generar el PDF" });
        }
    }

    /// <summary>
    /// Descarga directa del PDF
    /// </summary>
    [HttpGet("pdf/download")]
    public async Task<IActionResult> DescargarPdf(
        [FromQuery] int clienteId,
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin)
    {
        try
        {
            var pdfBytes = await _reporteService.GenerarEstadoCuentaPdfAsync(clienteId, fechaInicio, fechaFin);
            var fileName = $"EstadoCuenta_{clienteId}_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar PDF");
            return StatusCode(500, new { message = "Error al generar el PDF" });
        }
    }
}
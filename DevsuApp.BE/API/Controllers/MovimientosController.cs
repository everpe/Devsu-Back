using DevsuApp.BE.Application.DTOs;
using DevsuApp.BE.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevsuApp.BE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MovimientosController : ControllerBase
{
    private readonly IMovimientoService _movimientoService;
    private readonly ILogger<MovimientosController> _logger;

    public MovimientosController(
        IMovimientoService movimientoService,
        ILogger<MovimientosController> logger)
    {
        _movimientoService = movimientoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los movimientos
    /// </summary>
    /// <returns>Lista de movimientos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MovimientoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MovimientoDto>>> GetAll()
    {
        _logger.LogInformation("Obteniendo todos los movimientos");
        var movimientos = await _movimientoService.GetAllAsync();
        return Ok(movimientos);
    }

    /// <summary>
    /// Obtiene un movimiento por su ID
    /// </summary>
    /// <param name="id">ID del movimiento</param>
    /// <returns>Movimiento encontrado</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MovimientoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovimientoDto>> GetById(int id)
    {
        _logger.LogInformation("Obteniendo movimiento con ID: {MovimientoId}", id);
        
        var movimiento = await _movimientoService.GetByIdAsync(id);
        
        if (movimiento == null)
        {
            _logger.LogWarning("Movimiento con ID {MovimientoId} no encontrado", id);
            return NotFound(new { message = $"Movimiento con ID {id} no encontrado" });
        }

        return Ok(movimiento);
    }

    /// <summary>
    /// Obtiene todos los movimientos de una cuenta
    /// </summary>
    /// <param name="cuentaId">ID de la cuenta</param>
    /// <returns>Lista de movimientos de la cuenta</returns>
    [HttpGet("cuenta/{cuentaId}")]
    [ProducesResponseType(typeof(IEnumerable<MovimientoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MovimientoDto>>> GetByCuentaId(int cuentaId)
    {
        _logger.LogInformation("Obteniendo movimientos de la cuenta con ID: {CuentaId}", cuentaId);
        
        var movimientos = await _movimientoService.GetByCuentaIdAsync(cuentaId);
        
        return Ok(movimientos);
    }

    /// <summary>
    /// Crea un nuevo movimiento (depósito o retiro)
    /// </summary>
    /// <remarks>
    /// Validaciones aplicadas:
    /// - Si el saldo es cero y se intenta débito: "Saldo no disponible"
    /// - Si el cupo diario (límite $1000) se excede: "Cupo diario Excedido"
    /// - Los valores de crédito son positivos
    /// - Los valores de débito son negativos
    /// - Se almacena el saldo disponible después de cada transacción
    /// </remarks>
    /// <param name="dto">Datos del movimiento a crear</param>
    /// <returns>Movimiento creado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MovimientoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovimientoDto>> Create([FromBody] CreateMovimientoDto dto)
    {
        _logger.LogInformation(
            "Creando nuevo movimiento - Tipo: {TipoMovimiento}, Valor: {Valor}, CuentaId: {CuentaId}",
            dto.TipoMovimiento, dto.Valor, dto.CuentaId);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var movimiento = await _movimientoService.CreateAsync(dto);
        
        _logger.LogInformation(
            "Movimiento creado exitosamente - ID: {MovimientoId}, Saldo resultante: {Saldo}",
            movimiento.Id, movimiento.Saldo);
        
        return CreatedAtAction(
            nameof(GetById), 
            new { id = movimiento.Id }, 
            movimiento);
    }

    /// <summary>
    /// Elimina un movimiento
    /// </summary>
    /// <param name="id">ID del movimiento</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        _logger.LogInformation("Eliminando movimiento con ID: {MovimientoId}", id);
        
        var resultado = await _movimientoService.DeleteAsync(id);
        
        if (!resultado)
        {
            _logger.LogWarning("Movimiento con ID {MovimientoId} no encontrado para eliminar", id);
            return NotFound(new { message = $"Movimiento con ID {id} no encontrado" });
        }

        _logger.LogInformation("Movimiento con ID {MovimientoId} eliminado exitosamente", id);
        
        return NoContent();
    }
}
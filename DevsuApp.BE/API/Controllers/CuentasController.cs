using DevsuApp.BE.Application.DTOs;
using DevsuApp.BE.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevsuApp.BE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CuentasController : ControllerBase
{
    private readonly ICuentaService _cuentaService;
    private readonly ILogger<CuentasController> _logger;

    public CuentasController(
        ICuentaService cuentaService,
        ILogger<CuentasController> logger)
    {
        _cuentaService = cuentaService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las cuentas
    /// </summary>
    /// <returns>Lista de cuentas</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CuentaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CuentaDto>>> GetAll()
    {
        _logger.LogInformation("Obteniendo todas las cuentas");
        var cuentas = await _cuentaService.GetAllAsync();
        return Ok(cuentas);
    }

    /// <summary>
    /// Obtiene una cuenta por su ID
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <returns>Cuenta encontrada</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CuentaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CuentaDto>> GetById(int id)
    {
        _logger.LogInformation("Obteniendo cuenta con ID: {CuentaId}", id);
        
        var cuenta = await _cuentaService.GetByIdAsync(id);
        
        if (cuenta == null)
        {
            _logger.LogWarning("Cuenta con ID {CuentaId} no encontrada", id);
            return NotFound(new { message = $"Cuenta con ID {id} no encontrada" });
        }

        return Ok(cuenta);
    }

    /// <summary>
    /// Obtiene todas las cuentas de un cliente
    /// </summary>
    /// <param name="clienteId">ID del cliente</param>
    /// <returns>Lista de cuentas del cliente</returns>
    [HttpGet("cliente/{clienteId}")]
    [ProducesResponseType(typeof(IEnumerable<CuentaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CuentaDto>>> GetByClienteId(int clienteId)
    {
        _logger.LogInformation("Obteniendo cuentas del cliente con ID: {ClienteId}", clienteId);
        
        var cuentas = await _cuentaService.GetByClienteIdAsync(clienteId);
        
        return Ok(cuentas);
    }

    /// <summary>
    /// Crea una nueva cuenta
    /// </summary>
    /// <param name="dto">Datos de la cuenta a crear</param>
    /// <returns>Cuenta creada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CuentaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CuentaDto>> Create([FromBody] CreateCuentaDto dto)
    {
        _logger.LogInformation("Creando nueva cuenta: {NumeroCuenta}", dto.NumeroCuenta);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var cuenta = await _cuentaService.CreateAsync(dto);
        
        _logger.LogInformation("Cuenta creada exitosamente con ID: {CuentaId}", cuenta.Id);
        
        return CreatedAtAction(
            nameof(GetById), 
            new { id = cuenta.Id }, 
            cuenta);
    }

    /// <summary>
    /// Actualiza una cuenta existente
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="dto">Datos a actualizar</param>
    /// <returns>Cuenta actualizada</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CuentaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CuentaDto>> Update(int id, [FromBody] UpdateCuentaDto dto)
    {
        _logger.LogInformation("Actualizando cuenta con ID: {CuentaId}", id);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var cuenta = await _cuentaService.UpdateAsync(id, dto);
        
        _logger.LogInformation("Cuenta con ID {CuentaId} actualizada exitosamente", id);
        
        return Ok(cuenta);
    }

    /// <summary>
    /// Actualiza parcialmente una cuenta (PATCH)
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="dto">Datos a actualizar</param>
    /// <returns>Cuenta actualizada</returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(CuentaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CuentaDto>> Patch(int id, [FromBody] UpdateCuentaDto dto)
    {
        _logger.LogInformation("Actualizando parcialmente cuenta con ID: {CuentaId}", id);
        
        var cuenta = await _cuentaService.UpdateAsync(id, dto);
        
        return Ok(cuenta);
    }

    /// <summary>
    /// Elimina una cuenta
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        _logger.LogInformation("Eliminando cuenta con ID: {CuentaId}", id);
        
        var resultado = await _cuentaService.DeleteAsync(id);
        
        if (!resultado)
        {
            _logger.LogWarning("Cuenta con ID {CuentaId} no encontrada para eliminar", id);
            return NotFound(new { message = $"Cuenta con ID {id} no encontrada" });
        }

        _logger.LogInformation("Cuenta con ID {CuentaId} eliminada exitosamente", id);
        
        return NoContent();
    }
}
using DevsuApp.BE.Application.DTOs;
using DevsuApp.BE.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevsuApp.BE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly ILogger<ClientesController> _logger;

    public ClientesController(
        IClienteService clienteService,
        ILogger<ClientesController> logger)
    {
        _clienteService = clienteService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los clientes
    /// </summary>
    /// <returns>Lista de clientes</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
    {
        _logger.LogInformation("Obteniendo todos los clientes");
        var clientes = await _clienteService.GetAllAsync();
        return Ok(clientes);
    }

    /// <summary>
    /// Obtiene un cliente por su ID
    /// </summary>
    /// <param name="id">ID del cliente</param>
    /// <returns>Cliente encontrado</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDto>> GetById(int id)
    {
        _logger.LogInformation("Obteniendo cliente con ID: {ClienteId}", id);
        
        var cliente = await _clienteService.GetByIdAsync(id);
        
        if (cliente == null)
        {
            _logger.LogWarning("Cliente con ID {ClienteId} no encontrado", id);
            return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
        }

        return Ok(cliente);
    }

    /// <summary>
    /// Crea un nuevo cliente
    /// </summary>
    /// <param name="dto">Datos del cliente a crear</param>
    /// <returns>Cliente creado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClienteDto>> Create([FromBody] CreateClienteDto dto)
    {
        _logger.LogInformation("Creando nuevo cliente: {Nombre}", dto.Nombre);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var cliente = await _clienteService.CreateAsync(dto);
        
        _logger.LogInformation("Cliente creado exitosamente con ID: {ClienteId}", cliente.ClienteId);
        
        return CreatedAtAction(
            nameof(GetById), 
            new { id = cliente.ClienteId }, 
            cliente);
    }

    /// <summary>
    /// Actualiza un cliente existente
    /// </summary>
    /// <param name="id">ID del cliente</param>
    /// <param name="dto">Datos a actualizar</param>
    /// <returns>Cliente actualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClienteDto>> Update(int id, [FromBody] UpdateClienteDto dto)
    {
        _logger.LogInformation("Actualizando cliente con ID: {ClienteId}", id);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var cliente = await _clienteService.UpdateAsync(id, dto);
        
        _logger.LogInformation("Cliente con ID {ClienteId} actualizado exitosamente", id);
        
        return Ok(cliente);
    }

    /// <summary>
    /// Actualiza parcialmente un cliente (PATCH)
    /// </summary>
    /// <param name="id">ID del cliente</param>
    /// <param name="dto">Datos a actualizar</param>
    /// <returns>Cliente actualizado</returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDto>> Patch(int id, [FromBody] UpdateClienteDto dto)
    {
        _logger.LogInformation("Actualizando parcialmente cliente con ID: {ClienteId}", id);
        
        var cliente = await _clienteService.UpdateAsync(id, dto);
        
        return Ok(cliente);
    }

    /// <summary>
    /// Elimina un cliente
    /// </summary>
    /// <param name="id">ID del cliente</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        _logger.LogInformation("Eliminando cliente con ID: {ClienteId}", id);
        
        var resultado = await _clienteService.DeleteAsync(id);
        
        if (!resultado)
        {
            _logger.LogWarning("Cliente con ID {ClienteId} no encontrado para eliminar", id);
            return NotFound(new { message = $"Cliente con ID {id} no encontrado" });
        }

        _logger.LogInformation("Cliente con ID {ClienteId} eliminado exitosamente", id);
        
        return NoContent();
    }
}
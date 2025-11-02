using DevsuApp.BE.Application.DTOs;
using DevsuApp.BE.Application.Exceptions;
using DevsuApp.BE.Application.Interfaces.Repositories;
using DevsuApp.BE.Application.Interfaces.Services;
using DevsuApp.BE.Domain.Entities;

namespace DevsuApp.BE.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IUnitOfWork _unitOfWork;

    public ClienteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ClienteDto>> GetAllAsync()
    {
        var clientes = await _unitOfWork.Clientes.GetAllAsync();
        
        return clientes.Select(c => new ClienteDto
        {
            ClienteId = c.ClienteId,
            Nombre = c.Nombre,
            Genero = c.Genero,
            Edad = c.Edad,
            Identificacion = c.Identificacion,
            Direccion = c.Direccion,
            Telefono = c.Telefono,
            Estado = c.Estado
        });
    }

    public async Task<ClienteDto?> GetByIdAsync(int id)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
        
        if (cliente == null)
            return null;

        return new ClienteDto
        {
            ClienteId = cliente.ClienteId,
            Nombre = cliente.Nombre,
            Genero = cliente.Genero,
            Edad = cliente.Edad,
            Identificacion = cliente.Identificacion,
            Direccion = cliente.Direccion,
            Telefono = cliente.Telefono,
            Estado = cliente.Estado
        };
    }

    public async Task<ClienteDto> CreateAsync(CreateClienteDto dto)
    {
        // Validar que no exista un cliente con la misma identificación
        if (await _unitOfWork.Clientes.ExistsByIdentificacionAsync(dto.Identificacion))
        {
            throw new BusinessException($"Ya existe un cliente con la identificación {dto.Identificacion}");
        }

        var cliente = new Cliente
        {
            Nombre = dto.Nombre,
            Genero = dto.Genero,
            Edad = dto.Edad,
            Identificacion = dto.Identificacion,
            Direccion = dto.Direccion,
            Telefono = dto.Telefono,
            Contrasena = dto.Contrasena,
            Estado = dto.Estado
        };

        await _unitOfWork.Clientes.AddAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        return new ClienteDto
        {
            ClienteId = cliente.ClienteId,
            Nombre = cliente.Nombre,
            Genero = cliente.Genero,
            Edad = cliente.Edad,
            Identificacion = cliente.Identificacion,
            Direccion = cliente.Direccion,
            Telefono = cliente.Telefono,
            Estado = cliente.Estado
        };
    }

    public async Task<ClienteDto> UpdateAsync(int id, UpdateClienteDto dto)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
        
        if (cliente == null)
            throw new BusinessException($"Cliente con ID {id} no encontrado");

        // Actualizar solo los campos que vienen en el DTO
        if (dto.Nombre != null) cliente.Nombre = dto.Nombre;
        if (dto.Genero != null) cliente.Genero = dto.Genero;
        if (dto.Edad.HasValue) cliente.Edad = dto.Edad.Value;
        if (dto.Direccion != null) cliente.Direccion = dto.Direccion;
        if (dto.Telefono != null) cliente.Telefono = dto.Telefono;
        if (dto.Contrasena != null) cliente.Contrasena = dto.Contrasena;
        if (dto.Estado.HasValue) cliente.Estado = dto.Estado.Value;

        await _unitOfWork.Clientes.UpdateAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        return new ClienteDto
        {
            ClienteId = cliente.ClienteId,
            Nombre = cliente.Nombre,
            Genero = cliente.Genero,
            Edad = cliente.Edad,
            Identificacion = cliente.Identificacion,
            Direccion = cliente.Direccion,
            Telefono = cliente.Telefono,
            Estado = cliente.Estado
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
        
        if (cliente == null)
            return false;

        await _unitOfWork.Clientes.DeleteAsync(cliente);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _unitOfWork.Clientes.ExistsAsync(c => c.ClienteId == id);
    }
}
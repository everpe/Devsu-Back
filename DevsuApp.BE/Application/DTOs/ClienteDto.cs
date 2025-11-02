namespace DevsuApp.BE.Application.DTOs;

public class ClienteDto
{
    public int ClienteId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public int Edad { get; set; }
    public string Identificacion { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public bool Estado { get; set; }
}

public class CreateClienteDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public int Edad { get; set; }
    public string Identificacion { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string Contrasena { get; set; } = string.Empty;
    public bool Estado { get; set; } = true;
}

public class UpdateClienteDto
{
    public string? Nombre { get; set; }
    public string? Genero { get; set; }
    public int? Edad { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Contrasena { get; set; }
    public bool? Estado { get; set; }
}
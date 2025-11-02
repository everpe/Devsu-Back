using DevsuApp.BE.Domain.Enums;

namespace DevsuApp.BE.Application.DTOs;

public class CuentaDto
{
    public int Id { get; set; }
    public string NumeroCuenta { get; set; } = string.Empty;
    public string TipoCuenta { get; set; } = string.Empty;
    public decimal SaldoInicial { get; set; }
    public decimal SaldoActual { get; set; }
    public bool Estado { get; set; }
    public int ClienteId { get; set; }
    public string? NombreCliente { get; set; }
}

public class CreateCuentaDto
{
    public string NumeroCuenta { get; set; } = string.Empty;
    public TipoCuenta TipoCuenta { get; set; }
    public decimal SaldoInicial { get; set; }
    public bool Estado { get; set; } = true;
    public int ClienteId { get; set; }
}

public class UpdateCuentaDto
{
    public string? NumeroCuenta { get; set; }
    public TipoCuenta? TipoCuenta { get; set; }
    public bool? Estado { get; set; }
}
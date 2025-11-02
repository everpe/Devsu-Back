using DevsuApp.BE.Domain.Enums;

namespace DevsuApp.BE.Application.DTOs;

public class MovimientoDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoMovimiento { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal Saldo { get; set; }
    public int CuentaId { get; set; }
    public string? NumeroCuenta { get; set; }
}

public class CreateMovimientoDto
{
    public TipoMovimiento TipoMovimiento { get; set; }
    public decimal Valor { get; set; }
    public int CuentaId { get; set; }
}
namespace DevsuApp.BE.Application.DTOs;

public class ReporteDto
{
    public DateTime Fecha { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string NumeroCuenta { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal SaldoInicial { get; set; }
    public bool Estado { get; set; }
    public decimal Movimiento { get; set; }
    public decimal SaldoDisponible { get; set; }
}

public class EstadoCuentaDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public ClienteReporteDto Cliente { get; set; } = new();
    public List<CuentaReporteDto> Cuentas { get; set; } = new();
    public decimal TotalCreditos { get; set; }
    public decimal TotalDebitos { get; set; }
}

public class ClienteReporteDto
{
    public int ClienteId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Identificacion { get; set; } = string.Empty;
}

public class CuentaReporteDto
{
    public string NumeroCuenta { get; set; } = string.Empty;
    public string TipoCuenta { get; set; } = string.Empty;
    public decimal SaldoInicial { get; set; }
    public decimal SaldoActual { get; set; }
    public List<MovimientoReporteDto> Movimientos { get; set; } = new();
}

public class MovimientoReporteDto
{
    public DateTime Fecha { get; set; }
    public string TipoMovimiento { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal Saldo { get; set; }
}
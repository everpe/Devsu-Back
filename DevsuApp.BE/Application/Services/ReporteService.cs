using DevsuApp.BE.Application.DTOs;
using DevsuApp.BE.Application.Exceptions;
using DevsuApp.BE.Application.Interfaces.Repositories;
using DevsuApp.BE.Application.Interfaces.Services;
using DevsuApp.BE.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DevsuApp.BE.Application.Services;


public class ReporteService : IReporteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ICuentaRepository _cuentaRepository;
    private readonly IMovimientoRepository _movimientoRepository;

    public ReporteService(
        IClienteRepository clienteRepository,
        ICuentaRepository cuentaRepository,
        IMovimientoRepository movimientoRepository)
    {
        _clienteRepository = clienteRepository;
        _cuentaRepository = cuentaRepository;
        _movimientoRepository = movimientoRepository;
    }


    public async Task<EstadoCuentaDto> GenerarEstadoCuentaAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin)
    {
        // Validar que el cliente existe
        var cliente = await _clienteRepository.GetByIdAsync(clienteId);
        if (cliente == null)
        {
            throw new KeyNotFoundException($"Cliente con ID {clienteId} no encontrado");
        }

        // Obtener cuentas del cliente
        var cuentas = await _cuentaRepository.GetByClienteIdAsync(clienteId);

        var estadoCuenta = new EstadoCuentaDto
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            Cliente = new ClienteReporteDto
            {
                ClienteId = cliente.ClienteId,
                Nombre = cliente.Nombre,
                Identificacion = cliente.Identificacion
            },
            Cuentas = new List<CuentaReporteDto>()
        };

        decimal totalCreditos = 0;
        decimal totalDebitos = 0;

        foreach (var cuenta in cuentas)
        {
            // Obtener movimientos de la cuenta en el rango de fechas
            var movimientos = await _movimientoRepository.GetByCuentaIdAsync(cuenta.Id);
            var movimientosFiltrados = movimientos
                //.Where(m => m.Fecha >= fechaInicio && m.Fecha <= fechaFin)
                .Where(m => m.Fecha.Date >= fechaInicio.Date && m.Fecha.Date <= fechaFin.Date)
                .OrderBy(m => m.Fecha)
                .ToList();

            // ⭐ Calcular saldo actual: obtener el último movimiento
            var ultimoMovimiento = movimientos
                .OrderByDescending(m => m.Fecha)
                .FirstOrDefault();

            decimal saldoActual = ultimoMovimiento?.Saldo ?? cuenta.SaldoInicial;

            var cuentaReporte = new CuentaReporteDto
            {
                NumeroCuenta = cuenta.NumeroCuenta,
                TipoCuenta = cuenta.TipoCuenta.ToString(),
                SaldoInicial = cuenta.SaldoInicial,
                SaldoActual = saldoActual, // ⭐ Usar el saldo calculado
                Movimientos = movimientosFiltrados.Select(m => new MovimientoReporteDto
                {
                    Fecha = m.Fecha,
                    TipoMovimiento = m.TipoMovimiento.ToString(),
                    Valor = m.Valor,
                    Saldo = m.Saldo
                }).ToList()
            };

            // Calcular totales
            foreach (var mov in movimientosFiltrados)
            {
                if (mov.Valor > 0)
                    totalCreditos += mov.Valor;
                else
                    totalDebitos += Math.Abs(mov.Valor);
            }

            estadoCuenta.Cuentas.Add(cuentaReporte);
        }

        estadoCuenta.TotalCreditos = totalCreditos;
        estadoCuenta.TotalDebitos = totalDebitos;

        return estadoCuenta;
    }


    public async Task<byte[]> GenerarEstadoCuentaPdfAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin)
    {
        var estadoCuenta = await GenerarEstadoCuentaAsync(clienteId, fechaInicio, fechaFin);

        // Configurar licencia de QuestPDF (Community - gratis)
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                // Header
                page.Header()
                    .Text($"Estado de Cuenta - {estadoCuenta.Cliente.Nombre}")
                    .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                // Content
                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Spacing(15);

                        // Información del Cliente
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Cliente: {estadoCuenta.Cliente.Nombre}").Bold();
                                col.Item().Text($"Identificación: {estadoCuenta.Cliente.Identificacion}");
                            });

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Fecha: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}").Bold();
                                col.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}");
                            });
                        });

                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Cuentas y Movimientos
                        foreach (var cuenta in estadoCuenta.Cuentas)
                        {
                            // Información de la Cuenta
                            column.Item().Background(Colors.Grey.Lighten3)
                                .Padding(10)
                                .Column(col =>
                                {
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text($"Cuenta: {cuenta.NumeroCuenta}").Bold();
                                        row.RelativeItem().Text($"Tipo: {cuenta.TipoCuenta}");
                                        row.RelativeItem().Text($"Saldo: ${cuenta.SaldoActual:N2}").Bold();
                                    });
                                });

                            // Tabla de Movimientos
                            if (cuenta.Movimientos.Any())
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2); // Fecha
                                        columns.RelativeColumn(2); // Tipo
                                        columns.RelativeColumn(2); // Valor
                                        columns.RelativeColumn(2); // Saldo
                                    });

                                    // Header de la tabla
                                    table.Header(header =>
                                    {
                                        header.Cell().Background(Colors.Blue.Lighten3)
                                            .Padding(5).Text("Fecha").Bold();
                                        header.Cell().Background(Colors.Blue.Lighten3)
                                            .Padding(5).Text("Tipo").Bold();
                                        header.Cell().Background(Colors.Blue.Lighten3)
                                            .Padding(5).Text("Valor").Bold();
                                        header.Cell().Background(Colors.Blue.Lighten3)
                                            .Padding(5).Text("Saldo").Bold();
                                    });

                                    // Filas de movimientos
                                    foreach (var movimiento in cuenta.Movimientos)
                                    {
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                            .Padding(5).Text(movimiento.Fecha.ToString("dd/MM/yyyy HH:mm"));

                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                            .Padding(5).Text(movimiento.TipoMovimiento);

                                        var valorColor = movimiento.Valor >= 0 ? Colors.Green.Darken2 : Colors.Red.Darken2;
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                            .Padding(5).Text($"${movimiento.Valor:N2}").FontColor(valorColor);

                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                            .Padding(5).Text($"${movimiento.Saldo:N2}");
                                    }
                                });
                            }
                            else
                            {
                                column.Item().Padding(10).Text("No hay movimientos en este período").Italic();
                            }

                            column.Item().PaddingTop(10);
                        }

                        // Resumen Total
                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
                        column.Item().PaddingTop(10)
                            .Background(Colors.Blue.Lighten4)
                            .Padding(10)
                            .Row(row =>
                            {
                                row.RelativeItem().Text("RESUMEN TOTAL").Bold().FontSize(12);
                                row.RelativeItem().Text($"Total Créditos: ${estadoCuenta.TotalCreditos:N2}")
                                    .FontColor(Colors.Green.Darken2).Bold();
                                row.RelativeItem().Text($"Total Débitos: ${estadoCuenta.TotalDebitos:N2}")
                                    .FontColor(Colors.Red.Darken2).Bold();
                            });
                    });

                // Footer
                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }
}
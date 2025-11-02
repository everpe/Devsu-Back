using DevsuApp.BE.Application.DTOs;
using DevsuApp.BE.Application.Exceptions;
using DevsuApp.BE.Application.Interfaces.Repositories;
using DevsuApp.BE.Application.Interfaces.Services;
using DevsuApp.BE.Domain.Enums;

namespace DevsuApp.BE.Application.Services;

 public class ReporteService : IReporteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReporteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EstadoCuentaDto> GetEstadoCuentaAsync(
            int clienteId, 
            DateTime fechaInicio, 
            DateTime fechaFin)
        {
            // Validar que el cliente exista
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(clienteId);
            if (cliente == null)
            {
                throw new BusinessException($"Cliente con ID {clienteId} no encontrado");
            }

            // Ajustar fechas para incluir todo el día
            var fechaInicioAjustada = fechaInicio.Date;
            var fechaFinAjustada = fechaFin.Date.AddDays(1).AddTicks(-1);

            // Obtener todas las cuentas del cliente
            var cuentas = await _unitOfWork.Cuentas.GetByClienteIdAsync(clienteId);

            // Obtener movimientos del cliente en el rango de fechas
            var movimientos = await _unitOfWork.Movimientos
                .GetByClienteIdAndFechaRangoAsync(clienteId, fechaInicioAjustada, fechaFinAjustada);

            decimal totalCreditos = 0;
            decimal totalDebitos = 0;

            var cuentasReporte = new List<CuentaReporteDto>();

            foreach (var cuenta in cuentas)
            {
                var movimientosCuenta = movimientos
                    .Where(m => m.CuentaId == cuenta.Id)
                    .OrderBy(m => m.Fecha)
                    .ToList();

                var movimientosDto = movimientosCuenta.Select(m =>
                {
                    if (m.TipoMovimiento == TipoMovimiento.Credito)
                        totalCreditos += Math.Abs(m.Valor);
                    else
                        totalDebitos += Math.Abs(m.Valor);

                    return new MovimientoReporteDto
                    {
                        Fecha = m.Fecha,
                        TipoMovimiento = m.TipoMovimiento.ToString(),
                        Valor = m.Valor,
                        Saldo = m.Saldo
                    };
                }).ToList();

                // Calcular saldo actual
                var ultimoMovimiento = await _unitOfWork.Movimientos
                    .GetUltimoMovimientoByCuentaAsync(cuenta.Id);
                var saldoActual = ultimoMovimiento?.Saldo ?? cuenta.SaldoInicial;

                cuentasReporte.Add(new CuentaReporteDto
                {
                    NumeroCuenta = cuenta.NumeroCuenta,
                    TipoCuenta = cuenta.TipoCuenta.ToString(),
                    SaldoInicial = cuenta.SaldoInicial,
                    SaldoActual = saldoActual,
                    Movimientos = movimientosDto
                });
            }

            return new EstadoCuentaDto
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Cliente = new ClienteReporteDto
                {
                    ClienteId = cliente.ClienteId,
                    Nombre = cliente.Nombre,
                    Identificacion = cliente.Identificacion
                },
                Cuentas = cuentasReporte,
                TotalCreditos = totalCreditos,
                TotalDebitos = totalDebitos
            };
        }

        public async Task<byte[]> GetEstadoCuentaPdfAsync(
            int clienteId, 
            DateTime fechaInicio, 
            DateTime fechaFin)
        {
            var estadoCuenta = await GetEstadoCuentaAsync(clienteId, fechaInicio, fechaFin);

            // TODO: Implementar generación de PDF
            // Puedes usar librerías como:
            // - QuestPDF (recomendada, moderna y fácil)
            // - iTextSharp
            // - DinkToPdf

            // Por ahora retornamos un placeholder
            var pdfContent = System.Text.Encoding.UTF8.GetBytes("PDF Placeholder");
            return pdfContent;
        }
    }
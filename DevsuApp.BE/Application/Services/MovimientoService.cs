using DevsuApp.BE.Application.DTOs;
using DevsuApp.BE.Application.Exceptions;
using DevsuApp.BE.Application.Interfaces.Repositories;
using DevsuApp.BE.Application.Interfaces.Services;
using DevsuApp.BE.Domain.Entities;
using DevsuApp.BE.Domain.Enums;

namespace DevsuApp.BE.Application.Services;

 public class MovimientoService : IMovimientoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const decimal LIMITE_DIARIO_RETIRO = 1000m;//para que lo tome como decimal $

        public MovimientoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MovimientoDto>> GetAllAsync()
        {
            var movimientos = await _unitOfWork.Movimientos.GetAllAsync();
            
            return movimientos.Select(m => new MovimientoDto
            {
                Id = m.Id,
                Fecha = m.Fecha,
                TipoMovimiento = m.TipoMovimiento.ToString(),
                Valor = m.Valor,
                Saldo = m.Saldo,
                CuentaId = m.CuentaId
            });
        }

        public async Task<MovimientoDto?> GetByIdAsync(int id)
        {
            var movimiento = await _unitOfWork.Movimientos.GetByIdAsync(id);
            
            if (movimiento == null)
                return null;

            return new MovimientoDto
            {
                Id = movimiento.Id,
                Fecha = movimiento.Fecha,
                TipoMovimiento = movimiento.TipoMovimiento.ToString(),
                Valor = movimiento.Valor,
                Saldo = movimiento.Saldo,
                CuentaId = movimiento.CuentaId
            };
        }

        public async Task<IEnumerable<MovimientoDto>> GetByCuentaIdAsync(int cuentaId)
        {
            var movimientos = await _unitOfWork.Movimientos.GetByCuentaIdAsync(cuentaId);
            
            return movimientos.Select(m => new MovimientoDto
            {
                Id = m.Id,
                Fecha = m.Fecha,
                TipoMovimiento = m.TipoMovimiento.ToString(),
                Valor = m.Valor,
                Saldo = m.Saldo,
                CuentaId = m.CuentaId,
                NumeroCuenta = m.Cuenta?.NumeroCuenta
            });
        }

        public async Task<MovimientoDto> CreateAsync(CreateMovimientoDto dto)
        {
            // Iniciar transacción para garantizar consistencia
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Validar que la cuenta exista
                var cuenta = await _unitOfWork.Cuentas.GetByIdAsync(dto.CuentaId);
                if (cuenta == null)
                {
                    throw new BusinessException($"Cuenta con ID {dto.CuentaId} no encontrada");
                }

                if (!cuenta.Estado)
                {
                    throw new BusinessException("La cuenta está inactiva");
                }

                // 2. Obtener el último movimiento para calcular saldo actual
                var ultimoMovimiento = await _unitOfWork.Movimientos
                    .GetUltimoMovimientoByCuentaAsync(dto.CuentaId);
                
                var saldoActual = ultimoMovimiento?.Saldo ?? cuenta.SaldoInicial;

                // 3. Normalizar el valor según el tipo de movimiento
                decimal valorMovimiento = dto.Valor;
                
                if (dto.TipoMovimiento == TipoMovimiento.Credito)
                {
                    // Crédito: valor positivo (depósito)
                    valorMovimiento = Math.Abs(dto.Valor);
                }
                else if (dto.TipoMovimiento == TipoMovimiento.Debito)
                {
                    // Débito: valor negativo (retiro)
                    valorMovimiento = -Math.Abs(dto.Valor);
                    
                    // 4. VALIDACIÓN: Saldo no disponible
                    if (saldoActual <= 0)
                    {
                        throw new SaldoInsuficienteException();
                    }

                    if (Math.Abs(valorMovimiento) > saldoActual)
                    {
                        throw new SaldoInsuficienteException();
                    }

                    // 5. VALIDACIÓN: Cupo diario excedido
                    var debitosHoy = await _unitOfWork.Movimientos
                        .GetTotalDebitosDelDiaAsync(dto.CuentaId, DateTime.Now);
                    
                    var totalDebitosConNuevo = debitosHoy + Math.Abs(valorMovimiento);

                    if (totalDebitosConNuevo > LIMITE_DIARIO_RETIRO)
                    {
                        throw new CupoDiarioExcedidoException(LIMITE_DIARIO_RETIRO, debitosHoy);
                    }
                }

                // 6. Calcular el nuevo saldo
                var nuevoSaldo = saldoActual + valorMovimiento;

                // 7. Crear el movimiento
                var movimiento = new Movimiento
                {
                    Fecha = DateTime.Now,
                    TipoMovimiento = dto.TipoMovimiento,
                    Valor = valorMovimiento,
                    Saldo = nuevoSaldo,
                    CuentaId = dto.CuentaId
                };

                await _unitOfWork.Movimientos.AddAsync(movimiento);
                await _unitOfWork.CommitTransactionAsync();

                return new MovimientoDto
                {
                    Id = movimiento.Id,
                    Fecha = movimiento.Fecha,
                    TipoMovimiento = movimiento.TipoMovimiento.ToString(),
                    Valor = movimiento.Valor,
                    Saldo = movimiento.Saldo,
                    CuentaId = movimiento.CuentaId,
                    NumeroCuenta = cuenta.NumeroCuenta
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var movimiento = await _unitOfWork.Movimientos.GetByIdAsync(id);
            
            if (movimiento == null)
                return false;

            // NOTA: En un sistema real, considerar si se permite eliminar movimientos
            // o mejor manejar reversos/anulaciones
            await _unitOfWork.Movimientos.DeleteAsync(movimiento);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
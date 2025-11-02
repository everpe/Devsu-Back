using DevsuApp.BE.Application.DTOs;
using DevsuApp.BE.Application.Exceptions;
using DevsuApp.BE.Application.Interfaces.Repositories;
using DevsuApp.BE.Application.Interfaces.Services;
using DevsuApp.BE.Domain.Entities;

namespace DevsuApp.BE.Application.Services;

 public class CuentaService : ICuentaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CuentaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CuentaDto>> GetAllAsync()
        {
            var cuentas = await _unitOfWork.Cuentas.GetAllAsync();
            
            var cuentasDto = new List<CuentaDto>();
            
            foreach (var cuenta in cuentas)
            {
                var saldoActual = await CalcularSaldoActualAsync(cuenta.Id, cuenta.SaldoInicial);
                
                cuentasDto.Add(new CuentaDto
                {
                    Id = cuenta.Id,
                    NumeroCuenta = cuenta.NumeroCuenta,
                    TipoCuenta = cuenta.TipoCuenta.ToString(),
                    SaldoInicial = cuenta.SaldoInicial,
                    SaldoActual = saldoActual,
                    Estado = cuenta.Estado,
                    ClienteId = cuenta.ClienteId
                });
            }

            return cuentasDto;
        }

        public async Task<CuentaDto?> GetByIdAsync(int id)
        {
            var cuenta = await _unitOfWork.Cuentas.GetByIdWithClienteAsync(id);
            
            if (cuenta == null)
                return null;

            var saldoActual = await CalcularSaldoActualAsync(cuenta.Id, cuenta.SaldoInicial);

            return new CuentaDto
            {
                Id = cuenta.Id,
                NumeroCuenta = cuenta.NumeroCuenta,
                TipoCuenta = cuenta.TipoCuenta.ToString(),
                SaldoInicial = cuenta.SaldoInicial,
                SaldoActual = saldoActual,
                Estado = cuenta.Estado,
                ClienteId = cuenta.ClienteId,
                NombreCliente = cuenta.Cliente?.Nombre
            };
        }

        public async Task<IEnumerable<CuentaDto>> GetByClienteIdAsync(int clienteId)
        {
            var cuentas = await _unitOfWork.Cuentas.GetByClienteIdAsync(clienteId);
            
            var cuentasDto = new List<CuentaDto>();
            
            foreach (var cuenta in cuentas)
            {
                var saldoActual = await CalcularSaldoActualAsync(cuenta.Id, cuenta.SaldoInicial);
                
                cuentasDto.Add(new CuentaDto
                {
                    Id = cuenta.Id,
                    NumeroCuenta = cuenta.NumeroCuenta,
                    TipoCuenta = cuenta.TipoCuenta.ToString(),
                    SaldoInicial = cuenta.SaldoInicial,
                    SaldoActual = saldoActual,
                    Estado = cuenta.Estado,
                    ClienteId = cuenta.ClienteId,
                    NombreCliente = cuenta.Cliente?.Nombre
                });
            }

            return cuentasDto;
        }

        public async Task<CuentaDto> CreateAsync(CreateCuentaDto dto)
        {
            // Validar que el cliente exista
            if (!await _unitOfWork.Clientes.ExistsAsync(c => c.ClienteId == dto.ClienteId))
            {
                throw new BusinessException($"Cliente con ID {dto.ClienteId} no encontrado");
            }

            // Validar que no exista una cuenta con el mismo número
            if (await _unitOfWork.Cuentas.ExistsByNumeroCuentaAsync(dto.NumeroCuenta))
            {
                throw new BusinessException($"Ya existe una cuenta con el número {dto.NumeroCuenta}");
            }

            var cuenta = new Cuenta
            {
                NumeroCuenta = dto.NumeroCuenta,
                TipoCuenta = dto.TipoCuenta,
                SaldoInicial = dto.SaldoInicial,
                Estado = dto.Estado,
                ClienteId = dto.ClienteId
            };

            await _unitOfWork.Cuentas.AddAsync(cuenta);
            await _unitOfWork.SaveChangesAsync();

            return new CuentaDto
            {
                Id = cuenta.Id,
                NumeroCuenta = cuenta.NumeroCuenta,
                TipoCuenta = cuenta.TipoCuenta.ToString(),
                SaldoInicial = cuenta.SaldoInicial,
                SaldoActual = cuenta.SaldoInicial,
                Estado = cuenta.Estado,
                ClienteId = cuenta.ClienteId
            };
        }

        public async Task<CuentaDto> UpdateAsync(int id, UpdateCuentaDto dto)
        {
            var cuenta = await _unitOfWork.Cuentas.GetByIdAsync(id);
            
            if (cuenta == null)
                throw new BusinessException($"Cuenta con ID {id} no encontrada");

            // Validar número de cuenta único si se está actualizando
            if (dto.NumeroCuenta != null && 
                await _unitOfWork.Cuentas.ExistsByNumeroCuentaAsync(dto.NumeroCuenta, id))
            {
                throw new BusinessException($"Ya existe una cuenta con el número {dto.NumeroCuenta}");
            }

            // Actualizar solo los campos que vienen en el DTO
            if (dto.NumeroCuenta != null) cuenta.NumeroCuenta = dto.NumeroCuenta;
            if (dto.TipoCuenta.HasValue) cuenta.TipoCuenta = dto.TipoCuenta.Value;
            if (dto.Estado.HasValue) cuenta.Estado = dto.Estado.Value;

            await _unitOfWork.Cuentas.UpdateAsync(cuenta);
            await _unitOfWork.SaveChangesAsync();

            var saldoActual = await CalcularSaldoActualAsync(cuenta.Id, cuenta.SaldoInicial);

            return new CuentaDto
            {
                Id = cuenta.Id,
                NumeroCuenta = cuenta.NumeroCuenta,
                TipoCuenta = cuenta.TipoCuenta.ToString(),
                SaldoInicial = cuenta.SaldoInicial,
                SaldoActual = saldoActual,
                Estado = cuenta.Estado,
                ClienteId = cuenta.ClienteId
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cuenta = await _unitOfWork.Cuentas.GetByIdAsync(id);
            
            if (cuenta == null)
                return false;

            await _unitOfWork.Cuentas.DeleteAsync(cuenta);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private async Task<decimal> CalcularSaldoActualAsync(int cuentaId, decimal saldoInicial)
        {
            var ultimoMovimiento = await _unitOfWork.Movimientos.GetUltimoMovimientoByCuentaAsync(cuentaId);
            return ultimoMovimiento?.Saldo ?? saldoInicial;
        }
    }
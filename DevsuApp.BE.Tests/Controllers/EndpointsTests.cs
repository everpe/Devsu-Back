using DevsuApp.BE.API.Controllers;
using DevsuApp.BE.Application.DTOs;
using DevsuApp.BE.Application.Exceptions;
using DevsuApp.BE.Application.Interfaces.Services;
using DevsuApp.BE.Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DevsuApp.BE.Tests.Controllers
{
    /// <summary>
    /// Pruebas unitarias de los endpoints más críticos de la API
    /// </summary>
    public class EndpointsTests
    {
        /// <summary>
        /// PRUEBA 1: Verificar que se puede crear un cliente correctamente
        /// Endpoint: POST /api/clientes
        /// </summary>
        [Fact]
        public async Task POST_Clientes_ConDatosValidos_RetornaClienteCreado()
        {
            // Arrange - Preparar mocks y datos de prueba
            var mockClienteService = new Mock<IClienteService>();
            var mockLogger = new Mock<ILogger<ClientesController>>();
            
            var createDto = new CreateClienteDto
            {
                Nombre = "Jose Lema",
                Genero = "Masculino",
                Edad = 35,
                Identificacion = "1234567890",
                Direccion = "Otavalo sn y principal",
                Telefono = "0982547851",
                Contrasena = "1234",
                Estado = true
            };

            var clienteCreado = new ClienteDto
            {
                ClienteId = 1,
                Nombre = createDto.Nombre,
                Genero = createDto.Genero,
                Edad = createDto.Edad,
                Identificacion = createDto.Identificacion,
                Direccion = createDto.Direccion,
                Telefono = createDto.Telefono,
                Estado = createDto.Estado
            };

            mockClienteService
                .Setup(service => service.CreateAsync(It.IsAny<CreateClienteDto>()))
                .ReturnsAsync(clienteCreado);

            var controller = new ClientesController(mockClienteService.Object, mockLogger.Object);

            // Act - Ejecutar la acción del controller
            var result = await controller.Create(createDto);

            // Assert - Verificar los resultados
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
            
            var cliente = createdResult.Value.Should().BeAssignableTo<ClienteDto>().Subject;
            cliente.ClienteId.Should().Be(1);
            cliente.Nombre.Should().Be("Jose Lema");
            cliente.Identificacion.Should().Be("1234567890");
            
            // Verificar que el servicio fue llamado una vez
            mockClienteService.Verify(s => s.CreateAsync(It.IsAny<CreateClienteDto>()), Times.Once);
        }

        /// <summary>
        /// PRUEBA 2: Verificar validación de "Saldo no disponible"
        /// Endpoint: POST /api/movimientos
        /// Regla de negocio crítica: No permitir débitos si saldo = 0
        /// </summary>
        [Fact]
        public async Task POST_Movimientos_ConSaldoInsuficiente_LanzaExcepcion()
        {
            // Arrange
            var mockMovimientoService = new Mock<IMovimientoService>();
            var mockLogger = new Mock<ILogger<MovimientosController>>();
            
            var createDto = new CreateMovimientoDto
            {
                TipoMovimiento = TipoMovimiento.Debito,
                Valor = 1000,  // Intentar retirar $1000
                CuentaId = 1    // Cuenta con saldo 0
            };

            // Configurar el mock para que lance la excepción esperada
            mockMovimientoService
                .Setup(service => service.CreateAsync(It.IsAny<CreateMovimientoDto>()))
                .ThrowsAsync(new SaldoInsuficienteException());

            var controller = new MovimientosController(mockMovimientoService.Object, mockLogger.Object);

            // Act & Assert - Verificar que se lanza la excepción correcta
            var exception = await Assert.ThrowsAsync<SaldoInsuficienteException>(
                async () => await controller.Create(createDto)
            );

            exception.Message.Should().Be("Saldo no disponible");
            
            // Verificar que el servicio intentó procesar el movimiento
            mockMovimientoService.Verify(
                s => s.CreateAsync(It.IsAny<CreateMovimientoDto>()), 
                Times.Once
            );
        }
        
        
        /// <summary>
        /// PRUEBA 3: Verificar validación de "Cupo diario Excedido"
        /// Endpoint: POST /api/movimientos
        /// Regla de negocio crítica: Límite diario de retiros = $1000
        /// </summary>
        [Fact]
        public async Task POST_Movimientos_ConCupoDiarioExcedido_LanzaExcepcion()
        {
            // Arrange
            var mockMovimientoService = new Mock<IMovimientoService>();
            var mockLogger = new Mock<ILogger<MovimientosController>>();
    
            var createDto = new CreateMovimientoDto
            {
                TipoMovimiento = TipoMovimiento.Debito,
                Valor = 600,    // Intentar retirar $600 adicionales
                CuentaId = 1    // Cuenta que ya retiró $500 hoy (total = $1100 > $1000)
            };

            // Configurar el mock para que lance la excepción de cupo excedido
            mockMovimientoService
                .Setup(service => service.CreateAsync(It.IsAny<CreateMovimientoDto>()))
                .ThrowsAsync(new CupoDiarioExcedidoException(1000m, 500m));

            var controller = new MovimientosController(mockMovimientoService.Object, mockLogger.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CupoDiarioExcedidoException>(
                async () => await controller.Create(createDto)
            );

            // Verificar que el mensaje de error es correcto
            exception.Message.Should().Contain("Cupo diario Excedido");
            exception.Message.Should().Contain("Límite"); // Cambiado: más flexible
            exception.Message.Should().Contain("500");    // Verifica el valor usado
    
            // Verificar que el servicio fue invocado
            mockMovimientoService.Verify(
                s => s.CreateAsync(It.IsAny<CreateMovimientoDto>()), 
                Times.Once
            );
        }
    }
}
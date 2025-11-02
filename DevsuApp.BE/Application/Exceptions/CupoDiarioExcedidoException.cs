namespace DevsuApp.BE.Application.Exceptions;

public class CupoDiarioExcedidoException : BusinessException
{
    public CupoDiarioExcedidoException() 
        : base("Cupo diario Excedido")
    {
    }

    public CupoDiarioExcedidoException(decimal limite, decimal usado) 
        : base($"Cupo diario Excedido. Límite: ${limite:N2}, Usado hoy: ${usado:N2}")
    {
    }
}
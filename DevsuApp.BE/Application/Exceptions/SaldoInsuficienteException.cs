namespace DevsuApp.BE.Application.Exceptions;

public class SaldoInsuficienteException : BusinessException
{
    public SaldoInsuficienteException() 
        : base("Saldo no disponible")
    {
    }

    public SaldoInsuficienteException(string message) 
        : base(message)
    {
    }
}
namespace DevsuApp.BE.Application.Exceptions;

/// <summary>
/// Clase generica de excepciones de negocio
/// </summary>
public class BusinessException : Exception
{
    public BusinessException(string message) : base(message)
    {
    }

    public BusinessException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
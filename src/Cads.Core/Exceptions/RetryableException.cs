namespace Cads.Core.Exceptions;

public class RetryableException : Exception
{
    public RetryableException(string message) : base(message)
    {
    }
}
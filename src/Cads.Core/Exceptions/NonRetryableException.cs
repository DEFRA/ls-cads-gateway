namespace Cads.Core.Exceptions;

public class NonRetryableException : Exception
{
    public NonRetryableException(string message) : base(message)
    {
    }
}
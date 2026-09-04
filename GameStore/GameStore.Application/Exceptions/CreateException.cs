using Microsoft.Extensions.Logging;

namespace GameStore.Application.Exceptions;

public class CreateException : Exception
{
    public CreateException(string message, ILogger logger) : base(message)
    {
        logger.LogError("{Message}", message);
    }
}

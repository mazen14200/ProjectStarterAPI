
// This file is part of the ClientSatisfaction project.
namespace Application.Exceptions
{
    /// Exception class for handling service-related errors.
    public class ServiceException : Exception
    {
        // Default constructor with a predefined message.
        public ServiceException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
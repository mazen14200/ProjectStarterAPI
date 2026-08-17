using System;

namespace Domain.Entities
{
    public class LogsHistory
    {
        public int Id { get; set; }
        public string OperationType { get; set; }  // GET, POST, PUT, DELETE, etc.
        public string OperationName { get; set; }  // Controller/Action name or description
        public string? Path { get; set; }  // Request path
        public string? Method { get; set; }  // HTTP method
        public int? StatusCode { get; set; }  // Response status code
        public string? UserName { get; set; }  // User who made the request
        public string? IpAddress { get; set; }  // Client IP address
        public string? UserAgent { get; set; }  // Browser/user agent
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ErrorMessage { get; set; }  // Error message if any
        public long? DurationMs { get; set; }  // Request duration in milliseconds
    }
}

using System;

namespace TodoApp.Application.Common
{
    public class ServiceResponse<T>
    {
        public int StatusCode { get; set; }
        public T? Content { get; set; }
        public string? Message { get; set; }

        public ServiceResponse(int statusCode, T? content = default, string? message = null)
        {
            StatusCode = statusCode;
            Content = content;
            Message = message;
        }
    }
}

using System.Collections.Generic;
using System.Net;

namespace hoteru_be.DTOs
{
    public class MethodResultDTO
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Message { get; set; } = "";
        public Dictionary<string, List<string>> Errors { get; set; } = new();

        public static MethodResultDTO Ok(string message = "OK") =>
            new() { HttpStatusCode = HttpStatusCode.OK, Message = message };

        public static MethodResultDTO Created(string message = "Created") =>
            new() { HttpStatusCode = HttpStatusCode.Created, Message = message };

        public static MethodResultDTO BadRequest(string message = "Bad Request", Dictionary<string, List<string>>? errors = null) =>
            new() { HttpStatusCode = HttpStatusCode.BadRequest, Message = message, Errors = errors ?? new() };

        public static MethodResultDTO Unauthorized(string message = "Unauthorized") =>
            new() { HttpStatusCode = HttpStatusCode.Unauthorized, Message = message };

        public static MethodResultDTO Forbidden(string message = "Forbidden") =>
            new() { HttpStatusCode = HttpStatusCode.Forbidden, Message = message };

        public static MethodResultDTO NotFound(string message = "Not Found") =>
            new() { HttpStatusCode = HttpStatusCode.NotFound, Message = message };

        public static MethodResultDTO Conflict(string message = "Conflict") =>
            new() { HttpStatusCode = HttpStatusCode.Conflict, Message = message };

        public static MethodResultDTO Unprocessable(string message = "Unprocessable Entity", Dictionary<string, List<string>>? errors = null) =>
            new() { HttpStatusCode = HttpStatusCode.UnprocessableEntity, Message = message, Errors = errors ?? new() };

        public static MethodResultDTO Error(string message = "Internal Server Error") =>
            new() { HttpStatusCode = HttpStatusCode.InternalServerError, Message = message };
    }

    public class MethodResultDTO<T>
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Message { get; set; } = "";
        public Dictionary<string, List<string>> Errors { get; set; } = new();
        public T? Data { get; set; }


        public static MethodResultDTO<T> Ok(T data, string message = "OK") =>
            new() { HttpStatusCode = HttpStatusCode.OK, Message = message, Data = data };

        public static MethodResultDTO<T> Created(T data, string message = "Created") =>
            new() { HttpStatusCode = HttpStatusCode.Created, Message = message, Data = data };

        public static MethodResultDTO<T> BadRequest(string message = "Bad Request", Dictionary<string, List<string>>? errors = null) =>
            new() { HttpStatusCode = HttpStatusCode.BadRequest, Message = message, Errors = errors ?? new() };

        public static MethodResultDTO<T> Unauthorized(string message = "Unauthorized") =>
            new() { HttpStatusCode = HttpStatusCode.Unauthorized, Message = message };

        public static MethodResultDTO<T> Forbidden(string message = "Forbidden") =>
            new() { HttpStatusCode = HttpStatusCode.Forbidden, Message = message };

        public static MethodResultDTO<T> NotFound(string message = "Not Found") =>
            new() { HttpStatusCode = HttpStatusCode.NotFound, Message = message };

        public static MethodResultDTO<T> Conflict(string message = "Conflict") =>
            new() { HttpStatusCode = HttpStatusCode.Conflict, Message = message };

        public static MethodResultDTO<T> Unprocessable(string message = "Unprocessable Entity", Dictionary<string, List<string>>? errors = null) =>
            new() { HttpStatusCode = HttpStatusCode.UnprocessableEntity, Message = message, Errors = errors ?? new() };

        public static MethodResultDTO<T> Error(string message = "Internal Server Error") =>
            new() { HttpStatusCode = HttpStatusCode.InternalServerError, Message = message };
    }
}

using System.Diagnostics;
using DTB.Data.App.User;
using Microsoft.Extensions.Logging;

namespace DTB.Controllers
{
    public class ApiResponse<T>
    {
        private readonly ILogger<ApiResponse<T>> _logger;

        // 添加无参构造函数
        public ApiResponse()
        {
            Timestamp = DateTime.Now;
        }

        // 保留带 logger 的构造函数
        public ApiResponse(ILogger<ApiResponse<T>> logger) : this()
        {
            _logger = logger;
        }

        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string TraceId { get; set; }
        public DateTime Timestamp { get; set; }

        public static ApiResponse<T> Success(T data, string message = "Operation successful")
        {
            var traceId = Activity.Current?.Id ?? string.Empty;

            return new ApiResponse<T>
            {
                Code = 200,
                Message = message,
                Data = data,
                TraceId = traceId
            };
        }

        public static ApiResponse<T> Error(int code, string message, ILogger<ApiResponse<T>> logger = null, Exception ex = null, T data = default)
        {
            var traceId = Activity.Current?.Id ?? string.Empty;

            // 如果提供了 logger，则记录错误日志
            if (logger != null)
            {
                logger.LogError(ex,
                    "Error occurred. TraceId: {TraceId}, Code: {Code}, Message: {Message}, Exception: {Exception}",
                    traceId,
                    code,
                    message,
                    ex?.ToString() ?? "No exception details");

                // 如果是特定类型的异常，可以记录额外信息
                if (ex is ApplicationException appEx)
                {
                    logger.LogInformation(
                        "Additional application context. TraceId: {TraceId}, Context: {Context}",
                        traceId,
                        appEx.Data);
                }
            }

            return new ApiResponse<T>
            {
                Code = code,
                Message = message,
                Data = data,
                TraceId = traceId
            };
        }
    }

   
}
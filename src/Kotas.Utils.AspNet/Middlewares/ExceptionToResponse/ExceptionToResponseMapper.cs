using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kotas.Utils.AspNet.Middlewares.ExceptionToResponse
{
    public class ExceptionToResponseMapper
    {
        private readonly ILogger<ExceptionToResponseMapper> _logger;
        private readonly ExceptionMapping _mapping;

        public ExceptionToResponseMapper(ILogger<ExceptionToResponseMapper> logger, ExceptionMapping mapping)
        {
            _logger = logger;
            _mapping = mapping;
        }

        public ObjectResult Map(Exception exception)
        {
            if (exception == null)
                return CreateEmptyResult();

            LogException(exception);
            if (exception is AggregateException aggregateException)
                return CreateObjectResult(aggregateException);
            var statusCode = _mapping.MapExceptionToStatusCode(exception);
            return CreateObjectResult(exception, statusCode);
        }

        private ObjectResult CreateEmptyResult()
        {
            return CreateObjectResult("Executed exception mapper middleware but no exception provided. Fix it!", HttpStatusCode.InternalServerError);
        }

        private static ObjectResult CreateObjectResult(Exception exception, HttpStatusCode statusCode)
        {
            return CreateObjectResult(GenerateExceptionMsg(exception), statusCode);
        }

        private ObjectResult CreateObjectResult(AggregateException exception)
        {
            if (exception == null)
            {
                return CreateObjectResult("There is no aggregated exception", HttpStatusCode.InternalServerError);
            }

            var httpStatus = GetStatusCodeFromAggregateException(exception);
            var errors = exception.InnerExceptions.Select(x => new ResponseError
            {
                Message = GenerateExceptionMsg(x),
                Code = Map(x).StatusCode?.ToString() ?? "500"
            });

            return CreateObjectResult(errors.ToArray(), httpStatus);
        }

        private static ObjectResult CreateObjectResult(string message, HttpStatusCode statusCode)
        {
            var response = new ResponseError
            {
                Message = message,
                Code = ((int)statusCode).ToString()
            };

            return CreateObjectResult(new[] { response }, statusCode);
        }

        private static ObjectResult CreateObjectResult(ResponseError[] errors, HttpStatusCode mainStatusCode)
        {
            var response = new ResponseErrors
            {
                Errors = errors
            };
            return CreateObjectResult(response, mainStatusCode);
        }

        private static ObjectResult CreateObjectResult(ResponseErrors error, HttpStatusCode mainStatusCode)
        {
            var serialized = JsonConvert.SerializeObject(error, Common.Json.JsonSerializer.Settings);
            return CreateLastResult(serialized, mainStatusCode);
        }

        private static ObjectResult CreateLastResult(string message, HttpStatusCode statusCode)
        {
            var result = new ObjectResult(message)
            {
                StatusCode = (int)statusCode
            };
            return result;
        }

        private static string GenerateExceptionMsg(Exception exception)
        {
            if (exception == null)
            {
                return "No exception provided";
            }

            return exception is AggregateException aggrEx ? GenerateExceptionMsg(aggrEx) : exception.Message;
        }

        private static string GenerateExceptionMsg(AggregateException exception)
        {
            var exceptionMessages = string.Join($"{Environment.NewLine}{Environment.NewLine}", exception.InnerExceptions);
            return $"More errors occurred. {Environment.NewLine}{exceptionMessages}";
        }

        private static Exception GetFirstException(AggregateException exception)
        {
            var ex = exception.InnerExceptions.FirstOrDefault();
            if (ex == null)
            {
                return new Exception("There is no aggregated exception");
            }

            return ex;
        }

        private HttpStatusCode GetStatusCodeFromAggregateException(AggregateException exception)
        {
            var firstException = GetFirstException(exception);
            var intStatusCode = Map(firstException).StatusCode;

            return (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), intStatusCode.ToString());
        }

        private void LogException(Exception exception)
        {
            _logger?.LogError(exception, exception.Message);
        }
    }
}

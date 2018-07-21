using System;
using System.Collections.Generic;
using System.Net;
using Kotas.Utils.AspNet.Exceptions;
using Kotas.Utils.Common;

namespace Kotas.Utils.AspNet.Middlewares.ExceptionToResponse
{
    public class ExceptionMapping
    {
        private readonly Dictionary<Type, HttpStatusCode> _mappings;

        public ExceptionMapping()
        {
            _mappings = InitMappings();
        }

        public HttpStatusCode MapExceptionToStatusCode(Exception exception)
        {
            if (exception == null)
                return HttpStatusCode.InternalServerError;
            return MapExceptionToStatusCode(exception.GetType());
        }

        public HttpStatusCode MapExceptionToStatusCode(Type exceptionType)
        {
            if (exceptionType == null)
                return HttpStatusCode.InternalServerError;
            if (_mappings.ContainsKey(exceptionType))
                return _mappings[exceptionType];
            return HttpStatusCode.InternalServerError;
        }

        public void RegisterMapping(Type exceptionType, HttpStatusCode statusCode)
        {
            Validations.ValidateInput(exceptionType, nameof(exceptionType));
            _mappings[exceptionType] = statusCode;
        }

        public void RegisterMapping(IDictionary<Type, HttpStatusCode> mapping)
        {
            if(mapping == null)
                return;
            foreach (var definition in mapping)
            {
                RegisterMapping(definition.Key, definition.Value);
            }
        }

        private Dictionary<Type, HttpStatusCode> InitMappings()
        {
            return new Dictionary<Type, HttpStatusCode>
            {
                {
                    typeof(NotFoundException),
                    HttpStatusCode.NotFound
                },
                {
                    typeof(UnauthorizedException),
                    HttpStatusCode.Unauthorized
                },
                {
                    typeof(ForbiddenException),
                    HttpStatusCode.Forbidden
                },
                {
                    typeof(BadInputException),
                    HttpStatusCode.BadRequest
                },
                {
                    typeof(ArgumentException),
                    HttpStatusCode.BadRequest
                },
                {
                    typeof(ArgumentNullException),
                    HttpStatusCode.BadRequest
                },
                {
                    typeof(ArgumentOutOfRangeException),
                    HttpStatusCode.BadRequest
                },
                {
                    typeof(InvalidOperationException),
                    HttpStatusCode.NotAcceptable
                },
                {
                    typeof(NotImplementedException),
                    HttpStatusCode.NotImplemented
                },
                {
                    typeof(ConflictException),
                    HttpStatusCode.Conflict
                }
            };
        }
    }
}

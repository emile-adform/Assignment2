﻿using Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Assignment2.WebApi.Middlewares;

// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {

        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            var response = httpContext.Response;
            response.ContentType = "application/json";

            switch (ex)
            {
                case InvalidDateException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case ExternalApiDataException:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
                default:
                    _logger.LogError(ex, ex.Message);
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            var result = JsonSerializer.Serialize(new { message = ex?.Message });
            await response.WriteAsync(result);
        }
    }
}

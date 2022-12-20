﻿using System.Net;

using Newtonsoft.Json;

namespace GreenLocator;


public class LogMiddleware
{
    private readonly RequestDelegate _next;
    public LogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await LogException(context, ex);
        }
    }

    private Task LogException(HttpContext context, Exception ex)
    {
        HttpStatusCode code = HttpStatusCode.InternalServerError;

        if (ex is IndexOutOfRangeException) code = HttpStatusCode.BadRequest;
        string result = JsonConvert.SerializeObject(new { error = ex.Message });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}


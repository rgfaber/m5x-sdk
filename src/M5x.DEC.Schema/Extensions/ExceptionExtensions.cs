﻿using System;
using System.Collections.Generic;

namespace M5x.DEC.Schema.Extensions;

public static class ExceptionExtensions
{
    public static string InnerAndOuter(this Exception ex)
    {
        var result = ex.Message;
        if (ex.InnerException != null) result += $"\n -> {ex.InnerException.InnerAndOuter()}";
        return result;
    }


    public static Error AsApiError(this Exception e)
    {
        var res = new Error
        {
            Subject = "Unexpected Error",
            ErrorMessage = e.Message,
            LastModified = DateTime.UtcNow,
            Source = e.Source,
            Content = e.Message,
            Stack = e.StackTrace
        };
        if (e.InnerException != null)
            res.InnerError = e.InnerException.AsApiError();
        return res;
    }


    public static IDictionary<string, Error> AddRange(this IDictionary<string, Error> target,
        IDictionary<string, Error> source)
    {
        if (source == null) return target;
        foreach (var it in source) target.Add(it);
        return target;
    }
}
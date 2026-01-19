using System;
using System.Collections.Generic;
using System.Text;
using Hermes.Results;
using HoroscopeApi.Shared.Constants;

namespace HoroscopeApi.Shared.Results;

public static class ResultExtensions
{
    public static Result<T> WithMetadata<T>(this Result<T> result, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(result);
        result.Metadata ??= [];
        result.Metadata[key] = value;
        return result;
    }

    public static Result<T> BadRequest<T>(IReadOnlyList<IError> errors)
    {
        var result = Result.Ko<T>(errors, new Dictionary<string, string?>
        {
            {  ErrorConsts.ErrorType, ErrorConsts.BadRequestCode }
        });

        return result;
    }

    public static Result<T> NotFound<T>(IReadOnlyList<IError> errors)
    {
        var result = Result.Ko<T>(errors, new Dictionary<string, string?>
        {
            {  ErrorConsts.ErrorType, ErrorConsts.NotFoundCode }
        });

        return result;
    }

    public static Result<T> InternalServerError<T>(IReadOnlyList<IError> errors)
    {
        var result = Result.Ko<T>(errors, new Dictionary<string, string?>
        {
            {  ErrorConsts.ErrorType, ErrorConsts.InternalServerErrorCode }
        });

        return result;
    }

    public static Result<T> Unauthorized<T>(IReadOnlyList<IError> errors)
    {
        var result = Result.Ko<T>(errors, new Dictionary<string, string?>
        {
            {  ErrorConsts.ErrorType, ErrorConsts.UnauthorizedCode }
        });

        return result;
    }
}

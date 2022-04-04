﻿using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;

namespace Nike.Api.Activators;

public static class CommandValidationResultExtension
{
    public static void RaiseExceptionIfRequired(this ValidationResult validationResult)
    {
        if (!validationResult.IsValid)
            throw new ValidationException(string.Join(Environment.NewLine,
                validationResult.Errors.Select(error => error.ErrorMessage)));
    }
}
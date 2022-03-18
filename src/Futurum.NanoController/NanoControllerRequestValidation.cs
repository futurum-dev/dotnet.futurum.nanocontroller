using FluentValidation;

using Futurum.Core.Result;
using Futurum.FluentValidation;

namespace Futurum.NanoController;

public interface INanoControllerRequestValidation<TRequest>
{
    Task<Result> ExecuteAsync(TRequest request);
}

internal class NanoControllerRequestValidation<TRequest> : INanoControllerRequestValidation<TRequest>
{
    private readonly IValidator<TRequest>[] _validator;

    public NanoControllerRequestValidation(IEnumerable<IValidator<TRequest>> validator)
    {
        _validator = validator.ToArray();
    }

    public Task<Result> ExecuteAsync(TRequest request) =>
        _validator.Length switch
        {
            0 => Result.OkAsync(),
            1 => ValidateAsync(_validator[0], request),
            _ => _validator.FlatMapAsync(validator => ValidateAsync(validator, request))
        };

    private static Task<Result> ValidateAsync(IValidator<TRequest> validator, TRequest request)
    {
        var validationResult = validator.Validate(request);

        return validationResult.IsValid
            ? Result.OkAsync()
            : Result.FailAsync(validationResult.ToResultError());
    }
}
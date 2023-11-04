using System.Collections.Immutable;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using MediatR;

namespace CsMediatR.Infrastructure;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IValidator<TRequest>[] _validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators.ToArray();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();
        var context = new ValidationContext<TRequest>(request);
        var failures = (await Task.WhenAll(_validators
                .Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
using System.Collections.Immutable;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using MediatR;

namespace CsMediatR.Infrastructure;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly CompositeValidator<TRequest> _validators;

    private class CompositeValidator<T> : IValidator<T>
    {
        private readonly IReadOnlyCollection<IValidator<T>> validators;

        public CompositeValidator(IReadOnlyCollection<IValidator<T>> validators)
        {
            this.validators = validators;
        }

        public ValidationResult Validate(IValidationContext context)
        {
            return new ValidationResult(from validator in validators
                let result = validator.Validate(context)
                where !result.IsValid
                from error in result.Errors 
                select error);
        }

        public async Task<ValidationResult> ValidateAsync(IValidationContext context, CancellationToken cancellation = new CancellationToken())
        {
            return new ValidationResult(from result in 
                    await Task.WhenAll(
                        from validator in validators
                        let asyncResult = validator.ValidateAsync(context,cancellation)
                        select asyncResult)
                where !result.IsValid
                from error in result.Errors 
                select error);

        }

        public IValidatorDescriptor CreateDescriptor()
        {
            return new ValidatorDescriptor<T>(
                from v in validators 
                let descriptor=v.CreateDescriptor()
                from rule in descriptor.Rules
                select rule);
        }

        public bool CanValidateInstancesOfType(Type? type) => typeof(T).IsAssignableFrom(type);

        public ValidationResult Validate(T instance) => Validate(new ValidationContext<T>(instance, new PropertyChain(), ValidatorOptions.Global.ValidatorSelectors.DefaultValidatorSelectorFactory()));

        public async Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellation = new CancellationToken()) => 
            await ValidateAsync(new ValidationContext<T>(instance, new PropertyChain(), ValidatorOptions.Global.ValidatorSelectors.DefaultValidatorSelectorFactory()), cancellation);
    }

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = new CompositeValidator<TRequest>(validators.ToImmutableList());
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        await _validators.ValidateAndThrowAsync(request,cancellationToken);

        return await next();
    }
}
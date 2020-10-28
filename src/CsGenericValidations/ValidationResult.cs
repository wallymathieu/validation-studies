using OneOf;

namespace CsGenericValidations
{
    public abstract class ValidationResult<TSuccess,TFailure> : OneOfBase<ValidationResult<TSuccess,TFailure>.Success, ValidationResult<TSuccess,TFailure>.Failure>
    {
        public class Success : ValidationResult<TSuccess,TFailure> { public TSuccess Value { get; protected internal set; } }  
        public class Failure  : ValidationResult<TSuccess,TFailure> { public TFailure Value { get; protected internal set; } }

        public bool IsValid => this is Success;

        public static ValidationResult<TSuccess, TFailure> NewSuccess(TSuccess success) => new Success { Value = success };

        public static ValidationResult<TSuccess, TFailure> NewFailure(TFailure failure) => new Failure { Value = failure };
    }
}
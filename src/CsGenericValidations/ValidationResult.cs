using OneOf;

namespace CsGenericValidations
{
    public class ValidationResult<TSuccess,TFailure> : OneOfBase<TSuccess,TFailure>
    {
        public bool IsValid => this.IsT0;

        public static ValidationResult<TSuccess, TFailure> NewSuccess(TSuccess success) =>  new( success);

        public static ValidationResult<TSuccess, TFailure> NewFailure(TFailure failure) => new (failure);

        protected ValidationResult(OneOf<TSuccess,TFailure> input) : base(input)
        {
        }
    }
}
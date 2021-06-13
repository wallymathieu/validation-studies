namespace CsGenericVisitors
{
    public interface IValidationResultVisitor<in TSuccess, in TFailure>
    {
        void Success(TSuccess success);
        void Failure(TFailure failure);
    }
}
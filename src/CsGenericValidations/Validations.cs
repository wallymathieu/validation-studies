using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CsGenericValidations
{
    class Validations
    {
        public static string ToString<TSuccess,TFailure>(ValidationResult<TSuccess,TFailure> validations)
        {
            return validations.Match<string>(success => "Success", failure =>
                failure.Value is IEnumerable
                    ? string.Join(";", ValuesToString((IEnumerable)failure.Value))
                    : failure.Value.ToString()
            );
        }

        private static IEnumerable<string> ValuesToString(IEnumerable enumerable )
        {
            return from f in enumerable.Cast<object>() select f.ToString();
        }
    }
}

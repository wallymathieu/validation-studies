using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace CsAttributes
{
    /// see https://github.com/reustmd/DataAnnotationsValidatorRecursive
    class Validations
    {
        public static IEnumerable<ValidationResult> Validate(Person o)
        {
            var validationResults = new List<ValidationResult>();
            TryValidateObjectRecursive(o, validationResults);
            return validationResults;
        }
        private static bool TryValidateObject(object obj, ICollection<ValidationResult> results)
        {
            return Validator.TryValidateObject(obj, new ValidationContext(obj), results, true);
        }
        private static bool TryValidateObjectRecursive<T>(T obj, List<ValidationResult> results)
        {

            bool result = TryValidateObject(obj, results);

            var properties = obj.GetType().GetProperties().Where(prop => prop.CanRead
                && prop.GetIndexParameters().Length == 0).ToList();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType) continue;

                var value = property.GetValue(obj);

                if (value == null) continue;

                var asEnumerable = value as IEnumerable;
                if (asEnumerable != null)
                {
                    var i = 0;
                    foreach (var enumObj in asEnumerable)
                    {
                        var nestedResults = new List<ValidationResult>();
                        if (!TryValidateObjectRecursive(enumObj, nestedResults))
                        {
                            result = false;
                            foreach (var validationResult in nestedResults)
                            {
                                PropertyInfo property1 = property;
                                results.Add(new ValidationResult(validationResult.ErrorMessage,
                                    validationResult.MemberNames.Select(x => $"{property1.Name}[{i}].{x}").ToArray()));
                            }
                        };
                        i++;
                    }
                }
                else
                {
                    var nestedResults = new List<ValidationResult>();
                    if (!TryValidateObjectRecursive(value, nestedResults))
                    {
                        result = false;
                        foreach (var validationResult in nestedResults)
                        {
                            PropertyInfo property1 = property;
                            results.Add(new ValidationResult(validationResult.ErrorMessage,
                                validationResult.MemberNames.Select(x => property1.Name + '.' + x)));
                        }
                    }
                }
            }

            return result;
        }
        public static string ToString(IEnumerable<ValidationResult> validations)
        {
            return string.Join(";", validations.Select(v =>
                 $"{v.ErrorMessage}:{String.Join(",", v.MemberNames)}"
            ).ToArray());
        }
    }
}

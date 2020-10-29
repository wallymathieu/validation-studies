using System;
/*
 * Copyright (c) 2016 Harry McIntyre
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * This is essentially OneOf<,>:
 * https://github.com/mcintyre321/OneOf/blob/master/OneOf/OneOf.cs
 * Please contribute any relevant changes to the above repository
 */

namespace CsGenericVisitors
{
    public interface IValidationResultVisitor<in TSuccess, in TFailure>
    {
        T Success<T>(TSuccess success);
        T Failure<T>(TFailure failure);
    }
    public readonly struct ValidationResult<TSuccess,TFailure>
    {

        readonly TSuccess _value0;
        readonly TFailure _value1;
        readonly int _index;

        ValidationResult(int index, TSuccess value0 = default(TSuccess), TFailure value1 = default(TFailure))
        {
            _index = index;
            _value0 = value0;
            _value1 = value1;
        }

        public object Value
        {
            get
            {
                switch (_index)
                {
                    case 0:
                        return _value0;
                    case 1:
                        return _value1;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public bool IsSuccess => _index == 0;

        public TSuccess AsSuccess
        {
            get
            {
                if (_index != 0)
                {
                    throw new InvalidOperationException($"Cannot return as TSuccess as result is T{_index}");
                }
                return _value0;
            }
        }

        public static implicit operator ValidationResult<TSuccess, TFailure>(TSuccess t) => new ValidationResult<TSuccess, TFailure>(0, value0: t);


        public bool IsFailure => _index == 1;

        public TFailure AsFailure
        {
            get
            {
                if (_index != 1)
                {
                    throw new InvalidOperationException($"Cannot return as TFailure as result is T{_index}");
                }
                return _value1;
            }
        }

        public static implicit operator ValidationResult<TSuccess, TFailure>(TFailure t) => new ValidationResult<TSuccess, TFailure>(1, value1: t);


        public void Switch(Action<TSuccess> f0, Action<TFailure> f1)
        {
            switch (_index)
            {
                case 0 when f0 != null:
                    f0(_value0);
                    return;
                case 1 when f1 != null:
                    f1(_value1);
                    return;
                default:
                    throw new InvalidOperationException();
            }
        }

        public T Accept<T>(IValidationResultVisitor<TSuccess, TFailure> visitor)
        {
            switch (_index)
            {
                case 0:
                    return visitor.Success<T>(_value0);
                case 1:
                    return visitor.Failure<T>(_value1);
                default:
                    throw new InvalidOperationException();
            }
        }

        public TResult Match<TResult>(Func<TSuccess, TResult> f0, Func<TFailure, TResult> f1)
        {
            switch (_index)
            {
                case 0 when f0 != null:
                    return f0(_value0);
                case 1 when f1 != null:
                    return f1(_value1);
                default:
                    throw new InvalidOperationException();
            }
        }

        public static ValidationResult<TSuccess, TFailure> FromSuccess(TSuccess input)
        {
            return input;
        }

        public static ValidationResult<TSuccess, TFailure> FromFailure(TFailure input)
        {
            return input;
        }

        public ValidationResult<TResult, TFailure> MapSuccess<TResult>(Func<TSuccess, TResult> mapFunc)
        {
            if(mapFunc == null)
            {
                throw new ArgumentNullException(nameof(mapFunc));
            }
            return Match<ValidationResult<TResult, TFailure>>(
                inputSuccess => mapFunc(inputSuccess),
                inputFailure => inputFailure
            );
        }

        public ValidationResult<TSuccess, TResult> MapFailure<TResult>(Func<TFailure, TResult> mapFunc)
        {
            if(mapFunc == null)
            {
                throw new ArgumentNullException(nameof(mapFunc));
            }
            return Match<ValidationResult<TSuccess, TResult>>(
                inputSuccess => inputSuccess,
                inputFailure => mapFunc(inputFailure)
            );
        }

		public bool TryPickSuccess(out TSuccess value, out TFailure remainder)
		{
			value = this.IsSuccess ? this.AsSuccess : default(TSuccess);
			remainder = this.IsSuccess ? default(TFailure) : this.AsFailure;
			return this.IsSuccess;
		}

		public bool TryPickFailure(out TFailure value, out TSuccess remainder)
		{
			value = this.IsFailure ? this.AsFailure : default(TFailure);
			remainder = this.IsFailure ? default(TSuccess) : this.AsSuccess;
			return this.IsFailure;
		}

        bool Equals(ValidationResult<TSuccess, TFailure> other)
        {
            if (_index != other._index)
            {
                return false;
            }
            switch (_index)
            {
                case 0: return Equals(_value0, other._value0);
                case 1: return Equals(_value1, other._value1);
                default: return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            

            return obj is ValidationResult<TSuccess, TFailure> && Equals((ValidationResult<TSuccess, TFailure>)obj);
        }

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}: {value?.ToString()}";
            switch(_index) {
                case 0: return FormatValue(typeof(TSuccess), _value0);
                case 1: return FormatValue(typeof(TFailure), _value1);
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOf codegen.");
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode;
                switch (_index)
                {
                    case 0:
                    hashCode = _value0?.GetHashCode() ?? 0;
                    break;
                    case 1:
                    hashCode = _value1?.GetHashCode() ?? 0;
                    break;
                    default:
                        hashCode = 0;
                        break;
                }
                return (hashCode*397) ^ _index;
            }
        }

    }
}
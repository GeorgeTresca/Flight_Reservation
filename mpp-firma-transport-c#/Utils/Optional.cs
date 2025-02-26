using System;
namespace Utils
{
    public class Optional<T>
    {
        private readonly T _value;
        private readonly bool _hasValue;

        public Optional(T value)
        {
            this._value = value;
            this._hasValue = true;
        }

        private Optional()
        {
            if (_value == null)
            {
                throw new ArgumentNullException(nameof(_value));
            }
            this._value = _value;
            this._hasValue = false;
        }

        public bool HasValue => _hasValue;

        public static Optional<T> Empty() => new Optional<T>();

        public static Optional<T> Of(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");

            return new Optional<T>(value);
        }

        public T Value
        {
            get
            {
                if (!_hasValue)
                    throw new InvalidOperationException("No value present");
                return _value;
            }
        }

        public T GetValueOrDefault() => _value;

        public T GetValueOrDefault(T defaultValue) => _hasValue ? _value : defaultValue;

        public override string ToString() => _hasValue ? _value.ToString() : "No value";

        // Implementations of Equals, GetHashCode, etc. could be added based on requirements
    }
}
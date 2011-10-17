using System;
using System.Text;
using CuttingEdge.Conditions;

namespace BuildingBlocks.Common.ValueObjects
{
    public struct PersonName
    {
        public static PersonName Parse(string sourceValue)
        {
            //Condition.Requires(sourceValue, "sourceValue").IsNotNull();

            if (string.IsNullOrEmpty(sourceValue) || string.IsNullOrEmpty(sourceValue.Trim()))
                return new PersonName();

            var names = sourceValue.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var lastName = names.Length > 0 ? names[0] : null;
            var firstName = names.Length > 1 ? names[1] : null;
            var middleName = names.Length > 2 ? names[2] : null;
            return new PersonName(lastName, firstName, middleName, false);
        }

        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _middleName;

        public PersonName(string lastName, string firstName, string middleName, bool requiredNotEmptyNames = true)
        {
            if (requiredNotEmptyNames)
            {
                Condition.Requires(firstName, "firstName").IsNotNullOrEmpty();
                Condition.Requires(lastName, "lastName").IsNotNullOrEmpty();
            }

            _firstName = firstName;
            _lastName = lastName;
            _middleName = middleName;
        }

        public string FirstName
        {
            get { return _firstName; }
        }
        
        public string LastName
        {
            get { return _lastName; }
        }
        
        public string MiddleName
        {
            get { return _middleName; }
        }

        public string FullName
        {
            get
            {
                if (IsEmpty)
                    return string.Empty;
                var result = _lastName + " " + _firstName;
                return string.IsNullOrEmpty(_middleName) ? result : result + " " + _middleName;
            }
            private set { }
        }

        public string GetIntitialName()
        {
            var initialsBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(LastName))
            {
                initialsBuilder.Append(LastName);
                initialsBuilder.Append(' ');
            }

            if (!string.IsNullOrEmpty(FirstName))
            {
                initialsBuilder.Append(FirstName[0]);
                initialsBuilder.Append('.');
            }

            if (!string.IsNullOrEmpty(MiddleName))
            {
                initialsBuilder.Append(MiddleName[0]);
                initialsBuilder.Append('.');
            }

            return initialsBuilder.ToString();
        }

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(_lastName) && !string.IsNullOrEmpty(_firstName); }
        }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(_lastName) && string.IsNullOrEmpty(_firstName); }
        }

        public bool Equals(PersonName other)
        {
            return Equals(other._firstName, _firstName) &&
                   Equals(other._lastName, _lastName) &&
                   Equals(other._middleName, _middleName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (obj.GetType() != typeof (PersonName)) 
                return false;
            return Equals((PersonName) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (_firstName != null ? _firstName.GetHashCode() : 0);
                result = (result*397) ^ (_lastName != null ? _lastName.GetHashCode() : 0);
                result = (result*397) ^ (_middleName != null ? _middleName.GetHashCode() : 0);
                return result;
            }
        }
    }
}
using System;
using SafeMessages.Models.BaseModel;

namespace SafeMessages.Models
{
    public class UserId : ObservableObject, IComparable, IEquatable<UserId>
    {
        public UserId(string name) => Name = name;

        public string Name { get; }

        public int CompareTo(object obj)
        {
            if (!(obj is UserId other))
                throw new NotSupportedException();

            return string.CompareOrdinal(Name, other.Name);
        }

        public bool Equals(UserId other)
        {
            if (other is null)
                return false;
            return ReferenceEquals(this, other) || string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((UserId)obj);
        }

        public override int GetHashCode()
            => Name != null ? Name.GetHashCode() : 0;
    }
}
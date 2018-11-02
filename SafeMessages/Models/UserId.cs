using SafeMessages.Models.BaseModel;
using System;

namespace SafeMessages.Models {
  public class UserId : ObservableObject, IComparable, IEquatable<UserId> {
    public string Name { get; }

    public UserId(string name) {
      Name = name;
    }

    public int CompareTo(object obj) {
      var other = obj as UserId;
      if (other == null) {
        throw new NotSupportedException();
      }

      return string.CompareOrdinal(Name, other.Name);
    }

    public bool Equals(UserId other) {
      if (ReferenceEquals(null, other)) {
        return false;
      }
      return ReferenceEquals(this, other) || string.Equals(Name, other.Name);
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) {
        return false;
      }
      if (ReferenceEquals(this, obj)) {
        return true;
      }
      return obj.GetType() == GetType() && Equals((UserId)obj);
    }

    public override int GetHashCode() {
      return Name != null ? Name.GetHashCode() : 0;
    }
  }
}

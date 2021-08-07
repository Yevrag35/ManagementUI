using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ManagementUI.Functionality.Models.Converters;

namespace ManagementUI.Functionality.Models
{
    [JsonConverter(typeof(UserTagConverter))]
    public struct UserTag : IEnumerable<char>,
        IComparable<UserTag>, IComparable<string>, IEquatable<UserTag>, IEquatable<string>, IEquatable<int>
    {
        #region JSON PROPERTIES
        public int Id;
        public string Value;

        public UserTag(int id, string value)
        {
            Id = id;
            Value = value;
        }

        public IEnumerator<char> GetEnumerator()
        {
            return Value.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int CompareTo(UserTag other)
        {
            return Id.CompareTo(other.Id);
        }
        public int CompareTo(string other)
        {
            return StringComparer.CurrentCultureIgnoreCase.Compare(Value, other);
        }
        public bool Equals(UserTag other)
        {
            return Id.Equals(other.Id);
        }
        public bool Equals(int other)
        {
            return Id.Equals(other);
        }
        public bool Equals(string str)
        {
            return StringComparer.CurrentCultureIgnoreCase.Equals(Value, str);
        }

        #endregion

        public static explicit operator string(UserTag tag) => tag.Value;
        public static explicit operator char[](UserTag tag) => tag.Value?.ToCharArray();
        public static implicit operator UserTag(string tag) => new UserTag { Value = tag };
    }
}
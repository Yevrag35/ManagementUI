using System;
using System.Collections.Generic;

namespace ManagementUI
{
    internal struct IgnoreCaseEquality : IEqualityComparer<string>
    {
        public bool Equals(string x, string y) => x.Equals(y, StringComparison.CurrentCultureIgnoreCase);

        public int GetHashCode(string str) => str.ToLower().GetHashCode();
    }
}

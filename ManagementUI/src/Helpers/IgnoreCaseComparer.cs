using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementUI
{
    internal class IgnoreCaseComparer : IComparer<string>
    {
        private CaseInsensitiveComparer _comparer;

        public IgnoreCaseComparer() => _comparer = CaseInsensitiveComparer.Default;

        public int Compare(string x, string y) => _comparer.Compare(x, y);
    }
}

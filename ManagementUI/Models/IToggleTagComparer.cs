using System;
using System.Collections.Generic;

namespace ManagementUI
{
    public interface IToggleTagComparer : IEqualityComparer<ToggleTag>
    {
        IEqualityComparer<string> StringComparer { get; }
    }
}

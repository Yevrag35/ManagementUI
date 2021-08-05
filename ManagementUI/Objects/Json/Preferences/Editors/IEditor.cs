using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementUI.Json.Preferences.Editors
{
    public interface IEditor
    {
        string Name { get; }
        Task LaunchAsync();
    }
}

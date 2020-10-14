using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ManagementUI.Json.Apps
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class AppModel
    {
    }
}

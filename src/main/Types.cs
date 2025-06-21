using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Plugins.Tree
{
    public enum ContextMenuOption
    {
        NotSet,
        New,
        Edit,
        Delete,
        AddRelative,
        ExpandUntilPostsynapticExternalReferences,
        ExpandUntilFarthestPresynaptic
    }
    public class Constants
    {
        public static class QueryParameters
        {
            public static string ExpandUntilPostsynapticMirrors = "eupm";
        }
    }
}

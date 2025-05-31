using System;

namespace ei8.Cortex.Diary.Plugins.Tree
{
    internal static class Helper
    {
        internal static void ReinitializeOption(Action<ContextMenuOption> optionSetter)
        {
            optionSetter(ContextMenuOption.NotSet);
            optionSetter(ContextMenuOption.New);
        }
    }
}

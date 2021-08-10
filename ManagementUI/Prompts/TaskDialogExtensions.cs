using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI.Prompts
{
    public static class TaskDialogExtensions
    {
        internal static TaskDialog AddInstruction(this TaskDialog dialog, string instruction, params object[] args)
        {
            dialog.MainInstruction = string.Format(instruction, args);
            return dialog;
        }

        internal static TaskDialog AddYesNo(this TaskDialog dialog, bool defaultToNo = true)
        {
            dialog.Buttons.Add(new TaskDialogButton(ButtonType.Yes) { Default = !defaultToNo });
            dialog.Buttons.Add(new TaskDialogButton(ButtonType.No) { Default = defaultToNo });

            return dialog;
        }
        internal static TaskDialog AddOk(this TaskDialog dialog, bool isDefault = true)
        {
            dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok) { Default = isDefault });
            return dialog;
        }
        internal static TaskDialog AddClose(this TaskDialog dialog, bool isDefault)
        {
            dialog.Buttons.Add(new TaskDialogButton(ButtonType.Close) { Default = isDefault });
            return dialog;
        }
        internal static TaskDialog AddContent(this TaskDialog dialog, string message, params object[] args)
        {
            dialog.Content = string.Format(message, args);
            return dialog;
        }

        internal static TaskDialog SetIcon(this TaskDialog dialog, TaskDialogIcon icon)
        {
            dialog.MainIcon = icon;
            return dialog;
        }
    }
}

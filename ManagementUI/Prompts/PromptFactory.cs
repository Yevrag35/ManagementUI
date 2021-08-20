using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ManagementUI.Prompts
{
    public static class PromptFactory
    {
        public static TaskDialog NewDialog(string title, TaskDialogIcon icon, params Action<TaskDialog>[] actions)
        {
            var dialog = new TaskDialog
            {
                WindowTitle = title,
                MainIcon = icon
            };

            if (null == actions && actions.Length <= 0)
                return dialog;

            for (int i = 0; i < actions.Length; i++)
            {
                actions[i]?.Invoke(dialog);
            }

            if (dialog.Buttons.Count <= 0)
                _ = dialog.AddOk();

            return dialog;
        }

        public static void DoOkPrompt(Window parent, string title, TaskDialogIcon icon, params Action<TaskDialog>[] extraActions)
        {
            using (TaskDialog dialog = NewDialog(title, icon, extraActions))
            {
                _ = dialog.ClearButtons().AddOk().SwallowError(parent);
            }
        }
        public static bool DoYesNoPrompt(Window parent, string title, TaskDialogIcon icon, bool noIsDefault, params Action<TaskDialog>[] extraActions)
        {
            using (TaskDialog dialog = NewDialog(title, icon, extraActions))
            {
                return ButtonType.Yes == dialog
                    .ClearButtons()
                        .AddYesNo(noIsDefault)
                            .SwallowError(parent)?.ButtonType;
            }
        }

        private static TaskDialog ClearButtons(this TaskDialog dialog)
        {
            if (dialog.Buttons.Count > 0)
            {
                dialog.Buttons.Clear();
            }

            return dialog;
        }
        private static TaskDialogButton SwallowError(this TaskDialog dialog, Window parent)
        {
            if (SwallowError(dialog, parent, out TaskDialogButton taskDialogButton))
            {
                return taskDialogButton;
            }
            else
                return null;
        }
        private static bool SwallowError(TaskDialog dialog, Window parent, out TaskDialogButton buttonPressed)
        {
            buttonPressed = null;
            dialog.CenterParent = true;
            try
            {
                buttonPressed = dialog.ShowDialog(parent);
            }
            catch
            {
                return false;
            }

            return null != buttonPressed;
        }
    }
}

using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ManagementUI
{
    public partial class MUI
    {
        private const string ERR_MSG = "An error occurred: {0}{1}{1}{2}";
        private const string ERR_MSG_TITLE = "ERROR!";
        private const MessageBoxButton ERR_MSG_BTN = MessageBoxButton.OK;
        private const MessageBoxImage ERR_MSG_IMG = MessageBoxImage.Error;

        internal static void ShowErrorMessage(Exception e)
        {
            string msg = string.Format(ERR_MSG, e.GetType().FullName, Environment.NewLine, e.Message);
            MessageBox.Show(msg, ERR_MSG_TITLE, ERR_MSG_BTN, ERR_MSG_IMG);
        }

        internal static bool ShowErrorMessage(Exception e, bool canRetry)
        {
            string msg = string.Format(ERR_MSG, e.GetType().FullName, Environment.NewLine, e.Message);
            MsgBoxStyle btn = canRetry
                ? MsgBoxStyle.RetryCancel | MsgBoxStyle.DefaultButton1 | MsgBoxStyle.Critical | MsgBoxStyle.SystemModal | MsgBoxStyle.MsgBoxSetForeground
                : MsgBoxStyle.OkOnly | MsgBoxStyle.DefaultButton1 | MsgBoxStyle.Critical | MsgBoxStyle.SystemModal | MsgBoxStyle.MsgBoxSetForeground;

            MsgBoxResult res = Interaction.MsgBox(msg, btn, ERR_MSG_TITLE);
            return res == MsgBoxResult.Retry;
        }
    }
}

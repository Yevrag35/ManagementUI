using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Strings = ManagementUI.Properties.Resources;

namespace ManagementUI.Controls
{
    public class UIntTextBox : TextBox
    {
        private const string UNICODE_TEXT = "UnicodeText";
        private const uint MINIMUM = 0u;
        private const uint DEFAULT_MAXIMUM = 50u;

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(uint), typeof(UIntTextBox));
        public uint Maximum
        {
            get => (uint)base.GetValue(MaximumProperty);
            set => this.SetValue(MaximumProperty, value);
        }

        public UIntTextBox()
            : base()
        {
        }

        private static bool IsTextValid(IDataObject obj, uint high)
        {
            return obj.GetData(UNICODE_TEXT) is string text && IsTextValid(text, high);
        }
        private static bool IsTextValid(string text, uint high)
        {
            return uint.TryParse(text, out uint found) &&
                MINIMUM <= found && high >= found;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.Maximum == MINIMUM)
                this.Maximum = DEFAULT_MAXIMUM;

            this.MaxLength = this.Maximum.ToString().Length;

            DataObject.AddPastingHandler(this, this.PastingEventHandler);
            this.LostKeyboardFocus += this.ResetInvalidEntryAsync;
        }

        private void PastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (!IsTextValid(e.DataObject, this.Maximum))
            {
                e.CancelCommand();
                e.Handled = true;
            }
        }

        private async void ResetInvalidEntryAsync(object sender, KeyboardFocusChangedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (!IsTextValid(this.Text, this.Maximum))
                {
                    this.Text = MINIMUM.ToString();
                }
            });
        }
    }
}

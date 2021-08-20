using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ManagementUI.Controls
{
    public class UIntTextBox : TextBox
    {
        private const string UNICODE_TEXT = "UnicodeText";

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(uint), typeof(UIntTextBox));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(uint), typeof(UIntTextBox));

        public uint Minimum
        {
            get => (uint)base.GetValue(MinimumProperty);
            set => base.SetValue(MinimumProperty, value);
        }
        public uint Maximum
        {
            get => (uint)base.GetValue(MaximumProperty);
            set => base.SetValue(MaximumProperty, value);
        }

        public UIntTextBox()
            : base()
        {
        }

        private static bool IsTextValid(IDataObject obj, uint low, uint high)
        {
            return obj.GetData(UNICODE_TEXT) is string text && IsTextValid(text, low, high);
        }
        private static bool IsTextValid(string text, uint low, uint high)
        {
            return uint.TryParse(text, out uint found) &&
                low <= found && high >= found;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.Maximum < this.Minimum)
                this.Maximum = this.Minimum + 1u;

            this.MaxLength = this.Maximum.ToString().Length;

            DataObject.AddPastingHandler(this, this.PastingEventHandler);
            this.LostKeyboardFocus += this.ResetInvalidEntryAsync;
        }

        private void PastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (!IsTextValid(e.DataObject, this.Minimum, this.Maximum))
            {
                e.CancelCommand();
                e.Handled = true;
            }
        }

        private async void ResetInvalidEntryAsync(object sender, KeyboardFocusChangedEventArgs e)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                if (!IsTextValid(this.Text, this.Minimum, this.Maximum))
                {
                    this.Text = 0.ToString();
                }
            });
        }
    }
}

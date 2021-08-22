using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

using Strings = ManagementUI.Properties.Resources;

namespace ManagementUI
{
    public enum OverrideCursorStatus : int
    {
        Normal = 0,
        Wait = 1,
        Disposed = 2
    }

    public interface ICursorManager : IDisposable
    {
        bool IsOverriden { get; }

        OverrideCursorStatus GetStatus();
        Cursor GetWaitCursor();
        bool SetCursorStatus(OverrideCursorStatus status);
        void ToggleCursorStatus();
    }

    public static class CursorFactory
    {
        public static ICursorManager Create()
        {
            return new CursorManager();
        }

        private sealed class CursorManager : ICursorManager
        {
            private bool _disposed;
            private Cursor _wait;
            private OverrideCursorStatus _currentStatus;

            bool ICursorManager.IsOverriden => _currentStatus != OverrideCursorStatus.Normal
                && !_disposed;

            internal CursorManager()
            {
                _wait = Cursors.Wait;
                _currentStatus = OverrideCursorStatus.Normal;
            }

            private bool CannotModify()
            {
                return _disposed || _currentStatus == OverrideCursorStatus.Disposed;
            }
            private void Dispose(bool disposing)
            {
                if (this.CannotModify())
                    return;

                if (disposing)
                {
                    _wait.Dispose();
                    _currentStatus = OverrideCursorStatus.Disposed;
                    _disposed = true;
                }
            }
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
            OverrideCursorStatus ICursorManager.GetStatus() => _currentStatus;
            Cursor ICursorManager.GetWaitCursor() => !this.CannotModify() ? _wait : null;
            public bool SetCursorStatus(OverrideCursorStatus status)
            {
                if (this.CannotModify())
                    return false;

                switch (status)
                {
                    case OverrideCursorStatus.Wait:
                        {
                            Mouse.OverrideCursor = _wait;
                            _currentStatus = status;

                            return true;
                        }
                    default:
                        {
                            Mouse.OverrideCursor = null;
                            _currentStatus = status;
                            if (status == OverrideCursorStatus.Disposed)
                                this.Dispose();

                            return true;
                        }
                }
            }
            void ICursorManager.ToggleCursorStatus()
            {
                if (this.CannotModify())
                    return;

                _ = this.SetCursorStatus((OverrideCursorStatus)((((int)_currentStatus) - 1) * -1));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;
using ManagementUI.Functionality.Auth;
using ManagementUI.Functionality.Executable;
using ManagementUI.Json;

namespace ManagementUI
{
    public static partial class LaunchFactory
    {
        private static bool _parentIsElevated;
        private static IUserIdentity _userId;
        private static EditorManager _manager;
        public static void Initialize(EditorManager manager)
        {
            _manager = manager;
            _parentIsElevated = IsElevated();
        }
        public static void AddCredentials(IUserIdentity userId)
        {
            Deinitialize();
            _userId = userId;
        }
        public static void Deinitialize()
        {
            if (null != _userId)
            {
                _userId.Dispose();
            }
        }

        public static bool Execute(ILaunchable launchable, out Exception caughtException)
        {
            bool result = false;
            caughtException = null;
            using (Process process = launchable.MakeProcess(_parentIsElevated, true, _userId))
            {
                try
                {
                    result = process.Start();
                }
                catch (Exception e)
                {
                    caughtException = e;
                }
            }

            return result;
        }
        public static void ExecuteEditor(EditorManager manager, string key)
        {
            manager.Start(key, _parentIsElevated);
        }
        private static bool IsElevated()
        {
            var winId = WindowsIdentity.GetCurrent();
            var prinId = new WindowsPrincipal(winId);
            return prinId.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}

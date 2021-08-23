using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Strings = ManagementUI.Functionality.Properties.Resources;

namespace ManagementUI.Functionality.Executable
{
    public class ArgumentLibrary
    {
        private ReadOnlyDictionary<string, Action<ProcessStartInfo, string>> _fromArgs;
        private Dictionary<Expression<Func<ProcessStartInfo, object>>, Func<object, string>> _toArgs;

        public ArgumentLibrary()
        {
            _toArgs =
                new Dictionary<Expression<Func<ProcessStartInfo, object>>, Func<object, string>>(6, new ExpressionComparer())
                {
                    { x => x.FileName, x => Format(nameof(ProcessStartInfo.FileName), x) },
                    { x => x.Arguments, x => Format(nameof(ProcessStartInfo.Arguments), x) },
                    { x => x.UseShellExecute, x => Format(nameof(ProcessStartInfo.UseShellExecute), x) },
                    { x => x.CreateNoWindow, x => Format(nameof(ProcessStartInfo.CreateNoWindow), x) },
                    { x => x.LoadUserProfile, x => Format(nameof(ProcessStartInfo.LoadUserProfile), x) },
                    { x => x.ErrorDialogParentHandle, x => string.Format(Strings.Argument_Format, nameof(ProcessStartInfo.ErrorDialogParentHandle), Convert.ToInt32(x)) }
                };
            _fromArgs = new ReadOnlyDictionary<string, Action<ProcessStartInfo, string>>(
                new Dictionary<string, Action<ProcessStartInfo, string>>(6, StringComparer.CurrentCultureIgnoreCase)
                {
                    { nameof(ProcessStartInfo.FileName), (psi, val) => psi.FileName = val },
                    { nameof(ProcessStartInfo.Arguments), (psi, val) => psi.Arguments = val },
                    { nameof(ProcessStartInfo.ErrorDialogParentHandle), (psi, val) =>
                    {
                        if (int.TryParse(val, out int ptrInt))
                        {
                            psi.ErrorDialogParentHandle = new IntPtr(ptrInt);
                        }
                    }},
                    { nameof(ProcessStartInfo.UseShellExecute), (psi, val) => psi.UseShellExecute = Convert.ToBoolean(val) },
                    { nameof(ProcessStartInfo.CreateNoWindow), (psi, val) => psi.CreateNoWindow = Convert.ToBoolean(val) },
                    { nameof(ProcessStartInfo.LoadUserProfile), (psi, val) => psi.LoadUserProfile = Convert.ToBoolean(val) }
                }
            );
        }

        public bool TryGetArgument(ProcessStartInfo psi, Expression<Func<ProcessStartInfo, object>> expression, out string argument)
        {
            argument = null;
            if (_toArgs.TryGetValue(expression, out Func<object, string> func))
            {
                var expFunc = expression.Compile();
                object value = expFunc(psi);
                if (null != value)
                {
                    argument = func(value);
                }
            }

            return !string.IsNullOrEmpty(argument);
        }
        public bool TryGetAction(string key, out Action<ProcessStartInfo, string> action)
        {
            return _fromArgs.TryGetValue(key, out action);
        }

        private static string Format(string name, object value)
        {
            string valStr = FormatValue(value);
            return string.Format(Strings.Argument_Format, name, valStr);
        }
        private static string FormatValue(object value)
        {
            if (value is string oStr)
            {
                if (oStr.Contains("\""))
                    return Regex.Replace(oStr, "\\\"", "\"\"\"\"\"");

                else if (oStr.Contains(" "))
                    return string.Format("\"{0}\"", oStr);

                else
                    return oStr;
            }
            else
            {
                return Convert.ToString(value);
            }
        }

        private class ExpressionComparer : EqualityComparer<Expression<Func<ProcessStartInfo, object>>>
        {
            public override bool Equals(Expression<Func<ProcessStartInfo, object>> x, Expression<Func<ProcessStartInfo, object>> y)
            {
                if (TryGetMemberInfo(x, out MemberInfo xMember) && TryGetMemberInfo(y, out MemberInfo yMember))
                {
                    return xMember.DeclaringType.Equals(yMember.DeclaringType) &&
                        xMember.Name.Equals(yMember.Name) && xMember.MemberType == yMember.MemberType;
                }
                else
                    return false;
            }

            private static bool TryGetMemberInfo(Expression<Func<ProcessStartInfo, object>> expression, out MemberInfo memInfo)
            {
                memInfo = null;
                if (expression.Body is MemberExpression xMem)
                {
                    memInfo = xMem.Member;
                }
                else if (expression.Body is UnaryExpression unEx && unEx.Operand is MemberExpression unExMem)
                {
                    memInfo = unExMem.Member;
                }

                return null != memInfo;
            }

            public override int GetHashCode(Expression<Func<ProcessStartInfo, object>> obj)
            {
                if (!TryGetMemberInfo(obj, out MemberInfo memInfo))
                    return obj.GetHashCode();

                return memInfo.GetHashCode();
            }
        }
    }
}

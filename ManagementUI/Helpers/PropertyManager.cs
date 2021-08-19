using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ManagementUI
{
    public partial class MUI
    {
        private void Set<T>(string name, T value, Action<T> action)
        {
            action(value);
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void SetField<T>(T value, Expression<Func<MUI, T>> memberExpression)
        {
            if (this.TryGetMember(value, memberExpression, out FieldInfo fi))
            {
                fi.SetValue(this, value);
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(fi.Name));
            }
        }
        private void SetProperty<T>(T value, Expression<Func<MUI, T>> memberExpression)
        {
            if (this.TryGetMember(value, memberExpression, out PropertyInfo pi))
            {
                pi.SetValue(this, value);
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pi.Name));
            }
        }
        private bool TryGetMember<T, TMem>(T value, Expression<Func<MUI, T>> memberExpression, out TMem member)
            where TMem : MemberInfo
        {
            member = null;

            if (memberExpression.Body is MemberExpression memEx)
            {
                member = memEx.Member as TMem;
            }
            else if (memberExpression.Body is UnaryExpression unEx && unEx.Operand is MemberExpression unExMem)
            {
                member = unExMem.Member as TMem;
            }

            return null != member;
        }
    }
}

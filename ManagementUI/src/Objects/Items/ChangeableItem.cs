using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace ManagementUI
{
    public abstract class ChangeableItem : INotifyPropertyChanged
    {
        public abstract event PropertyChangedEventHandler PropertyChanged;

        protected string GetPropertyName<T, TMember>(Expression<Func<T, TMember>> expression) where T : ChangeableItem
        {
            string memberName = null;
            if (expression.Body is MemberExpression memEx)
            {
                memberName = memEx.Member.Name;
            }
            else if (expression.Body is UnaryExpression unEx && unEx.Operand is MemberExpression unExMem)
            {
                memberName = unExMem.Member.Name;
            }
            return memberName;
        }
    }
}

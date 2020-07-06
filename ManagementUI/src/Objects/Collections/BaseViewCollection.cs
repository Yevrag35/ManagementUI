using ManagementUI.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Data;

namespace ManagementUI
{
    /// <summary>
    /// A custom abstract implementation of <see cref="ObservableCollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of element the collection holds.</typeparam>
    public abstract class BaseViewCollection<T> : ObservableCollection<T>
    {
        private SortDescription? _defaultSortDescription;
        private ListCollectionView _backingView;

        /// <summary>
        /// Indicates whether the <see cref="BaseViewCollection{T}"/> has a default
        /// <see cref="SortDescription"/> added the generated View.
        /// </summary>
        public bool HasDefaultSort => _defaultSortDescription.HasValue;

        /// <summary>
        /// The constructed view of the <see cref="BaseViewCollection{T}"/>.
        /// </summary>
        public ListCollectionView View => _backingView;

        public BaseViewCollection(ListSortDirection direction, Expression<Func<T, object>> sortExpression)
            : base()
        {
            _defaultSortDescription = GetDescriptionFromExpression(direction, sortExpression);
            this.UpdateView();
        }
        public BaseViewCollection(IEnumerable<T> items, SortDescription? sortDescription)
            : base(items.Where(x => x != null))
        {
            _defaultSortDescription = sortDescription;
            this.UpdateView();
        }

        public BaseViewCollection(IEnumerable<T> items, ListSortDirection direction, Expression<Func<T, object>> sortExpression)
            : this(items, GetDescriptionFromExpression(direction, sortExpression))
        {

        }

        #region PUBLIC METHODS
        /// <summary>
        /// Adds the non-null elements of the specified collection to the end of the <see cref="BaseViewCollection{T}"/>.
        /// </summary>
        /// <param name="collection">
        ///     The collection whose elements should be added to the end of the <see cref="BaseViewCollection{T}"/>.
        ///     The collections itself nor the items contained can be null.
        /// </param>
        public void AddRange(IEnumerable<T> collection)
        {
            List<T> addList = new List<T>(collection.Where(x => x != null));

            if (addList.Count > 0)
            {
                addList.ForEach((x) =>
                {
                    base.Items.Add(x);
                });
                base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, addList));
            }
        }

        /// <summary>
        /// Creates and adds a new <see cref="SortDescription"/> from the specified <see cref="MemberExpression"/> and adds it
        /// to the <see cref="ListCollectionView.SortDescriptions"/> collection.  If a view has not been created yet, no operation
        /// occurs.
        /// </summary>
        /// <typeparam name="U">The type returned by the member from the expression.</typeparam>
        /// <param name="memberExpression">The <see cref="MemberExpression"/> used to construct the <see cref="SortDescription"/>.</param>
        /// <param name="sortDirection">The direction to sort the <see cref="SortDescription"/>.</param>
        public void AddSortDescription<U>(Expression<Func<T, U>> memberExpression, ListSortDirection sortDirection)
        {
            if (_backingView != null && TryGetMemberNameFromExpression(memberExpression, out string name))
            {
                var sortDesc = new SortDescription(name, sortDirection);
                if (_backingView.SortDescriptions.Count <= 0 || !_backingView.SortDescriptions.Contains(sortDesc))
                {
                    _backingView.SortDescriptions.Add(sortDesc);
                }
            }
        }

        public void AddOppositeDirectionSort(string propertyName)
        {
            if (this.View.SortDescriptions.TryGet(propertyName, out SortDescription foundSortDesc))
            {
                this.View.SortDescriptions.ChangeDirection(foundSortDesc);
            }
            else
            {
                this.View.SortDescriptions.AddOnly(propertyName, ListSortDirection.Ascending);
            }
        }

        /// <summary>
        /// Updates the <see cref="ListCollectionView"/> of the <see cref="HsObservableCollection{T}.View"/> with the 
        /// default <see cref="SortDescription"/> if one is present.
        /// </summary>
        public void UpdateView()
        {
            this.CreateDefaultView();
            if (_defaultSortDescription.HasValue)
            {
                _backingView.SortDescriptions.Add(_defaultSortDescription.Value);
            }
        }
        /// <summary>
        /// Updates the <see cref="ListCollectionView"/> of the <see cref="HsObservableCollection{T}.View"/> with the 
        /// specified member expression creating a new <see cref="ListSortDescription"/> in the specified direction.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="memberExpression"></param>
        /// <param name="sortDirection"></param>
        public void UpdateView<U>(Expression<Func<T, U>> memberExpression, ListSortDirection sortDirection)
        {
            this.CreateDefaultView();
            this.AddSortDescription(memberExpression, sortDirection);
        }

        #endregion

        #region BACKEND/PRIVATE METHODS
        private void CreateDefaultView() => _backingView = CollectionViewSource.GetDefaultView(this) as ListCollectionView;

        private static SortDescription? GetDescriptionFromExpression(ListSortDirection direction, Expression<Func<T, object>> memberExpression)
        {
            SortDescription? maybe = null;
            if (TryGetMemberNameFromExpression(memberExpression, out string memberName))
            {
                var sortDesc = new SortDescription(memberName, direction);
                maybe = sortDesc;
            }

            return maybe;
        }
        
        private static string GetMemberNameFromExpression<U>(Expression<Func<T, U>> memberExpression)
        {
            string name = null;

            if (memberExpression.Body is MemberExpression memEx)
            {
                name = memEx.Member.Name;
            }
            else if (memberExpression.Body is UnaryExpression unEx && unEx.Operand is MemberExpression unExMem)
            {
                name = unExMem.Member.Name;
            }

            return name;
        }
        protected static bool TryGetMemberNameFromExpression<U>(Expression<Func<T, U>> memberExpression, out string memberName)
        {
            memberName = GetMemberNameFromExpression(memberExpression);
            return !string.IsNullOrEmpty(memberName);
        }

        #endregion
    }
}

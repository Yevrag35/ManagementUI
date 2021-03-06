﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace ManagementUI
{
    public class AppSettingCollection : BaseMuiCollection<AppIconSetting>, ICloneable
    {
        #region PROPERTIES
        public string[] Tags => this.Where(x => x.Tags != null).SelectMany(x => x.Tags).Distinct().ToArray();

        #endregion

        #region CONSTRUCTORS
        public AppSettingCollection() : base() { }
        public AppSettingCollection(int capacity) : base(capacity) { }
        public AppSettingCollection(IEnumerable<AppIconSetting> settings) : base(settings) { }

        #endregion

        #region PUBLIC METHODS
        public AppSettingCollection Clone() => new AppSettingCollection(this);
        public AppIconSetting Find(Predicate<AppIconSetting> match) => base.InnerList.Find(match);
        public List<AppIconSetting> FindAll(Predicate<AppIconSetting> match) => base.InnerList.FindAll(match);
        public int FindIndex(Predicate<AppIconSetting> match) => base.InnerList.FindIndex(match);
        public int RemoveAll(Predicate<AppIconSetting> match) => base.InnerList.RemoveAll(match);

        #endregion

        #region INTERFACE IMPLEMENTATIONS
        object ICloneable.Clone() => this.Clone();

        #endregion

        public class AppIconSettingDefaultSorter : IComparer<AppIconSetting>
        {
            public int Compare(AppIconSetting x, AppIconSetting y) => x.Name.CompareTo(y.Name);
        }
    }
}
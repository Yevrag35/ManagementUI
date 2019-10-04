using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace ManagementUI
{
    public class AppSettingCollection : List<AppIconSetting>, INotifyCollectionChanged
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PROPERTIES
        public string[] Tags => this.Where(x => x.Tags != null).SelectMany(x => x.Tags).Distinct().ToArray();

        #endregion

        #region CONSTRUCTORS
        public AppSettingCollection()
        {

        }

        #region EVENTS

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #endregion

        #region PUBLIC METHODS


        #endregion

        #region BACKEND/PRIVATE METHODS


        #endregion
    }
}
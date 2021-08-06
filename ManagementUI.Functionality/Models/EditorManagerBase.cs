using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ManagementUI.Functionality.Settings;
using ManagementUI.Functionality.Models.Converters;

namespace ManagementUI.Functionality.Models
{
    public abstract class EditorManagerBase : IEnumerable<KeyValuePair<string, IEditor>>
    {
        private Dictionary<string, IEditor> _editors;
        private string _filePath;

        public IEditor this[string key]
        {
            get => _editors[key];
        }

        public int Count => _editors.Count;
        public bool DefaultsAdded { get; private set; }
        public string FilePath
        {
            get => _filePath;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _filePath = string.Format("\"{0}\"", value);

                else
                    _filePath = value;
            }
        }

        public EditorManagerBase(string filePath)
        {
            this.FilePath = filePath;
            _editors = new Dictionary<string, IEditor>(5, StringComparer.CurrentCultureIgnoreCase)
            {
                { "vsCode", new VisualStudioCodeEditor(filePath) },
                { "notepad", new NotepadEditor(filePath) },
                { "notepad++", new NotepadPlusPlusEditor(filePath) }
            };
            this.DefaultsAdded = true;
        }
        public EditorManagerBase()
        {
            _editors = new Dictionary<string, IEditor>(5, StringComparer.CurrentCultureIgnoreCase);
        }


        public void AddDefaults(string filePath)
        {
            this.FilePath = filePath;
            if (this.DefaultsAdded)
                return;

            _editors.Add("vsCode", new VisualStudioCodeEditor(this.FilePath));
            _editors.Add("notepad", new NotepadEditor(this.FilePath));
            _editors.Add("notepad++", new NotepadPlusPlusEditor(this.FilePath));
            this.DefaultsAdded = true;
        }
        public void Add(JObject editor)
        {
            CustomEditor ce = JsonConvert.DeserializeObject<CustomEditor>(editor.ToString());
            if (null != ce && !_editors.ContainsKey(ce.Key))
            {
                _editors.Add(ce.Key, ce);
            }
        }

        #region ENUMERATORS
        public IEnumerator<KeyValuePair<string, IEditor>> GetEnumerator()
        {
            return _editors.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}

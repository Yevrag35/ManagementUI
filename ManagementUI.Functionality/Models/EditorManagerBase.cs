using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using ManagementUI.Functionality.Executable;
using ManagementUI.Functionality.Settings;
using ManagementUI.Functionality.Models.Converters;

namespace ManagementUI.Functionality.Models
{
    public abstract class EditorManagerBase : IDisposable, IEnumerable<KeyValuePair<string, IEditor>>
    {
        private bool _disposed;
        private Dictionary<string, IEditor> _editors;
        protected Hashtable RunningProcesses { get; private set; }
        private string _filePath;

        public event EditorEventHandler EditorExited;

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
            this.FilePath = Environment.ExpandEnvironmentVariables(filePath);
            this.RunningProcesses = Hashtable.Synchronized(new Hashtable(1));
            _editors = new Dictionary<string, IEditor>(5, StringComparer.CurrentCultureIgnoreCase);
            this.AddDefaults();
        }

        private void AddDefaults()
        {
            if (this.DefaultsAdded)
                return;

            this.Add("vsCode", new VisualStudioCodeEditor(this.FilePath));
            this.Add("notepad", new NotepadEditor(this.FilePath));
            this.Add("notepad++", new NotepadPlusPlusEditor(this.FilePath));
            this.DefaultsAdded = true;
        }
        private void Add(string key, IEditor editor)
        {
            editor.ProcessExited += this.Editor_Exited;
            _editors.Add(key, editor);
        }
        public void Add(JObject editor)
        {
            CustomEditor ce = JsonConvert.DeserializeObject<CustomEditor>(editor.ToString());
            if (null != ce && !_editors.ContainsKey(ce.Key))
            {
                this.Add(ce.Key, ce);
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            if (null != this.RunningProcesses && this.RunningProcesses.Count > 0)
            {
                foreach (DictionaryEntry de in this.RunningProcesses)
                {
                    if (de.Value is IDisposable idis)
                    {
                        idis.Dispose();
                    }
                }

                this.RunningProcesses.Clear();
            }

            _disposed = true;
        }

        public void Start(string key, bool isParentElevated, bool runAs = false)
        {
            if (_editors.ContainsKey(key))
            {
                Process startedProcess = _editors[key].Start(isParentElevated, runAs);
                this.RunningProcesses.Add(startedProcess.Id, startedProcess);
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

        private void Editor_Exited(object sender, EditorEventArgs e)
        {
            if (this.RunningProcesses.ContainsKey(e.ProcessId) &&
                this.RunningProcesses[e.ProcessId] is IDisposable idis)
            {
                idis.Dispose();
                this.RunningProcesses.Remove(e.ProcessId);
            }

            this.EditorExited?.Invoke(this, e);
        }
    }
}

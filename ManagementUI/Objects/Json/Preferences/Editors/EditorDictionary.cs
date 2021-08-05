using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementUI.Json.Preferences.Editors
{
    public class EditorDictionary : Dictionary<string, IEditor>
    {
        public EditorDictionary()
            : base()
        {
        }

        public EditorDictionary(IEnumerable<IEditor> editors)
            : base(editors.ToDictionary(x => x.Name, StringComparer.CurrentCultureIgnoreCase))
        {
        }

        public void Add(IEditor editor)
        {
            base.Add(editor.Name, editor);
        }
        public bool Remove(IEditor editor)
        {
            return base.Remove(editor.Name);
        }
    }
}

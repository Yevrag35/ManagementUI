using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementUI.Functionality.Models.Collections
{
    public class TagIdDictionary : IEnumerable<UserTag>
    {
        private Dictionary<string, int> _tagsToIds;
        private int _nextId;

        public int Count => _tagsToIds.Count;
        public int NextId => _nextId;

        public UserTag this[string key]
        {
            get => new UserTag { Id = _tagsToIds[key], Value = key };
        }

        public TagIdDictionary()
        {
            _tagsToIds = new Dictionary<string, int>(3, StringComparer.CurrentCultureIgnoreCase);
            _nextId = 1;
        }

        public bool Add(string newKey)
        {
            try
            {
                _tagsToIds.Add(newKey, this.NextId);
            }
            catch
            {
                return false;
            }

            _nextId++;
            return true;
        }
        internal void Clear()
        {
            _tagsToIds.Clear();
        }
        public bool Contains(object key)
        {
            return key is string strKey && _tagsToIds.ContainsKey(strKey);
        }
        public bool ContainsKey(string tag)
        {
            return _tagsToIds.ContainsKey(tag);
        }

        public IEnumerator<UserTag> GetEnumerator()
        {
            foreach (var kvp in _tagsToIds)
            {
                yield return new UserTag(kvp.Value, kvp.Key);
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI
{
    public class TagList : IEnumerable<FilterTag>
    {
        private SortedList<string, FilterTag> _list;

        public FilterTag this[int index] => _list.Values[index];
        public FilterTag this[string tag] => _list[tag];

        public int Count => _list.Count;

        public TagList()
        {
            IComparer<string> comparer = new IgnoreCaseComparer();
            _list = new SortedList<string, FilterTag>(comparer);
        }

        public void Add(string newTag)
        {
            var ft = new FilterTag(newTag);
            _list.Add(ft.Tag, ft);
        }
        public void Add(FilterTag ft) => _list.Add(ft.Tag, ft);
        public void AddRange(IEnumerable<FilterTag> fts)
        {
            foreach (FilterTag ft in fts)
            {
                _list.Add(ft.Tag, ft);
            }
        }

        public IEnumerator<FilterTag> GetEnumerator() => _list.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public void Remove(params string[] tags)
        {
            foreach (string t in tags)
            {
                _list.Remove(t);
            }
        }
    }
}

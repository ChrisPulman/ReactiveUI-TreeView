using ReactiveUI;
using System.Collections.Generic;

namespace TreeViewInheritedItem
{
    public class Person : TreeItem
    {
        private string _name;

        public Person(string name, IEnumerable<TreeItem> children = null)
            : base(children) => Name = name;

        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
    }
}

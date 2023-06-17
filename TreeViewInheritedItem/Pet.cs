using ReactiveUI;

namespace TreeViewInheritedItem
{
    public class Pet : TreeItem
    {
        private string _name;

        public Pet(string name) => Name = name;

        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
    }
}

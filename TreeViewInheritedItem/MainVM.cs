using System;
using System.Reactive;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace TreeViewInheritedItem
{
    public class MainVM : ReactiveObject
    {
        private readonly ReactiveCommand<Unit, Unit> _addPerson;
        private readonly ReactiveCommand<Unit, Unit> _addPet;
        private readonly ReactiveCommand<Unit, Unit> _collapse;
        private readonly ReactiveCommand<Unit, Unit> _clear;

        public MainVM()
        {
            var bobbyJoe = new Person("Bobby Joe", new[] { new Pet("Fluffy") });
            var bob = new Person("Bob", new[] { bobbyJoe });
            var joe = new Person("Joe (expands 1000 nodes, look at memory usage)");

            // These 1000 nodes don't show up in memory usage until their parent node is expanded.
            // Shouldn't a thousand very simple TreeView node VM's use a bit less memory? Now it's an about 120mb increase on my machine.
            // Also expanding the node is a very slow operation.
            // Collapsing the parent node does nothing for memory usage. 
            for (int i = 0; i < 1000; i++)
            {
                joe.AddChild(new Person("Little Joe" + i));
            }
            _familySrc.Add(bob);
            _familySrc.Add(joe);

            _addPerson = ReactiveCommand.Create(() =>
            {
                if (SelectedItem == null) return;
                var p = new Person(NewName);
                SelectedItem.AddChild(p);
                p.IsSelected = true;
                p.ExpandPath();
            });

            _addPet = ReactiveCommand.Create(() =>
            {
                if (SelectedItem == null) return;
                var p = new Pet(PetName);
                SelectedItem.AddChild(p);
                p.IsSelected = true;
                p.ExpandPath();
            });

            _collapse = ReactiveCommand.Create(() =>
            {
                SelectedItem?.CollapsePath();
            });

            _clear = ReactiveCommand.Create(() =>
            {
                _familySrc.Clear();
                _familySrc.Add(new Person("Tree cleared, look at memory usage"));
            });

            _familySrc.Connect().Bind(_family).Subscribe();
        }

        private SourceList<TreeItem> _familySrc = new SourceList<TreeItem>();
        private readonly ObservableCollectionExtended<TreeItem> _family = new ObservableCollectionExtended<TreeItem>();
        public ObservableCollectionExtended<TreeItem> Family => this._family;
        public ReactiveCommand<Unit, Unit> AddPerson => this._addPerson;
        public ReactiveCommand<Unit, Unit> AddPet => this._addPet;
        public ReactiveCommand<Unit, Unit> Collapse => this._collapse;

        // Clearing the tree, and adding a node does nothing for memory usage. Why?
        public ReactiveCommand<Unit, Unit> Clear => this._clear;

        string _newName;
        public string NewName
        {
            get { return _newName; }
            set { this.RaiseAndSetIfChanged(ref _newName, value); }
        }
        string _petName;
        public string PetName
        {
            get { return _petName; }
            set { this.RaiseAndSetIfChanged(ref _petName, value); }
        }

        TreeItem _selectedItem;
        public TreeItem SelectedItem
        {
            get { return _selectedItem; }
            set { this.RaiseAndSetIfChanged(ref _selectedItem, value); }
        }
    }
}

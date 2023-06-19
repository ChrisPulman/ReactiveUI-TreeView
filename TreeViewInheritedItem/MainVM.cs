using System;
using System.Linq;
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
        private readonly SourceList<ITreeItem> _familySrc = new();
        private readonly ObservableCollectionExtended<ITreeItem> _family = new();

        public MainVM()
        {
            var bobbyJoe = new Person("Bobby Joe", new[] { new Pet("Fluffy") });
            var bob = new Person("Bob", new[] { bobbyJoe });
            var joe = new Person("Joe (expands 1000 nodes, look at memory usage)");

            // These 1000 nodes don't show up in memory usage until their parent node is expanded.
            // Shouldn't a thousand very simple TreeView node VM's use a bit less memory? Now it's an about 120mb increase on my machine.
            // Also expanding the node is a very slow operation.
            // Collapsing the parent node does nothing for memory usage. 
            var children = Enumerable.Range(0, 1000).Select(x => new Person("Little Joe" + x));
            joe.AddRange(children);

            _familySrc.Add(bob);
            _familySrc.Add(joe);

            _addPerson = ReactiveCommand.Create(() =>
            {
                if (SelectedItem == null) return;
                var p = new Person(NewName);
                SelectedItem.Add(p);
                p.IsSelected = true;
                p.ExpandPath();
            });

            _addPet = ReactiveCommand.Create(() =>
            {
                if (SelectedItem == null) return;
                var p = new Pet(PetName);
                SelectedItem.Add(p);
                p.IsSelected = true;
                p.ExpandPath();
            });

            _collapse = ReactiveCommand.Create(() =>
            {
                SelectedItem?.CollapsePath();
            });

            _clear = ReactiveCommand.Create(() =>
            {
                // Clearing the tree, and adding a node does nothing for memory usage. Why?
                // When you call Clear() all that happens is the SourceList is cleared, and the ObservableCollection is updated.
                // The cleared items are still in memory, and the ObservableCollection is still holding references to them.
                // In order to clear the memory, you need to call Dispose() on the items followed by GC.Collect().
                // Therefore I added IDisposable to TreeItem and a ClearDispose() extension method to SourceList.
                // This method calls Dispose() on all items in the SourceList, and then clears the list.
                // GC.Collect() is called, to expedite the collection of the disposed items.
                // If you don't call GC.Collect(), the disposed items will be collected eventually, but it takes a while.
                _familySrc.ClearDispose();
                _familySrc.Add(new Person("Tree cleared, look at memory usage after 2 seconds"));
            });

            _familySrc.Connect().Bind(_family).Subscribe();
        }

        public ObservableCollectionExtended<ITreeItem> Family => _family;
        public ReactiveCommand<Unit, Unit> AddPerson => _addPerson;
        public ReactiveCommand<Unit, Unit> AddPet => _addPet;
        public ReactiveCommand<Unit, Unit> Collapse => _collapse;

        /// <summary>
        /// Clearing the tree, and adding a node does nothing for memory usage. Why?
        /// When you call Clear() all that happens is the SourceList is cleared, and the ObservableCollection is updated.
        /// The cleared items are still in memory, and the ObservableCollection is still holding references to them.
        /// In order to clear the memory, you need to call Dispose() on the items followed by GC.Collect().
        /// Therefore I added a ClearDispose() extension method to SourceList.
        /// This method calls Dispose() on all items in the SourceList, and then clears the list.
        /// GC.Collect() is called, to expedite the collection of the memory for the purposes of this demonstration.
        /// However, in a real application you should not call GC.Collect() as it is very expensive, it will be done automatically.
        /// </summary>
        public ReactiveCommand<Unit, Unit> Clear => _clear;

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

        ITreeItem _selectedItem;
        public ITreeItem SelectedItem
        {
            get { return _selectedItem; }
            set { this.RaiseAndSetIfChanged(ref _selectedItem, value); }
        }
    }
}

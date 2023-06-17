using System.Reactive.Disposables;
using ReactiveUI;
using Splat;

namespace TreeViewInheritedItem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // This is needed to ensure that the treeview only loads items when they are in view (i.e. virtualization)
            // If a TreeView contains many items, the amount of time it takes to load may cause a significant delay in the user interface.
            // You can improve the load time by setting the VirtualizingStackPanel.IsVirtualizing attached property to true.
            // The UI might also be slow to react when a user scrolls the TreeView by using the mouse wheel or dragging the thumb of a scrollbar.
            // You can improve the performance of the TreeView when the user scrolls by setting the VirtualizingStackPanel.VirtualizationMode attached property to VirtualizationMode.Recycling.
            FamilyTree.OnlyLoadItemsInView();

            this.WhenActivated(d =>
            {
                //build viewmodel
                ViewModel = new MainVM();
                //Register views
                Locator.CurrentMutable.Register(() => new PersonView(), typeof(IViewFor<Person>));
                Locator.CurrentMutable.Register(() => new PetView(), typeof(IViewFor<Pet>));
                //NB. ! Do not use 'this.OneWayBind ... ' for the top level binding to the tree view
                //this.OneWayBind(ViewModel, vm => vm.Family, v => v.FamilyTree.ItemsSource);
                FamilyTree.ItemsSource = ViewModel.Family;
                //Add some commands to prove dynamic capability
                this.Bind(ViewModel, vm => vm.NewName, v => v.NewName.Text).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.AddPerson, v => v.AddPerson).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.PetName, v => v.PetName.Text).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.AddPet, v => v.AddPet).DisposeWith(d);
                this.WhenAnyValue(x => x.FamilyTree.SelectedItem).BindTo(this, x => x.ViewModel.SelectedItem).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.Collapse, v => v.Collapse).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.Clear, v => v.Clear).DisposeWith(d);
            });
        }
    }
}

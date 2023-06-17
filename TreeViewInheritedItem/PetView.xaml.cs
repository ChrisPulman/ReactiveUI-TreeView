using System.Reactive.Disposables;
using ReactiveUI;

namespace TreeViewInheritedItem
{
    /// <summary>
    /// Interaction logic for PetView.xaml
    /// </summary>
    public partial class PetView
    {
        public PetView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.PetName.Text).DisposeWith(d);
            });
        }
    }
}

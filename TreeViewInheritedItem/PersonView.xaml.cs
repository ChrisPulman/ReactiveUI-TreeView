using System.Reactive.Disposables;
using ReactiveUI;

namespace TreeViewInheritedItem
{
    /// <summary>
    /// Interaction logic for PersonView.xaml
    /// </summary>
    public partial class PersonView
    {
        public PersonView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.PersonName.Text).DisposeWith(d);
            });
        }
    }
}

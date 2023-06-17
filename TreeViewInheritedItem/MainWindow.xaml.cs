﻿using System.Reactive.Disposables;
using System.Windows;
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
                this.BindCommand(ViewModel, vm => vm.AddPerson, v => v.AddPerson);
                this.Bind(ViewModel, vm => vm.PetName, v => v.PetName.Text).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.AddPet, v => v.AddPet);
                this.WhenAnyValue(x => x.FamilyTree.SelectedItem).BindTo(this, x => x.ViewModel.SelectedItem).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.Collapse, v => v.Collapse);
                this.BindCommand(ViewModel, vm => vm.Clear, v => v.Clear);
            });
        }
    }
}

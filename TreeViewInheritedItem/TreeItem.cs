using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace TreeViewInheritedItem
{

    public abstract class TreeItem : ReactiveObject, IDisposable
    {
        private readonly Type _viewModelType;
        private bool disposedValue;
        private TreeItem _parent;

        bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { this.RaiseAndSetIfChanged(ref _isExpanded, value); }
        }

        bool _isSelected;
        private IDisposable _disposable;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
        }

        protected TreeItem(IEnumerable<TreeItem> children = null)
        {
            _disposable = _childrenSrc.Connect().Bind(_children).Subscribe();
            if (children == null) return;
            foreach (var child in children)
            {
                AddChild(child);
            }
        }

        public abstract object ViewModel { get; }

        private readonly SourceList<TreeItem> _childrenSrc = new SourceList<TreeItem>();
        private readonly ObservableCollectionExtended<TreeItem> _children = new ObservableCollectionExtended<TreeItem>();
        public ObservableCollectionExtended<TreeItem> Children => this._children;

        public void AddChild(TreeItem child)
        {
            child._parent = this;
            _childrenSrc.Add(child);
        }

        public void ExpandPath()
        {
            IsExpanded = true;
            _parent?.ExpandPath();
        }
        public void CollapsePath()
        {
            IsExpanded = false;
            _parent?.CollapsePath();
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var child in Children)
                    {
                        child.Dispose();
                    }

                    _disposable.Dispose();
                    _childrenSrc.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}

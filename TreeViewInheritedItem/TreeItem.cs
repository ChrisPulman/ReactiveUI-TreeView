using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace TreeViewInheritedItem
{
    public abstract class TreeItem : ReactiveObject, IDisposable
    {
        private readonly SourceList<TreeItem> _childrenSrc = new SourceList<TreeItem>();
        private readonly ObservableCollectionExtended<TreeItem> _children = new ObservableCollectionExtended<TreeItem>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool disposedValue;
        private TreeItem _parent;

        protected TreeItem(IEnumerable<TreeItem> children = null)
        {
            _childrenSrc.Connect().Bind(_children).Subscribe().DisposeWith(_disposables);
            _childrenSrc.ClearIfNotActivated(this.WhenAnyValue(x => x.IsExpanded)).DisposeWith(_disposables);
            if (children == null) return;
            foreach (var child in children)
            {
                AddChild(child);
            }
        }

        bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { this.RaiseAndSetIfChanged(ref _isExpanded, value); }
        }

        bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
        }

        public ObservableCollectionExtended<TreeItem> Children => this._children;

        public void AddChild(TreeItem child)
        {
            child._parent = this;
            _childrenSrc.Add(child);
        }

        public void AddChildRange(IEnumerable<TreeItem> children)
        {
            _childrenSrc.Edit(innerList =>
            {
                foreach (var item in children)
                {
                    item._parent = this;
                }

                innerList.AddRange(children);
            });
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// Ensures that the children are disposed.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _childrenSrc.ClearDispose();
                    foreach (var child in Children)
                    {
                        child.Dispose();
                    }

                    _disposables.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}

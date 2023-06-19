using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace TreeViewInheritedItem
{
    /// <summary>
    /// A base class for tree items.
    /// </summary>
    /// <seealso cref="ReactiveObject" />
    /// <seealso cref="ITreeItem" />
    public abstract class TreeItem : ReactiveObject, ITreeItem
    {
        private readonly SourceList<ITreeItem> _childrenSrc = new();
        private readonly ObservableCollectionExtended<ITreeItem> _children = new();
        private readonly CompositeDisposable _disposables = new();
        private ITreeItem _parent;
        bool _isExpanded;
        bool _isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeItem"/> class.
        /// </summary>
        /// <param name="children">The children.</param>
        protected TreeItem(IEnumerable<ITreeItem> children = null)
        {
            _childrenSrc.Connect().Bind(_children).Subscribe().DisposeWith(_disposables);
            _childrenSrc.ClearIfNotActivated(this.WhenAnyValue(x => x.IsExpanded)).DisposeWith(_disposables);
            if (children == null) return;
            AddRange(children);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { this.RaiseAndSetIfChanged(ref _isExpanded, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has children.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has children; otherwise, <c>false</c>.
        /// </value>
        public bool HasChildren => _children.Count > 0;

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public ObservableCollectionExtended<ITreeItem> Children => _children;

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public ITreeItem Parent
        {
            get { return _parent; }
            set { this.RaiseAndSetIfChanged(ref _parent, value); }
        }

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public bool IsDisposed => _disposables.IsDisposed;

        /// <summary>
        /// Adds the specified child.
        /// </summary>
        /// <param name="child">The child.</param>
        public void Add(ITreeItem child)
        {
            child.DisposeWith(_disposables);
            child.Parent = this;
            _childrenSrc.Add(child);
        }

        /// <summary>
        /// Adds the range of children.
        /// </summary>
        /// <param name="children">The children.</param>
        public void AddRange(IEnumerable<ITreeItem> children)
        {
            _childrenSrc.Edit(innerList =>
            {
                foreach (var child in children)
                {
                    child.DisposeWith(_disposables);
                    child.Parent = this;
                }

                innerList.AddRange(children);
            });
        }

        /// <summary>
        /// Expands the path until parent is null.
        /// </summary>
        public void ExpandPath()
        {
            IsExpanded = true;
            _parent?.ExpandPath();
        }

        /// <summary>
        /// Collapses the path until parent is null.
        /// </summary>
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

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                _childrenSrc.ClearDispose();
                _disposables.Dispose();
            }
        }
    }
}

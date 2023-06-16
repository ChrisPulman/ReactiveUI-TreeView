using System;
using System.Collections.Generic;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace TreeViewInheritedItem
{

    public abstract class TreeItem : ReactiveObject
    {
        private readonly Type _viewModelType;

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

        private TreeItem _parent;

        protected TreeItem(IEnumerable<TreeItem> children = null)
        {
            _childrenSrc.Connect().Bind(_children).Subscribe();
            if (children == null) return;
            foreach (var child in children)
            {
                AddChild(child);
            }
        }

        public abstract object ViewModel { get; }

        private SourceList<TreeItem> _childrenSrc = new SourceList<TreeItem>();
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
    }
}

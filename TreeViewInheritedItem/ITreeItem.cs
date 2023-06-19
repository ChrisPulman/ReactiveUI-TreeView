using DynamicData.Binding;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace TreeViewInheritedItem
{
    public interface ITreeItem : ICancelable
    {
        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        ObservableCollectionExtended<ITreeItem> Children { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has children.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has children; otherwise, <c>false</c>.
        /// </value>
        bool HasChildren { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        ITreeItem Parent { get; set; }

        /// <summary>
        /// Adds the specified child.
        /// </summary>
        /// <param name="child">The child.</param>
        void Add(ITreeItem child);

        /// <summary>
        /// Adds the range of children.
        /// </summary>
        /// <param name="children">The children.</param>
        void AddRange(IEnumerable<ITreeItem> children);

        /// <summary>
        /// Collapses the path until parent is null.
        /// </summary>
        void CollapsePath();

        /// <summary>
        /// Expands the path until parent is null.
        /// </summary>
        void ExpandPath();
    }
}
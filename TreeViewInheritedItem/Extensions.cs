using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TreeViewInheritedItem
{
    internal static class Extensions
    {
        /// <summary>
        /// Call when large numbers of items are in a Treeview.
        /// This is needed to ensure that the treeview only loads items when they are in view (i.e. virtualization).
        /// If a TreeView contains many items, the amount of time it takes to load may cause a significant delay in the user interface.
        /// You can improve the load time by setting the VirtualizingStackPanel.IsVirtualizing attached property to true.
        /// The UI might also be slow to react when a user scrolls the TreeView by using the mouse wheel or dragging the thumb of a scrollbar.
        /// You can improve the performance of the TreeView when the user scrolls by setting the VirtualizingStackPanel.VirtualizationMode attached property to VirtualizationMode.Recycling.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        public static void OnlyLoadItemsInView(this TreeView treeView)
        {
            treeView.SetValue(VirtualizingStackPanel.VirtualizationModeProperty, VirtualizationMode.Recycling);
            treeView.SetValue(VirtualizingStackPanel.IsVirtualizingProperty, true);
        }

        /// <summary>
        /// Clears collapsed Views from a TreeView when using ReactiveUI to save memory.
        /// NOTE: When using this method, you must also use the 'OnlyLoadItemsInView' method.
        ///       When the treeview is expanded, the items will be reloaded with a new XAML view.
        ///       Do not rely on values on the view to be retained, they will be lost, use the viewmodel to store any state.
        /// </summary>
        /// <param name="sourceList">The source list.</param>
        /// <param name="activated">The activated observable.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <returns></returns>
        public static IDisposable ClearIfNotActivated(this ISourceList<ITreeItem> sourceList, IObservable<bool> activated, IScheduler scheduler = null)
        {
            List<ITreeItem> listOfItems = null;
            return activated.DistinctUntilChanged().ObserveOn(scheduler ?? RxApp.MainThreadScheduler).Subscribe(x =>
            {
                if (sourceList.Count == 0)
                {
                    return;
                }

                sourceList.Edit(innerList =>
                {
                    if (x)
                    {
                        if (listOfItems != null)
                        {
                            // Dispose of the dummy item
                            foreach (var item in innerList)
                            {
                                ClearAndDisposeChildren(item);
                            }

                            innerList.Clear();

                            // Reload the list of items that were cleared when the treeview was collapsed
                            innerList.AddRange(listOfItems);
                            listOfItems = null;
                        }
                    }
                    else
                    {
                        // Store the list of items to be cleared so that they can be reloaded when the treeview is expanded
                        listOfItems = new List<ITreeItem>(innerList);

                        // Only Clear the list, do not dispose of the items
                        innerList.Clear();

                        // Add a dummy item to the list to ensure that the treeview is not empty
                        innerList.Add(new Person(""));

                        // Only for testing purposes to demonstrate that the memory is released
                        GC_Collect();
                    }
                });
            });
        }

        /// <summary>
        /// This method is used to clear and dispose of all items in a SourceList.
        /// </summary>
        /// <param name="sourceList">The source list.</param>
        public static void ClearDispose(this ISourceList<ITreeItem> sourceList)
        {
            sourceList.Edit(innerList =>
            {
                foreach (var item in innerList)
                {
                    ClearAndDisposeChildren(item);
                }

                innerList.Clear();
            });

            // Only for testing purposes to demonstrate that the memory is released
            GC_Collect();
        }

        private static void ClearAndDisposeChildren(ITreeItem item)
        {
            if (item.HasChildren)
            {
                foreach (var child in item.Children)
                {
                    ClearAndDisposeChildren(child);
                    child.Dispose();
                }

                item.Children.Clear();
            }

            item.Dispose();
        }

        public static void GC_Collect(int delay = 1000)
        {
            Task.Run(() =>
            {
                // GC.Collect() is blocking, so we need to run it on a separate thread
                // GC.Collect() is not guaranteed to collect all garbage, we are calling it with no parameters to collect all generations
                // GC.Collect() has to be called several times to collect all garbage, there is a factor of around 10x between each call
                // before it collects all garbage, so we call it once with no parameters to collect all generations, then we call it again for Generation two.
                // Generation two is the oldest generation and is the most likely to contain large garbage that is not collected.
                // We then wait for all finalizers to complete before continuing.
                GC.Collect();
                Task.Delay(delay).Wait();
                GC.Collect(2);
                GC.WaitForPendingFinalizers();
            });
        }
    }
}

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
        public static IDisposable ClearIfNotActivated(this SourceList<TreeItem> sourceList, IObservable<bool> activated, IScheduler scheduler = null)
        {
            List<TreeItem> listOfItems = null;
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
                            innerList.Clear();
                            innerList.AddRange(listOfItems);
                            listOfItems = null;
                        }
                    }
                    else
                    {
                        listOfItems = new List<TreeItem>(innerList);
                        innerList.Clear();
                        innerList.Add(new Person(""));

                        // Only for testing purposes to demonstrate that the memory is released
                        GC_Collect();
                    }
                });
            });
        }

        public static void ClearDispose(this SourceList<TreeItem> sourceList)
        {
            sourceList.Edit(innerList =>
            {
                foreach (var item in innerList)
                {
                    ClearAndDisposeChildren(item);
                }

                innerList.Clear();

                void ClearAndDisposeChildren(TreeItem item)
                {
                    if (item.Children.Any())
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
            });

            // Only for testing purposes to demonstrate that the memory is released
            GC_Collect();
        }

        public static void GC_Collect(int delay = 1000)
        {
            Task.Run(() =>
            {
                GC.Collect();
                Task.Delay(delay).Wait();
                GC.Collect(2);
                GC.WaitForPendingFinalizers();
                Task.Delay(delay).Wait();
            });
        }
    }
}

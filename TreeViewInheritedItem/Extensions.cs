using DynamicData;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TreeViewInheritedItem
{
    internal static class Extensions
    {
        public static void ClearDispose(this SourceList<TreeItem> sourceList)
        {
            sourceList.Edit(innerList =>
            {
                foreach (var item in innerList)
                {
                    ClearAndDisposeChildren(item);
                }

                innerList.Clear();
                GC.Collect();

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

            GC.Collect();
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                GC.Collect();
            });
        }
    }
}

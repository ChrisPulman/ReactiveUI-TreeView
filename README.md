# ReactiveUI-TreeView
Oliver Vierkens took the liberty of using a fork of condron's simple ReactiveUI TreeView "showcase", to show some memory and performance issues he's been running into using WPF TreeView and ReactiveUI. These kinds of problems with the TreeView have not been apparent to me before switching to RxUI. To replicate the problems, try following these steps:

1. The "Joe" node has a 1000 child nodes. Try expanding it and look at memory usage. On my machine there is a ~120mb increase, and it takes ~5 seconds to perform the expansion.
2. Try collapsing the node, and look at memory use. On my machine, there is no memory freed up. I would expect (or at least aim for) that the memory use would get back to what it was when I started the app.
3. Try pressing the "Clear" button. This clears the "Family" SourceList, and puts a single new node in it. Again this does not do anything to release memory.

Oliver was out of ideas how to mitigate these issues. He was thinking of using WhenActivated, but not sure how deactivation would get triggered with the TreeNodes. Also I think it would be a bummer to have to write a lot of custom deactivation logic for this. Or is there something that I and condron have been doing wrong?

# The updated code from Chris Pulman

I migrated to PackageReference and a SDK Project to simplify the project.

Added Targets for net7.0-windows10.0.17763 and .NET Framework 4.8 to allow the project to be demonstrated in both .Net Core and .Net Framework.

I've update the TreeItem class to be IDisposable and to dispose of the children when it is disposed. 

I've added an Extension Method for SourceList to Dispose of TreeItem items, this allows the Clear button to dispose of the items and free memory.

I've added a WhenActivated to the Views to dispose of the bindings when the View is deactivated.

I've added a Extension Method for SourceList to clear items when the treeview is collapsed.

I've added an Extension Method for TreeView to speed up the expansion of the treeview and loading of the items.


ORIGINAL README:
Sample project for a WPF TreeView using ReactiveUI 6.5

Uses User Controls for item templates.

Added dynamic insertion of any type inheriting from TreeItem

Added IsSelected and IsExpanded Support

Added Expand path support

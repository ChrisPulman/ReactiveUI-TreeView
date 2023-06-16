# ReactiveUI-TreeView
I took the liberty of using a fork of this simple ReactiveUI TreeView "showcase", to show some memory and performance issues I've been running into using WPF TreeView and ReactiveUI. These kinds of problems with the TreeView have not been apparent to me before switching to RxUI. To replicate the problems, try following these steps:

1. The "Joe" node has a 1000 child nodes. Try expanding it and look at memory usage. On my machine there is a ~120mb increase, and it takes ~5 seconds to perform the expansion.
2. Try collapsing the node, and look at memory use. On my machine, there is no memory freed up. I would expect (or at least aim for) that the memory use would get back to what it was when I started the app.
3. Try pressing the "Clear" button. This clears the "Family" SourceList, and puts a single new node in it. Again this does not do anything to release memory.

I'm out of ideas how to mitigate these issues. I was thinking of using WhenActivated, but not sure how deactivation would get triggered with the TreeNodes. Also I think it would be a bummer to have to write a lot of custom deactivation logic for this. Or is there something that I and condron have been doing wrong?

ORIGINAL README:
Sample project for a WPF TreeView using ReactiveUI

Uses User Controls for item templates.

Added dynamic insertion of any type inheriting from TreeItem

Added IsSelected and IsExpanded Support

Added Expand path support

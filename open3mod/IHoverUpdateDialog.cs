using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace open3mod
{
    // Class of "Details" dialogs used in sidebar: if dialog is open for item A and the user
    // hovers over item B in the sidebar that also supports the dialog, we auto-update the
    // dialog contents to refer to item B.
    interface IHoverUpdateDialog
    {
        // Indicates if "hover-update" is currently for this dialog.
        //
        // For non-surprising UX, dialogs usually disable the feature if they display
        // sub-dialogs which normally do not hover-update.
        bool HoverUpdateEnabled { get; }
    }
}

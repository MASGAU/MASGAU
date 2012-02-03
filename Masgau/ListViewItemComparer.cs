using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Masgau
{
        // Ruthlessly snatched from the ListView.Sort page on MSDN
        class ListViewItemComparer : System.Collections.IComparer
        {
            private int col;
            public ListViewItemComparer()
            {
                col = 0;
            }
            public ListViewItemComparer(int column)
            {
                col = column;
            }
            public int Compare(object x, object y)
            {
                return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
            }
        }
}

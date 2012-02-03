using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MASGAU
{
        // Ruthlessly snatched from the ListView.Sort page on MSDN
        class ListViewItemComparer : System.Collections.IComparer
        {
            //private int col;
            //public ListViewItemComparer()
            //{
            //    col = 0;
            //}
            //public ListViewItemComparer(int column)
            //{
            //    col = column;
            //}
            //public int Compare(object x, object y)
            //{
            //    return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
            //}
                private int col;
                private SortOrder order;
                public ListViewItemComparer() {
                    col=0;
                    order = SortOrder.Ascending;
                }
                public ListViewItemComparer(int column, SortOrder order) 
                {
                    col=column;
                    this.order = order;
                }
                public int Compare(object x, object y) 
                {
                    int returnVal= -1;
                    returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                            ((ListViewItem)y).SubItems[col].Text);
                    // Determine whether the sort order is descending.
                    if(order == SortOrder.Descending)
                        // Invert the value returned by String.Compare.
                        returnVal *= -1;
                    return returnVal;
                }
        }
}

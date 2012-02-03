using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using wyDay.Controls;

namespace MASGAU
{
    class invokes
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);

        public invokes() {}

        private delegate void showProgressTaskBarDelegate(Windows7ProgressBar bar, bool show);
        public void showProgressTaskBar(Windows7ProgressBar bar, bool show) {
            if(bar.InvokeRequired) {
                bar.BeginInvoke(new showProgressTaskBarDelegate(showProgressTaskBar),new Object[] {bar,show});
            } else {
                lock(bar) {
                    bar.ShowInTaskbar = show;
                    bar.Update();
                }
            }
        }


        private delegate void setProgressBarMaxDelegate(Windows7ProgressBar bar, int new_max);
        public void setProgressBarMax(Windows7ProgressBar bar, int new_max) {
            if(bar.InvokeRequired) {
                bar.BeginInvoke(new setProgressBarMaxDelegate(setProgressBarMax),new Object[] {bar,new_max});
            } else {
                lock(bar) {
                    bar.Maximum = new_max;
                    bar.Update();
                }
            }
        }

        private delegate void setProgressBarStyleDelegate(Windows7ProgressBar bar, ProgressBarStyle new_style);
        public void setProgressBarStyle(Windows7ProgressBar bar, ProgressBarStyle new_style) {
            if(bar.InvokeRequired) {
                bar.Invoke(new setProgressBarStyleDelegate(setProgressBarStyle),new Object[] {bar,new_style});
            } else {
                lock(bar) {
                    bar.Style = new_style;
                    bar.Update();
                }
            }
        }

        private delegate void setProgressBarStateDelegate(Windows7ProgressBar bar, ProgressBarState new_state);
        public void setProgressBarState(Windows7ProgressBar bar, ProgressBarState new_state) {
            if(bar.InvokeRequired) {
                bar.Invoke(new setProgressBarStateDelegate(setProgressBarState),new Object[] {bar,new_state});
            } else {
                lock(bar) {
                    bar.State = new_state;
                    bar.Update();
                }
            }
        }

        private delegate void setProgressBarMinDelegate(Windows7ProgressBar bar, int new_min);
        public void setProgressBarMin(Windows7ProgressBar bar, int new_min) {
            if(bar.InvokeRequired) {
                bar.BeginInvoke(new setProgressBarMinDelegate(setProgressBarMin),new Object[] {bar,new_min});
            } else {
                lock(bar) {
                    bar.Minimum = new_min;
                    bar.Update();
                }
            }
        }


        private delegate void performStepDelegate(Windows7ProgressBar bar);
        public void performStep(Windows7ProgressBar bar) {
            if(bar.InvokeRequired) {
                bar.BeginInvoke(new performStepDelegate(performStep), new Object[] { bar });
            } else {
                lock(bar) {
                    bar.Increment(1);
                    bar.Update();
                }
            }
        }

        private delegate void setProgressBarValueDelegate(Windows7ProgressBar bar, int new_value);
        public void setProgressBarValue(Windows7ProgressBar bar, int new_value) {
            if(bar.InvokeRequired) {
                bar.BeginInvoke(new setProgressBarValueDelegate(setProgressBarValue),new Object[] {bar,new_value});
            } else {
                lock(bar) {
                    if(new_value>bar.Maximum)
                        bar.Value = bar.Maximum;
                    else if(new_value<bar.Minimum)
                        bar.Value = bar.Minimum;
                    else
                        bar.Value = new_value;
                    bar.Update();
                }
            }
        }

        private delegate void addTreeNodeDelegate(TreeView tree, TreeNode add_me);
        public void addTreeNode(TreeView tree, TreeNode add_me) {
            if(tree.InvokeRequired) {
                tree.Invoke(new addTreeNodeDelegate(addTreeNode),new Object[] {tree,add_me});
            } else {
                lock(tree) {
                    tree.Nodes.Add(add_me);
                }
            }
        }

        private delegate void addTreeItemDelegate(TreeView tree, string add_me, string named_me);
        public void addTreeItem(TreeView tree, string add_me, string named_me) {
            if(tree.InvokeRequired) {
                tree.Invoke(new addTreeItemDelegate(addTreeItem),new Object[] {tree,add_me,named_me});
            } else {
                lock(tree) {
                    tree.Nodes.Add(add_me, named_me);
                }
            }
        }

        private delegate void addTreeSubItemDelegate(TreeView tree, string add_here, string add_me, string named_me);
        public void addTreeSubItem(TreeView tree, string add_here, string add_me, string named_me) {
            if(tree.InvokeRequired) {
                tree.Invoke(new addTreeSubItemDelegate(addTreeSubItem),new Object[] {tree,add_here,add_me,named_me});
            } else {
                lock(tree) {
                    tree.Nodes[add_here].Nodes.Add(add_me, named_me);
                }
            }
        }

        private delegate void addListViewItemDelegate(ListView list, ListViewItem add_me);
        public void addListViewItem(ListView list, ListViewItem add_me) {
            if(list.InvokeRequired) {
                list.Invoke(new addListViewItemDelegate(addListViewItem),new Object[] {list,add_me});
            } else {
                lock(list) {
                    list.Items.Add(add_me);
                }
            }
        }

        private delegate void addListViewGroupItemDelegate(ListView list, string group_key, ListViewItem add_me);
        public void addListViewGroupItem(ListView list, string group_key, ListViewItem add_me) {
            if(list.InvokeRequired) {
                list.Invoke(new addListViewGroupItemDelegate(addListViewGroupItem),new Object[] {list,group_key,add_me});
            } else {
                lock(list) {
                    list.Groups[group_key].Items.Add(add_me);
                }
            }
        }

        private delegate void addListViewGroupDelegate(ListView list, ListViewGroup add_me);
        public void addListViewGroup(ListView list, ListViewGroup add_me) {
            if(list.InvokeRequired) {
                list.Invoke(new addListViewGroupDelegate(addListViewGroup),new Object[] {list,add_me});
            } else {
                lock(list) {
                    list.Groups.Add(add_me);
                }
            }
        }

        private delegate int getTabDelegate(TabControl from_me);
        public int getTab(TabControl from_me) {
            if(from_me.InvokeRequired) {
                return (int)from_me.Invoke(new getTabDelegate(getTab),new Object[] {from_me});
            } else {
                lock(from_me) {
                    return from_me.SelectedIndex;
                }
            }
        }

        private delegate void clearListViewDelegate(ListView list);
        public void clearListView(ListView list) {
            if(list.InvokeRequired) {
                list.Invoke(new clearListViewDelegate(clearListView),new Object[] {list});
            } else {
                lock(list.Items) {
                    list.Items.Clear();
                }
            }
        }

        private delegate void clearTreeViewDelegate(TreeView tree);
        public void clearTreeView(TreeView tree) {
            if(tree.InvokeRequired) {
                tree.Invoke(new clearTreeViewDelegate(clearTreeView), new Object[] { tree });
            } else {
                lock(tree.Nodes) {
                    tree.Nodes.Clear();
                }
            }
        }
       
        private delegate void clearToolStripItemsDelegate(Form in_me, ToolStripMenuItem clear_me);
        public void clearToolStripItems(Form in_me, ToolStripMenuItem clear_me) {
            if(in_me.InvokeRequired) {
                in_me.Invoke(new clearToolStripItemsDelegate(clearToolStripItems),new Object[] {in_me, clear_me});
            } else {
                lock(clear_me) {
                    clear_me.DropDownItems.Clear();
                }
            }
        }
       
        private delegate void addToolStripItemDelegate(Form in_me, ToolStripMenuItem to_me, ToolStripMenuItem add_me);
        public void addToolStripItem(Form in_me, ToolStripMenuItem to_me, ToolStripMenuItem add_me) {
            if(in_me.InvokeRequired) {
                in_me.Invoke(new addToolStripItemDelegate(addToolStripItem),new Object[] {in_me, to_me, add_me});
            } else {
                lock(to_me) {
                    to_me.DropDownItems.Add(add_me);
                }
            }
        }

        private delegate void setControlEnabledDelegate(Control set_me, bool to_me);
        public void setControlEnabled(Control set_me, bool to_me) {
            if(set_me.InvokeRequired) {
                set_me.BeginInvoke(new setControlEnabledDelegate(setControlEnabled), new Object[] {set_me,to_me});
            } else {
                lock(set_me) {
                    set_me.Enabled = to_me;
                }
            }
        }

        private delegate void setControlVisibleDelegate(object set_me, bool to_me);
        public void setControlVisible(object set_me, bool to_me) {
            if(((Control)set_me).InvokeRequired) {
                ((Control)set_me).BeginInvoke(new setControlVisibleDelegate(setControlVisible), new Object[] {set_me,to_me});
            } else {
                lock(set_me) {
                    ((Control)set_me).Visible = to_me;
                }
            }
        }

        private delegate void setControlTextDelegate(object set_me, string to_me);
        public void setControlText(object set_me, string to_me) {
            if(((Control)set_me).InvokeRequired) {
                ((Control)set_me).BeginInvoke(new setControlTextDelegate(setControlText), new Object[] {set_me,to_me});
            } else {
                lock(set_me) {
                    ((Control)set_me).Text = to_me;
                }
            }
        }

        private delegate void setFormShowInTaskbarDelegate(Form set_me, bool to_me);
        public void setFormShowInTaskbar(Form set_me, bool to_me) {
            if(set_me.InvokeRequired) {
                set_me.BeginInvoke(new setFormShowInTaskbarDelegate(setFormShowInTaskbar), new Object[] {set_me,to_me});
            } else {
                lock(set_me) {
                    set_me.ShowInTaskbar = to_me;
                }
            }
        }

        private delegate void setListViewScrollableDelegate(ListView set_me, bool to_me);
        public void setListViewScrollable(ListView set_me, bool to_me) {
            if((set_me).InvokeRequired) {
                (set_me).BeginInvoke(new setListViewScrollableDelegate(setListViewScrollable), new Object[] {set_me,to_me});
            } else {
                lock(set_me) {
                    (set_me).Scrollable = to_me;
                }
            }
        }

        private delegate void setListViewTileSizeDelegate(ListView set_me, System.Drawing.Size to_me);
        public void setListViewTileSize(ListView set_me, System.Drawing.Size to_me) {
            if((set_me).InvokeRequired) {
                (set_me).BeginInvoke(new setListViewTileSizeDelegate(setListViewTileSize), new Object[] {set_me,to_me});
            } else {
                lock(set_me) {
                    (set_me).TileSize = to_me;
                }
            }
        }

        private delegate void sortListViewDelegate(ListView sort_me, int by_me, SortOrder this_way);
        public void sortListView(ListView sort_me, int by_me, SortOrder this_way) {
           if(sort_me.InvokeRequired) {
                sort_me.Invoke(new sortListViewDelegate(sortListView), new Object[] {sort_me,by_me,this_way});
            } else {
                lock(sort_me) {
                    sort_me.ListViewItemSorter = new ListViewItemComparer(by_me,this_way);
                }
            }
        }

        private delegate void setListViewItemBackColorDelegate(ListView in_me, int set_me, System.Drawing.Color to_me);
        public void setListViewItemBackColor(ListView in_me, int set_me, System.Drawing.Color to_me) {
            if(in_me.InvokeRequired) {
                in_me.BeginInvoke(new setListViewItemBackColorDelegate(setListViewItemBackColor), new Object[] {in_me,set_me,to_me});
            } else {
                lock(in_me) {
                    in_me.Items[set_me].BackColor = to_me;
                }
            }
        }

        private delegate void setListViewItemSubItemTextDelegate(ListView in_me, int and_me, int set_me, string to_me);
        public void setListViewItemSubItemText(ListView in_me, int and_me, int set_me, string to_me) {
            if(in_me.InvokeRequired) {
                in_me.BeginInvoke(new setListViewItemSubItemTextDelegate(setListViewItemSubItemText), new Object[] {in_me,and_me,set_me,to_me});
            } else {
                lock(in_me) {
                    in_me.Items[and_me].SubItems[set_me].Text = to_me;
                }
            }
        }

        private delegate void setNotifyIconVisibleDelegate(Form in_me, NotifyIcon set_me, bool to_me);
        public void setNotifyIconVisible(Form in_me, NotifyIcon set_me, bool to_me) {
            if(in_me.InvokeRequired) {
                in_me.BeginInvoke(new setNotifyIconVisibleDelegate(setNotifyIconVisible), new Object[] {in_me,set_me,to_me});
            } else {
                lock(set_me) {
                    set_me.Visible = to_me;
                }
            }
        }

        private delegate void setNotifyIconTextDelegate(Form in_me, NotifyIcon set_me, string to_me);
        public void setNotifyIconText(Form in_me, NotifyIcon set_me, string to_me) {
            if(in_me.InvokeRequired) {
                in_me.BeginInvoke(new setNotifyIconTextDelegate(setNotifyIconText), new Object[] {in_me,set_me,to_me});
            } else {
                lock(set_me) {
                    set_me.Text = to_me;
                }
            }
        }

        private delegate void setToolStripMenuItemCheckStateDelegate(Form in_me, ToolStripMenuItem set_me, CheckState to_me);
        public void setToolStripMenuItemCheckState(Form in_me, ToolStripMenuItem set_me, CheckState to_me) {
            if(in_me.InvokeRequired) {
                in_me.BeginInvoke(new setToolStripMenuItemCheckStateDelegate(setToolStripMenuItemCheckState), new Object[] {in_me,set_me,to_me});
            } else {
                lock(set_me) {
                    set_me.CheckState = to_me;
                }
            }
        }

        private delegate void setToolStripMenuItemEnabledDelegate(Form in_me, ToolStripMenuItem set_me, bool to_me);
        public void setToolStripMenuItemEnabled(Form in_me, ToolStripMenuItem set_me, bool to_me) {
            if(in_me.InvokeRequired) {
                in_me.BeginInvoke(new setToolStripMenuItemEnabledDelegate(setToolStripMenuItemEnabled), new Object[] {in_me,set_me,to_me});
            } else {
                lock(set_me) {
                    set_me.Enabled = to_me;
                }
            }
        }

        private delegate void setToolStripMenuItemToolTipTextDelegate(Form in_me, ToolStripMenuItem set_me, string to_me);
        public void setToolStripMenuItemToolTipText(Form in_me, ToolStripMenuItem set_me, string to_me) {
            if(in_me.InvokeRequired) {
                in_me.BeginInvoke(new setToolStripMenuItemToolTipTextDelegate(setToolStripMenuItemToolTipText), new Object[] {in_me,set_me,to_me});
            } else {
                lock(set_me) {
                    set_me.ToolTipText = to_me;
                }
            }
        }

        private delegate void removeTabDelegate(TabControl from_me, TabPage remove_me);
        public void removeTab(TabControl from_me, TabPage remove_me) {
            if(from_me.InvokeRequired) {
                from_me.BeginInvoke(new removeTabDelegate(removeTab), new Object[] {from_me,remove_me});
            } else {
                lock(from_me) {
                    from_me.TabPages.Remove(remove_me);
                }
            }
        }

        private delegate DialogResult showMessageBoxDelegate(Form use_me, String title, String message, MessageBoxButtons buttons, MessageBoxIcon icon);
        public DialogResult showMessageBox(Form use_me, String title, String message, MessageBoxButtons buttons, MessageBoxIcon icon) {
            if(Environment.UserInteractive) {
                if(use_me.InvokeRequired) {
                    return (DialogResult)use_me.Invoke(new showMessageBoxDelegate(showMessageBox), new Object[] {use_me,title,message,buttons,icon});
                } else {
                    return MessageBox.Show(use_me,message,title,buttons,icon);
                }
            } else {
                lock(use_me) {
                    return MessageBox.Show(message,title,buttons,icon,MessageBoxDefaultButton.Button1,MessageBoxOptions.ServiceNotification);
                }
            }
        }

        private delegate DialogResult showConfirmDialogDelegate(Form use_me, String title, String message);
        public DialogResult showConfirmDialog(Form use_me, String title, String message) {
            if(Environment.UserInteractive) {
                if(use_me.InvokeRequired) {
                    return (DialogResult)use_me.Invoke(new showConfirmDialogDelegate(showConfirmDialog), new Object[] {use_me,title,message});
                } else {
                    return MessageBox.Show(use_me,message,title,MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2);
                }
            } else {
                lock(use_me) {
                    return MessageBox.Show(message,title, MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2,MessageBoxOptions.ServiceNotification);
                }
            }
        }

        private delegate DialogResult showOKDialogDelegate(Form use_me, String title, String message);
        public DialogResult showOKDialog(Form use_me, String title, String message) {
            if(Environment.UserInteractive) {
                if(use_me.InvokeRequired) {
                    return (DialogResult)use_me.Invoke(new showOKDialogDelegate(showOKDialog), new Object[] {use_me,title,message});
                } else {
                    return MessageBox.Show(use_me,message,title,MessageBoxButtons.OK,MessageBoxIcon.Information,MessageBoxDefaultButton.Button1);
                }
            } else {
                lock(use_me) {
                    return MessageBox.Show(message,title, MessageBoxButtons.OK,MessageBoxIcon.Information,MessageBoxDefaultButton.Button1,MessageBoxOptions.ServiceNotification);
                }
            }
        }

        private delegate DialogResult showUserSelectorDelegate(Form daddy, userSelector baby);
        public DialogResult showUserSelector(Form daddy, userSelector baby) {
            if(daddy.InvokeRequired) {
                return (DialogResult)daddy.Invoke(new showUserSelectorDelegate(showUserSelector), new Object[] { daddy, baby });
            } else {
                lock(daddy) {
                    lock(baby) {
                        return baby.ShowDialog(daddy);
                    }
                }
            }
        }

        private delegate DialogResult showDriveSelectorDelegate(Form daddy, driveSelector baby);
        public DialogResult showDriveSelector(Form daddy, driveSelector baby) {
            if(daddy.InvokeRequired) {
                return (DialogResult)daddy.Invoke(new showDriveSelectorDelegate(showDriveSelector), new Object[] { daddy, baby });
            } else {
                lock(daddy) {
                    lock(baby) {
                        return baby.ShowDialog(daddy);
                    }
                }
            }
        }

        private delegate DialogResult showRootSelectorDelegate(Form daddy, rootSelector baby);
        public DialogResult showRootSelector(Form daddy, rootSelector baby) {
            if(daddy.InvokeRequired) {
                return (DialogResult)daddy.Invoke(new showRootSelectorDelegate(showRootSelector), new Object[] { daddy, baby });
            } else {
                lock(daddy) {
                    lock(baby) {
                        return baby.ShowDialog(daddy);
                    }
                }
            }
        }

        private delegate DialogResult showDialogDelegate(Form daddy, Form baby);
        public DialogResult showDialog(Form daddy, Form baby) {
            if(daddy.InvokeRequired) {
                return (DialogResult)daddy.Invoke(new showDialogDelegate(showDialog), new Object[] { daddy, baby });
            } else {
                lock(daddy) {
                    lock(baby) {
                        return baby.ShowDialog(daddy);
                    }
                }
            }
        }

        private delegate DialogResult showFolderBrowserDialogDelegate(Form daddy, FolderBrowserDialog baby);
        public DialogResult showFolderBrowserDialog(Form daddy, FolderBrowserDialog baby) {
            if(daddy.InvokeRequired) {
                return (DialogResult)daddy.Invoke(new showFolderBrowserDialogDelegate(showFolderBrowserDialog), new Object[] { daddy, baby });
            } else {
                lock(daddy) {
                    lock(baby) {
                        return baby.ShowDialog(daddy);
                    }
                }
            }
        }
        
        private delegate void showDelegate(Form daddy, Form baby);
        public void show(Form daddy, Form baby) {
            if(daddy.InvokeRequired) {
                daddy.Invoke(new showDelegate(show), new Object[] { daddy, baby });
            } else {
                lock(daddy) {
                    lock(baby) {
                        baby.Show(daddy);
                    }
                }
            }
        }
     
        private delegate void closeFormDelegate(Form close_me);
        public void closeForm(Form close_me) {
            if(close_me.InvokeRequired) {
                close_me.BeginInvoke(new closeFormDelegate(closeForm), new Object[] {close_me});
            } else {
                lock(close_me) {
                    close_me.Close();
                }
            }
        }

        private delegate void setToolStripImageDelegate(StatusStrip in_me, ToolStripStatusLabel set_me, System.Drawing.Image to_me);
        public void setToolStripImage(StatusStrip in_me, ToolStripStatusLabel set_me, System.Drawing.Image to_me) {
            if(in_me.InvokeRequired) {
                in_me.BeginInvoke(new setToolStripImageDelegate(setToolStripImage), new Object[] { in_me, set_me, to_me});
            } else {
                lock(in_me) {
                    set_me.Image = to_me;
                }
            }
        }

        private delegate void setToolStripTextDelegate(StatusStrip in_me, ToolStripStatusLabel set_me, String to_me);
        public void setToolStripText(StatusStrip in_me, ToolStripStatusLabel set_me, String to_me) {
            if(in_me.InvokeRequired) {
                in_me.BeginInvoke(new setToolStripTextDelegate(setToolStripText), new Object[] { in_me, set_me, to_me});
            } else {
                lock(in_me) {
                    set_me.Text = to_me;
                }
            }
        }

        private delegate void setControlThemeDelegate(Control set_me, string to_me);
        public void setControlTheme(Control set_me, string to_me) {
            if(set_me.InvokeRequired) {
                set_me.Invoke(new setControlThemeDelegate(setControlTheme), new Object[] { set_me, to_me});
            } else {
                lock(set_me) {
                    SetWindowTheme(set_me.Handle,to_me,null);
                }
            }
        }


    
    }
}

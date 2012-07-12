using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Windows.Controls.Ribbon;
namespace MASGAU.Main {
    public partial class MainWindowNew {
        private RibbonGroup getRibbonGroup(String name) {
            foreach (Control control in ribbon.Items) {
                if (control is RibbonTab) {
                    RibbonTab tab = control as RibbonTab;
                    foreach (Control c2 in tab.Items) {
                        if (c2 is RibbonGroup) {
                            RibbonGroup group = c2 as RibbonGroup;
                            if (group.Name == name)
                                return group;
                        }
                    }
                }
            }


            throw new KeyNotFoundException("Could not find ribbon group called " + name);
        }

        private void ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!disabled) {

                if (!this.IsLoaded || e.AddedItems.Count == 0)
                    return;

                if (e.AddedItems[0] == AnalyzerTab) {
                    GameGrid.Visibility = System.Windows.Visibility.Collapsed;
                    AnalyzerReportGrid.Visibility = System.Windows.Visibility.Visible;
                } else {
                    GameGrid.Visibility = System.Windows.Visibility.Visible;
                    AnalyzerReportGrid.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

        }


    }
}

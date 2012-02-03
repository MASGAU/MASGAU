using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Masgau
{
    public partial class driveSelector : Form
    {
        public driveSelector()
        {
            InitializeComponent();
            foreach(DriveInfo look_here in DriveInfo.GetDrives()) {
                if(look_here.IsReady&&look_here.DriveType==DriveType.Removable&&(look_here.DriveFormat=="FAT32"||look_here.DriveFormat=="FAT16")) {
                    driveCombo.Items.Add(look_here.Name + " [" + look_here.VolumeLabel + "]");
                }
            } 
            if(driveCombo.Items.Count>0)
                driveCombo.SelectedIndex = 0;

        }

        public void setDrive(string drive) {
            for(int i = 0;i<driveCombo.Items.Count;i++){
                if(((string)driveCombo.Items[i]).StartsWith(drive))
                    driveCombo.SelectedIndex = i;
            }
        }


    }
}
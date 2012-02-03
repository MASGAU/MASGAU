using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Masgau
{
    public partial class userSelector : Form
    {
        public userSelector()
        {
            InitializeComponent();
        }

        public void setCombo(string to_me) {
            userSelectorCombo.SelectedIndex = 0;
            for(int i=0;i<userSelectorCombo.Items.Count;i++) {
                if(userSelectorCombo.Items[i].ToString()==to_me)
                    userSelectorCombo.SelectedIndex = i;
            }
        }


        private void userSelectorCombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
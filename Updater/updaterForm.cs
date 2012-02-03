using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;

namespace MASGAU
{
    public partial class updaterForm : Form
    {

        private updateHandler updates;
//        Color up_to_date = Color.FromArgb(138,226,52);
        Color up_to_date = Color.FromArgb(115,210,22);
        Color out_of_date = Color.FromArgb(239,41,41);
        Color updating = Color.FromArgb(252,233,79);

        public updaterForm()
        {
            InitializeComponent();
        }
        private void updateManager_Shown(object sender, EventArgs e)
        {
            SecurityHandler red_shirt = new SecurityHandler();
            if(!red_shirt.amAdmin()) {
                Application.Exit();
            }

            updates = new updateHandler();

            if(updates.checkVersions()) {
                if(updates.update_program) {
                    programUpdateForm program_update = new programUpdateForm(updates.latest_program_version.path);
                    //program_update.Show(this);
                    program_update.ShowDialog(this);
                    Application.Exit();
                } else {
                    ListViewItem add_me;
                    updateList.Items.Clear();

                    foreach(update_data update_me in updates.new_data.Values) {
                        add_me = new ListViewItem(update_me.name);
                        add_me.SubItems.Add(updates.existing_data[update_me.name].majorVersion.ToString() + "." + updates.existing_data[update_me.name].minorVersion.ToString() + "." + updates.existing_data[update_me.name].revisionVersion.ToString());
                        add_me.SubItems.Add(update_me.majorVersion.ToString() + "." + update_me.minorVersion.ToString() + "." + update_me.revisionVersion.ToString());
                        if(update_me.revisionVersion>updates.existing_data[update_me.name].revisionVersion) {
                            add_me.BackColor = out_of_date;
                        } else {
                            add_me.BackColor = up_to_date;
                        }
                        updateList.Items.Add(add_me);
                    } 
                    button1.Enabled = true;
                    button2.Enabled = true;
                } 
            } else {
                MessageBox.Show("There are no updates to install.","Wasn't necessary, man.",MessageBoxButtons.OK,MessageBoxIcon.Information);
                Application.Exit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;

            Thread downloadin_time = new Thread(downloadUpdates);
            downloadin_time.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Application.Exit();
        }

        private void downloadUpdates() {
            SecurityHandler secure = new SecurityHandler();
            invokes invoke = new invokes();
            if(secure.amAdmin()) {
                WebClient Client;
                Stream new_file;
                FileStream writer;
                int counter = 0;
                foreach(update_data update_me in updates.new_data.Values) {
                    Client = new WebClient();
                    if(update_me.revisionVersion>updates.existing_data[update_me.name].revisionVersion) {
                        invoke.setListViewItemBackColor(updateList,counter,updating);
                        try {
                            new_file = Client.OpenRead(update_me.path);
                            writer = new FileStream(updates.existing_data[update_me.name].path,FileMode.Truncate,FileAccess.Write);

                            int Length = 256;
                            Byte [] buffer = new Byte[Length];
                            int bytesRead = new_file.Read(buffer,0,Length);
                            while( bytesRead > 0 ) {
                                writer.Write(buffer,0,bytesRead);
                                bytesRead = new_file.Read(buffer,0,Length);
                            }

                            writer.Close();
                            new_file.Close();



                            invoke.setListViewItemBackColor(updateList,counter,up_to_date);
                            invoke.setListViewItemSubItemText(updateList,counter,1,update_me.majorVersion.ToString() + "." + update_me.minorVersion.ToString() + "." + update_me.revisionVersion.ToString());
                        } catch(WebException exception)  {
                            invoke.setListViewItemBackColor(updateList,counter,out_of_date);
                            MessageBox.Show(update_me.name + " failed to download. Here's why:" + Environment.NewLine + exception.Message,"Getting Old",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            //invoke.showMessageBox(this,"Getting Old", update_me.name + " failed to download. Here's why:" + Environment.NewLine + exception.Message,MessageBoxButtons.OK,MessageBoxIcon.Error);
                        }
                    } 
                    counter++;
                } 
                MessageBox.Show("Update Finished","Feeling Better",MessageBoxButtons.OK,MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
            } else {
                if(MessageBox.Show("In order to update, updater must be run with elevated permissions. Would you like to do that?","I can't do that, Dave",MessageBoxButtons.OKCancel,MessageBoxIcon.Question)==DialogResult.OK) {
                    secure.elevation(null);
                    Application.Exit();
                } else {
                    DialogResult = DialogResult.Cancel;
                }
            }
            DialogResult = DialogResult.OK;
            Application.Exit();
        }


    }
}

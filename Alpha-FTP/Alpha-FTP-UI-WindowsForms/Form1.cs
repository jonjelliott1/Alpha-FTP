using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Alpha_FTP_UI_WindowsForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        //botton 1 is the connect button
        private void button1_Click(object sender, EventArgs e)
        {
            string host = textBox1.Text;
            string userName = textBox2.Text;
            string password = textBox3.Text;
            bool success = false;
            success = ConnectToFTP(host, userName, password);
        }

        private bool ConnectToFTP(string host, string userName, string password)
        {
            textBox4.Text = "Host: " + host + " Username: " + userName + " Password: " + password;

            toolStripStatusLabel1.Text = "Attempting to connect...";
            ftp ftpClient = new ftp(host, userName, password);


            /* Get Contents of a Directory (Names Only) */
            string[] simpleDirectoryListing = ftpClient.directoryListDetailed($"/home/{userName}");
            for (int i = 0; i < simpleDirectoryListing.Count(); i++) { listBox2.Items.Add(simpleDirectoryListing[i]); }

            //toolStripStatusLabel1.Text = "Connection Failed Try Again.";
            return false;
        }

        private void buttonLoadDirectory_Click(object sender, EventArgs e)
        {

            // Setting Inital Value of Progress Bar  
            progressBar1.Value = 0;
            // Clear All Nodes if Already Exists  
            treeView1.Nodes.Clear();
         //   toolTip1.ShowAlways = true;
            LoadDirectory(@"C:\TestFiles");

            progressBar1.Value = 0;
        }



        public void LoadDirectory(string Dir)
        {
            DirectoryInfo di = new DirectoryInfo(Dir);
            //Setting ProgressBar Maximum Value  
            progressBar1.Maximum = Directory.GetFiles(Dir, "*.*", SearchOption.AllDirectories).Length + Directory.GetDirectories(Dir, "**", SearchOption.AllDirectories).Length;
            TreeNode tds = treeView1.Nodes.Add(di.Name);
            tds.Tag = di.FullName;
            tds.StateImageIndex = 0;
            tds.ImageKey = "folder.png";
            LoadFiles(Dir, tds);
            LoadSubDirectories(Dir, tds);
        }

        private void LoadSubDirectories(string dir, TreeNode td)
        {
            // Get all subdirectories  
            string[] subdirectoryEntries = Directory.GetDirectories(dir);
            // Loop through them to see if they have any other subdirectories  
            foreach (string subdirectory in subdirectoryEntries)
            {

                DirectoryInfo di = new DirectoryInfo(subdirectory);
                TreeNode tds = td.Nodes.Add(di.Name);
                tds.StateImageIndex = 0;
                tds.Tag = di.FullName;
   
                LoadFiles(subdirectory, tds);
                LoadSubDirectories(subdirectory, tds);
                UpdateProgress();

            }
        }

        private void LoadFiles(string dir, TreeNode td)
        {
            string[] Files = Directory.GetFiles(dir, "*.*");

            // Loop through them to see files  
            foreach (string file in Files)
            {
                FileInfo fi = new FileInfo(file);
                TreeNode tds = td.Nodes.Add(fi.Name);
                tds.Tag = fi.FullName;
                tds.StateImageIndex = 1;
               tds.ImageKey = "folder2.png";
                UpdateProgress();

            }
        }

        private void UpdateProgress()
        {
            if (progressBar1.Value < progressBar1.Maximum)
            {
                progressBar1.Value++;
                int percent = (int)(((double)progressBar1.Value / (double)progressBar1.Maximum) * 100);
                progressBar1.CreateGraphics().DrawString(percent.ToString() + "%", new Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black, new PointF(progressBar1.Width / 2 - 10, progressBar1.Height / 2 - 7));

                Application.DoEvents();
            }
        }

        //private void treeView1_MouseMove(object sender, MouseEventArgs e)
        //{ 
        //   // Get the node at the current mouse pointer location.  
        //    TreeNode theNode = this.treeView1.GetNodeAt(e.X, e.Y);  
  
        //   // Set a ToolTip only if the mouse pointer is actually paused on a node.  
        //   if (theNode != null && theNode.Tag != null)  
        //   {  
        //       // Change the ToolTip only if the pointer moved to a new node.  
        //       if (theNode.Tag.ToString() != this.toolTip1.GetToolTip(this.treeView1))  
        //           this.toolTip1.SetToolTip(this.treeView1, theNode.Tag.ToString());

        //     }  
        //   else     // Pointer is not over a node so clear the ToolTip.  
        //   {  
        //       this.toolTip1.SetToolTip(this.treeView1, "");
        //}
    //}



}

}

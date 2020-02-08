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
        ftp _ftpClient = null;
        string _currentDirectory = "";
        public Form1()
        {
            InitializeComponent();
            //Prefill connection information
            textBox1.Text = "ftp://192.168.1.18";
            textBox2.Text = "ftpuser";
            textBox3.Text = "ftpuser";

           


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

            _currentDirectory = $"/home/{userName}";


            UpdateListViewFTPItems(_currentDirectory);
        }

        private void UpdateListViewFTPItems(string directory)
        {
            listViewFTPItems.Clear();

            listViewFTPItems.Columns.Add("Name", 100);
            listViewFTPItems.Columns.Add("Type", 100);
            listViewFTPItems.Columns.Add("Size", 100);
            listViewFTPItems.Columns.Add("Last Updated", 100);
            /* Get Contents of a Directory (Names Only) */
            string[] detailDirectoryListing = _ftpClient.directoryListDetailed(directory);

            var fileOrDirectoryName = "";

            var fileType = "";
            var fileSize = "";
            var lastUpdatedDateTime = "";


            for (int i = 0; i < detailDirectoryListing.Count(); i++)
            {
                if (detailDirectoryListing[i].StartsWith("-"))
                {
                    //If it starts with a "-" then it means that this is a file not a directory.

                    fileOrDirectoryName = detailDirectoryListing[i].Substring(56);
                    fileType = "File";
                    fileSize = detailDirectoryListing[i].Substring(30, 12);
                    lastUpdatedDateTime = detailDirectoryListing[i].Substring(43, 12);
                }

                else if (detailDirectoryListing[i].StartsWith("d"))
                {
                    //If it starts with a "d" then it is a directory.
                    fileOrDirectoryName = detailDirectoryListing[i].Substring(56);
                    fileType = "Directory";
                    fileSize = detailDirectoryListing[i].Substring(30, 12);
                    lastUpdatedDateTime = detailDirectoryListing[i].Substring(43, 12);
                }
                else if (detailDirectoryListing[i] == "")
                {
                    break;
                }
                else
                {
                    MessageBox.Show("Unexpected value");
                }



                string[] arr = new string[4];
                ListViewItem itm;
                //add items to ListView
                arr[0] = fileOrDirectoryName;
                arr[1] = fileType;
                arr[2] = fileSize;
                arr[3] = lastUpdatedDateTime;
                itm = new ListViewItem(arr);
                listViewFTPItems.Items.Add(itm);

            }
        }

        private bool ConnectToFTP(string host, string userName, string password)
        {
            textBox4.Text = "Host: " + host + " Username: " + userName + " Password: " + password;

            toolStripStatusLabel1.Text = "Attempting to connect...";
            ftp ftpClient = new ftp(host, userName, password);
            _ftpClient = ftpClient;

           return true;
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

        private void buttonDeleteSelectedFtpItem_Click(object sender, EventArgs e)
        {
            //See if item is selected
            //If No then return message "No Item Selected"
            var items = listViewFTPItems.SelectedItems;
            if (items == null)
            {
                MessageBox.Show("Error: No Item on the list is selected");
                return;
            }

            if (listViewFTPItems.SelectedItems.Count > 1)
            {
                MessageBox.Show("Selected file count is above one. Please select only one file to delete");
                return;
            }

            if (listViewFTPItems.SelectedItems.Count > 0)
            {
                
                var itemName = listViewFTPItems.SelectedItems[0].SubItems[0].Text;
                var itemType = listViewFTPItems.SelectedItems[0].SubItems[1].Text;
                var itemSize = listViewFTPItems.SelectedItems[0].SubItems[2].Text;
                //rest of your logic
                if (itemType == "File")
                {
                    DeleteFTPFile(itemName);
                }
                else if (itemType == "Directory")
                {
                    DeleteFTPDiretory(itemName);
                }
                else
                {
                    MessageBox.Show("Error: Unexpected Value. Not a File or Directory");
                }

                UpdateListViewFTPItems(_currentDirectory);
            }
          
            
            //Add this later: If item is selected then show message confirming that the user really wants to delete this file or directory.


            //If item selected is a directory then check to see if the directory is empty.

            //If it is empty then delete directory

            //If it is not empty then delete each file inside and then 

            //If the selected item is a file then delete file.
           
        }

        private void DeleteFTPDiretory(string directoryName)
        {
            string fullPathofFileToBeDeleted = _currentDirectory + '/' + directoryName;
            _ftpClient.removeDirectory(fullPathofFileToBeDeleted);
           
        }

        private void DeleteFTPFile(string file)
        {
            string fullPathofFileToBeDeleted = _currentDirectory+ '/' + file;
            _ftpClient.delete(fullPathofFileToBeDeleted);
         
        }

        private void buttonCreateNewDirectory_Click(object sender, EventArgs e)
        {
            string promptValue = Prompt.ShowDialog("What do you want to name the new directory?", "Create a New Directory");
            _ftpClient.createDirectory(promptValue);

            UpdateListViewFTPItems(_currentDirectory);
        }

        private void buttonRenameSelected_Click(object sender, EventArgs e)
        {
            string newName = Prompt.ShowDialog("Please input new name.", "Rename");

            var items = listViewFTPItems.SelectedItems;
            if (items == null)
            {
                MessageBox.Show("Error: No Item on the list is selected");
                return;
            }

            if (listViewFTPItems.SelectedItems.Count > 1)
            {
                MessageBox.Show("Selected file count is above one. Please select only one to rename");
                return;
            }

            if (listViewFTPItems.SelectedItems.Count > 0)
            {
                var itemName = listViewFTPItems.SelectedItems[0].SubItems[0].Text;
                var itemType = listViewFTPItems.SelectedItems[0].SubItems[1].Text;
                var itemSize = listViewFTPItems.SelectedItems[0].SubItems[2].Text;

                var fullPathAndName = _currentDirectory + '/' + itemName;
                if (itemType == "File")
                {
                    _ftpClient.renameFile(fullPathAndName, newName);
                }
                else if (itemType == "Directory")
                {
                    _ftpClient.renameFile(fullPathAndName, newName);
                }
                else
                {
                    MessageBox.Show("Error: Unexpected Value. Not a File or Directory");
                }

                UpdateListViewFTPItems(_currentDirectory);
            }





         


            UpdateListViewFTPItems(_currentDirectory);
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

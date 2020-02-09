using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Alpha_FTP_UI_WindowsForms
{
    public partial class Form1 : Form
    {
        ftp _ftpClient = null;
        string _currentDirectory = "";
        string _currentLocalPath = "";
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
            listViewFTPItems.Columns.Add("Type", 55);
            listViewFTPItems.Columns.Add("Size", 75);
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

        private void UpdateListViewLocalItems()
        {

            FolderBrowserDialog folderPicker = new FolderBrowserDialog();
            if (folderPicker.ShowDialog() == DialogResult.OK)
            {

                listViewLocalFiles.Items.Clear();
                listViewLocalFiles.View = View.Details;
                listViewLocalFiles.Columns.Add("Name", 100);
                listViewLocalFiles.Columns.Add("Type", 50);
                listViewLocalFiles.Columns.Add("Size", 75);
                listViewLocalFiles.Columns.Add("Last Updated", 100);

                _currentLocalPath = folderPicker.SelectedPath;

                string[] files = Directory.GetFiles(folderPicker.SelectedPath);
                foreach (string file in files)
                {

                    string fileName = Path.GetFileName(file);
                    ListViewItem item = new ListViewItem(fileName);
                    item.Tag = file;

                    listViewLocalFiles.Items.Add(item);

                }

            }

        }

        private void RefreshListViewLocalItems()
        {

            if (_currentLocalPath != "")
            {

                listViewLocalFiles.Items.Clear();
                listViewLocalFiles.View = View.Details;
                listViewLocalFiles.Columns.Add("Name", 100);
                listViewLocalFiles.Columns.Add("Type", 50);
                listViewLocalFiles.Columns.Add("Size", 75);
                listViewLocalFiles.Columns.Add("Last Updated", 100);
                               
                string[] files = Directory.GetFiles(_currentLocalPath);
                foreach (string file in files)
                {

                    string fileName = Path.GetFileName(file);
                    ListViewItem item = new ListViewItem(fileName);
                    item.Tag = file;

                    listViewLocalFiles.Items.Add(item);

                }

            }



        }

        private bool ConnectToFTP(string host, string userName, string password)
        {

            ftp ftpClient = new ftp(host, userName, password);
            _ftpClient = ftpClient;

           return true;
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

        private void buttonRefreshLocalFileView_Click(object sender, EventArgs e)
        {
            UpdateListViewLocalItems();
        }

        private void buttonDownloadSelectedFile_Click(object sender, EventArgs e)
        {

            DownloadSelectedFileFromFTPtoLocal();
        }

        private void DownloadSelectedFileFromFTPtoLocal()
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
                    DownloadFTPFile(itemName);
                }
                else if (itemType == "Directory")
                {
                    MessageBox.Show("Sorry we cannot download directories at this time. Please select a file instead for download.");
                }
                else
                {
                    MessageBox.Show("Error: Unexpected Value. Not a File or Directory");
                }

                UpdateListViewFTPItems(_currentDirectory);
            }

        }

        private void DownloadFTPFile(string itemName)
        {
            string fullPathofFileToBeDownloaded = _currentDirectory + '/' + itemName;
            string fullPathofFileDestination = _currentLocalPath + "\\" + itemName;

            ///* Download a File */
            //ftpClient.download("/home/ftpuser/HelloWorld.txt", @"C:\TestFiles\HelloWorld.txt");
            _ftpClient.download(fullPathofFileToBeDownloaded, fullPathofFileDestination);
            RefreshListViewLocalItems();
        }

        private void UploadSelectedFileFromFTPtoLocal()
        {
            var fileName = listViewLocalFiles.SelectedItems[0].SubItems[0].Text;
           // var _currentLocalPath = @"C:\\jonje\\Desktop\\Test";
          //  var fullPathOfFile = _currentLocalPath + "\\" + fileName;

            string fullPathofFileToBeUploaded = _currentLocalPath + '\\' + fileName;
            string fullPathofFileDestination = _currentDirectory + "/" + fileName;

            ///* Download a File */
            //ftpClient.download("/home/ftpuser/HelloWorld.txt", @"C:\TestFiles\HelloWorld.txt");
            _ftpClient.upload(fullPathofFileDestination, fullPathofFileToBeUploaded);
            UpdateListViewFTPItems(_currentDirectory);


        }

        private void buttonUploadSelectedFile_Click(object sender, EventArgs e)
        {
            UploadSelectedFileFromFTPtoLocal();
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

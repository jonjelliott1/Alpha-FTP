using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    }
}

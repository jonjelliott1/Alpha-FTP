using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpha_FTP_terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Create Object Instance */
            ftp ftpClient = new ftp(@"ftp://192.168.1.18/", "ftpuser", "ftpuser");

            ///* Upload a File */
            //ftpClient.upload("/home/ftpuser/testfile.txt", @"C:\TestFiles\TestFile.txt");

            ///* Download a File */
            //ftpClient.download("/home/ftpuser/HelloWorld.txt", @"C:\TestFiles\HelloWorld.txt");

            ///* Delete a File */
            //ftpClient.delete("/home/ftpuser/DeleteMe.txt");

            ///* Rename a File */
            //ftpClient.rename("/home/ftpuser/RenameMe.txt", "IHaveBeenRenamed.txt");

            ///* Create a New Directory */
            //ftpClient.createDirectory("/home/ftpuser/CreatedDirectory");

            /* Get the Date/Time a File was Created */
            string fileDateTime = ftpClient.getFileCreatedDateTime("/home/ftpuser/HelloWorld.txt");
            Console.WriteLine(fileDateTime);

            /* Get the Size of a File */
            string fileSize = ftpClient.getFileSize("/home/ftpuser/HelloWorld.txt");
            Console.WriteLine(fileSize);

            /* Get Contents of a Directory (Names Only) */
            string[] simpleDirectoryListing = ftpClient.directoryListDetailed("/home/ftpuser");
            for (int i = 0; i < simpleDirectoryListing.Count(); i++) { Console.WriteLine(simpleDirectoryListing[i]); }

            /* Get Contents of a Directory with Detailed File/Directory Info */
            string[] detailDirectoryListing = ftpClient.directoryListDetailed("/home/ftpuser");
            for (int i = 0; i < detailDirectoryListing.Count(); i++) { Console.WriteLine(detailDirectoryListing[i]); }
            /* Release Resources */
            ftpClient = null;
        }
    }
}

using System;
using System.IO;
using Azure.Storage.Blobs;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using Azure.Storage.Blobs.Models;
using System.Threading.Tasks;
using Azure.Storage;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //var storageURI = Environment.GetEnvironmentVariable("CONNECT_STR");
            var cm = new ContainerManipulation();
            var containerName = cm.MakeNewContainer();
            Console.WriteLine("There are: " + cm.ListContainers().Count().ToString() + " containers");

            var lc = cm.ListContainers();

            foreach(var item in lc)
            {
                Console.WriteLine(item);
                //var bc = new BlobContainerClient(Environment.GetEnvironmentVariable("CONNECT_STR"), item);
                //bc.Delete();
                //var bsc_Delete = new BlobClient(Environment.GetEnvironmentVariable("CONNECT_STR"), item, string.Empty);
                //bsc_Delete.DeleteIfExists();

            }

            //Console.WriteLine("Downloading Blobs now boss");
            //var blobServiceClient = new BlobClient(Environment.GetEnvironmentVariable("CONNECT_STR"),containerName.First().Key, containerName.First().Value);
            //cm.DownloadFile(blobServiceClient);
            Console.ReadKey();
        }
    }

    public class ContainerManipulation
    {

        public Dictionary<string,string> MakeNewContainer()
        {
            var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("CONNECT_STR"));
            var TestContainer = "just_testing_this_out" + Guid.NewGuid().ToString();
            var containerClient = blobServiceClient.CreateBlobContainer(TestContainer);
            var blobName = PutDocumentInContainer(containerClient, TestContainer);

            var dict = new Dictionary<string, string>
            {
                { TestContainer, blobName }
            };
            return dict;
        }

        public async void DeleteContainer(BlobContainerClient BCC)
        {
            await BCC.DeleteAsync();
        }
        public int HowManyContainers()
        {
            var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("CONNECT_STR"));
            var randyTestContainer = "justtestingthisout" + Guid.NewGuid().ToString();
            return blobServiceClient.GetBlobContainers().Count();
        }
        public List<string> ListContainers()
        {
            var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("CONNECT_STR"));
            var randyTestContainer = "justtestingthisout" + Guid.NewGuid().ToString();
            return blobServiceClient.GetBlobContainers().Select(s => s.Name).ToList();
            
        }

        public async Task<bool> ListBlobsInContainer(BlobContainerClient containerClient)
        {
            Console.WriteLine("Listing blobs...");

            // List all blobs in the container

            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine("\t" + blobItem.Name);
            }

            return true;
        }
        private string PutDocumentInContainer(BlobContainerClient containerClient, string fileName)
        {
            BlobClient blobClient = containerClient.GetBlobClient(GetDocuments());
            var blobName = blobClient.Name;

            FileStream uploadFileStream = File.OpenRead(blobName);
            var blah = blobClient.UploadAsync(uploadFileStream).Result;
            uploadFileStream.Close();
            return blobName;
        }

        private string GetDocuments()
        {
           
            return @"D://TestDocuments//arandomfilehere.pdf";
        }

        public async void DownloadFile(BlobClient blobClient)
        {
            // Download the blob to a local file
            // Append the string "DOWNLOAD" before the .txt extension so you can see both files in MyDocuments
            //var localFilePath = @"D:\NewDownloads\downloadedfromazure\downloadedfile_" + Guid.NewGuid() + "_.pdf";
            var localFilePath = @"D:\NewDownloads\downloadedfromazure";

            Console.WriteLine("\nDownloading blob to\n\t{0}\n", localFilePath);

            // Download the blob's contents and save it to a file
            BlobDownloadInfo download = blobClient.Download();
            
            FileStream downloadFileStream = File.OpenWrite(localFilePath);
            await download.Content.CopyToAsync(downloadFileStream);
            downloadFileStream.Close();
        }
    }
}

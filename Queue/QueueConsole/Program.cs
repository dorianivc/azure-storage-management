using System.IO;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Storage.Blob;
namespace QueueConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();

            CloudStorageAccount myClient = CloudStorageAccount.Parse(configuration["connectionstring"]);
            CloudQueueClient queueClient= myClient.CreateCloudQueueClient();


            CloudQueue queue= queueClient.GetQueueReference("filaprocesos");
            queue.CreateIfNotExists();

            CloudQueueMessage peekedMessage= queue.PeekMessage();
            CloudBlobClient blobClient = myClient.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("toolsdebug");
            container.CreateIfNotExists();

            foreach(CloudQueueMessage item in queue.GetMessages(32,TimeSpan.FromMilliseconds(1))){
                string filePath=string.Format(@"log{0}.txt", item.Id);
                TextWriter tempFile= File.CreateText(filePath);
                var message = queue.GetMessage().AsString;
                tempFile.WriteLine(message);
                Console.WriteLine("Archivo Creado");
                tempFile.Close();

                using( var fileStream = System.IO.File.OpenRead(filePath)){
                    CloudBlockBlob myBlob = container.GetBlockBlobReference(string.Format(@"log{0}.txt", item.Id));
                    myBlob.UploadFromStream(fileStream);
                    Console.WriteLine("Blob Creado");
                    

                }
                queue.DeleteMessage(item);
                File.Delete(filePath);
                Console.WriteLine("Archivo Temporal Eliminado");

            }

            Console.ReadLine();
        }
    }
}

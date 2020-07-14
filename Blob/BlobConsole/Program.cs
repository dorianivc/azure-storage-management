using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.FileExtensions;
using System.IO;
using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace BlobConsole
{
    class Program
    {
        static void Main(string[] args)
        {
           var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();
         
           
          //Manipulamos un Blob Storage
        CloudStorageAccount storageAccount= CloudStorageAccount.Parse(configuration["connectionstring"]);
        //Creamos el container
        CloudBlobClient clienteBlob= storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container= clienteBlob.GetContainerReference("toolsdebug");
        container.CreateIfNotExists();
        container.SetPermissions(new BlobContainerPermissions{PublicAccess = BlobContainerPublicAccessType.Blob});
        
        CloudBlockBlob miBlob= container.GetBlockBlobReference("foto.jpg");
        using (var fileStream= System.IO.File.OpenRead(@"RAYO.jpg")){
            miBlob.UploadFromStream(fileStream);
        }
        Console.WriteLine("Tu archivo fue subido a la nube con existo");
        Console.ReadLine();



        }
    }
}

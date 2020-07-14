using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.FileExtensions;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure;


namespace TableConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();


             CreateTableAsync("testingDorian001", configuration["connectionstring"]).Wait();


            Console.WriteLine("Estamos listos, chao!");

        }
        private static async Task InsertOperationAsync(CloudTable table)
        {
            {
                Contacto contacto = new Contacto("Espinoza", "Miranda")
                {
                    Email = "miranda@outlook.com",
                    Telefono = "8118748461"
                };

                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(contacto);
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                Contacto insertedCustomer = result.Result as Contacto;
                Console.WriteLine("Entidad modificada");
            }
        }
        private static async Task DeleteOperationAsync(CloudTable table)
        {
            Contacto contacto = new Contacto("Espinoza", "Miranda")
            {
                Email = "miranda@outlook.com",
                Telefono = "8118748461",
                ETag = "*"
            };

            TableOperation deleteOperation = TableOperation.Delete(contacto);
            TableResult result = await table.ExecuteAsync(deleteOperation);

            Console.WriteLine("Resultado de la operación: " + result.RequestCharge);
        }
        private static async Task GetOperationAsync(CloudTable table)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<Contacto>("Espinoza", "Miranda");
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            Contacto customer = result.Result as Contacto;

            if (customer != null)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", customer.PartitionKey, customer.RowKey, customer.Email, customer.Telefono);
            }
        }


        private static async Task<CloudTable> CreateTableAsync(string tableName, string connectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(tableName);
            if (await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Tabla Creada: {0}: ", tableName);
            }
            else
            {
                Console.WriteLine("La tabla {0} ya existe", tableName);
            }
            await InsertOperationAsync(table);
            Console.WriteLine("Esta todo hecho");
            return table;
        }
    }
}

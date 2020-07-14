using Microsoft.Azure.Cosmos.Table;

namespace TableConsole
{
    public class Contacto: TableEntity
    {
        public Contacto(){

        }
        public Contacto(string apellido, string nombre){
            PartitionKey=apellido;
            RowKey=nombre;
        }
        public string Email{get; set;}

        public string Telefono{get; set;}
    }
}
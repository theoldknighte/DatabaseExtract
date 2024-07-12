using System.CodeDom;
using System.Data.SQLite;

namespace DatabaseExtract;

internal class Program
{
    static void Main(string[] args)
    {
        SQLiteConnection connection = null;
        try
        {
            connection = new SQLiteConnection("Data Source=C:\\Users\\ejber\\AppData\\Roaming\\LINQPad\\ChinookDemoDb.sqlite");
            connection.Open();
            var transaction = connection.BeginTransaction();

            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * " +
                                  "FROM Invoice invoice " + 
                                  "JOIN InvoiceLine line ON invoice.InvoiceId = line.InvoiceLineId " +
                                  "WHERE invoice.BillingState is null";
            var reader = command.ExecuteReader(System.Data.CommandBehavior.Default);
            List<string> columns = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i));
            }
            foreach (var row in reader)
            {
                string rowValue = row.ToString();
                Console.WriteLine(rowValue);
            }
        }
        
        catch(Exception ex)
        {

        }
        finally
        {
            if (connection != null)
            {
                connection.Close();
            }
        }

        Console.ReadLine();
    }
}

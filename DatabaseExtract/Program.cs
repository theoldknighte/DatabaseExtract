using System.CodeDom;
using System.Data.SqlClient;
using System.Data;
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
            using (connection)
            {
                connection.Open();
                string sqlQuery = "SELECT * " +
                                           "FROM Invoice invoice " +
                                           "JOIN InvoiceLine line ON invoice.InvoiceId = line.InvoiceLineId " +
                                           "WHERE invoice.BillingState is null";
                //GetSchemaInfo(connection, sqlQuery);

                //var transaction = connection.BeginTransaction();

                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = sqlQuery;
                var reader = command.ExecuteReader(System.Data.CommandBehavior.Default);
                var schema = reader.GetSchemaTable();

                List<string> columns = new List<string>();
                string currentColumn = string.Empty;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    currentColumn = reader.GetName(i);
                    Console.WriteLine($"Current column: {currentColumn}");
                    columns.Add(currentColumn);
                }
                List<string> ouput = new List<string>();
                string temp = string.Empty;
                while (reader.Read())
                {
                    for (int i = 0; i < columns.Count; i++)
                    {
                        temp = reader.GetValue(i).ToString();
                        Console.Write($"{temp}");
                        if(i !=  columns.Count - 1)
                        {
                            Console.Write(", ");
                        }
                        ouput.Add(temp);
                    }
                    Console.WriteLine();
                }



                //foreach (var row in reader)
                //{
                //    string rowValue = row.ToString();
                //    Console.WriteLine(rowValue);
                //}
            }
        }
        
        catch(Exception ex)
        {
            Console.WriteLine($"Exception throw: {ex.ToString()}");
        }
        //finally
        //{
        //    if (connection != null && connection.State == ConnectionState.Open)
        //    {
        //        connection.Close();
        //    }
        //}

        Console.ReadLine();
    }


    static void GetSchemaInfo(SQLiteConnection connection, string sqlQuery)
    {
        //using (connection)
        //{
            SQLiteCommand command = new SQLiteCommand(
                                                      sqlQuery,
                                                      connection);
            //connection.Open();

            SQLiteDataReader reader = command.ExecuteReader();
            DataTable schemaTable = reader.GetSchemaTable();

            foreach (DataRow row in schemaTable.Rows)
            {
                Console.WriteLine("New Data Row \r\n");
                foreach (DataColumn column in schemaTable.Columns)
                {
                    Console.WriteLine(string.Format("{0} = {1}",
                       column.ColumnName, row[column]));
                }
            }
        //}
    }
}

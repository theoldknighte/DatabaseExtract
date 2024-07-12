using System.CodeDom;
using System.Data.SqlClient;
using System.Data;
using System.Data.SQLite;

namespace DatabaseExtract;

internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Must provide output path");
            Console.ReadLine();
            throw new ArgumentException("Must provide output path");
        }
        string pathToDesiredOutputLocation = Path.GetFullPath(args[0]);
        //string pathToDesiredOutputLocation = @"C:\DatabaseTest\DefaultArgsOutput.csv";
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
                List<List<string>> rows = new List<List<string>>();
                List<string> currentRow = null;
                string temp = string.Empty;
                while (reader.Read())
                {
                    currentRow = new List<string>();
                    rows.Add(currentRow);
                    for (int i = 0; i < columns.Count; i++)
                    {
                        temp = reader.GetValue(i).ToString();
                        Console.Write($"{temp}");
                        if(i !=  columns.Count - 1)
                        {
                            Console.Write(", ");
                        }
                        currentRow.Add(temp);
                    }
                    Console.WriteLine();
                }

                WriteCSV(columns, rows, pathToDesiredOutputLocation);

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

    private static void WriteCSV(List<string> columns, List<List<string>> rows, string outputLocation)
    {
        if(columns == null)
        {
            throw new NullReferenceException($"{nameof(columns)} cannot be null");
        }
        if (rows == null)
        {
            throw new NullReferenceException($"{nameof(rows)} cannot be null");
        }
        if (outputLocation == null)
        {
            throw new NullReferenceException($"{nameof(outputLocation)} cannot be null, specify a path");
        }

        using (StreamWriter streamWriter = File.CreateText(outputLocation))
        {
            for (int i = 0; i < columns.Count; i++)
            {
                streamWriter.Write(columns[i]);
                if (i != columns.Count - 1)
                {
                    streamWriter.Write(",");
                }
            }
            streamWriter.WriteLine();
            foreach(List<string> row in rows)
            {
                for (int i = 0; i < row.Count; i++)
                {
                    streamWriter.Write(row[i]);
                    if (i != row.Count - 1)
                    {
                        streamWriter.Write(",");
                    }
                    else
                    {
                        streamWriter.WriteLine();
                    }
                }
            }
        }
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

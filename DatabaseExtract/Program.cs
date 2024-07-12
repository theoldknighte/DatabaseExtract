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

                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = sqlQuery;
                var reader = command.ExecuteReader(System.Data.CommandBehavior.Default);
                var ColumnsAndRows = ParseDataFromReader(reader);

                WriteCSV(ColumnsAndRows.Item1, ColumnsAndRows.Item2, pathToDesiredOutputLocation);
            }
        }
        
        catch(Exception ex)
        {
            Console.WriteLine($"Exception throw: {ex.ToString()}");
        }

        Console.ReadLine();
    }

    private static Tuple<List<string>, List<List<string>>> ParseDataFromReader(SQLiteDataReader dataReader)
    {
        if(dataReader == null)
        {
            throw new ArgumentException($"{nameof(dataReader)} cannot be null");
        }
        var schema = dataReader.GetSchemaTable();

        List<string> columns = new List<string>();
        string currentColumn = string.Empty;
        for (int i = 0; i < dataReader.FieldCount; i++)
        {
            currentColumn = dataReader.GetName(i);
            Console.WriteLine($"Current column: {currentColumn}");
            columns.Add(currentColumn);
        }
        List<List<string>> rows = new List<List<string>>();
        List<string> currentRow = null;
        string temp = string.Empty;
        while (dataReader.Read())
        {
            currentRow = new List<string>();
            rows.Add(currentRow);
            for (int i = 0; i < columns.Count; i++)
            {
                temp = dataReader.GetValue(i).ToString();
                Console.Write($"{temp}");
                if (i != columns.Count - 1)
                {
                    Console.Write(", ");
                }
                currentRow.Add(temp);
            }
            Console.WriteLine();
        }

        return new Tuple<List<string>, List<List<string>>>(columns, rows);
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
        SQLiteCommand command = new SQLiteCommand( sqlQuery,
                                                   connection);

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
    }
}

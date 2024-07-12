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
            connection = new SQLiteConnection("Data Source = C:\\DatabaseTest\\chinook.db;");
            connection.Open();
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

        Console.WriteLine("Hello, World!");
    }
}

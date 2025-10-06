using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Acuo.IHEAudit.DAL48
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connString = "Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase";

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);
            var dataSource = dataSourceBuilder.Build();

            var conn = dataSource.OpenConnection();

            // Insert some data
            using (var cmd = new NpgsqlCommand("INSERT INTO data (some_field) VALUES (@p)", conn))
            {
                cmd.Parameters.AddWithValue("p", "Hello world");
                cmd.ExecuteNonQuery();
            }

            // Retrieve all rows
            using (var cmd = new NpgsqlCommand("SELECT some_field FROM data", conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(reader.GetString(0));
                    }   
                }
            }
        }
    }
}

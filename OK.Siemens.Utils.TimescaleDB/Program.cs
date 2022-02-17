using Microsoft.Extensions.Configuration;
using Npgsql;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

TimescaleHelper ts = new TimescaleHelper();

ts.CheckDatabaseConnection(config["PgsqlConnectionString"]!);
ts.ChangeToHypertable(config["PgsqlConnectionString"]!, "DataRecords", "TimeStamp");

public class TimescaleHelper
{
    /// <summary>
    /// Returns connection to DB
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    NpgsqlConnection getConnection(string connectionString)
    {
        var Connection = new NpgsqlConnection(connectionString);
        Connection.Open();
        return Connection;
    }

    /// <summary>
    /// Check connection and print versions info
    /// </summary>
    /// <param name="connectionString"></param>
    public void CheckDatabaseConnection(string connectionString)
    {
        using (var conn = getConnection(connectionString))
        {

            var sql = "SELECT default_version, comment FROM pg_available_extensions WHERE name = 'timescaledb';";

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                using NpgsqlDataReader rdr = cmd.ExecuteReader();

                if (!rdr.HasRows)
                {
                    Console.WriteLine("Missing TimescaleDB extension!");
                    conn.Close();
                    return;
                }

                while (rdr.Read())
                {
                    Console.WriteLine("TimescaleDB Default Version: {0}\n{1}", rdr.GetString(0), rdr.GetString(1));
                }

                conn.Close();
            }
        }
    }

    /// <summary>
    /// Converts table to hypertable
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="tableName"></param>
    /// <param name="columnName"></param>
    public void ChangeToHypertable(string connectionString, string tableName, string columnName)
    {
        using (var conn = getConnection(connectionString))
        {
            var sql = $"SELECT create_hypertable('\"{tableName}\"', '{columnName}');";

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
                Console.Out.WriteLine($"Converted the {tableName} table into a TimescaleDB hypertable!");
                conn.Close();
            }
        }
    }
}
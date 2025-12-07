using Npgsql;

namespace InventarWorkerCommon.Services.Database;

/// <summary>
/// Provides services for interacting with a PostgreSQL database.
/// This class facilitates connection management and initialization
/// of the database using the Npgsql ADO.NET library.
/// </summary>
public class PgSqlDbService
{
    private readonly string _connectionString;

    /// <summary>
    /// Provides services for interacting with a PostgreSQL database.
    /// This class simplifies managing the PostgreSQL database connection
    /// and enables database initialization.
    /// </summary>
    public PgSqlDbService(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Initializes the PostgreSQL database by opening a connection
    /// using the provided connection string. This method ensures
    /// that the database is ready for operations.
    /// </summary>
    public void InitializeDatabase()
    {
        // Parse connection string to extract target database name
        var targetCsBuilder = new NpgsqlConnectionStringBuilder(_connectionString);
        var targetDatabase = targetCsBuilder.Database;

        // Build an admin connection string to the maintenance database (postgres)
        var adminCsBuilder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
            Database = string.IsNullOrWhiteSpace(targetCsBuilder?.Database) ? "postgres" : "postgres"
        };

        // Connect to maintenance DB and ensure target DB exists
        using (var adminConnection = new NpgsqlConnection(adminCsBuilder.ConnectionString))
        {
            adminConnection.Open();

            // Check if database exists
            using (var checkCmd = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = @dbname", adminConnection))
            {
                checkCmd.Parameters.AddWithValue("dbname", targetDatabase);
                var exists = checkCmd.ExecuteScalar() is not null;

                if (!exists)
                {
                    // Create database with a quoted identifier to handle special characters/casing
                    var quotedDbName = QuoteIdentifier(targetDatabase);
                    using var createCmd = new NpgsqlCommand($"CREATE DATABASE {quotedDbName}", adminConnection);
                    createCmd.ExecuteNonQuery();
                }
            }
        }

        // Finally, try opening a connection to the target DB to ensure it's ready
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
    }

    private static string QuoteIdentifier(string identifier)
    {
        // Quote with double quotes and escape existing quotes per PostgreSQL rules
        return "\"" + identifier.Replace("\"", "\"\"") + "\"";
    }


}
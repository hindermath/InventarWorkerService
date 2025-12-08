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
#if DEBUG
        // Determine the target database name from the connection string
        var targetDatabase = GetTargetDatabaseName();
        // Ensure the target database exists, create it if necessary
        EnsureDatabaseExists(targetDatabase);
#else
        EnsureDatabaseExists(GetTargetDatabaseName());
#endif

        // Finally, try opening a connection to the target DB to ensure it's ready
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
    }

    /// <summary>
    /// Ensures that the specified PostgreSQL database exists on the server.
    /// Connects to the maintenance database and creates the target database if it doesn't exist.
    /// </summary>
    /// <param name="targetDatabase">The name of the database that should exist.</param>
    private void EnsureDatabaseExists(string targetDatabase)
    {
        // Build an admin connection string to the maintenance database (postgres)
        var adminCsBuilder = new NpgsqlConnectionStringBuilder(_connectionString)
        {
            Database = "postgres"
        };

        // Connect to maintenance DB and ensure target DB exists
        using var adminConnection = new NpgsqlConnection(adminCsBuilder.ConnectionString);
        adminConnection.Open();

        // Check if database exists
        using var checkDbExistsCmd = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = @dbname", adminConnection);
        checkDbExistsCmd.Parameters.AddWithValue("dbname", targetDatabase);
        var dbExists = checkDbExistsCmd.ExecuteScalar() is not null;

        if (dbExists is false)
        {
            // Create database with a quoted identifier to handle special characters/casing
            var quotedDbName = QuoteIdentifier(targetDatabase);
            using var createCmd = new NpgsqlCommand($"CREATE DATABASE {quotedDbName}", adminConnection);
            createCmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Extracts and validates the target database name from the configured connection string.
    /// </summary>
    /// <returns>The database name specified in the connection string.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no database is specified in the connection string.</exception>
    private string GetTargetDatabaseName()
    {
        var csStringBuilder = new NpgsqlConnectionStringBuilder(_connectionString);
        var dbName = csStringBuilder.Database;

        if (string.IsNullOrWhiteSpace(dbName))
        {
            throw new InvalidOperationException("No target database specified in the PostgreSQL connection string.");
        }

        return dbName;
    }

    /// <summary>
    /// Quotes the provided identifier to ensure it is valid and safe for use
    /// in PostgreSQL database operations. The method adds double quotes around
    /// the identifier and escapes any existing quotes within it, adhering
    /// to PostgreSQL rules for quoted identifiers.
    /// </summary>
    /// <param name="identifier">The identifier to be quoted, which may contain special characters or reserved words.</param>
    /// <returns>A quoted string compliant with PostgreSQL identifier rules.</returns>
    private static string QuoteIdentifier(string identifier)
    {
        // Quote with double quotes and escape existing quotes per PostgreSQL rules
        return "\"" + identifier.Replace("\"", "\"\"") + "\"";
    }


}
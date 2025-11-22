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
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
    }


}
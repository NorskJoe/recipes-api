using Application.Common.Interfaces;
using Dapper;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Runs on application startup. Detects whether the schema exists and,
    /// if tables are missing, executes the .sql scripts in Migrations/ (in order)
    /// to create tables and seed lookup data.
    ///
    /// You write the SQL in the Migrations folder. This class only orchestrates
    /// reading and executing those files via Dapper.
    /// </summary>
    public class DatabaseInitializer
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DatabaseInitializer(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync(
                cancellationToken
            );

            // TODO: locate the .sql files under Persistence/Migrations, read each one,
            //       and execute in filename order:
            //
            //       await connection.ExecuteAsync(scriptSql);
        }
    }
}

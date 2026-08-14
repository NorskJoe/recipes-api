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

            await connection.ExecuteAsync(
                @"
                    IF OBJECT_ID(N'dbo.Migrations', N'U') IS NULL BEGIN
                        CREATE TABLE dbo.Migrations (
                                ScriptName VARCHAR(50),
                                ExecutedDate DATETIME DEFAULT SYSUTCDATETIME(),
                                PRIMARY KEY (ScriptName)
                                )
                    END;
                    "
            );

            var executedScript = (
                await connection.QueryAsync<string>("SELECT ScriptName FROM Migrations")
            ).ToList();

            var migrationFiles = Directory
                .GetFiles(
                    Path.Combine(AppContext.BaseDirectory, "Migrations"),
                    "*.sql"
                )
                .OrderBy(f => Path.GetFileName(f));

            foreach (var file in migrationFiles)
            {
                var scriptName = Path.GetFileName(file);

                if (executedScript.Contains(scriptName))
                    continue;

                var scriptContent = File.ReadAllText(file);
                await connection.ExecuteAsync(scriptContent);

                await connection.ExecuteAsync(
                    @"
                        INSERT INTO Migrations (ScriptName) VALUES (@Name)
                        ",
                    new { Name = scriptName }
                );
            }
        }
    }
}

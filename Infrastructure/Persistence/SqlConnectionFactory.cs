using System.Data;
using Application.Common.Interfaces;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Concrete implementation of IDbConnectionFactory using SQL Server.
    /// This is the only place the SQL Server client type is referenced.
    /// </summary>
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IDbConnection> CreateOpenConnectionAsync(
            CancellationToken cancellationToken = default
        )
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
    }
}

namespace Application.Common.Interfaces
{
    /// <summary>
    /// Abstraction for obtaining an open database connection.
    /// Defined in Application so handlers depend on this interface, not on SQL Server.
    /// Implemented in Infrastructure (SqlConnectionFactory).
    /// </summary>
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateOpenConnectionAsync(
            CancellationToken cancellationToken = default
        );
    }
}

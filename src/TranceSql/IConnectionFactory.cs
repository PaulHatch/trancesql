using System.Data.Common;

namespace TranceSql
{
    /// <summary>
    /// Defines a factory responsible for providing DB connections, including
    /// connection credentials. 
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Creates a new DB connection. It is the responsiblity of
        /// implementations to ensure that the DB type returned is compatible
        /// with the dialect being used.
        /// </summary>
        /// <returns>New DB connection.</returns>
        DbConnection CreateConnection();
    }
}
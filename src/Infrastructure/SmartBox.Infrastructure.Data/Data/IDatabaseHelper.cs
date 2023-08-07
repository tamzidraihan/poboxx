using System.Data;
using System.Data.SqlClient;

namespace SmartBox.Infrastructure.Data.Data
{
    /// <summary>
    
    /// </summary>
    public interface IDatabaseHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();
    }
}
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MySqlConnector;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace SmartBox.Infrastructure.Data.Data
{

    public class DatabaseHelper : IDatabaseHelper
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;

        public DatabaseHelper(string connectionString, IWebHostEnvironment environment)
        {
            //Synchronous
            DapperExtensions.DapperExtensions.SqlDialect = new DapperExtensions.Sql.MySqlDialect();

            //Asynchronous
            DapperExtensions.DapperAsyncExtensions.SqlDialect = new DapperExtensions.Sql.MySqlDialect();
            
            this._connectionString = connectionString;
            _environment = environment;
        }

        public IDbConnection GetConnection()
        {
            var conn = new MySqlConnection(this._connectionString);
            
            if (_environment.IsDevelopment())
                return new ProfiledDbConnection(conn, MiniProfiler.Current);

            return conn;
        }
    }
}
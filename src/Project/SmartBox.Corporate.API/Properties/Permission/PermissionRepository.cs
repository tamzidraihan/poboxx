using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Permission;
using SmartBox.Business.Core.Entities.User;
using SmartBox.Business.Core.Entities.UserRole;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Permission
{
    public class RolePermissionRepository : GenericRepositoryBase<PermissionEntity, RolePermissionRepository>, IRolePermissionRepository
    {
        public RolePermissionRepository(IDatabaseHelper databaseHelper, ILogger<RolePermissionRepository> logger) : base(databaseHelper,
          logger)
        {

        }
        string BuildInsertScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.Permission));

            sql.Append(" (");
            sql.Append(nameof(PermissionEntity.PermissionId));
            sql.Append(nameof(PermissionEntity.Name));
            sql.Append($", {nameof(PermissionEntity.DateCreated)}");
            sql.Append($", {nameof(PermissionEntity.DateModified)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append(nameof(PermissionEntity.PermissionId));
            sql.Append(nameof(PermissionEntity.Name));
            sql.Append($", {nameof(PermissionEntity.DateCreated)}");
            sql.Append($", {nameof(PermissionEntity.DateModified)}");


            sql.Append(")");

            return sql.ToString();
        }

        string BuildUpdateScript()
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.Permission} SET ");

            sql.Append($"{nameof(PermissionEntity.PermissionId)} = @{nameof(PermissionEntity.PermissionId)}");
            sql.Append($"{nameof(PermissionEntity.Name)} = @{nameof(PermissionEntity.Name)}");
            
           
            sql.Append(", ");
            sql.Append($"{nameof(PermissionEntity.DateCreated)} = @{nameof(PermissionEntity.DateCreated)}");
            sql.Append(", ");
            sql.Append($"{nameof(PermissionEntity.DateModified)} = @{nameof(PermissionEntity.DateModified)}");
            sql.Append(", ");
          
            sql.Append($" WHERE {nameof(PermissionEntity.PermissionId)} = @{nameof(PermissionEntity.PermissionId)}");

            return sql.ToString();
        }

        string BuildDeleteScript()
        {
            var sql = new StringBuilder(string.Concat("DELETE ", GlobalDatabaseConstants.DatabaseTables.Permission));

            sql.Append($"{nameof(PermissionEntity.PermissionId)} = @{nameof(PermissionEntity.PermissionId)}");

            return sql.ToString();
        }

        string BuildGetAllPermission()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Permission}");
            sql.Append($" WHERE {nameof(PermissionEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");

            return sql.ToString();
        }

        public async Task<int> SetPermissionEntity(PermissionEntity permissionEntity)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(PermissionEntity.PermissionId)), permissionEntity.PermissionId);
            p.Add(string.Concat("@", nameof(PermissionEntity.Name)), permissionEntity.Name);
          
            p.Add(string.Concat("@", nameof(PermissionEntity.DateCreated)), permissionEntity.DateCreated);
            p.Add(string.Concat("@", nameof(PermissionEntity.DateModified)), permissionEntity.DateModified);

            if (permissionEntity.PermissionId <= 0)
            {
                sql = BuildInsertScript();
                
            }
            else
            {
                sql = BuildUpdateScript();
               
            }

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                    {
                        if (permissionEntity.PermissionId <= 0)
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
                        else
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated;
                    }
                    else
                    {
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;
                    }

                    transaction.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }
         
        public async Task<List<PermissionEntity>> GetPermission()
        {
            var p = new DynamicParameters();
         

            var sql = BuildGetAllPermission();
            
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<PermissionEntity>(sql, p);

                return dbModel.ToList();
            }
        }
         
        public async Task<int> DeletePermission(int permissionId)
        {
            var p = new DynamicParameters();

             p.Add(string.Concat("@", nameof(PermissionEntity.IsDeleted)), 1);

            // {nameof(UserEntity.PhoneNumber)} = {Constants.QueryParameters.PhoneNumber}

            var sql = BuildDeleteScript();

            //var sql = $"SELECT * FROM {Constants.DatabaseTables.Users} WHERE {nameof(UserEntity.UserKeyId)} = @{nameof(UserEntity.UserKeyId)} " +
            //            $" AND  {nameof(UserEntity.IsActivated)} = @{nameof(UserEntity.IsActivated)} ";

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);

                    ret = ret > 0
                        ? GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated
                        : GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;

                    transaction.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }




    }
}

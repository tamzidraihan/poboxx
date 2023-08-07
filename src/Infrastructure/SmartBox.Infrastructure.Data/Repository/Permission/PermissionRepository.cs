using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Permission;
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
    public class PermissionRepository : GenericRepositoryBase<PermissionEntity, PermissionRepository>, IPermissionRepository
    {
        public PermissionRepository(IDatabaseHelper databaseHelper, ILogger<PermissionRepository> logger) : base(databaseHelper,
        logger)
        {

        }


        string BuildInsertScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.Permission));

            sql.Append(" (");
            sql.Append(nameof(PermissionEntity.PermissionId));
            sql.Append($", {nameof(PermissionEntity.Name)}");
            sql.Append($", {nameof(PermissionEntity.DateCreated)}");
            sql.Append($", {nameof(PermissionEntity.DateModified)}");
            sql.Append($", {nameof(PermissionEntity.IsDeleted)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"@{nameof(PermissionEntity.PermissionId)}");
            sql.Append($", @{nameof(PermissionEntity.Name)}");
            sql.Append($", @{nameof(PermissionEntity.DateCreated)}");
            sql.Append($", @{nameof(PermissionEntity.DateModified)}");
            sql.Append($", @{nameof(PermissionEntity.IsDeleted)}");


            sql.Append(")");

            return sql.ToString();
        }

        string BuildUpdateScript(PermissionEntity permissionEntity)
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.Permission} SET ");

            sql.Append(string.Concat(" ", nameof(permissionEntity.Name), " = ",
                          "@", nameof(permissionEntity.Name)));

            sql.Append(string.Concat(" WHERE ", nameof(permissionEntity.PermissionId), " = ", "@", nameof(permissionEntity.PermissionId)));

            return sql.ToString();
        }

        string BuildDeleteScript(PermissionEntity permissionEntity)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.Permission));

            sql.Append(string.Concat(" WHERE ", nameof(permissionEntity.PermissionId), " = ", "@", nameof(permissionEntity.PermissionId)));

            return sql.ToString();
        }

        string BuildGetAllPermissionssScript()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Permission}");
            sql.Append($" WHERE {nameof(PermissionEntity.IsDeleted)} = 0");

            return sql.ToString();
        }
        string BuildGetScript()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Permission}");
            sql.Append($" WHERE {nameof(PermissionEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");
            sql.Append($" AND {nameof(PermissionEntity.PermissionId)} = {GlobalDatabaseConstants.QueryParameters.PermissionId}");

            return sql.ToString();
        }
        string BuildScriptGetByPermissionName()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Permission}");
            sql.Append($" WHERE {nameof(PermissionEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");
            sql.Append($" AND {nameof(PermissionEntity.Name)} = {GlobalDatabaseConstants.QueryParameters.Name}");

            return sql.ToString();
        }

        public async Task<int> Save(PermissionEntity permissionEntity)
        {
            var p = new DynamicParameters();
            string sql;
            bool isInsert = true;
            p.Add(string.Concat("@", nameof(PermissionEntity.PermissionId)), permissionEntity.PermissionId);
            p.Add(string.Concat("@", nameof(PermissionEntity.Name)), permissionEntity.Name);

            p.Add(string.Concat("@", nameof(PermissionEntity.IsDeleted)), permissionEntity.IsDeleted);

            if (permissionEntity.PermissionId == 0)
            {
                p.Add(string.Concat("@", nameof(permissionEntity.DateCreated)), DateTime.Now);
                sql = BuildInsertScript();

            }
            else
            {
                p.Add(string.Concat("@", nameof(permissionEntity.DateModified)), DateTime.Now);
                sql = BuildUpdateScript(permissionEntity);
                isInsert = false;

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
                        if (isInsert)
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
                        else
                        {
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated;
                            isInsert = false;
                        }
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

        public async Task<List<PermissionEntity>> GetPermissions()
        {
            var p = new DynamicParameters();


            var sql = BuildGetAllPermissionssScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<PermissionEntity>(sql, p);

                return dbModel.ToList();
            }
        }

        public async Task<int> DeletePermission(int Id)
        {
            PermissionEntity entity = new PermissionEntity { PermissionId = Id };
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(entity.PermissionId)), Id);
            string sql = BuildDeleteScript(entity);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordDeleted;
                    else
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;


                    transaction.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);
                    if (e.Message.Contains("Cannot delete or update a parent row: a foreign key constraint fails"))
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.PermissionDeleteConstraints;
                    else
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

        public async Task<PermissionEntity> GetById(int Id)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(PermissionEntity.IsDeleted)), 0);
            p.Add(string.Concat("@", nameof(PermissionEntity.PermissionId)), Id);

            var sql = BuildGetScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<PermissionEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }

        public async Task<PermissionEntity> GetByName(string Name)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(PermissionEntity.IsDeleted)), 0);
            p.Add(GlobalDatabaseConstants.QueryParameters.Name, Name);

            var sql = BuildScriptGetByPermissionName();
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<PermissionEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }
    }
}

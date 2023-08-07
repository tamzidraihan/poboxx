using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.RolePermission;
using SmartBox.Business.Core.Models.RolePermission;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.RolePermission
{
    public class RolePermissionsRepository : GenericRepositoryBase<RolePermissionEntity, RolePermissionsRepository>, IRolePermissionsRepository
    {
        public RolePermissionsRepository(IDatabaseHelper databaseHelper, ILogger<RolePermissionsRepository> logger) : base(databaseHelper,
        logger)
        {

        }
        string BuildInsertScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.RolePermission));

            sql.Append(" (");
            sql.Append(nameof(RolePermissionEntity.RolePermissionId));
            sql.Append($", {nameof(RolePermissionEntity.PermissionId)}");
            sql.Append($", {nameof(RolePermissionEntity.RoleId)}");
            sql.Append($", {nameof(RolePermissionEntity.DateCreated)}");
            sql.Append($", {nameof(RolePermissionEntity.DateModified)}");
            sql.Append($", {nameof(RolePermissionEntity.IsDeleted)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"@{nameof(RolePermissionEntity.RolePermissionId)}");
            sql.Append($",@{nameof(RolePermissionEntity.PermissionId)}");
            sql.Append($",@{nameof(RolePermissionEntity.RoleId)}");
            sql.Append($", @{nameof(RolePermissionEntity.DateCreated)}");
            sql.Append($", @{nameof(RolePermissionEntity.DateModified)}");
            sql.Append($", @{nameof(RolePermissionEntity.IsDeleted)}");


            sql.Append(")");

            return sql.ToString();


        }

        string BuildUpdateScript(RolePermissionEntity rolePermissionEntity)
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.RolePermission} SET ");

            sql.Append(string.Concat("", nameof(rolePermissionEntity.PermissionId), " = ",
                    "@", nameof(rolePermissionEntity.PermissionId)));

            sql.Append(string.Concat(", ", nameof(rolePermissionEntity.RoleId), " = ",
                   "@", nameof(rolePermissionEntity.RoleId)));

            sql.Append(string.Concat(", ", nameof(rolePermissionEntity.DateModified), " = ",
                      "@", nameof(rolePermissionEntity.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(rolePermissionEntity.RolePermissionId), " = ", "@", nameof(rolePermissionEntity.RolePermissionId)));

            return sql.ToString();


        }

        string BuildDeleteScript(RolePermissionEntity rolePermissionEntity)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.RolePermission));

            sql.Append(string.Concat(" WHERE ", nameof(rolePermissionEntity.RolePermissionId), " = ", "@", nameof(rolePermissionEntity.RolePermissionId)));
            return sql.ToString();


        }

        string BuildGetAllRolePermissions(int? roleId = null)
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.Views.RolePermission}");

            if (roleId.HasValue && roleId.Value > 0)
                sql.Append($" WHERE {nameof(RolePermissionEntity.RoleId)} = @{nameof(RolePermissionEntity.RoleId)}");

            return sql.ToString();



        }
        string BuildGetScript()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.RolePermission}");
            sql.Append($" WHERE {nameof(RolePermissionEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");
            sql.Append($" AND {nameof(RolePermissionEntity.RolePermissionId)} = {GlobalDatabaseConstants.QueryParameters.RolePermissionId}");

            return sql.ToString();
        }


        public async Task<int> Save(RolePermissionEntity rolePermission)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(RolePermissionEntity.RolePermissionId)), rolePermission.RolePermissionId);
            p.Add(string.Concat("@", nameof(RolePermissionEntity.PermissionId)), rolePermission.PermissionId);
            p.Add(string.Concat("@", nameof(RolePermissionEntity.RoleId)), rolePermission.RoleId);

            p.Add(string.Concat("@", nameof(RolePermissionEntity.IsDeleted)), rolePermission.IsDeleted);

            if (rolePermission.RolePermissionId <= 0)
            {
                p.Add(string.Concat("@", nameof(rolePermission.DateCreated)), DateTime.Now);
                sql = BuildInsertScript();

            }
            else
            {
                p.Add(string.Concat("@", nameof(rolePermission.DateModified)), DateTime.Now);
                sql = BuildUpdateScript(rolePermission);

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
                        if (rolePermission.RolePermissionId <= 0)
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

        public async Task<List<RolePermissionEntity>> GetRolePermissions(int? roleId = null)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(RolePermissionEntity.RoleId)), roleId);

            var sql = BuildGetAllRolePermissions(roleId);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<RolePermissionEntity>(sql, p);

                return dbModel.ToList();
            }
        }


        public async Task<int> DeleteRolePermission(int rolePermissionId)
        {
            RolePermissionEntity entity = new RolePermissionEntity { RolePermissionId = rolePermissionId };
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(entity.RolePermissionId)), rolePermissionId);
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
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

        public async Task<RolePermissionEntity> GetById(int Id)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(RolePermissionEntity.IsDeleted)), 0);
            p.Add(string.Concat("@", nameof(RolePermissionEntity.RolePermissionId)), Id);

            var sql = BuildGetScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<RolePermissionEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }
    }
}

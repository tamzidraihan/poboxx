using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.User;
using SmartBox.Business.Core.Entities.Role;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBox.Infrastructure.Data.Repository.Role;
using System.Data;

namespace SmartBox.Infrastructure.Data.Repository.Role
{
    public class RoleRepository : GenericRepositoryBase<RoleEntity, RoleRepository>, IRoleRepository
    {
        public RoleRepository(IDatabaseHelper databaseHelper, ILogger<RoleRepository> logger) : base(databaseHelper,
          logger)
        {

        }
        string BuildInsertScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.Role));

            sql.Append(" (");
            sql.Append(nameof(RoleEntity.RoleId));
            sql.Append($", {nameof(RoleEntity.RoleName)}");
            sql.Append($", {nameof(RoleEntity.DateCreated)}");
            sql.Append($", {nameof(RoleEntity.DateModified)}");
            sql.Append($", {nameof(RoleEntity.IsDeleted)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append(nameof(RoleEntity.RoleId));
            sql.Append($", @{nameof(RoleEntity.RoleName)}");
            sql.Append($", @{nameof(RoleEntity.DateCreated)}");
            sql.Append($", @{nameof(RoleEntity.DateModified)}");
            sql.Append($", @{nameof(RoleEntity.IsDeleted)}");
      
         
            sql.Append(")");

            return sql.ToString();
        }

        string BuildUpdateScript(RoleEntity roleEntity)
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.Role} SET ");
           
            sql.Append(string.Concat(" ", nameof(roleEntity.RoleName), " = ",
                          "@", nameof(roleEntity.RoleName)));

            sql.Append(string.Concat(", DateModified=Now() WHERE ", nameof(roleEntity.RoleId), " = ", "@", nameof(roleEntity.RoleId)));

            return sql.ToString();
        }

        string BuildDeleteScript(RoleEntity roleEntity)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.Role));

            sql.Append(string.Concat(" WHERE ", nameof(roleEntity.RoleId), " = ", "@", nameof(roleEntity.RoleId)));

            return sql.ToString();

        }

        string BuildGetAllRolesScript()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Role}");
            sql.Append($" WHERE {nameof(RoleEntity.IsDeleted)} = 0");

            return sql.ToString();
        }
        string BuildGetScript()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Role}");
            sql.Append($" WHERE {nameof(RoleEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");
            sql.Append($" AND {nameof(RoleEntity.RoleId)} = {GlobalDatabaseConstants.QueryParameters.RoleId}");

            return sql.ToString();
        }
        string BuildScriptGetByRoleName()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Role}");
            sql.Append($" WHERE {nameof(RoleEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");
            sql.Append($" AND {nameof(RoleEntity.RoleName)} = {GlobalDatabaseConstants.QueryParameters.RoleName}");

            return sql.ToString();
        }

        public async Task<int> Save(RoleEntity roleEntity)
        {
            var p = new DynamicParameters();
            string sql;
            bool isInsert = true;
            p.Add(string.Concat("@", nameof(RoleEntity.RoleId)), roleEntity.RoleId);
            p.Add(string.Concat("@", nameof(RoleEntity.RoleName)), roleEntity.RoleName);
           
            p.Add(string.Concat("@", nameof(RoleEntity.IsDeleted)), roleEntity.IsDeleted);

            if (roleEntity.RoleId == 0)
            {
                p.Add(string.Concat("@", nameof(roleEntity.DateCreated)), DateTime.Now);
                sql = BuildInsertScript();
                
            }
            else
            {
                p.Add(string.Concat("@", nameof(roleEntity.DateModified)), DateTime.Now);
                sql = BuildUpdateScript(roleEntity);
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
         
        public async Task<List<RoleEntity>> GetRoles()
        {
            var p = new DynamicParameters();
         

            var sql = BuildGetAllRolesScript();
            
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<RoleEntity>(sql, p);

                return dbModel.ToList();
            }
        }
         
        public async Task<int> DeleteRole(int roleId)
        {
            RoleEntity entity = new RoleEntity { RoleId = roleId };
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(entity.RoleId)), roleId);
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
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.RoleDeleteConstraints;
                    else
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

        public async Task<RoleEntity> GetById(int Id)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(RoleEntity.IsDeleted)), 0);
            p.Add(string.Concat("@", nameof(RoleEntity.RoleId)), Id);

            var sql = BuildGetScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<RoleEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }

        public async Task<RoleEntity> GetByName(string roleName)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(RoleEntity.IsDeleted)), 0);
            p.Add(GlobalDatabaseConstants.QueryParameters.RoleName, roleName);

            var sql = BuildScriptGetByRoleName();
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<RoleEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }
    }
}

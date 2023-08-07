using Dapper;
using Microsoft.Extensions.Logging;
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

namespace SmartBox.Infrastructure.Data.Repository.UserRole
{
    public class UserRoleRepository : GenericRepositoryBase<UserRoleEntity, UserRoleRepository>, IUserRoleRepository
    {
        public UserRoleRepository(IDatabaseHelper databaseHelper, ILogger<UserRoleRepository> logger) : base(databaseHelper,
          logger)
        {

        }
        string BuildInsertScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.UserRole));

            sql.Append(" (");
            sql.Append(nameof(UserRoleEntity.UserRoleId));
            sql.Append($", {nameof(UserRoleEntity.RoleId)}");
            sql.Append($", {nameof(UserRoleEntity.UserId)}");
            sql.Append($", {nameof(UserRoleEntity.DateCreated)}");
            sql.Append($", {nameof(UserRoleEntity.DateModified)}");
            sql.Append($", {nameof(UserRoleEntity.IsDeleted)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"@{nameof(UserRoleEntity.UserRoleId)}");
            sql.Append($",@{ nameof(UserRoleEntity.RoleId)}");
            sql.Append($",@{nameof(UserRoleEntity.UserId)}");
            sql.Append($", @{nameof(UserRoleEntity.DateCreated)}");
            sql.Append($", @{nameof(UserRoleEntity.DateModified)}");
             sql.Append($", @{nameof(UserRoleEntity.IsDeleted)}");


            sql.Append(")");

            return sql.ToString();

          
        }

        string BuildUpdateScript(UserRoleEntity userRoleEntity)
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.UserRole} SET ");

            sql.Append(string.Concat(" ", nameof(userRoleEntity.RoleId), " = ",
                   "@", nameof(userRoleEntity.RoleId)));

            sql.Append(string.Concat(", ", nameof(userRoleEntity.UserId), " = ",
                    "@", nameof(userRoleEntity.UserId)));

            sql.Append(string.Concat(", ", nameof(userRoleEntity.DateModified), " = ",
                      "@", nameof(userRoleEntity.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(userRoleEntity.UserRoleId), " = ", "@", nameof(userRoleEntity.UserRoleId)));

            return sql.ToString();

            
        }

        string BuildDeleteScript(UserRoleEntity userRoleEntity)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.UserRole));

            sql.Append(string.Concat(" WHERE ", nameof(userRoleEntity.UserRoleId), " = ", "@", nameof(userRoleEntity.UserRoleId)));

            return sql.ToString();


        }

        string BuildGetAllUserRoles(int userId)
        {
            var sql = new StringBuilder($"SELECT UserRoleId, ur.RoleId, ur.UserId, Username, RoleName, ur.DateCreated, ur.DateModified FROM {GlobalDatabaseConstants.DatabaseTables.UserRole} ur " );
            sql.Append("JOIN admin_users au on ur.UserId = au.AdminUserId ");
            sql.Append("JOIN roles r on ur.RoleId = r.RoleId ");
            sql.Append($" WHERE  ur.isDeleted = 0 ");
            sql.Append($" AND  ur.UserId = {GlobalDatabaseConstants.QueryParameters.UserId}");
            return sql.ToString();
             
        }
        string BuildGetScript()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.UserRole}");
            sql.Append($" WHERE {nameof(UserRoleEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");
            sql.Append($" AND {nameof(UserRoleEntity.UserRoleId)} = {GlobalDatabaseConstants.QueryParameters.UserRoleId}");

            return sql.ToString();
        }


        public async Task<int> Save(UserRoleEntity userRoleEntity)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(UserRoleEntity.UserRoleId)), userRoleEntity.UserRoleId);
            p.Add(string.Concat("@", nameof(UserRoleEntity.RoleId)), userRoleEntity.RoleId);
            p.Add(string.Concat("@", nameof(UserRoleEntity.UserId)), userRoleEntity.UserId);
   
            p.Add(string.Concat("@", nameof(UserRoleEntity.IsDeleted)), userRoleEntity.IsDeleted);

            if (userRoleEntity.UserRoleId <= 0)
            {
                p.Add(string.Concat("@", nameof(userRoleEntity.DateCreated)), DateTime.Now);
                sql = BuildInsertScript();
                
            }
            else
            {
                p.Add(string.Concat("@", nameof(userRoleEntity.DateModified)), DateTime.Now);
                sql = BuildUpdateScript(userRoleEntity);
               
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
                        if (userRoleEntity.UserRoleId <= 0)
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
         
        public async Task<List<UserRoleEntity>> GetUserRoles(int userId)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(UserRoleEntity.UserId)), userId);

            var sql = BuildGetAllUserRoles(userId);
            
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<UserRoleEntity>(sql, p);

                return dbModel.ToList();
            }
        }
        
         
        public async Task<int> DeleteUserRole(int userRoleId)
        {
            UserRoleEntity entity = new UserRoleEntity { UserRoleId = userRoleId };
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(entity.UserRoleId)), userRoleId);
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

        public async Task<UserRoleEntity> GetById(int Id)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(UserRoleEntity.IsDeleted)), 0);
            p.Add(string.Concat("@", nameof(UserRoleEntity.UserRoleId)), Id);

            var sql = BuildGetScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<UserRoleEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }
    }
}

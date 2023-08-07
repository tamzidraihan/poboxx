using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.CompanyUser;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.CompanyUser
{
    public class CompanyUserRepository : GenericRepositoryBase<CompanyUserEntity, CompanyUserRepository>, ICompanyUserRepository
    {
        public CompanyUserRepository(IDatabaseHelper databaseHelper, ILogger<CompanyUserRepository> logger) : base(databaseHelper, logger)
        {
        }

        public async Task<List<CompanyUserEntity>> GetCompanyUser(string userKeyId = null, string username = null,
                                                            string email = null, bool? isDeleted = null,
                                                            bool? isActivated = null, bool? isSystemGenerated = null)
        {
            var builder = new SqlBuilder();
            DynamicParameters p = new();
            builder.Select("*");

            if (userKeyId.HasText())
            {
                p.Add(GlobalDatabaseConstants.QueryParameters.UserKeyId, userKeyId, DbType.String, ParameterDirection.Input);
                builder.Where(nameof(CompanyUserEntity.UserKeyId) + " = " + GlobalDatabaseConstants.QueryParameters.UserKeyId);
            }
            if (username.HasText())
            {
                p.Add(GlobalDatabaseConstants.QueryParameters.Username, username, DbType.String, ParameterDirection.Input);
                builder.Where(nameof(CompanyUserEntity.Username) + " = " + GlobalDatabaseConstants.QueryParameters.Username);
            }
            if (email.HasText())
            {
                p.Add(GlobalDatabaseConstants.QueryParameters.Email, email, DbType.String, ParameterDirection.Input);
                builder.Where(nameof(CompanyUserEntity.Email) + " = " + GlobalDatabaseConstants.QueryParameters.Email);
            }
            if (isDeleted.HasValue)
            {
                p.Add(GlobalDatabaseConstants.QueryParameters.IsDeleted, isDeleted, DbType.Boolean, ParameterDirection.Input);
                builder.Where($"{nameof(CompanyUserEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");
            }
            if (isActivated.HasValue)
            {
                p.Add(GlobalDatabaseConstants.QueryParameters.IsActivated, isActivated, DbType.Boolean, ParameterDirection.Input);
                builder.Where($"{nameof(CompanyUserEntity.IsActivated)} = {GlobalDatabaseConstants.QueryParameters.IsActivated}");
            }
            if (isSystemGenerated.HasValue)
            {
                p.Add(GlobalDatabaseConstants.QueryParameters.IsSystemGenerated, isSystemGenerated, DbType.Boolean, ParameterDirection.Input);
                builder.Where($"{nameof(CompanyUserEntity.IsSystemGenerated)} = {GlobalDatabaseConstants.QueryParameters.IsSystemGenerated}");
            }

            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.DatabaseTables.CompanyUsers} /**where**/ ");

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<CompanyUserEntity>(builderTemplate.RawSql, p);
                return dbModel.ToList();
            }
        }
        public async Task<int> Delete(int id)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(CompanyUserEntity.CompanyUserId)), id);
            string sql = BuildDeleteCommand();

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
        string BuildDeleteCommand()
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.CompanyUsers));

            sql.Append(string.Concat(" WHERE ", nameof(CompanyUserEntity.CompanyUserId), " = ", "@", nameof(CompanyUserEntity.CompanyUserId)));

            return sql.ToString();

        }
        public int GetLastIdentity()
        {

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();

                try
                {
                    var ret = conn.ExecuteScalar<int>($"SELECT {nameof(CompanyUserEntity.CompanyUserId)} FROM {GlobalDatabaseConstants.DatabaseTables.CompanyUsers} ORDER BY {nameof(CompanyUserEntity.CompanyUserId)} DESC LIMIT 1");

                    return ret;
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error on fetching the last user inserted id. Error: {e.Message}");
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

        public async Task<int> SaveCompanyUser(CompanyUserEntity companyUserEntity, bool isInsert)
        {
            var p = new DynamicParameters();
            string queryValues;
            string sql;

            List<string> columns = new List<string> {
             nameof(CompanyUserEntity.FirstName),
             nameof(CompanyUserEntity.LastName),
             nameof(CompanyUserEntity.Email),
             nameof(CompanyUserEntity.IsDeleted),
             nameof(CompanyUserEntity.IsAdmin),
             nameof(CompanyUserEntity.CompanyId),
            };

            p.Add(string.Concat("@", nameof(CompanyUserEntity.Username)), companyUserEntity.Username);
            p.Add(string.Concat("@", nameof(CompanyUserEntity.CompanyId)), companyUserEntity.CompanyId);
            p.Add(string.Concat("@", nameof(CompanyUserEntity.FirstName)), companyUserEntity.FirstName);
            p.Add(string.Concat("@", nameof(CompanyUserEntity.LastName)), companyUserEntity.LastName);
            p.Add(string.Concat("@", nameof(CompanyUserEntity.Email)), companyUserEntity.Email);
            p.Add(string.Concat("@", nameof(CompanyUserEntity.IsDeleted)), companyUserEntity.IsDeleted);
            p.Add(string.Concat("@", nameof(CompanyUserEntity.IsAdmin)), companyUserEntity.IsAdmin);

            if (isInsert)
            {
                p.Add(string.Concat("@", nameof(CompanyUserEntity.Password)), companyUserEntity.Password);
                columns.Add(nameof(CompanyUserEntity.Password));

                p.Add(string.Concat("@", nameof(CompanyUserEntity.UserKeyId)), companyUserEntity.UserKeyId);
                columns.Add(nameof(CompanyUserEntity.UserKeyId));

                columns.Add(nameof(CompanyUserEntity.Username));

                queryValues = SharedServices.InsertQueryBuilder(columns);
                sql = string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.CompanyUsers, queryValues);
            }
            else
            {
                queryValues = SharedServices.UpdateQueryBuilder(columns);
                sql = $"UPDATE {GlobalDatabaseConstants.DatabaseTables.CompanyUsers} SET {queryValues} " +
                      $"WHERE {nameof(CompanyUserEntity.Username)}=@{nameof(CompanyUserEntity.Username)}";
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

        public async Task<int> ActivateCompanyUser(CompanyUserEntity companyUserEntity, bool? isSystemGenerated)
        {
            var p = new DynamicParameters();
            string queryValues;
            string sql;

            List<string> columns = new List<string> {
             nameof(CompanyUserEntity.IsActivated),
             nameof(CompanyUserEntity.Password)
            };

            p.Add(string.Concat("@", nameof(CompanyUserEntity.Username)), companyUserEntity.Username);
            p.Add(string.Concat("@", nameof(CompanyUserEntity.IsActivated)), companyUserEntity.IsActivated);
            p.Add(string.Concat("@", nameof(CompanyUserEntity.Password)), companyUserEntity.Password);

            if (isSystemGenerated != null && isSystemGenerated == true)
            {
                columns.Add(nameof(CompanyUserEntity.Username));
                columns.Add(nameof(CompanyUserEntity.FirstName));
                columns.Add(nameof(CompanyUserEntity.LastName));
                columns.Add(nameof(CompanyUserEntity.Email));

                p.Add(string.Concat("@", nameof(CompanyUserEntity.UserKeyId)), companyUserEntity.UserKeyId);
                p.Add(string.Concat("@", nameof(CompanyUserEntity.FirstName)), companyUserEntity.FirstName);
                p.Add(string.Concat("@", nameof(CompanyUserEntity.LastName)), companyUserEntity.LastName);
                p.Add(string.Concat("@", nameof(CompanyUserEntity.Email)), companyUserEntity.Email);

                queryValues = SharedServices.UpdateQueryBuilder(columns);
                sql = $"UPDATE {GlobalDatabaseConstants.DatabaseTables.CompanyUsers} SET {queryValues} " +
                      $"WHERE {nameof(CompanyUserEntity.UserKeyId)}=@{nameof(CompanyUserEntity.UserKeyId)}";
            }
            else
            {
                queryValues = SharedServices.UpdateQueryBuilder(columns);
                sql = $"UPDATE {GlobalDatabaseConstants.DatabaseTables.CompanyUsers} SET {queryValues} " +
                      $"WHERE {nameof(CompanyUserEntity.Username)}=@{nameof(CompanyUserEntity.Username)}";
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

        public async Task<int> UpdateCompanyUserPassword(string username, string password)
        {
            var p = new DynamicParameters();
            string queryValues;
            string sql;

            List<string> columns = new List<string> {
             nameof(CompanyUserEntity.Username),
             nameof(CompanyUserEntity.Password)
            };

            p.Add(string.Concat("@", nameof(CompanyUserEntity.Username)), username);
            p.Add(string.Concat("@", nameof(CompanyUserEntity.Password)), password);

            queryValues = SharedServices.UpdateQueryBuilder(columns);
            sql = $"UPDATE {GlobalDatabaseConstants.DatabaseTables.CompanyUsers} SET {queryValues} " +
                  $"WHERE {nameof(CompanyUserEntity.Username)}=@{nameof(CompanyUserEntity.Username)}";

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();
                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                    {
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

        public bool? IsSystemGenerated(bool isSystemGenerated, string userKeyId = null, string username = null)
        {
            var dbModel = new List<CompanyUserEntity>();

            if (userKeyId.HasText())
                dbModel = GetCompanyUser(userKeyId: userKeyId, isSystemGenerated: isSystemGenerated).Result;
            else
                dbModel = GetCompanyUser(username: username, isSystemGenerated: isSystemGenerated).Result;

            var model = dbModel.FirstOrDefault();
            if (model != null)
                return model.IsSystemGenerated;

            return null;
        }
        public async Task<int> ActivateDeactivate(int Id, int? isActive)
        {
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();

                try
                {
                    var sql = $"UPDATE {GlobalDatabaseConstants.DatabaseTables.CompanyUsers} SET isActivated = {isActive} " +
                              $"WHERE CompanyUserId= {Id}";

                    var ret = await conn.ExecuteAsync(sql);
                    if (ret > 0)
                    {
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated;
                    }
                    else
                    {
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;
                    }

                    return ret;
                }
                catch (Exception e)
                {
                
                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }
    }
}

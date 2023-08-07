using Dapper;
using DapperExtensions;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.ParentCompany;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.ParentCompany
{
    public class ParentCompanyRepository : GenericRepositoryBase<ParentCompanyEntity, ParentCompanyRepository>, IParentCompanyRepository
    {
        public ParentCompanyRepository(IDatabaseHelper databaseHelper, ILogger<ParentCompanyRepository> logger) : base(databaseHelper, logger)
        {
        }

        public async Task<int> GetLastIdentity()
        {
            var builder = new SqlBuilder();
            builder.Select(nameof(ParentCompanyEntity.ParentCompanyId));
            builder.OrderBy($"{nameof(ParentCompanyEntity.ParentCompanyId)} DESC");
            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.DatabaseTables.ParentCompany} /**orderby**/ LIMIT 1 ");

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();

                try
                {
                    var dbModel = await conn.QueryAsync<int>(builderTemplate.RawSql);
                    return dbModel.FirstOrDefault();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error on fetching the last parent company inserted id");
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

        public async Task<bool> CheckParentCompanyKeyId(string parentCompanyKeyId)
        {
            var builder = new SqlBuilder();
            DynamicParameters parameters = new();
            builder.Select("*");

            parameters.Add(GlobalDatabaseConstants.QueryParameters.ParentCompanyKeyId, parentCompanyKeyId, DbType.String, ParameterDirection.Input);
            builder.Where(nameof(ParentCompanyEntity.ParentCompanyKeyId) + " = " + GlobalDatabaseConstants.QueryParameters.ParentCompanyKeyId);

            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.DatabaseTables.ParentCompany} /**where**/ ");
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<ParentCompanyEntity>(builderTemplate.RawSql, parameters);
                return dbModel.AsEnumerable().Any();
            }
        }

        public async Task<bool> CheckParentCompanyId(int parentCompanyId)
        {
            var builder = new SqlBuilder();
            DynamicParameters parameters = new();
            builder.Select("*");

            parameters.Add(GlobalDatabaseConstants.QueryParameters.ParentCompanyId, parentCompanyId, DbType.Int32, ParameterDirection.Input);
            builder.Where(nameof(ParentCompanyEntity.ParentCompanyId) + " = " + GlobalDatabaseConstants.QueryParameters.ParentCompanyId);

            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.DatabaseTables.ParentCompany} /**where**/ ");
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<ParentCompanyEntity>(builderTemplate.RawSql, parameters);
                return dbModel.AsEnumerable().Any();
            }
        }

        public async Task<bool> CheckParentCompanyName(string parentCompanyName)
        {
            var builder = new SqlBuilder();
            DynamicParameters parameters = new();
            builder.Select("*");

            parameters.Add(GlobalDatabaseConstants.QueryParameters.ParentCompanyKeyId, parentCompanyName, DbType.String, ParameterDirection.Input);
            builder.Where(nameof(ParentCompanyEntity.ParentCompanyName) + " = " + GlobalDatabaseConstants.QueryParameters.ParentCompanyName);

            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.DatabaseTables.ParentCompany} /**where**/ LIMIT 1");
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<ParentCompanyEntity>(builderTemplate.RawSql, parameters);
                return dbModel.ToList().Any();
            }
        }

        public async Task<List<ParentCompanyEntity>> GetParentCompany(bool isAndOperator = true,string parentCompanyKeyId = null, 
                                                                      string parentCompanyName = null)
        {
            var builder = new SqlBuilder();
            DynamicParameters parameters = new();
            builder.Select("*");
            
            if (parentCompanyKeyId.HasText())
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.ParentCompanyKeyId, parentCompanyKeyId, DbType.String, ParameterDirection.Input);
                if(!isAndOperator)
                    builder.OrWhere(nameof(ParentCompanyEntity.ParentCompanyKeyId) + " = " + GlobalDatabaseConstants.QueryParameters.ParentCompanyKeyId);
                else
                    builder.Where(nameof(ParentCompanyEntity.ParentCompanyKeyId) + " = " + GlobalDatabaseConstants.QueryParameters.ParentCompanyKeyId);
            }

            if (parentCompanyName.HasText())
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.ParentCompanyName, "%" + parentCompanyName.ToLower() + "%", 
                              DbType.String, ParameterDirection.Input);
                
                if (!isAndOperator)
                    builder.OrWhere($"{nameof(ParentCompanyEntity.ParentCompanyName)} LIKE  {GlobalDatabaseConstants.QueryParameters.ParentCompanyName}");
                else
                    builder.Where($"{nameof(ParentCompanyEntity.ParentCompanyName)} LIKE  {GlobalDatabaseConstants.QueryParameters.ParentCompanyName}");
            }

            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.DatabaseTables.ParentCompany} /**where**/ ");
            
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<ParentCompanyEntity>(builderTemplate.RawSql, parameters);
                return dbModel.ToList();
            }
        }

        public async Task<int> SaveParentCompany(ParentCompanyEntity parentCompanyEntity, bool isInsert)
        {
            var p = new DynamicParameters();
            string queryValues;
            string sql;

            List<string> columns = new List<string> {
             nameof(ParentCompanyEntity.ParentCompanyName),
             nameof(ParentCompanyEntity.ParentCompanyLogo),
             nameof(ParentCompanyEntity.ParentCompanyAddress),
             nameof(ParentCompanyEntity.ParentCompanyContactNumber),
             nameof(ParentCompanyEntity.ParentCompanyContactPerson),
            };

            p.Add(string.Concat("@", nameof(ParentCompanyEntity.ParentCompanyKeyId)), parentCompanyEntity.ParentCompanyKeyId);
            p.Add(string.Concat("@", nameof(ParentCompanyEntity.ParentCompanyName)), parentCompanyEntity.ParentCompanyName);
            p.Add(string.Concat("@", nameof(ParentCompanyEntity.ParentCompanyLogo)), parentCompanyEntity.ParentCompanyLogo);
            p.Add(string.Concat("@", nameof(ParentCompanyEntity.ParentCompanyAddress)), parentCompanyEntity.ParentCompanyAddress);
            p.Add(string.Concat("@", nameof(ParentCompanyEntity.ParentCompanyContactNumber)), parentCompanyEntity.ParentCompanyContactNumber);
            p.Add(string.Concat("@", nameof(ParentCompanyEntity.ParentCompanyContactPerson)), parentCompanyEntity.ParentCompanyContactPerson);

            if (isInsert)
            {
                columns.Add(nameof(ParentCompanyEntity.ParentCompanyKeyId));
                queryValues = SharedServices.InsertQueryBuilder(columns);
                sql = string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.ParentCompany, queryValues);
            }
            else
            {
                queryValues = SharedServices.UpdateQueryBuilder(columns);
                sql = $"UPDATE { GlobalDatabaseConstants.DatabaseTables.ParentCompany} SET {queryValues} " +
                      $"WHERE {nameof(ParentCompanyEntity.ParentCompanyKeyId)}=@{nameof(ParentCompanyEntity.ParentCompanyKeyId)}";
            }

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();
                try
                {
                    var ret = await conn.ExecuteAsync(sql.ToString(),p, transaction);
                    
                    if (ret > 0)
                    {
                        if (isInsert)
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
                        else
                        {
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated;
                        }
                    }
                    else
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;
                
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

      

        public Task<List<ParentCompanyEntity>> GetParentCompany(string parentCompanyKeyId = null)
        {
            throw new NotImplementedException();
        }
    }
}

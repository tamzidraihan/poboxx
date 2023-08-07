using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Company;
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

namespace SmartBox.Infrastructure.Data.Repository.Company
{
    public class CompanyRepository : GenericRepositoryBase<CompanyEntity, CompanyRepository>, ICompanyRepository
    {
        public CompanyRepository(IDatabaseHelper databaseHelper, ILogger<CompanyRepository> logger) : base(databaseHelper, logger)
        {
        }

        string BuildActivationCompanyScript()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.Company, " SET "));

            sql.Append(string.Concat(" ", nameof(CompanyEntity.IsDeleted), " = ", "@", nameof(CompanyEntity.IsDeleted)));
            sql.Append(string.Concat(" WHERE ", nameof(CompanyEntity.CompanyKeyId), " = ", "@", nameof(CompanyEntity.CompanyKeyId)));

            return sql.ToString();
        }

        public async Task<List<CompanyLocationEntity>> GetCompany(string companyKeyId = null, int? companyId =null, bool isDeleted = false)
        {
            var builder = new SqlBuilder();
            DynamicParameters parameters = new();
            builder.Select("*");

            if (companyKeyId.HasText())
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.CompanyKeyId, companyKeyId, DbType.String, ParameterDirection.Input);
                builder.Where(nameof(CompanyEntity.CompanyKeyId) + " = " + GlobalDatabaseConstants.QueryParameters.CompanyKeyId);
            }

            if (companyId != null)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.CompanyId, companyId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(CompanyEntity.CompanyId) + " = " + GlobalDatabaseConstants.QueryParameters.CompanyId);
            }

            parameters.Add(GlobalDatabaseConstants.QueryParameters.IsDeleted, isDeleted, DbType.Boolean, ParameterDirection.Input);
            builder.Where($"{nameof(CompanyEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");

            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.Views.CompanyWithLocation} /**where**/ ");

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<CompanyLocationEntity>(builderTemplate.RawSql, parameters);
                return dbModel.ToList();
            }

            //var p = new DynamicParameters();

            //if (companyKeyId.HasText())
            //{
            //    p.Add(GlobalDatabaseConstants.QueryParameters.CompanyKeyId, companyKeyId);
            //}

            //if(currentActiveOnly)
            //    p.Add(GlobalDatabaseConstants.QueryParameters.IsActive, currentActiveOnly);

            //var sql = BuildSelectCompany(companyKeyId.HasText());


            //using (IDbConnection conn = this._databaseHelper.GetConnection())
            //{
            //    var dbModel = await conn.QueryAsync<CompanyEntity>(sql, p);

            //    return dbModel.ToList();
            //}
        }
        public async Task<int> GetLastIdentity()
        {
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();

                try
                {
                    var ret = await conn.ExecuteScalarAsync<int>($"SELECT {nameof(CompanyEntity.CompanyId)} FROM {GlobalDatabaseConstants.DatabaseTables.Company} ORDER BY {nameof(CompanyEntity.CompanyId)} DESC LIMIT 1");

                    return ret;
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error on fetching the last user inserted id. Error: {e.Message}");
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }
        public async Task<int> SaveCompany( CompanyEntity companyEntity, bool isInsert)
        {
            var p = new DynamicParameters();
            string queryValues;
            string sql;

            List<string> columns = new List<string> {
             nameof(CompanyEntity.ParentCompanyId),
             nameof(CompanyEntity.CompanyName),
             nameof(CompanyEntity.ContactNumber),
             nameof(CompanyEntity.CompanyLogo),
             nameof(CompanyEntity.ContactPerson),
             nameof(CompanyEntity.Address),

             nameof(CompanyEntity.Photo),
             nameof(CompanyEntity.Surname),
             nameof(CompanyEntity.FirstName),
             nameof(CompanyEntity.MiddleName),
             nameof(CompanyEntity.Age),
             nameof(CompanyEntity.ResidentialAddress),
             nameof(CompanyEntity.Email),
             nameof(CompanyEntity.MobileNumber),
             nameof(CompanyEntity.FaxNumber),
             nameof(CompanyEntity.MaritalStatus),
             nameof(CompanyEntity.Citizenship),
             nameof(CompanyEntity.TIN),
             nameof(CompanyEntity.SSSNo),
             nameof(CompanyEntity.DoB),
             nameof(CompanyEntity.PlaceOfBirth),
             nameof(CompanyEntity.BusinessName),
             nameof(CompanyEntity.NatureOfBusinessId),
             nameof(CompanyEntity.BankName),
             nameof(CompanyEntity.Branch),
             nameof(CompanyEntity.AccountName),
             nameof(CompanyEntity.AccountNumber),
             nameof(CompanyEntity.AccountType),
            };

            p.Add(string.Concat("@", nameof(CompanyEntity.ParentCompanyId)), companyEntity.ParentCompanyId);
            p.Add(string.Concat("@", nameof(CompanyEntity.CompanyKeyId)), companyEntity.CompanyKeyId);
            p.Add(string.Concat("@", nameof(CompanyEntity.CompanyName)), companyEntity.CompanyName);
            p.Add(string.Concat("@", nameof(CompanyEntity.ContactNumber)), companyEntity.ContactNumber);
            p.Add(string.Concat("@", nameof(CompanyEntity.CompanyLogo)), companyEntity.CompanyLogo);
            p.Add(string.Concat("@", nameof(CompanyEntity.ContactPerson)), companyEntity.ContactPerson);
            p.Add(string.Concat("@", nameof(CompanyEntity.Address)), companyEntity.Address);

            p.Add(string.Concat("@", nameof(CompanyEntity.Photo)), companyEntity.Photo);
            p.Add(string.Concat("@", nameof(CompanyEntity.Surname)), companyEntity.Surname);
            p.Add(string.Concat("@", nameof(CompanyEntity.FirstName)), companyEntity.FirstName);
            p.Add(string.Concat("@", nameof(CompanyEntity.MiddleName)), companyEntity.MiddleName);
            p.Add(string.Concat("@", nameof(CompanyEntity.MiddleName)), companyEntity.MiddleName);
            p.Add(string.Concat("@", nameof(CompanyEntity.Age)), companyEntity.Age);
            p.Add(string.Concat("@", nameof(CompanyEntity.ResidentialAddress)), companyEntity.ResidentialAddress);
            p.Add(string.Concat("@", nameof(CompanyEntity.Email)), companyEntity.Email);
            p.Add(string.Concat("@", nameof(CompanyEntity.MobileNumber)), companyEntity.MobileNumber);
            p.Add(string.Concat("@", nameof(CompanyEntity.FaxNumber)), companyEntity.FaxNumber);
            p.Add(string.Concat("@", nameof(CompanyEntity.MaritalStatus)), companyEntity.MaritalStatus);
            p.Add(string.Concat("@", nameof(CompanyEntity.Citizenship)), companyEntity.Citizenship);
            p.Add(string.Concat("@", nameof(CompanyEntity.TIN)), companyEntity.TIN);
            p.Add(string.Concat("@", nameof(CompanyEntity.SSSNo)), companyEntity.SSSNo);
            p.Add(string.Concat("@", nameof(CompanyEntity.DoB)), companyEntity.DoB);
            p.Add(string.Concat("@", nameof(CompanyEntity.PlaceOfBirth)), companyEntity.PlaceOfBirth);
            p.Add(string.Concat("@", nameof(CompanyEntity.BusinessName)), companyEntity.BusinessName);
            p.Add(string.Concat("@", nameof(CompanyEntity.NatureOfBusinessId)), companyEntity.NatureOfBusinessId);
            p.Add(string.Concat("@", nameof(CompanyEntity.BankName)), companyEntity.BankName);
            p.Add(string.Concat("@", nameof(CompanyEntity.Branch)), companyEntity.Branch);
            p.Add(string.Concat("@", nameof(CompanyEntity.AccountName)), companyEntity.AccountName);
            p.Add(string.Concat("@", nameof(CompanyEntity.AccountNumber)), companyEntity.AccountNumber);
            p.Add(string.Concat("@", nameof(CompanyEntity.AccountType)), companyEntity.AccountType);

            if (isInsert)
            {
                columns.Add(nameof(companyEntity.CompanyKeyId));
                queryValues = SharedServices.InsertQueryBuilder(columns);
                sql = string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.Company, queryValues);
            }
            else
            {
                queryValues = SharedServices.UpdateQueryBuilder(columns);
                sql = $"UPDATE { GlobalDatabaseConstants.DatabaseTables.Company} SET {queryValues} " +
                      $"WHERE {nameof(CompanyEntity.CompanyKeyId)}=@{nameof(CompanyEntity.CompanyKeyId)}";
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
        public async Task<int> SetCompanyActivation(string companyKeyId, bool isDeleted)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(CompanyEntity.CompanyKeyId)), companyKeyId);

            if (isDeleted)
                p.Add(string.Concat("@", nameof(CompanyEntity.IsDeleted)), 1);
            else
                p.Add(string.Concat("@", nameof(CompanyEntity.IsDeleted)), 0);

            sql = BuildActivationCompanyScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);

                    if (ret > 0)
                    {
                        if (isDeleted)
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordDeactivated;
                        else
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordActivated;
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
        public bool CheckCompanyIfExists(int companyId)
        {
            var dbmodel =  GetCompany(companyId:companyId).Result;
            if (dbmodel.Count == 0)
                return false;
            else
                return true;
        }
    }
}

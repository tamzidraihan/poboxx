using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Entities.Report;
using SmartBox.Business.Core.Models.Report;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Slapper.AutoMapper;

namespace SmartBox.Infrastructure.Data.Repository.Report
{
    public class CleanlinessReportRepository : GenericRepositoryBase<CleanlinessReportEntity, CleanlinessReportRepository>, ICleanlinessReportRepository
    {
        public CleanlinessReportRepository(IDatabaseHelper databaseHelper, ILogger<CleanlinessReportRepository> logger) : base(databaseHelper, logger)
        {
        }
        public async Task<List<CleanlinessReportEntity>> Get(int month,int year, int PageNumber, int PageSize, int? companyId = null,int? Status = null)
        {
            var p = new DynamicParameters();
            
            p.Add(GlobalDatabaseConstants.QueryParameters.Month, month);
            p.Add(GlobalDatabaseConstants.QueryParameters.Year, year); 
            p.Add(GlobalDatabaseConstants.QueryParameters.CompanyId, companyId);
            p.Add(GlobalDatabaseConstants.QueryParameters.StatusId, Status);
            var procedure = GlobalDatabaseConstants.StoredProcedures.GetCleanlinessReport;

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbmodel = await conn.QueryAsync<CleanlinessReportEntity>(procedure, p, commandType: CommandType.StoredProcedure);
                return dbmodel.ToList();
            }
        }


        public async Task<CleanlinessReportEntity> CheckIsExist(int companyId,int Month,int Year)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(CleanlinessReportEntity.CompanyId)), companyId);
            p.Add(string.Concat("@", nameof(CleanlinessReportEntity.Month)), Month);
            p.Add(string.Concat("@", nameof(CleanlinessReportEntity.DateSubmitted)), Year);
            

            var sql = BuildCheckIsExistCommand();


            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<CleanlinessReportEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }


        public async Task<int> Save(CleanlinessReportEntity model)
        {
            //Cleanliness Report Data
            var p = new DynamicParameters();
            string query;
           
            p.Add(string.Concat("@", nameof(model.Message)), model.Message);
            p.Add(string.Concat("@", nameof(model.IsSubmitted)), model.Status == CleanlinessReportStatus.Done ? true : false);
            p.Add(string.Concat("@", nameof(model.Month)), model.Month);
            p.Add(string.Concat("@", nameof(model.CabinetId)), model.CabinetId);
            p.Add(string.Concat("@", nameof(model.CompanyId)), model.CompanyId);
            p.Add(string.Concat("@", nameof(model.DateSubmitted)), model.Status == CleanlinessReportStatus.Done ? model.DateSubmitted : null);
            p.Add(string.Concat("@", nameof(model.Status)), model.Status);
            p.Add(string.Concat("@", nameof(model.FrontPhoto)), model.FrontPhoto);
            p.Add(string.Concat("@", nameof(model.LeftPhoto)), model.LeftPhoto);
            p.Add(string.Concat("@", nameof(model.RightPhoto)), model.RightPhoto);
            

            var isExist = await CheckIsExist(model.CompanyId, (int)model.Month, model.DateSubmitted.Value.Year);
            if(isExist != null)
            {
                p.Add(string.Concat("@", nameof(model.Id)), isExist.Id);
                p.Add(string.Concat("@", nameof(model.DateModified)), DateTime.Now); 
                query = BuildUpdateCleanlinessReportCommand(model); 
            }
            else
            {
                p.Add(string.Concat("@", nameof(model.DateCreated)), DateTime.Now);
                query = BuildInsertCleanlinessReportCommand();
            }

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteScalarAsync<int>(query, p, transaction);
                    var cleanlinessReportId = ret;
                    if (ret > 0 || isExist != null)
                    {
                        //Locker Pictures
                        if (isExist != null)
                        { 
                            var pa = new DynamicParameters();
                            pa.Add(string.Concat("@", nameof(LockerPictureEntity.CleanlinessReportId)), isExist.Id);
                            string sql = BuildDeleteCommandForLockerPictures();
                            var childRet = await conn.ExecuteAsync(sql, pa, transaction);

                            foreach (var item in model.LockerPictures)
                            {
                                string query1;
                                var par = new DynamicParameters();
                                par.Add(string.Concat("@", nameof(LockerPictureEntity.LockerNumber)), item.LockerNumber);
                                par.Add(string.Concat("@", nameof(LockerPictureEntity.InsidePhoto)), item.InsidePhoto);
                                par.Add(string.Concat("@", nameof(LockerPictureEntity.OutsidePhoto)), item.OutsidePhoto);
                                par.Add(string.Concat("@", nameof(LockerPictureEntity.CleanlinessReportId)), isExist.Id);
                                query1 = BuildInsertLockerPictureCommand();
                                await conn.ExecuteAsync(query1, par, transaction);
                            }
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated;
                        }
                        else
                        {
                            foreach (var item in model.LockerPictures)
                            {
                                string query2;
                                var para = new DynamicParameters();
                                para.Add(string.Concat("@", nameof(LockerPictureEntity.LockerNumber)), item.LockerNumber);
                                para.Add(string.Concat("@", nameof(LockerPictureEntity.InsidePhoto)), item.InsidePhoto);
                                para.Add(string.Concat("@", nameof(LockerPictureEntity.OutsidePhoto)), item.OutsidePhoto);
                                para.Add(string.Concat("@", nameof(LockerPictureEntity.CleanlinessReportId)), cleanlinessReportId);
                                query2 = BuildInsertLockerPictureCommand();
                                await conn.ExecuteAsync(query2, para, transaction);
                            }
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
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
                    if (!string.IsNullOrEmpty(e.Message) && e.Message.Contains("foreign key"))
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.InvalidForeignId;
                    else
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }

        }
        string BuildGetCommand()
        {
            return string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.CleanlinessReport);
        }

        string BuildCheckIsExistCommand()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.CleanlinessReport}");
            sql.Append($" WHERE {nameof(CleanlinessReportEntity.CompanyId)} = {GlobalDatabaseConstants.QueryParameters.CompanyId}");
            sql.Append($" AND {nameof(CleanlinessReportEntity.Month)} = @month");
            sql.Append($" AND (YEAR({nameof(CleanlinessReportEntity.DateSubmitted)}) = @dateSubmitted OR {nameof(CleanlinessReportEntity.DateSubmitted)} IS NULL)");

            return sql.ToString();
        }



        string BuildInsertCleanlinessReportCommand()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.CleanlinessReport));

            sql.Append(" (");
            sql.Append(nameof(CleanlinessReportEntity.IsSubmitted));
            sql.Append($", {nameof(CleanlinessReportEntity.Message)}");
            sql.Append($", {nameof(CleanlinessReportEntity.Status)}");
            sql.Append($", {nameof(CleanlinessReportEntity.FrontPhoto)}");
            sql.Append($", {nameof(CleanlinessReportEntity.LeftPhoto)}");
            sql.Append($", {nameof(CleanlinessReportEntity.RightPhoto)}");
            sql.Append($", {nameof(CleanlinessReportEntity.CabinetId)}");
            sql.Append($", {nameof(CleanlinessReportEntity.CompanyId)}");
            sql.Append($", {nameof(CleanlinessReportEntity.DateSubmitted)}");
            sql.Append($", {nameof(CleanlinessReportEntity.Month)}");
            sql.Append($", {nameof(CleanlinessReportEntity.DateCreated)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(CleanlinessReportEntity.IsSubmitted)}");
            sql.Append($", @{nameof(CleanlinessReportEntity.Message)}");
            sql.Append($", @{nameof(CleanlinessReportEntity.Status)}");
            sql.Append($", @{nameof(CleanlinessReportEntity.FrontPhoto)}");
            sql.Append($", @{nameof(CleanlinessReportEntity.LeftPhoto)}");
            sql.Append($", @{nameof(CleanlinessReportEntity.RightPhoto)}");
            sql.Append($", @{nameof(CleanlinessReportEntity.CabinetId)}");
            sql.Append($", @{nameof(CleanlinessReportEntity.CompanyId)}");
            sql.Append($", @{nameof(CleanlinessReportEntity.DateSubmitted)}");
            sql.Append($", @{nameof(CleanlinessReportEntity.Month)}");
            sql.Append($", @{nameof(CleanlinessReportEntity.DateCreated)}");

            sql.Append(");SELECT LAST_INSERT_ID();");

            return sql.ToString();
        }
        string BuildUpdateCleanlinessReportCommand(CleanlinessReportEntity model)
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.CleanlinessReport} SET ");

            sql.Append(string.Concat(" ", nameof(model.CompanyId), " = ",
                  "@", nameof(model.CompanyId)));

            sql.Append(string.Concat(", ", nameof(model.CabinetId), " = ",
                   "@", nameof(model.CabinetId)));

            sql.Append(string.Concat(", ", nameof(model.FrontPhoto), " = ",
                  "@", nameof(model.FrontPhoto)));

            
            sql.Append(string.Concat(", ", nameof(model.LeftPhoto), " = ",
                  "@", nameof(model.LeftPhoto)));

            
            sql.Append(string.Concat(", ", nameof(model.RightPhoto), " = ",
                  "@", nameof(model.RightPhoto)));

            
            sql.Append(string.Concat(", ", nameof(model.Message), " = ",
                  "@", nameof(model.Message)));  

            sql.Append(string.Concat(", ", nameof(model.DateSubmitted), " = ",
                  "@", nameof(model.DateSubmitted)));

            sql.Append(string.Concat(", ", nameof(model.IsSubmitted), " = ",
              "@", nameof(model.IsSubmitted)));

            sql.Append(string.Concat(", ", nameof(model.Month), " = ",
                 "@", nameof(model.Month)));

            sql.Append(string.Concat(", ", nameof(model.Status), " = ",
                 "@", nameof(model.Status)));

            sql.Append(string.Concat(", ", nameof(model.DateModified), " = ",
                  "@", nameof(model.DateModified)));


            sql.Append(string.Concat(" WHERE ", nameof(model.Id), " = ", "@", nameof(model.Id)));

            return sql.ToString();


        }
        string BuildInsertLockerPictureCommand()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.LockerPictures));

            sql.Append(" (");
            sql.Append(nameof(LockerPictureEntity.CleanlinessReportId));
            sql.Append($", {nameof(LockerPictureEntity.LockerNumber)}");
            sql.Append($", {nameof(LockerPictureEntity.InsidePhoto)}");
            sql.Append($", {nameof(LockerPictureEntity.OutsidePhoto)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(LockerPictureEntity.CleanlinessReportId)}");
            sql.Append($", @{nameof(LockerPictureEntity.LockerNumber)}");
            sql.Append($", @{nameof(LockerPictureEntity.InsidePhoto)}");
            sql.Append($", @{nameof(LockerPictureEntity.OutsidePhoto)}");


            sql.Append(");");

            return sql.ToString();
        }
        string BuildUpdateLockerPicturesCommand(LockerPictureEntity model)
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.CleanlinessReport} SET ");

            sql.Append(string.Concat(" ", nameof(model.LockerNumber), " = ",
                  "@", nameof(model.LockerNumber)));

            sql.Append(string.Concat(", ", nameof(model.InsidePhoto), " = ",
                   "@", nameof(model.InsidePhoto)));

            sql.Append(string.Concat(", ", nameof(model.OutsidePhoto), " = ",
                  "@", nameof(model.OutsidePhoto)));


            sql.Append(string.Concat(", ", nameof(model.CleanlinessReportId), " = ",
                  "@", nameof(model.CleanlinessReportId)));
             

            sql.Append(string.Concat(" WHERE ", nameof(model.Id), " = ", "@", nameof(model.Id)));

            return sql.ToString();


        }
        public async Task<int> Delete(int id)
        {

            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(LockerPictureEntity.CleanlinessReportId)), id);
            string sql = BuildDeleteCommandForLockerPictures();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                    {
                        p = new DynamicParameters();
                        p.Add(string.Concat("@", nameof(CleanlinessReportEntity.Id)), id);
                        sql = BuildDeleteCommandForCleanlinessReport();
                        ret = await conn.ExecuteAsync(sql, p, transaction);
                        if (ret > 0)
                        {
                            transaction.Commit();
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordDeleted;
                        }

                        else
                        {
                            transaction.Rollback();
                            ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                        }
                    }

                    else
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;



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
        string BuildDeleteCommandForCleanlinessReport()
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.CleanlinessReport));

            sql.Append(string.Concat(" WHERE ", nameof(CleanlinessReportEntity.Id), " = ", "@", nameof(CleanlinessReportEntity.Id)));

            return sql.ToString();

        }
        string BuildDeleteCommandForLockerPictures()
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.LockerPictures));

            sql.Append(string.Concat(" WHERE ", nameof(LockerPictureEntity.CleanlinessReportId), " = ", "@", nameof(LockerPictureEntity.CleanlinessReportId)));

            return sql.ToString();

        }
        public async Task<List<UnsubmittedMaintenanceReportModel>> GetUnsubmittedMaintenanceReport(int month)
        {
            var command = GlobalDatabaseConstants.StoredProcedures.GetUnsubmittedCleanlinessReport;
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(CleanlinessReportEntity.Month).ToLowerInvariant()), month);
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<UnsubmittedMaintenanceReportModel>(command, p, commandType: CommandType.StoredProcedure)).ToList();
            }
        }

    }
}

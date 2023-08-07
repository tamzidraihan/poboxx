using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.FranchiseFeedbackAnswer;
using SmartBox.Business.Core.Entities.FranchiseFeedbackQuestion;
using SmartBox.Business.Core.Models.FranchiseFeedbackAnswer;
using SmartBox.Business.Core.Models.FranchiseFeedbackQuestion;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.FranchiseFeedbackQuestion
{

    public class FranchiseFeedbackRepository : GenericRepositoryBase<FranchiseFeedbackQuestionEntity, FranchiseFeedbackRepository>, IFranchiseFeedbackRepository
    {
        public FranchiseFeedbackRepository(IDatabaseHelper databaseHelper, ILogger<FranchiseFeedbackRepository> logger) : base(databaseHelper,
          logger)
        {

        }





        string BuildInsertScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.FranchiseFeedbackQuestion));



            sql.Append(" (");
            sql.Append(nameof(FranchiseFeedbackQuestionEntity.Id));
            sql.Append($", {nameof(FranchiseFeedbackQuestionEntity.Question)}");

            sql.Append($", {nameof(FranchiseFeedbackQuestionEntity.DateCreated)}");
            sql.Append($", {nameof(FranchiseFeedbackQuestionEntity.Type)}");
            sql.Append($", {nameof(FranchiseFeedbackQuestionEntity.IsDeleted)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append(nameof(FranchiseFeedbackQuestionEntity.Id));
            sql.Append($", @{nameof(FranchiseFeedbackQuestionEntity.Question)}");

            sql.Append($", @{nameof(FranchiseFeedbackQuestionEntity.DateCreated)}");
            sql.Append($", @{nameof(FranchiseFeedbackQuestionEntity.Type)}");
            sql.Append($", @{nameof(FranchiseFeedbackQuestionEntity.IsDeleted)}");


            sql.Append(")");

            return sql.ToString();


        }

        string BuildUpdateScript(FranchiseFeedbackQuestionEntity franchiseFeedbackQuestionEntity)
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.FranchiseFeedbackQuestion} SET ");

            sql.Append(string.Concat(" ", nameof(franchiseFeedbackQuestionEntity.Question), " = ",
                   "@", nameof(franchiseFeedbackQuestionEntity.Question)));

            sql.Append(string.Concat(", ", nameof(franchiseFeedbackQuestionEntity.DateModified), " = ",
                    "@", nameof(franchiseFeedbackQuestionEntity.DateModified)));

            sql.Append(string.Concat(", ", nameof(franchiseFeedbackQuestionEntity.Type), " = ",
                   "@", nameof(franchiseFeedbackQuestionEntity.Type)));




            sql.Append(string.Concat(" WHERE ", nameof(franchiseFeedbackQuestionEntity.Id), " = ", "@", nameof(franchiseFeedbackQuestionEntity.Id)));

            return sql.ToString();


        }

        string BuildDeleteScript(FranchiseFeedbackQuestionEntity franchiseFeedbackQuestionEntity)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.FranchiseFeedbackQuestion));

            sql.Append(string.Concat(" WHERE ", nameof(franchiseFeedbackQuestionEntity.Id), " = ", "@", nameof(franchiseFeedbackQuestionEntity.Id)));

            return sql.ToString();


        }

        string BuildGetAllFeedback()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.StoredProcedures.GetAllFranchiseFeedbackQuestion}");
            sql.Append($" WHERE {nameof(FranchiseFeedbackQuestionEntity.IsDeleted)} = 0");

            return sql.ToString();



        }
        string BuildGetScript()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.FranchiseFeedbackQuestion}");
            sql.Append($" WHERE {nameof(FranchiseFeedbackQuestionEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");
            sql.Append($" AND {nameof(FranchiseFeedbackQuestionEntity.Id)} = {GlobalDatabaseConstants.QueryParameters.FranchiseFeedbackQuestionEntityId}");

            return sql.ToString();
        }
        string BuildGetByQuestionScript()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.FranchiseFeedbackQuestion}");
            sql.Append($" WHERE {nameof(FranchiseFeedbackQuestionEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");
            sql.Append($" AND {nameof(FranchiseFeedbackQuestionEntity.Question)} = {GlobalDatabaseConstants.QueryParameters.Question}");

            return sql.ToString();
        }


        // Question 
        public async Task<int> Save(FranchiseFeedbackQuestionEntity franchiseFeedbackQuestionEntity)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(FranchiseFeedbackQuestionEntity.Id)), franchiseFeedbackQuestionEntity.Id);
            p.Add(string.Concat("@", nameof(FranchiseFeedbackQuestionEntity.Question)), franchiseFeedbackQuestionEntity.Question);
            p.Add(string.Concat("@", nameof(FranchiseFeedbackQuestionEntity.Type)), franchiseFeedbackQuestionEntity.Type);
            p.Add(string.Concat("@", nameof(FranchiseFeedbackQuestionEntity.IsDeleted)), franchiseFeedbackQuestionEntity.IsDeleted);

            if (franchiseFeedbackQuestionEntity.Id <= 0)
            {
                p.Add(string.Concat("@", nameof(franchiseFeedbackQuestionEntity.DateCreated)), DateTime.Now);
                sql = BuildInsertScript();

            }
            else
            {
                p.Add(string.Concat("@", nameof(franchiseFeedbackQuestionEntity.DateModified)), DateTime.Now);
                sql = BuildUpdateScript(franchiseFeedbackQuestionEntity);

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
                        if (franchiseFeedbackQuestionEntity.Id <= 0)
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

        public async Task<List<FranchiseFeedbackQuestionViewModel>> GetFranchiseFeedbackQuestions()
        {
            var sp_GetAllFranchiseFeedbackQuestion = GlobalDatabaseConstants.StoredProcedures.GetAllFranchiseFeedbackQuestion;


            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var questions = (await conn.QueryAsync<FranchiseFeedbackQuestionViewModel>(sp_GetAllFranchiseFeedbackQuestion, null, commandType: CommandType.StoredProcedure)).ToList();

                return questions;
            }
        }



        public async Task<int> DeleteFranchiseFeedbackQuestion(int id)
        {
            FranchiseFeedbackQuestionEntity entity = new FranchiseFeedbackQuestionEntity { Id = id };
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(entity.Id)), id);
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

        public async Task<FranchiseFeedbackQuestionEntity> GetById(int Id)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(FranchiseFeedbackQuestionEntity.IsDeleted)), 0);
            p.Add(string.Concat("@", nameof(FranchiseFeedbackQuestionEntity.Id)), Id);

            var sql = BuildGetScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<FranchiseFeedbackQuestionEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }

        public async Task<FranchiseFeedbackQuestionEntity> GetByQuestion(string question)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(FranchiseFeedbackQuestionEntity.IsDeleted)), 0);
            p.Add(string.Concat("@", nameof(FranchiseFeedbackQuestionEntity.Question)), question);

            var sql = BuildGetByQuestionScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<FranchiseFeedbackQuestionEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }


        //Answer

        string BuildFeedbackAnswerInsertScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.FranchiseFeedbackAnswer));



            sql.Append(" (");
            sql.Append(nameof(FranchiseFeedbackAnswerEntity.Id));
            sql.Append($", {nameof(FranchiseFeedbackAnswerEntity.Answer)}");
            sql.Append($", {nameof(FranchiseFeedbackAnswerEntity.CompanyId)}");
            sql.Append($", {nameof(FranchiseFeedbackAnswerEntity.QuestionId)}");
            sql.Append($", {nameof(FranchiseFeedbackAnswerEntity.DateCreated)}");
            sql.Append($", {nameof(FranchiseFeedbackAnswerEntity.DateModified)}");
            sql.Append($", {nameof(FranchiseFeedbackAnswerEntity.IsDeleted)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append(nameof(FranchiseFeedbackAnswerEntity.Id));
            sql.Append($", @{nameof(FranchiseFeedbackAnswerEntity.Answer)}");
            sql.Append($", @{nameof(FranchiseFeedbackAnswerEntity.CompanyId)}");
            sql.Append($", @{nameof(FranchiseFeedbackAnswerEntity.QuestionId)}");
            sql.Append($", @{nameof(FranchiseFeedbackAnswerEntity.DateCreated)}");
            sql.Append($", @{nameof(FranchiseFeedbackAnswerEntity.DateModified)}");
            sql.Append($", @{nameof(FranchiseFeedbackAnswerEntity.IsDeleted)}");


            sql.Append(")");

            return sql.ToString();


        }

        string BuildFeedbackAnswerUpdateScript(FranchiseFeedbackAnswerEntity franchiseFeedbackAnswerEntity)
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.FranchiseFeedbackAnswer} SET ");

            sql.Append(string.Concat(" ", nameof(franchiseFeedbackAnswerEntity.Answer), " = ",
                   "@", nameof(franchiseFeedbackAnswerEntity.Answer)));

            sql.Append(string.Concat(", ", nameof(franchiseFeedbackAnswerEntity.CompanyId), " = ",
                   "@", nameof(franchiseFeedbackAnswerEntity.CompanyId)));


            sql.Append(string.Concat(", ", nameof(franchiseFeedbackAnswerEntity.QuestionId), " = ",
                   "@", nameof(franchiseFeedbackAnswerEntity.QuestionId)));

            sql.Append(string.Concat(", ", nameof(franchiseFeedbackAnswerEntity.DateModified), " = ",
                    "@", nameof(franchiseFeedbackAnswerEntity.DateModified)));


            sql.Append(string.Concat(" WHERE ", nameof(franchiseFeedbackAnswerEntity.Id), " = ", "@", nameof(franchiseFeedbackAnswerEntity.Id)));

            return sql.ToString();


        }

        string BuildGetAllFeedbackAnswer()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.FranchiseFeedbackAnswer}");
            sql.Append($" WHERE {nameof(FranchiseFeedbackAnswerEntity.IsDeleted)} = 0");

            return sql.ToString();



        }

        string BuildFeedbackAnswerDeleteScript(FranchiseFeedbackAnswerEntity franchiseFeedbackAnswerEntity)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.FranchiseFeedbackAnswer));

            sql.Append(string.Concat(" WHERE ", nameof(franchiseFeedbackAnswerEntity.Id), " = ", "@", nameof(franchiseFeedbackAnswerEntity.Id)));

            return sql.ToString();


        }

        string BuildGetByAnswerScript()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.FranchiseFeedbackAnswer}");
            sql.Append($" WHERE {nameof(FranchiseFeedbackAnswerEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");
            sql.Append($" AND {nameof(FranchiseFeedbackAnswerEntity.Answer)} = {GlobalDatabaseConstants.QueryParameters.Answer}");

            return sql.ToString();
        }

        public async Task<int> FeedbackAnswerSave(FranchiseFeedbackAnswerEntity franchiseFeedbackAnswerEntity)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(FranchiseFeedbackAnswerEntity.Id)), franchiseFeedbackAnswerEntity.Id);
            p.Add(string.Concat("@", nameof(FranchiseFeedbackAnswerEntity.Answer)), franchiseFeedbackAnswerEntity.Answer);
            p.Add(string.Concat("@", nameof(FranchiseFeedbackAnswerEntity.CompanyId)), franchiseFeedbackAnswerEntity.CompanyId);
            p.Add(string.Concat("@", nameof(FranchiseFeedbackAnswerEntity.QuestionId)), franchiseFeedbackAnswerEntity.QuestionId);

            p.Add(string.Concat("@", nameof(FranchiseFeedbackAnswerEntity.IsDeleted)), franchiseFeedbackAnswerEntity.IsDeleted);

            if (franchiseFeedbackAnswerEntity.Id <= 0)
            {
                p.Add(string.Concat("@", nameof(franchiseFeedbackAnswerEntity.DateCreated)), DateTime.Now);
                sql = BuildFeedbackAnswerInsertScript();

            }
            else
            {
                p.Add(string.Concat("@", nameof(franchiseFeedbackAnswerEntity.DateModified)), DateTime.Now);
                sql = BuildFeedbackAnswerUpdateScript(franchiseFeedbackAnswerEntity);

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
                        if (franchiseFeedbackAnswerEntity.Id <= 0)
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
        public async Task<List<FranchiseFeedbackAnswerViewModel>> GetFranchiseFeedbackAnswer()
        {
            var sp_GetAllFranchiseFeedbackAnswer = GlobalDatabaseConstants.StoredProcedures.GetAllFranchiseFeedbackAnswer;


            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var questions = (await conn.QueryAsync<FranchiseFeedbackAnswerViewModel>(sp_GetAllFranchiseFeedbackAnswer, null, commandType: CommandType.StoredProcedure)).ToList();

                return questions;
            }
        }


        public async Task<FranchiseFeedbackAnswerEntity> GetByAnswer(string answer)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(FranchiseFeedbackAnswerEntity.IsDeleted)), 0);
            p.Add(string.Concat("@", nameof(FranchiseFeedbackAnswerEntity.Answer)), answer);

            var sql = BuildGetByAnswerScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<FranchiseFeedbackAnswerEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }

        public async Task<int> DeleteFranchiseFeedbackAnswer(int id)
        {
            FranchiseFeedbackAnswerEntity entity = new FranchiseFeedbackAnswerEntity { Id = id };
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(entity.Id)), id);
            string sql = BuildFeedbackAnswerDeleteScript(entity);

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

    }
}

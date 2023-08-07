using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Feedback
{
    public class FeedbackRepository: GenericRepositoryBase<FeedbackEntity, FeedbackRepository>, IFeedbackRepository
    {
        public FeedbackRepository(IDatabaseHelper databaseHelper, ILogger<FeedbackRepository> logger) : base(databaseHelper,
          logger)
        {

        }
         
        string BuildInsertScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.Feedback));

            

            sql.Append(" (");
            sql.Append(nameof(FeedbackEntity.FeedbackId));
            sql.Append($", {nameof(FeedbackEntity.AppRating)}");
            sql.Append($", {nameof(FeedbackEntity.FeaturesExpectations)}");
            sql.Append($", {nameof(FeedbackEntity.LockerEquipmentRating)}");
            sql.Append($", {nameof(FeedbackEntity.Suggestion)}");
            sql.Append($", {nameof(FeedbackEntity.LocationRating)}");
            sql.Append($", {nameof(FeedbackEntity.WantToSee)}");
            sql.Append($", {nameof(FeedbackEntity.BetterExperience)}");
            sql.Append($", {nameof(FeedbackEntity.UserKeyId)}");
            sql.Append($", {nameof(FeedbackEntity.DateCreated)}");
            sql.Append($", {nameof(FeedbackEntity.DateModified)}");
            sql.Append($", {nameof(FeedbackEntity.IsDeleted)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append(nameof(FeedbackEntity.FeedbackId));
            sql.Append($", @{nameof(FeedbackEntity.AppRating)}");
            sql.Append($", @{nameof(FeedbackEntity.FeaturesExpectations)}");
            sql.Append($", @{nameof(FeedbackEntity.LockerEquipmentRating)}");
            sql.Append($", @{nameof(FeedbackEntity.Suggestion)}");
            sql.Append($", @{nameof(FeedbackEntity.LocationRating)}");
            sql.Append($", @{nameof(FeedbackEntity.WantToSee)}");
            sql.Append($", @{nameof(FeedbackEntity.BetterExperience)}");
            sql.Append($", @{nameof(FeedbackEntity.UserKeyId)}");
            sql.Append($", @{nameof(FeedbackEntity.DateCreated)}");
            sql.Append($", @{nameof(FeedbackEntity.DateModified)}");
            sql.Append($", @{nameof(FeedbackEntity.IsDeleted)}");


            sql.Append(")");

            return sql.ToString();


        }

        string BuildUpdateScript(FeedbackEntity feedbackEntity)
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.Feedback} SET ");

            sql.Append(string.Concat(" ", nameof(feedbackEntity.AppRating), " = ",
                   "@", nameof(feedbackEntity.AppRating)));

            sql.Append(string.Concat(", ", nameof(feedbackEntity.FeaturesExpectations), " = ",
                   "@", nameof(feedbackEntity.FeaturesExpectations)));

            sql.Append(string.Concat(", ", nameof(feedbackEntity.LockerEquipmentRating), " = ",
                    "@", nameof(feedbackEntity.LockerEquipmentRating)));

            sql.Append(string.Concat(", ", nameof(feedbackEntity.Suggestion), " = ",
                    "@", nameof(feedbackEntity.Suggestion)));

            sql.Append(string.Concat(", ", nameof(feedbackEntity.LocationRating), " = ",
                    "@", nameof(feedbackEntity.LocationRating)));

            sql.Append(string.Concat(", ", nameof(feedbackEntity.WantToSee), " = ",
                    "@", nameof(feedbackEntity.WantToSee)));

            sql.Append(string.Concat(", ", nameof(feedbackEntity.BetterExperience), " = ",
                    "@", nameof(feedbackEntity.BetterExperience)));

            sql.Append(string.Concat(", ", nameof(feedbackEntity.UserKeyId), " = ",
               "@", nameof(feedbackEntity.UserKeyId)));

            sql.Append(string.Concat(", ", nameof(feedbackEntity.DateModified), " = ",
                    "@", nameof(feedbackEntity.DateModified)));


            sql.Append(string.Concat(" WHERE ", nameof(feedbackEntity.FeedbackId), " = ", "@", nameof(feedbackEntity.FeedbackId)));

            return sql.ToString();


        }

        string BuildDeleteScript(FeedbackEntity feedbackEntity)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.Feedback));

            sql.Append(string.Concat(" WHERE ", nameof(feedbackEntity.FeedbackId), " = ", "@", nameof(feedbackEntity.FeedbackId)));

            return sql.ToString();


        }

        string BuildGetAllFeedback()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Feedback}");
            sql.Append($" WHERE {nameof(FeedbackEntity.IsDeleted)} = 0");

            return sql.ToString();



        }
        string BuildGetScript()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Feedback}");
            sql.Append($" WHERE {nameof(FeedbackEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");
            sql.Append($" AND {nameof(FeedbackEntity.FeedbackId)} = {GlobalDatabaseConstants.QueryParameters.FeedbackId}");

            return sql.ToString();
        }


        public async Task<int> Save(FeedbackEntity feedbackEntity)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(FeedbackEntity.FeedbackId)), feedbackEntity.FeedbackId);
            p.Add(string.Concat("@", nameof(FeedbackEntity.AppRating)), feedbackEntity.AppRating);
            p.Add(string.Concat("@", nameof(FeedbackEntity.FeaturesExpectations)), feedbackEntity.FeaturesExpectations);
            p.Add(string.Concat("@", nameof(FeedbackEntity.LockerEquipmentRating)), feedbackEntity.LockerEquipmentRating);
            p.Add(string.Concat("@", nameof(FeedbackEntity.Suggestion)), feedbackEntity.Suggestion);
            p.Add(string.Concat("@", nameof(FeedbackEntity.LocationRating)), feedbackEntity.LocationRating);
            p.Add(string.Concat("@", nameof(FeedbackEntity.WantToSee)), feedbackEntity.WantToSee);
            p.Add(string.Concat("@", nameof(FeedbackEntity.BetterExperience)), feedbackEntity.BetterExperience);
            p.Add(string.Concat("@", nameof(FeedbackEntity.UserKeyId)), feedbackEntity.UserKeyId);
            

            p.Add(string.Concat("@", nameof(FeedbackEntity.IsDeleted)), feedbackEntity.IsDeleted);

            if (feedbackEntity.FeedbackId <= 0)
            {
                p.Add(string.Concat("@", nameof(feedbackEntity.DateCreated)), DateTime.Now);
                sql = BuildInsertScript(); 
            }
            else
            {
               
                sql = BuildUpdateScript(feedbackEntity);

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
                        if (feedbackEntity.FeedbackId <= 0)
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

        public  List<FeedbackViewModel> GetFeedbacks()
        {
            var sp_GetAlFeedbacks = GlobalDatabaseConstants.StoredProcedures.GetAllFeedback;


            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var feedbacks = conn.Query<FeedbackViewModel>(sp_GetAlFeedbacks, null, commandType: CommandType.StoredProcedure).ToList();

                return feedbacks;
            }
        }


        public async Task<int> DeleteFeedback(int feedbackId)
        {
            FeedbackEntity entity = new FeedbackEntity { FeedbackId = feedbackId };
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(entity.FeedbackId)), feedbackId);
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

        public async Task<FeedbackEntity> GetById(int Id)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(FeedbackEntity.IsDeleted)), 0);
            p.Add(string.Concat("@", nameof(FeedbackEntity.FeedbackId)), Id);

            var sql = BuildGetScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<FeedbackEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }


    }
}

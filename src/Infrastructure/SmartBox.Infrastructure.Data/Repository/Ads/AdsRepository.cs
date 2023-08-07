using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Entities.PromoAndDiscounts;
using SmartBox.Business.Core.Models.Ads;
using SmartBox.Business.Core.Models.PromoAndDiscounts;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using SmartBox.Infrastructure.Data.Repository.PromoAndDiscounts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Ads
{
    public class AdsRepository : GenericRepositoryBase<AdsEntity, AdsRepository>, IAdsRepository
    {
        public AdsRepository(IDatabaseHelper databaseHelper, ILogger<AdsRepository> logger) : base(databaseHelper,
        logger)
        {

        }

        string BuildInsertScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.Ads));



            sql.Append(" (");
            sql.Append(nameof(AdsEntity.AdsId));
            sql.Append(nameof(AdsEntity.Description));
            sql.Append(nameof(AdsEntity.Image));
            sql.Append(nameof(AdsEntity.Video));
            sql.Append(nameof(AdsEntity.Text));
            sql.Append(nameof(AdsEntity.ExteralLink));
            sql.Append(nameof(AdsEntity.DateCreated));
            sql.Append(nameof(AdsEntity.DateModified));
            sql.Append(nameof(AdsEntity.IsDeleted));

            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append(nameof(AdsEntity.AdsId));
            sql.Append($", @{nameof(AdsEntity.Description)}");
            sql.Append($", @{nameof(AdsEntity.Image)}");
            sql.Append($", @{nameof(AdsEntity.Video)}");
            sql.Append($", @{nameof(AdsEntity.Text)}");
            sql.Append($", @{nameof(AdsEntity.ExteralLink)}");
            sql.Append($", @{nameof(AdsEntity.DateCreated)}");
            sql.Append($", @{nameof(AdsEntity.DateModified)}");
            sql.Append($", @{nameof(AdsEntity.IsDeleted)}");
            sql.Append(")");

            return sql.ToString();


        }

        string BuildUpdateScript(AdsEntity adsEntity)
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.Ads} SET ");

            sql.Append(string.Concat(" ", nameof(adsEntity.Description), " = ",
                   "@", nameof(adsEntity.Description)));

            sql.Append(string.Concat(", ", nameof(adsEntity.Image), " = ",
                   "@", nameof(adsEntity.Image)));

            sql.Append(string.Concat(", ", nameof(adsEntity.Video), " = ",
                    "@", nameof(adsEntity.Video)));

            sql.Append(string.Concat(", ", nameof(adsEntity.Text), " = ",
                    "@", nameof(adsEntity.Text)));

            sql.Append(string.Concat(", ", nameof(adsEntity.ExteralLink), " = ",
                    "@", nameof(adsEntity.ExteralLink)));


            sql.Append(string.Concat(", ", nameof(adsEntity.DateModified), " = ",
                    "@", nameof(adsEntity.DateModified)));




            sql.Append(string.Concat(" WHERE ", nameof(adsEntity.AdsId), " = ", "@", nameof(adsEntity.AdsId)));

            return sql.ToString();


        }

        string BuildDeleteScript(AdsEntity adsEntity)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.Ads));

            sql.Append(string.Concat(" WHERE ", nameof(adsEntity.AdsId), " = ", "@", nameof(adsEntity.AdsId)));

            return sql.ToString();


        }

        string BuildGetAllAds()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Ads}");
            sql.Append($" WHERE {nameof(AdsEntity.IsDeleted)} = 0");

            return sql.ToString();



        }
        string BuildGetScript()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Ads}");
            sql.Append($" WHERE {nameof(AdsEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");
            sql.Append($" AND {nameof(AdsEntity.AdsId)} = {GlobalDatabaseConstants.QueryParameters.AdsId}");

            return sql.ToString();
        }


        public async Task<int> Save(AdsEntity adsEntity)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(adsEntity.AdsId)), adsEntity.AdsId);
            p.Add(string.Concat("@", nameof(adsEntity.Description)), adsEntity.Description);
            p.Add(string.Concat("@", nameof(adsEntity.Image)), adsEntity.Image);
            p.Add(string.Concat("@", nameof(adsEntity.ExteralLink)), adsEntity.ExteralLink);
            p.Add(string.Concat("@", nameof(adsEntity.Video)), adsEntity.Video);
            p.Add(string.Concat("@", nameof(adsEntity.Text)), adsEntity.Text);



            p.Add(string.Concat("@", nameof(adsEntity.IsDeleted)), adsEntity.IsDeleted);

            if (adsEntity.AdsId <= 0)
            {
                p.Add(string.Concat("@", nameof(adsEntity.DateCreated)), DateTime.Now);
                sql = BuildInsertScript();
            }
            else
            {

                sql = BuildUpdateScript(adsEntity);

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
                        if (adsEntity.AdsId <= 0)
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

        public List<AdsViewModel> GetAds()
        {
            var sp_Ads = GlobalDatabaseConstants.StoredProcedures.GetAllAds;


            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var ads = conn.Query<AdsViewModel>(sp_Ads, null, commandType: CommandType.StoredProcedure).ToList();

                return ads;
            }
        }


        public async Task<int> DeleteAds(int adsId)
        {
            AdsEntity entity = new AdsEntity { AdsId = adsId };
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(entity.AdsId)), adsId);
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

        public async Task<AdsEntity> GetById(int Id)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(AdsEntity.IsDeleted)), 0);
            p.Add(string.Concat("@", nameof(AdsEntity.AdsId)), Id);

            var sql = BuildGetScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<AdsEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }

    }
}

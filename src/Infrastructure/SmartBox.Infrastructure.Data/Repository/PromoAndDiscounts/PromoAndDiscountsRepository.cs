using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Entities.PromoAndDiscounts;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.PromoAndDiscounts;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using SmartBox.Infrastructure.Data.Repository.Feedback;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.PromoAndDiscounts
{
    public class PromoAndDiscountsRepository : GenericRepositoryBase<PromoAndDiscountsEntity, PromoAndDiscountsRepository>, IPromoAndDiscountsRepository
    {
        public PromoAndDiscountsRepository(IDatabaseHelper databaseHelper, ILogger<PromoAndDiscountsRepository> logger) : base(databaseHelper,
        logger)
        {

        }

        string BuildInsertScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.PromoAndDiscounts));



            sql.Append(" (");
            sql.Append(nameof(PromoAndDiscountsEntity.PromoAndDiscountsId));
            sql.Append(nameof(PromoAndDiscountsEntity.Description));
            sql.Append(nameof(PromoAndDiscountsEntity.Text));
            sql.Append(nameof(PromoAndDiscountsEntity.ExteralLink));
            sql.Append(nameof(PromoAndDiscountsEntity.BookingDiscount));
            sql.Append(nameof(PromoAndDiscountsEntity.Image));
            sql.Append(nameof(PromoAndDiscountsEntity.DateCreated));
            sql.Append(nameof(PromoAndDiscountsEntity.DateModified));
            sql.Append(nameof(PromoAndDiscountsEntity.IsDeleted));
           
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append(nameof(PromoAndDiscountsEntity.PromoAndDiscountsId));
            sql.Append($", @{nameof(PromoAndDiscountsEntity.Description)}");
            sql.Append($", @{nameof(PromoAndDiscountsEntity.Text)}");
            sql.Append($", @{nameof(PromoAndDiscountsEntity.ExteralLink)}");
            sql.Append($", @{nameof(PromoAndDiscountsEntity.BookingDiscount)}");
            sql.Append($", @{nameof(PromoAndDiscountsEntity.Image)}");
            sql.Append($", @{nameof(PromoAndDiscountsEntity.DateCreated)}");
            sql.Append($", @{nameof(PromoAndDiscountsEntity.DateModified)}");
            sql.Append($", @{nameof(PromoAndDiscountsEntity.IsDeleted)}");
            sql.Append(")");

            return sql.ToString();


        }

        string BuildUpdateScript(PromoAndDiscountsEntity promoAndDiscountsEntity)
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.PromoAndDiscounts} SET ");

            sql.Append(string.Concat(" ", nameof(promoAndDiscountsEntity.Description), " = ",
                   "@", nameof(promoAndDiscountsEntity.Description)));

            sql.Append(string.Concat(", ", nameof(promoAndDiscountsEntity.Text), " = ",
                   "@", nameof(promoAndDiscountsEntity.Text)));

            sql.Append(string.Concat(", ", nameof(promoAndDiscountsEntity.ExteralLink), " = ",
                    "@", nameof(promoAndDiscountsEntity.ExteralLink)));

            sql.Append(string.Concat(", ", nameof(promoAndDiscountsEntity.BookingDiscount), " = ",
                    "@", nameof(promoAndDiscountsEntity.BookingDiscount)));

            sql.Append(string.Concat(", ", nameof(promoAndDiscountsEntity.Image), " = ",
                    "@", nameof(promoAndDiscountsEntity.Image)));

          
            sql.Append(string.Concat(", ", nameof(promoAndDiscountsEntity.DateModified), " = ",
                    "@", nameof(promoAndDiscountsEntity.DateModified)));

           


            sql.Append(string.Concat(" WHERE ", nameof(promoAndDiscountsEntity.PromoAndDiscountsId), " = ", "@", nameof(promoAndDiscountsEntity.PromoAndDiscountsId)));

            return sql.ToString();


        }

        string BuildDeleteScript(PromoAndDiscountsEntity promoAndDiscountsEntity)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.PromoAndDiscounts));

            sql.Append(string.Concat(" WHERE ", nameof(promoAndDiscountsEntity.PromoAndDiscountsId), " = ", "@", nameof(promoAndDiscountsEntity.PromoAndDiscountsId)));

            return sql.ToString();


        }

        string BuildGetAllPromoAndDiscounts()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.PromoAndDiscounts}");
            sql.Append($" WHERE {nameof(PromoAndDiscountsEntity.IsDeleted)} = 0");

            return sql.ToString();



        }
        string BuildGetScript()
        {
            var sql = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.PromoAndDiscounts}");
            sql.Append($" WHERE {nameof(PromoAndDiscountsEntity.IsDeleted)} = {GlobalDatabaseConstants.QueryParameters.IsDeleted}");
            sql.Append($" AND {nameof(PromoAndDiscountsEntity.PromoAndDiscountsId)} = {GlobalDatabaseConstants.QueryParameters.PromoAndDiscountsId}");

            return sql.ToString();
        }


        public async Task<int> Save(PromoAndDiscountsEntity promoAndDiscountsEntity)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(promoAndDiscountsEntity.PromoAndDiscountsId)), promoAndDiscountsEntity.PromoAndDiscountsId);
            p.Add(string.Concat("@", nameof(promoAndDiscountsEntity.Description)), promoAndDiscountsEntity.Description);
            p.Add(string.Concat("@", nameof(promoAndDiscountsEntity.Text)), promoAndDiscountsEntity.Text);
            p.Add(string.Concat("@", nameof(promoAndDiscountsEntity.ExteralLink)), promoAndDiscountsEntity.ExteralLink);
            p.Add(string.Concat("@", nameof(promoAndDiscountsEntity.BookingDiscount)), promoAndDiscountsEntity.BookingDiscount);
            p.Add(string.Concat("@", nameof(promoAndDiscountsEntity.Image)), promoAndDiscountsEntity.Image);
        


            p.Add(string.Concat("@", nameof(promoAndDiscountsEntity.IsDeleted)), promoAndDiscountsEntity.IsDeleted);

            if (promoAndDiscountsEntity.PromoAndDiscountsId <= 0)
            {
                p.Add(string.Concat("@", nameof(promoAndDiscountsEntity.DateCreated)), DateTime.Now);
                sql = BuildInsertScript();
            }
            else
            {

                sql = BuildUpdateScript(promoAndDiscountsEntity);

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
                        if (promoAndDiscountsEntity.PromoAndDiscountsId <= 0)
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

        public List<PromoAndDiscountsViewModel> GetPromoAndDiscounts()
        {
            var sp_PromoAndDiscounts = GlobalDatabaseConstants.StoredProcedures.GetAllPromoAndDiscounts;


            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var promoAndDiscounts = conn.Query<PromoAndDiscountsViewModel>(sp_PromoAndDiscounts, null, commandType: CommandType.StoredProcedure).ToList();

                return promoAndDiscounts;
            }
        }


        public async Task<int> DeletePromoAndDiscounts(int promoAndDiscountsId)
        {
            PromoAndDiscountsEntity entity = new PromoAndDiscountsEntity { PromoAndDiscountsId = promoAndDiscountsId };
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(entity.PromoAndDiscountsId)), promoAndDiscountsId);
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

        public async Task<PromoAndDiscountsEntity> GetById(int Id)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(PromoAndDiscountsEntity.IsDeleted)), 0);
            p.Add(string.Concat("@", nameof(PromoAndDiscountsEntity.PromoAndDiscountsId)), Id);

            var sql = BuildGetScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<PromoAndDiscountsEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }

    }
}

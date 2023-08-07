using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.User;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.User
{
    public class ChargesRepository : GenericRepositoryBase<ChargesEntity, ChargesRepository>, IChargesRepository
    {
        public ChargesRepository(IDatabaseHelper databaseHelper, ILogger<ChargesRepository> logger) : base(databaseHelper, logger)
        {
        }
        public async Task<List<ChargesEntity>> GetChargesByUserKeyId(string userKeyId, string paymentRefNo = null)
        {
            if (string.IsNullOrEmpty(userKeyId))
                return null;
            var sql = BuildGetCommandOfChargesByUserKeyId(paymentRefNo);
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(ChargesEntity.UserKeyId)), userKeyId);
            p.Add(string.Concat("@", nameof(ChargesEntity.PaymentReferenceNo)), paymentRefNo);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<ChargesEntity>(sql, p)).ToList();
            }
        }

        public async Task<int> Save(ChargesEntity model)
        {
            var p = new DynamicParameters();
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(model.Id)), model.Id);
            p.Add(string.Concat("@", nameof(model.UserKeyId)), model.UserKeyId);
            p.Add(string.Concat("@", nameof(model.LockerTypeId)), model.LockerTypeId);
            p.Add(string.Concat("@", nameof(model.PaymentReferenceNo)), model.PaymentReferenceNo);
            p.Add(string.Concat("@", nameof(model.TotalAmount)), model.TotalAmount);
            p.Add(string.Concat("@", nameof(model.PaymentDate)), model.PaymentDate);
            p.Add(string.Concat("@", nameof(model.BookingAmount)), model.BookingAmount);
            p.Add(string.Concat("@", nameof(model.BookingDate)), model.BookingDate);
            p.Add(string.Concat("@", nameof(model.PaymentStatus)), model.PaymentStatus);
            p.Add(string.Concat("@", nameof(model.LockerDetailId)), model.LockerDetailId);
            p.Add(string.Concat("@", nameof(model.Charges)), model.Charges);

            string sql;
            if (model.Id == 0)
            {
                p.Add(string.Concat("@", nameof(model.DateCreated)), DateTime.Now);
                sql = BuildInsertCommand();
            }
            else
            {
                p.Add(string.Concat("@", nameof(model.DateModified)), DateTime.Now);
                sql = BuildUpdateCommand();
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
        string BuildGetCommandOfChargesByUserKeyId(string paymentRefNo = null)
        {
            var sql = new StringBuilder(string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.Charges));
            
            sql.Append(string.Concat(" WHERE ", nameof(ChargesEntity.UserKeyId), " = ", "@", nameof(ChargesEntity.UserKeyId)));

            if(!string.IsNullOrEmpty(paymentRefNo))
                sql.Append(string.Concat(" AND ", nameof(ChargesEntity.PaymentReferenceNo), " = ", "@", nameof(ChargesEntity.PaymentReferenceNo)));
            
            return sql.ToString();
        }

        string BuildInsertCommand()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.Charges));

            sql.Append(" (");
            sql.Append(nameof(ChargesEntity.PaymentStatus));
            sql.Append($", {nameof(ChargesEntity.LockerTypeId)}");
            sql.Append($", {nameof(ChargesEntity.UserKeyId)}");
            sql.Append($", {nameof(ChargesEntity.BookingAmount)}");
            sql.Append($", {nameof(ChargesEntity.TotalAmount)}");
            sql.Append($", {nameof(ChargesEntity.BookingDate)}");
            sql.Append($", {nameof(ChargesEntity.Charges)}");
            sql.Append($", {nameof(ChargesEntity.LockerDetailId)}");
            sql.Append($", {nameof(ChargesEntity.PaymentDate)}");
            sql.Append($", {nameof(ChargesEntity.PaymentReferenceNo)}");
            sql.Append($", {nameof(ChargesEntity.DateCreated)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(ChargesEntity.PaymentStatus)}");
            sql.Append($", @{nameof(ChargesEntity.LockerTypeId)}");
            sql.Append($", @{nameof(ChargesEntity.UserKeyId)}");
            sql.Append($", @{nameof(ChargesEntity.BookingAmount)}");
            sql.Append($", @{nameof(ChargesEntity.TotalAmount)}");
            sql.Append($", @{nameof(ChargesEntity.BookingDate)}");
            sql.Append($", @{nameof(ChargesEntity.Charges)}");
            sql.Append($", @{nameof(ChargesEntity.LockerDetailId)}");
            sql.Append($", @{nameof(ChargesEntity.PaymentDate)}");
            sql.Append($", @{nameof(ChargesEntity.PaymentReferenceNo)}");
            sql.Append($", @{nameof(ChargesEntity.DateCreated)}");

            sql.Append(")");

            return sql.ToString();
        }
        string BuildUpdateCommand()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.Charges, " SET "));


            sql.Append(string.Concat(" ", nameof(ChargesEntity.LockerTypeId), " = ",
                    "@", nameof(ChargesEntity.LockerTypeId)));

            sql.Append(string.Concat(", ", nameof(ChargesEntity.PaymentReferenceNo), " = ",
                     "@", nameof(ChargesEntity.PaymentReferenceNo)));

            sql.Append(string.Concat(", ", nameof(ChargesEntity.UserKeyId), " = ",
                    "@", nameof(ChargesEntity.UserKeyId)));

            sql.Append(string.Concat(", ", nameof(ChargesEntity.PaymentDate), " = ",
                    "@", nameof(ChargesEntity.PaymentDate)));

            sql.Append(string.Concat(", ", nameof(ChargesEntity.BookingAmount), " = ",
                    "@", nameof(ChargesEntity.BookingAmount)));

            sql.Append(string.Concat(", ", nameof(ChargesEntity.Charges), " = ",
                    "@", nameof(ChargesEntity.Charges)));

            sql.Append(string.Concat(", ", nameof(ChargesEntity.BookingDate), " = ",
                    "@", nameof(ChargesEntity.BookingDate)));

            sql.Append(string.Concat(", ", nameof(ChargesEntity.TotalAmount), " = ",
                   "@", nameof(ChargesEntity.TotalAmount)));

            sql.Append(string.Concat(", ", nameof(ChargesEntity.LockerDetailId), " = ",
                  "@", nameof(ChargesEntity.LockerDetailId)));

            sql.Append(string.Concat(", ", nameof(ChargesEntity.PaymentStatus), " = ",
                 "@", nameof(ChargesEntity.PaymentStatus)));

            sql.Append(string.Concat(", ", nameof(ChargesEntity.DateModified), " = ",
                    "@", nameof(ChargesEntity.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(ChargesEntity.Id), " = ", "@", nameof(ChargesEntity.Id)));

            return sql.ToString();

        }
        public async Task<int> Delete(int id)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(ChargesEntity.Id)), id);
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
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.Charges));

            sql.Append(string.Concat(" WHERE ", nameof(ChargesEntity.Id), " = ", "@", nameof(ChargesEntity.Id)));

            return sql.ToString();

        }
    }
}

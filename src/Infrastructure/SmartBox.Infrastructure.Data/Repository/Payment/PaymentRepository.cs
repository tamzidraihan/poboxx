using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Locker;
using SmartBox.Business.Core.Entities.Payment;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalEnums;

namespace SmartBox.Infrastructure.Data.Repository.Payment
{
    public class PaymentRepository : GenericRepositoryBase<PaymentMethodEntity, PaymentRepository>, IPaymentRepository
    {
        public PaymentRepository(IDatabaseHelper databaseHelper, ILogger<PaymentRepository> logger) : base(databaseHelper, logger)
        {
        }

        string BuildSelectScript()
        {
            var query = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.PaymentMethod}");

            return query.ToString();
        }
        string BuildInsertPaymentInfoScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.PaymongoTransaction));

            sql.Append(" (");
            sql.Append(nameof(PaymentInfoEntity.WebhookId));
            sql.Append($", {nameof(PaymentInfoEntity.PaymentSourceId)}");
            sql.Append($", {nameof(PaymentInfoEntity.Amount)}");
            sql.Append($", {nameof(PaymentInfoEntity.SourceStatus)}");
            sql.Append($", {nameof(PaymentInfoEntity.Type)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(PaymentInfoEntity.WebhookId)}");
            sql.Append($", @{nameof(PaymentInfoEntity.PaymentSourceId)}");
            sql.Append($", @{nameof(PaymentInfoEntity.Amount)}");
            sql.Append($", @{nameof(PaymentInfoEntity.SourceStatus)}");
            sql.Append($", @{nameof(PaymentInfoEntity.Type)}");
            sql.Append(")");

            return sql.ToString();
        }
        string BuildSelectGetPaymentInfo()
        {
            var query = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.PaymongoTransaction}");
            query.Append($" WHERE {nameof(PaymentInfoEntity.PaymentSourceId)} = @{nameof(PaymentInfoEntity.PaymentSourceId)}");

            return query.ToString();
        }

        string BuildInsertPaymentTransactionScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.PaymentTransaction));

            sql.Append(" (");
            sql.Append(nameof(PaymentTransactionEntity.TransactionId));
            sql.Append($", {nameof(PaymentTransactionEntity.Status)}");
            sql.Append($", {nameof(PaymentTransactionEntity.Type)}");
            sql.Append($", {nameof(PaymentTransactionEntity.Amount)}");
            sql.Append($", {nameof(PaymentTransactionEntity.LockerTransactionsId)}");
            sql.Append($", {nameof(PaymentTransactionEntity.InternalStatus)}");
            sql.Append($", {nameof(PaymentTransactionEntity.InternalType)}");
            sql.Append($", {nameof(PaymentTransactionEntity.DateCreated)}");
            sql.Append($", {nameof(PaymentTransactionEntity.DateModified)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(PaymentTransactionEntity.TransactionId)}");
            sql.Append($", @{nameof(PaymentTransactionEntity.Status)}");
            sql.Append($", @{nameof(PaymentTransactionEntity.Type)}");
            sql.Append($", @{nameof(PaymentTransactionEntity.Amount)}");
            sql.Append($", @{nameof(PaymentTransactionEntity.LockerTransactionsId)}");
            sql.Append($", @{nameof(PaymentTransactionEntity.InternalStatus)}");
            sql.Append($", @{nameof(PaymentTransactionEntity.InternalType)}");
            sql.Append($", @{nameof(PaymentTransactionEntity.DateCreated)}");
            sql.Append($", @{nameof(PaymentTransactionEntity.DateModified)}");
            sql.Append(")");

            return sql.ToString();
        }
        string BuildUpdatePaymentTransactionScript()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.PaymentTransaction, " SET "));


            sql.Append(string.Concat(" ", nameof(PaymentTransactionEntity.TransactionId), " = ",
                    "@", nameof(PaymentTransactionEntity.TransactionId)));

            sql.Append(string.Concat(", ", nameof(PaymentTransactionEntity.Status), " = ",
                     "@", nameof(PaymentTransactionEntity.Status)));

            sql.Append(string.Concat(", ", nameof(PaymentTransactionEntity.Type), " = ",
                    "@", nameof(PaymentTransactionEntity.Type)));

            sql.Append(string.Concat(", ", nameof(PaymentTransactionEntity.Amount), " = ",
                   "@", nameof(PaymentTransactionEntity.Amount)));

            sql.Append(string.Concat(", ", nameof(PaymentTransactionEntity.LockerTransactionsId), " = ",
                   "@", nameof(PaymentTransactionEntity.LockerTransactionsId)));

            sql.Append(string.Concat(", ", nameof(PaymentTransactionEntity.InternalStatus), " = ",
                   "@", nameof(PaymentTransactionEntity.InternalStatus)));

            sql.Append(string.Concat(", ", nameof(PaymentTransactionEntity.InternalType), " = ",
                   "@", nameof(PaymentTransactionEntity.InternalType)));

            sql.Append(string.Concat(", ", nameof(PaymentTransactionEntity.DateModified), " = ",
                  "@", nameof(PaymentTransactionEntity.DateModified)));


            sql.Append(string.Concat(" WHERE ", nameof(PaymentTransactionEntity.PaymentGatewayTransactionId), " = ", "@", nameof(PaymentTransactionEntity.PaymentGatewayTransactionId)));

            return sql.ToString();
        }
        string BuildSelectGetPaymentTransaction()
        {
            var query = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.PaymentTransaction}");
            query.Append($" WHERE {nameof(PaymentTransactionEntity.TransactionId)} = @{nameof(PaymentTransactionEntity.TransactionId)}");

            return query.ToString();
        }

        public async Task<List<PaymentMethodEntity>> GetPaymentMethod()
        {
            var sql = BuildSelectScript();
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<PaymentMethodEntity>(sql);

                return dbModel.ToList();
            }
        }

        public async Task<int> SavePaymentInfo(PaymentInfoEntity paymentInfoEntity)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(PaymentInfoEntity.WebhookId)), paymentInfoEntity.WebhookId);
            p.Add(string.Concat("@", nameof(PaymentInfoEntity.PaymentSourceId)), paymentInfoEntity.PaymentSourceId);
            p.Add(string.Concat("@", nameof(PaymentInfoEntity.Amount)), paymentInfoEntity.Amount);
            p.Add(string.Concat("@", nameof(PaymentInfoEntity.SourceStatus)), paymentInfoEntity.SourceStatus);
            p.Add(string.Concat("@", nameof(PaymentInfoEntity.Type)), paymentInfoEntity.Type);

            sql = BuildInsertPaymentInfoScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                    {
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
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

        public async Task<PaymentInfoEntity> GetPaymentInfoModel(string referenceId)
        {
            var sql = BuildSelectGetPaymentInfo();
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(PaymentInfoEntity.PaymentSourceId)), referenceId);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<PaymentInfoEntity>(sql, p);

                if (dbModel != null)
                    return dbModel.FirstOrDefault();
                else return null;
            }
        }

        public async Task<int> SavePaymentTransaction(PaymentTransactionEntity paymentTransactionEntity)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(PaymentTransactionEntity.PaymentGatewayTransactionId)), paymentTransactionEntity.PaymentGatewayTransactionId);
            p.Add(string.Concat("@", nameof(PaymentTransactionEntity.TransactionId)), paymentTransactionEntity.TransactionId);
            p.Add(string.Concat("@", nameof(PaymentTransactionEntity.Type)), paymentTransactionEntity.Type);
            p.Add(string.Concat("@", nameof(PaymentTransactionEntity.Amount)), paymentTransactionEntity.Amount);
            p.Add(string.Concat("@", nameof(PaymentTransactionEntity.Status)), paymentTransactionEntity.Status);
            p.Add(string.Concat("@", nameof(PaymentTransactionEntity.InternalType)), paymentTransactionEntity.InternalType);
            p.Add(string.Concat("@", nameof(PaymentTransactionEntity.InternalStatus)), paymentTransactionEntity.InternalStatus);
            p.Add(string.Concat("@", nameof(PaymentTransactionEntity.LockerTransactionsId)), paymentTransactionEntity.LockerTransactionsId);


            if (paymentTransactionEntity.PaymentGatewayTransactionId == 0)
            {
                sql = BuildInsertPaymentTransactionScript();
                p.Add(string.Concat("@", nameof(PaymentTransactionEntity.DateCreated)), DateTime.Now);
            }

            else
            {
                sql = BuildUpdatePaymentTransactionScript();
                p.Add(string.Concat("@", nameof(PaymentTransactionEntity.DateModified)), DateTime.Now);

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

                        //add history for above changes
                        if ((paymentTransactionEntity.InternalStatus.HasValue
                            && paymentTransactionEntity.InternalStatus == PaymentInternalStatus.Paid) &&
                   (paymentTransactionEntity.InternalType.HasValue
                   && paymentTransactionEntity.InternalType ==
                    PaymentInternalType.CancelingBooking) &&
                    paymentTransactionEntity.LockerTransactionsId.HasValue)
                        {
                            var cancelBookingParams = new DynamicParameters();
                            cancelBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.LockerTransactionsId)), paymentTransactionEntity.LockerTransactionsId);
                            cancelBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.BookingStatus)), BookingTransactionStatus.Cancelled);
                            cancelBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.DateModified)), DateTime.Now);
                            cancelBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.CancelledDate)), DateTime.Now);

                            var cancelBookingSql = BuildCancelLockerBooking();
                            ret = await conn.ExecuteAsync(cancelBookingSql, cancelBookingParams, transaction);
                            if (ret > 0)
                            {
                                var lockerBookingHistoryParam = new DynamicParameters();
                                var description = new
                                {
                                    Action = "CancelLockerBooking",
                                    CancelAfter = "WebHook-Notification",
                                    NewBookingStatus = BookingTransactionStatus.Cancelled
                                };
                                lockerBookingHistoryParam.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.LockerTransactionsId)), paymentTransactionEntity.LockerTransactionsId);
                                lockerBookingHistoryParam.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.CreatedDate)), DateTime.Now);
                                lockerBookingHistoryParam.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.Description)), JsonSerializer.Serialize(description));

                                var lockerBookingHistorySql = BuildLockerBookingHistory();
                                await conn.ExecuteAsync(lockerBookingHistorySql, lockerBookingHistoryParam, transaction);
                                ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
                            }
                        }

                        if ((paymentTransactionEntity.InternalStatus.HasValue
                            && paymentTransactionEntity.InternalStatus == PaymentInternalStatus.Paid) &&
                          (paymentTransactionEntity.InternalType.HasValue && paymentTransactionEntity.InternalType ==
                           PaymentInternalType.ExtendingBooking) &&
                           paymentTransactionEntity.LockerTransactionsId.HasValue)
                        {
                            var extendBookingParams = new DynamicParameters();
                            extendBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.LockerTransactionsId)), paymentTransactionEntity.LockerTransactionsId);
                            extendBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.DateModified)), DateTime.Now);
                            extendBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.StoragePeriodEnd)), paymentTransactionEntity.NewStoragePeriodEndDate);
                            extendBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.NewStoragePeriodEndDate)), null);

                            var extendBookingSql = BuildExtendLockerBooking();
                            ret = await conn.ExecuteAsync(extendBookingSql, extendBookingParams, transaction);
                            if (ret > 0)
                            {
                                var lockerBookingHistoryParam = new DynamicParameters();
                                var description = new
                                {
                                    Action = "ExtendLockerBooking",
                                    CancelAfter = "WebHook-Notification",
                                    StoragePeriodEndDate = paymentTransactionEntity.NewStoragePeriodEndDate
                                };
                                lockerBookingHistoryParam.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.LockerTransactionsId)), paymentTransactionEntity.LockerTransactionsId);
                                lockerBookingHistoryParam.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.CreatedDate)), DateTime.Now);
                                lockerBookingHistoryParam.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.Description)), JsonSerializer.Serialize(description));

                                var lockerBookingHistorySql = BuildLockerBookingHistory();
                                await conn.ExecuteAsync(lockerBookingHistorySql, lockerBookingHistoryParam, transaction);
                                ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
                            }
                        }

                        // Update the New Storage Period Date to Locker Booking
                        if ((paymentTransactionEntity.InternalStatus.HasValue
                            && paymentTransactionEntity.InternalStatus == PaymentInternalStatus.Pending)
                            && (paymentTransactionEntity.InternalType.HasValue && paymentTransactionEntity.InternalType ==
                          PaymentInternalType.ExtendingBooking) &&
                          paymentTransactionEntity.LockerTransactionsId.HasValue
                          && paymentTransactionEntity.NewStoragePeriodEndDate.HasValue)
                        {
                            var updateNewStorageEndDateSql = BuildUpdateBookingNewStorageEndDate();
                            var updateStorageEndDateParams = new DynamicParameters();
                            updateStorageEndDateParams.Add(string.Concat("@", nameof(LockerBookingEntity.LockerTransactionsId)), paymentTransactionEntity.LockerTransactionsId);
                            updateStorageEndDateParams.Add(string.Concat("@", nameof(LockerBookingEntity.NewStoragePeriodEndDate)), paymentTransactionEntity.NewStoragePeriodEndDate);
                            updateStorageEndDateParams.Add(string.Concat("@", nameof(LockerBookingEntity.DateModified)), DateTime.Now);
                            ret = await conn.ExecuteAsync(updateNewStorageEndDateSql, updateStorageEndDateParams, transaction);
                            if (ret > 0)
                            {
                                var lockerBookingHistoryParam = new DynamicParameters();
                                var description = new
                                {
                                    Action = "ExtendLockerBooking(Pending)",
                                    CancelAfter = "WebHook-Notification",
                                    NewStoragePeriodEndDate = paymentTransactionEntity.NewStoragePeriodEndDate
                                };
                                lockerBookingHistoryParam.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.LockerTransactionsId)), paymentTransactionEntity.LockerTransactionsId);
                                lockerBookingHistoryParam.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.CreatedDate)), DateTime.Now);
                                lockerBookingHistoryParam.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.Description)), JsonSerializer.Serialize(description));

                                var lockerBookingHistorySql = BuildLockerBookingHistory();
                                await conn.ExecuteAsync(lockerBookingHistorySql, lockerBookingHistoryParam, transaction);
                                ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
                            }
                        }

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
        string BuildCancelLockerBooking()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerBooking, " SET "));

            sql.Append(string.Concat("  ", nameof(LockerBookingEntity.BookingStatus), "=",
               "@", nameof(LockerBookingEntity.BookingStatus)));
            sql.Append(string.Concat(" , ", nameof(LockerBookingEntity.DateModified), "=",
                "@", nameof(LockerBookingEntity.DateModified)));
            sql.Append(string.Concat(" , ", nameof(LockerBookingEntity.CancelledDate), "=",
               "@", nameof(LockerBookingEntity.CancelledDate)));

            sql.Append(string.Concat(" WHERE ", nameof(LockerBookingEntity.LockerTransactionsId), "=", "@", nameof(LockerBookingEntity.LockerTransactionsId)));

            return sql.ToString();
        }
        string BuildExtendLockerBooking()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerBooking, " SET "));

            sql.Append(string.Concat("  ", nameof(LockerBookingEntity.StoragePeriodEnd), "=",
               "@", nameof(LockerBookingEntity.StoragePeriodEnd)));
            sql.Append(string.Concat(" , ", nameof(LockerBookingEntity.DateModified), "=",
                "@", nameof(LockerBookingEntity.DateModified)));
            sql.Append(string.Concat(" , ", nameof(LockerBookingEntity.NewStoragePeriodEndDate), "=",
                "@", nameof(LockerBookingEntity.NewStoragePeriodEndDate)));

            sql.Append(string.Concat(" WHERE ", nameof(LockerBookingEntity.LockerTransactionsId), "=", "@", nameof(LockerBookingEntity.LockerTransactionsId)));

            return sql.ToString();
        }
        string BuildUpdateBookingNewStorageEndDate()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerBooking, " SET "));

            sql.Append(string.Concat("  ", nameof(LockerBookingEntity.NewStoragePeriodEndDate), "=",
               "@", nameof(LockerBookingEntity.NewStoragePeriodEndDate)));
            sql.Append(string.Concat(" , ", nameof(LockerBookingEntity.DateModified), "=",
                "@", nameof(LockerBookingEntity.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(LockerBookingEntity.LockerTransactionsId), "=", "@", nameof(LockerBookingEntity.LockerTransactionsId)));

            return sql.ToString();
        }
        string BuildLockerBookingHistory()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.LockerBookingHistory));

            sql.Append(" (");
            sql.Append(nameof(LockerBookingHistoryEntity.LockerTransactionsId));
            sql.Append($", {nameof(LockerBookingHistoryEntity.CreatedDate)}");
            sql.Append($", {nameof(LockerBookingHistoryEntity.Description)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(LockerBookingHistoryEntity.LockerTransactionsId)}");
            sql.Append($", @{nameof(LockerBookingHistoryEntity.CreatedDate)}");
            sql.Append($", @{nameof(LockerBookingHistoryEntity.Description)}");
            sql.Append(")");

            return sql.ToString();
        }
        public async Task<PaymentTransactionEntity> GetPaymentTransaction(string referenceId)
        {
            var sql = BuildSelectGetPaymentTransaction();
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(PaymentTransactionEntity.TransactionId)), referenceId);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<PaymentTransactionEntity>(sql, p);
                return dbModel.FirstOrDefault();
            }
        }
    }
}

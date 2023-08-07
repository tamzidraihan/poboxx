using Dapper;
using DapperExtensions;
using Hangfire.Annotations;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Entities.Locker;
using SmartBox.Business.Core.Entities.Pricing;
using SmartBox.Business.Core.Models.Booking;
using SmartBox.Business.Core.Models.Locker;
using SmartBox.Business.Core.Models.User;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static NPOI.HSSF.Util.HSSFColor;
using static SmartBox.Business.Shared.GlobalEnums;

namespace SmartBox.Infrastructure.Data.Repository.Locker
{
    public class LockerRepository : GenericRepositoryBase<LockerDetailEntity, LockerRepository>, ILockerRepository
    {
        public LockerRepository(IDatabaseHelper databaseHelper, ILogger<LockerRepository> logger) : base(databaseHelper, logger)
        {
        }

        string BuildInsertScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.LockerDetail));

            sql.Append(" (");
            sql.Append(nameof(LockerDetailEntity.CabinetId));
            sql.Append($", {nameof(LockerDetailEntity.LockerTypeId)}");
            sql.Append($", {nameof(LockerDetailEntity.BoardNumber)}");
            sql.Append($", {nameof(LockerDetailEntity.LockerNumber)}");
            sql.Append($", {nameof(LockerDetailEntity.OpenCommand)}");
            sql.Append($", {nameof(LockerDetailEntity.GetStatusCommand)}");
            sql.Append($", {nameof(LockerDetailEntity.PositionId)}");
            sql.Append($", {nameof(LockerDetailEntity.CompanyId)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(LockerDetailEntity.CabinetId)}");
            sql.Append($", @{nameof(LockerDetailEntity.LockerTypeId)}");
            sql.Append($", @{nameof(LockerDetailEntity.BoardNumber)}");
            sql.Append($", @{nameof(LockerDetailEntity.LockerNumber)}");
            sql.Append($", @{nameof(LockerDetailEntity.OpenCommand)}");
            sql.Append($", @{nameof(LockerDetailEntity.GetStatusCommand)}");
            sql.Append($", @{nameof(LockerDetailEntity.PositionId)}");
            sql.Append($", @{nameof(LockerDetailEntity.CompanyId)}");
            sql.Append(")");

            return sql.ToString();
        }

        string BuildInsertBookingScript(bool dropOffCodeRequired = false, bool dropOffQRCodeRequired = false, bool pickUpCodeRequired = false, bool pickUpQRCodeRequired = false)
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.LockerBooking));

            sql.Append(" (");
            sql.Append(nameof(LockerBookingEntity.LockerDetailId));
            sql.Append($", {nameof(LockerBookingEntity.UserKeyId)}");
            sql.Append($", {nameof(LockerBookingEntity.StoragePeriodStart)}");
            sql.Append($", {nameof(LockerBookingEntity.StoragePeriodEnd)}");
            sql.Append($", {nameof(LockerBookingEntity.SenderName)}");
            sql.Append($", {nameof(LockerBookingEntity.SenderMobile)}");
            sql.Append($", {nameof(LockerBookingEntity.SenderEmailAddress)}");
            sql.Append($", {nameof(LockerBookingEntity.ReceiverName)}");
            sql.Append($", {nameof(LockerBookingEntity.ReceiverEmailAddress)}");
            sql.Append($", {nameof(LockerBookingEntity.ReceiverPhoneNumber)}");
            if (dropOffCodeRequired)
                sql.Append($", {nameof(LockerBookingEntity.DropOffCode)}");
            if (dropOffQRCodeRequired)
                sql.Append($", {nameof(LockerBookingEntity.DropOffQRCode)}");
            if (pickUpCodeRequired)
                sql.Append($", {nameof(LockerBookingEntity.PickUpCode)}");
            if (pickUpQRCodeRequired)
                sql.Append($", {nameof(LockerBookingEntity.PickUpQRCode)}");
            sql.Append($", {nameof(LockerBookingEntity.TotalPrice)}");
            sql.Append($", {nameof(LockerBookingEntity.PaymentMethodId)}");
            sql.Append($", {nameof(LockerBookingEntity.PaymentReference)}");
            sql.Append($", {nameof(LockerBookingEntity.BookingStatus)}");
            sql.Append($", {nameof(LockerBookingEntity.AccessPlan)}");
            sql.Append($", {nameof(LockerBookingEntity.PackageImage)}");
            sql.Append($", {nameof(LockerBookingEntity.IsSubscriptionBooking)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(LockerBookingEntity.LockerDetailId)}");
            sql.Append($", @{nameof(LockerBookingEntity.UserKeyId)}");
            sql.Append($", @{nameof(LockerBookingEntity.StoragePeriodStart)}");
            sql.Append($", @{nameof(LockerBookingEntity.StoragePeriodEnd)}");
            sql.Append($", @{nameof(LockerBookingEntity.SenderName)}");
            sql.Append($", @{nameof(LockerBookingEntity.SenderMobile)}");
            sql.Append($", @{nameof(LockerBookingEntity.SenderEmailAddress)}");
            sql.Append($", @{nameof(LockerBookingEntity.ReceiverName)}");
            sql.Append($", @{nameof(LockerBookingEntity.ReceiverEmailAddress)}");
            sql.Append($", @{nameof(LockerBookingEntity.ReceiverPhoneNumber)}");
            if (dropOffCodeRequired)
                sql.Append($", @{nameof(LockerBookingEntity.DropOffCode)}");
            if (dropOffQRCodeRequired)
                sql.Append($", @{nameof(LockerBookingEntity.DropOffQRCode)}");
            if (pickUpCodeRequired)
                sql.Append($", @{nameof(LockerBookingEntity.PickUpCode)}");
            if (pickUpQRCodeRequired)
                sql.Append($", @{nameof(LockerBookingEntity.PickUpQRCode)}");

            sql.Append($", @{nameof(LockerBookingEntity.TotalPrice)}");
            sql.Append($", @{nameof(LockerBookingEntity.PaymentMethodId)}");
            sql.Append($", @{nameof(LockerBookingEntity.PaymentReference)}");
            sql.Append($", @{nameof(LockerBookingEntity.BookingStatus)}");
            sql.Append($", @{nameof(LockerBookingEntity.AccessPlan)}");
            sql.Append($", @{nameof(LockerBookingEntity.PackageImage)}");
            sql.Append($", @{nameof(LockerBookingEntity.IsSubscriptionBooking)}");
            sql.Append(");");
            sql.Append(" SELECT LAST_INSERT_ID();");
            return sql.ToString();
        }
        string BuildInsertBookingLockerHistoryScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.ReassignedBookingLockerHistory));

            sql.Append(" (");
            sql.Append(nameof(ReassignedBookingLockerEntity.ReassignedByAdminUser));
            sql.Append($", {nameof(ReassignedBookingLockerEntity.lockerTransactionsId)}");
            sql.Append($", {nameof(ReassignedBookingLockerEntity.OldLockerDetailId)}");
            sql.Append($", {nameof(ReassignedBookingLockerEntity.NewLockerDetailId)}");
            sql.Append($", {nameof(ReassignedBookingLockerEntity.ReassignmentDate)}");
            sql.Append($", {nameof(ReassignedBookingLockerEntity.ReassignedByCompanyUser)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"@{nameof(ReassignedBookingLockerEntity.ReassignedByAdminUser)}");
            sql.Append($", @{nameof(ReassignedBookingLockerEntity.lockerTransactionsId)}");
            sql.Append($", @{nameof(ReassignedBookingLockerEntity.OldLockerDetailId)}");
            sql.Append($", @{nameof(ReassignedBookingLockerEntity.NewLockerDetailId)}");
            sql.Append($", @{nameof(ReassignedBookingLockerEntity.ReassignmentDate)}");
            sql.Append($", @{nameof(ReassignedBookingLockerEntity.ReassignedByCompanyUser)}");
            sql.Append(");");
            return sql.ToString();
        }
        string BuildSelectActiveLockerScript(int? lockerDetailId, int? cabinetId)
        {

            var query = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.Views.ActiveLockers}");

            if (lockerDetailId.HasValue)
            {
                query.Append($" WHERE {nameof(LockerBookingEntity.LockerDetailId)} = {GlobalDatabaseConstants.QueryParameters.LockerDetailId}");
            }
            else if (cabinetId.HasValue)
            {
                query.Append($" WHERE {nameof(GlobalDatabaseConstants.QueryParameters.CabinetId)} = {GlobalDatabaseConstants.QueryParameters.CabinetId}");
            }


            return query.ToString();
        }

        string BuildSelectBookingScript(int? lockertransactionId)
        {

            var query = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.Views.BookingLockerDetail}");

            if (lockertransactionId.HasValue)
            {
                query.Append($" WHERE {nameof(LockerBookingEntity.LockerTransactionsId)} = {GlobalDatabaseConstants.QueryParameters.LockerTransactionId}");
            }

            return query.ToString();
        }

        string BuildUpdateLockerDetail(LockerDetailEntity lockerDetailEntity)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerDetail, " SET "));
            var hasComma = false;

            if (lockerDetailEntity.CabinetId != 0)
            {
                sql.Append(string.Concat(" ", nameof(LockerDetailEntity.CabinetId), " = ",
                    "@", nameof(LockerDetailEntity.CabinetId)));

                hasComma = true;
            }

            if (lockerDetailEntity.LockerTypeId != 0)
            {
                if (hasComma)
                {
                    sql.Append(string.Concat(", ", nameof(LockerDetailEntity.LockerTypeId), " = ",
                        "@", nameof(LockerDetailEntity.LockerTypeId)));
                }
                else
                {
                    sql.Append(string.Concat(" ", nameof(LockerDetailEntity.LockerTypeId), " = ",
                        "@", nameof(LockerDetailEntity.LockerTypeId)));

                    hasComma = true;
                }
            }
            if (lockerDetailEntity.CompanyId != 0)
            {
                if (hasComma)
                {
                    sql.Append(string.Concat(", ", nameof(LockerDetailEntity.CompanyId), " = ",
                        "@", nameof(LockerDetailEntity.CompanyId)));
                }
                else
                {
                    sql.Append(string.Concat(" ", nameof(LockerDetailEntity.CompanyId), " = ",
                        "@", nameof(LockerDetailEntity.CompanyId)));

                    hasComma = true;
                }
            }

            //if (lockerDetailEntity.LockerNumber.HasText())
            //{
            if (hasComma)
            {
                sql.Append(string.Concat(", ", nameof(LockerDetailEntity.LockerNumber), " = ",
                    "@", nameof(LockerDetailEntity.LockerNumber)));
            }
            else
            {
                sql.Append(string.Concat(" ", nameof(LockerDetailEntity.LockerNumber), " = ",
                    "@", nameof(LockerDetailEntity.LockerNumber)));

                hasComma = true;
            }
            //}

            // if (lockerDetailEntity.BoardNumber.HasText())
            //{
            if (hasComma)
            {
                sql.Append(string.Concat(", ", nameof(LockerDetailEntity.BoardNumber), " = ",
                    "@", nameof(LockerDetailEntity.BoardNumber)));
            }
            else
            {
                sql.Append(string.Concat(" ", nameof(LockerDetailEntity.BoardNumber), " = ",
                    "@", nameof(LockerDetailEntity.BoardNumber)));

                hasComma = true;
            }
            //}

            if (hasComma)
            {
                sql.Append(string.Concat(", ", nameof(LockerDetailEntity.OpenCommand), " = ",
                    "@", nameof(LockerDetailEntity.OpenCommand)));
            }
            else
            {
                sql.Append(string.Concat(" ", nameof(LockerDetailEntity.OpenCommand), " = ",
                    "@", nameof(LockerDetailEntity.OpenCommand)));

                hasComma = true;
            }

            if (hasComma)
            {
                sql.Append(string.Concat(", ", nameof(LockerDetailEntity.GetStatusCommand), " = ",
                    "@", nameof(LockerDetailEntity.GetStatusCommand)));
            }
            else
            {
                sql.Append(string.Concat(" ", nameof(LockerDetailEntity.GetStatusCommand), " = ",
                    "@", nameof(LockerDetailEntity.GetStatusCommand)));

                hasComma = true;
            }

            if (hasComma)
            {
                sql.Append(string.Concat(", ", nameof(LockerDetailEntity.PositionId), " = ",
                    "@", nameof(LockerDetailEntity.PositionId)));
            }
            else
            {
                sql.Append(string.Concat(" ", nameof(LockerDetailEntity.PositionId), " = ",
                    "@", nameof(LockerDetailEntity.PositionId)));

                hasComma = true;
            }

            if (hasComma)
                sql.Append(string.Concat(" WHERE ", nameof(LockerDetailEntity.LockerDetailId), " = ", "@", nameof(LockerDetailEntity.LockerDetailId)));

            return sql.ToString();
        }

        string BuildActivateScriptLockerDetail()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerDetail, " SET "));

            sql.Append(string.Concat(" ", nameof(LockerDetailEntity.IsAvailable), " = ", "@", nameof(LockerDetailEntity.IsAvailable)));

            sql.Append(string.Concat(" WHERE ", nameof(LockerDetailEntity.LockerDetailId), " = ", "@", nameof(LockerDetailEntity.LockerDetailId)));

            return sql.ToString();
        }
        public async Task<List<LockerDetailEntity>> GetActiveLocker(int? lockerDetailId = null, int? cabinetId = null, int? companyId = null)
        {
            var builder = new SqlBuilder();
            DynamicParameters parameters = new();
            builder.Select("*");

            if (lockerDetailId != null)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.LockerDetailId, lockerDetailId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(LockerDetailEntity.LockerDetailId) + " = " + GlobalDatabaseConstants.QueryParameters.LockerDetailId);
            }
            if (companyId.HasValue && companyId.Value > 0)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.CompanyId, companyId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(LockerDetailEntity.CompanyId) + " = " + GlobalDatabaseConstants.QueryParameters.CompanyId);
            }

            if (cabinetId != null)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.CabinetId, cabinetId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(LockerDetailEntity.CabinetId) + " = " + GlobalDatabaseConstants.QueryParameters.CabinetId);
            }

            //var sql = BuildSelectActiveLockerScript(lockerDetailId, null);
            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.Views.ActiveLockers} /**where**/ ");
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<LockerDetailEntity>(builderTemplate.RawSql, parameters);
                //var dbModel = await conn.QueryAsync<LockerDetailEntity>(sql, p);

                return dbModel.ToList();
            }
        }
        public async Task<int> ExtendLockerBooking(ExtendLockerBookingModel extendLockerBookingModel, LockerBookingEntity existingBooking, string userKeyId)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(LockerBookingEntity.LockerTransactionsId)), existingBooking.LockerTransactionsId);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.TotalPrice)), extendLockerBookingModel.NewTotalPrice);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.StoragePeriodEnd)), extendLockerBookingModel.NewStorageEndDate);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.DateModified)), DateTime.Now);

            var sql = BuildExtendLockerBooking();

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
                        var description = new
                        {
                            Action = "ExtendLockerBooking",
                            UserKeyId = userKeyId,
                            OldTotalPrice = existingBooking.TotalPrice,
                            NewTotalPrice = extendLockerBookingModel.NewTotalPrice,
                            OldStoragePeriodEnd = existingBooking.StoragePeriodEnd,
                            NewStoragePeriodEnd = extendLockerBookingModel.NewStorageEndDate,
                        };
                        p.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.LockerTransactionsId)), existingBooking.LockerTransactionsId);
                        p.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.CreatedDate)), DateTime.Now);
                        p.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.Description)), JsonSerializer.Serialize(description));

                        sql = BuildLockerBookingHistory();
                        await conn.ExecuteAsync(sql, p, transaction);
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
        public async Task<List<LockerDetailEntity>> GetActiveLockerStatus(int? lockerDetailId = null, int? cabinetId = null, int? companyId = null)
        {
            var builder = new SqlBuilder();
            DynamicParameters parameters = new();
            builder.Select("*");

            if (lockerDetailId != null)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.LockerDetailId, lockerDetailId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(LockerDetailEntity.LockerDetailId) + " = " + GlobalDatabaseConstants.QueryParameters.LockerDetailId);
            }
            if (companyId.HasValue && companyId.Value > 0)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.CompanyId, companyId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(LockerDetailEntity.CompanyId) + " = " + GlobalDatabaseConstants.QueryParameters.CompanyId);
            }

            if (cabinetId != null)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.CabinetId, cabinetId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(LockerDetailEntity.CabinetId) + " = " + GlobalDatabaseConstants.QueryParameters.CabinetId);
            }
            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.Views.ActiveLockersStatus} /**where**/ ");
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<LockerDetailEntity>(builderTemplate.RawSql, parameters);

                return dbModel.ToList();
            }
        }
        public async Task<List<ReassignedBookingLockerEntity>> GetReassignedBookingLockerHistory(int? lockerDetailId = null, int? lockerTransactionsId = null, int? adminUserId = null, int? companyUserId = null, int? companyId = null)
        {
            var builder = new SqlBuilder();
            DynamicParameters parameters = new();
            builder.Select("*");

            if (lockerTransactionsId.HasValue && lockerTransactionsId.Value > 0)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.LockerTransactionsId, lockerTransactionsId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(ReassignedBookingLockerEntity.lockerTransactionsId) + " = " + GlobalDatabaseConstants.QueryParameters.LockerTransactionsId);
            }
            if (lockerDetailId.HasValue && lockerDetailId.Value > 0)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.OldLockerDetailId, lockerDetailId, DbType.Int32, ParameterDirection.Input);
                parameters.Add(GlobalDatabaseConstants.QueryParameters.NewLockerDetailId, lockerDetailId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(ReassignedBookingLockerEntity.OldLockerDetailId) + " = " + GlobalDatabaseConstants.QueryParameters.OldLockerDetailId + " OR " + nameof(ReassignedBookingLockerEntity.NewLockerDetailId) + " = " + GlobalDatabaseConstants.QueryParameters.NewLockerDetailId);
            }

            if (adminUserId.HasValue && adminUserId.Value > 0)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.ReassignedByAdminUser, adminUserId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(ReassignedBookingLockerEntity.ReassignedByAdminUser) + " = " + GlobalDatabaseConstants.QueryParameters.ReassignedByAdminUser);
            }
            if (companyUserId.HasValue && companyUserId.Value > 0)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.ReassignedByCompanyUser, companyUserId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(ReassignedBookingLockerEntity.ReassignedByCompanyUser) + " = " + GlobalDatabaseConstants.QueryParameters.ReassignedByCompanyUser);
            }
            if (companyId.HasValue && companyId.Value > 0)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.CompanyId, companyId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(ReassignedBookingLockerEntity.CompanyId) + " = " + GlobalDatabaseConstants.QueryParameters.CompanyId);
            }
            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.Views.ReassignedBookingLockerHistory} /**where**/ ");
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<ReassignedBookingLockerEntity>(builderTemplate.RawSql, parameters)).ToList();
            }
        }
        public async Task<List<LockerDetailEntity>> GetLockerDetail(int? lockerDetailId = null, string lockerNumber = null,
                                                                    int? cabinetId = null, int? companyId = null)
        {
            var builder = new SqlBuilder();
            DynamicParameters parameters = new();
            builder.Select("*");

            if (lockerDetailId != null)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.LockerDetailId, lockerDetailId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(LockerDetailEntity.LockerDetailId) + " = " + GlobalDatabaseConstants.QueryParameters.LockerDetailId);
            }

            if (companyId.HasValue)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.CompanyId, companyId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(LockerDetailEntity.CompanyId) + " = " + GlobalDatabaseConstants.QueryParameters.CompanyId);
            }

            if (lockerNumber.HasText())
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.LockerNumber, lockerNumber.Trim(), DbType.String, ParameterDirection.Input);
                builder.Where(nameof(LockerDetailEntity.LockerNumber) + " = " + GlobalDatabaseConstants.QueryParameters.LockerNumber);
            }

            if (cabinetId != null)
            {
                parameters.Add(GlobalDatabaseConstants.QueryParameters.CabinetId, cabinetId, DbType.Int32, ParameterDirection.Input);
                builder.Where(nameof(LockerDetailEntity.CabinetId) + " = " + GlobalDatabaseConstants.QueryParameters.CabinetId);
            }

            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.Views.Locker} /**where**/ ");

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<LockerDetailEntity>(builderTemplate.RawSql, parameters);
                return dbModel.ToList();
            }
        }

        public async Task<LockerBookingEntity> GetLockerBooking(int lockertransactionId)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.LockerTransactionId, lockertransactionId);
            try
            {
                var sql = BuildSelectBookingScript(lockertransactionId);
                using (IDbConnection conn = this._databaseHelper.GetConnection())
                {
                    var dbModel = await conn.QueryAsync<LockerBookingEntity>(sql, p);

                    return dbModel.FirstOrDefault();
                }
            }catch (Exception ex)
            {
                _logger.LogError("Database error: " + ex.Message);
            }
            return null;
        }
        public async Task<List<ActiveLockerBookingEntity>> GetExpiredLockerBookings(int? lockertransactionId = null)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.LockerTransactionId, lockertransactionId);

            var procedure = GlobalDatabaseConstants.StoredProcedures.ExpiredLockerBookings;
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    var dbModel = await conn.QueryAsync<ActiveLockerBookingEntity>(procedure, p, commandType: CommandType.StoredProcedure);

                    return dbModel.ToList();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return new List<ActiveLockerBookingEntity>();
                }
            }
        }


        //public async Task<List<LockerDetailEntity>> GetLockers(int cabinetId)
        //{
        //    var p = new DynamicParameters();

        //    p.Add(Constants.QueryParameters.CabinetId, cabinetId);

        //    var sql = BuildSelectActiveLockerScript(null,cabinetId);
        //    using (IDbConnection conn = this._databaseHelper.GetConnection())
        //    {
        //        var dbModel = await conn.QueryAsync<LockerDetailEntity>(sql, p);

        //        return dbModel.ToList();
        //    }
        //}

        public async Task UpdateNotifiedLockerBookings(List<int> lockerTransactionIds)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(LockerBookingEntity.IsNotified)), true);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.NotificationDate)), DateTime.Now);

            string sql = BuildUpdateNotifiedLockerBookings(lockerTransactionIds);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    await conn.ExecuteAsync(sql, p, transaction);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }


        string BuildUpdateNotifiedLockerBookings(List<int> lockerTransactionIds)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerBooking, " SET "));

            sql.Append(string.Concat(" ", nameof(LockerBookingEntity.IsNotified), " = ",
                "@", nameof(LockerBookingEntity.IsNotified)));
            sql.Append(string.Concat(" , ", nameof(LockerBookingEntity.NotificationDate), " = ",
               "@", nameof(LockerBookingEntity.NotificationDate)));

            sql.Append(string.Concat(" WHERE ", nameof(LockerBookingEntity.LockerTransactionsId), "  IN(", string.Join(',', lockerTransactionIds), ")"));

            return sql.ToString();
        }

        public async Task<int> Delete(int id)
        {
            var entity = new LockerZoneEntity { PositionId = id };
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(entity.PositionId)), id);
            string sql = BuildDeleteCommand(entity);

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
        string BuildDeleteCommand(LockerZoneEntity model)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.LockerZones));

            sql.Append(string.Concat(" WHERE ", nameof(model.PositionId), " = ", "@", nameof(model.PositionId)));

            return sql.ToString();

        }
        public async Task<int> SaveLocker(LockerDetailEntity lockerDetailEntity)
        {
            var p = new DynamicParameters();
            bool isInsert = true;
            if (lockerDetailEntity.CompanyId == 0)
                lockerDetailEntity.CompanyId = null;
            p.Add(string.Concat("@", nameof(LockerDetailEntity.LockerDetailId)), lockerDetailEntity.LockerDetailId);
            p.Add(string.Concat("@", nameof(lockerDetailEntity.CabinetId)), lockerDetailEntity.CabinetId);
            p.Add(string.Concat("@", nameof(lockerDetailEntity.LockerTypeId)), lockerDetailEntity.LockerTypeId);
            p.Add(string.Concat("@", nameof(lockerDetailEntity.LockerNumber)), lockerDetailEntity.LockerNumber);
            p.Add(string.Concat("@", nameof(lockerDetailEntity.BoardNumber)), lockerDetailEntity.BoardNumber);
            p.Add(string.Concat("@", nameof(lockerDetailEntity.OpenCommand)), lockerDetailEntity.OpenCommand);
            p.Add(string.Concat("@", nameof(lockerDetailEntity.GetStatusCommand)), lockerDetailEntity.GetStatusCommand);
            p.Add(string.Concat("@", nameof(lockerDetailEntity.PositionId)), lockerDetailEntity.PositionId);
            p.Add(string.Concat("@", nameof(lockerDetailEntity.CompanyId)), lockerDetailEntity.CompanyId);
            string sql;
            if (lockerDetailEntity.LockerDetailId == 0)
                sql = BuildInsertScript();
            else
            {
                sql = BuildUpdateLockerDetail(lockerDetailEntity);
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
        //public async Task<bool> UpdateLockerDetail(int lockerDetailId, int lockerTransactionId, string qrCode, string pinCode, int isAvailable)
        //{

        //    var p = new DynamicParameters();

        //    p.Add(string.Concat("@", nameof(LockerDetailEntity.LockerDetailId)), lockerDetailId);
        //    p.Add(string.Concat("@", nameof(LockerDetailEntity.LockerTransactionsId)), lockerTransactionId);
        //    p.Add(string.Concat("@", nameof(LockerDetailEntity.QRCode)), qrCode);
        //    p.Add(string.Concat("@", nameof(LockerDetailEntity.PinCode)), pinCode);
        //    p.Add(string.Concat("@", nameof(LockerDetailEntity.IsAvailable)), isAvailable);

        //    var sql = BuildUpdateLockerDetail(lockerTransactionId, qrCode, pinCode, isAvailable);

        //    using (IDbConnection conn = this._databaseHelper.GetConnection())
        //    {
        //        conn.Open();
        //        IDbTransaction transaction = conn.BeginTransaction();

        //        try
        //        {
        //            var ret = await conn.ExecuteAsync(sql, p, transaction);

        //            ret = ret > 0
        //                ? GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated
        //                : GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;

        //            transaction.Commit();
        //            return true;
        //        }
        //        catch (Exception e) 
        //        {
        //            transaction.Rollback();
        //            _logger.LogError(e.Message);
        //            return false;
        //        }
        //    }
        //}

        public async Task<List<AvailableLockerEntity>> GetAvailableLocker(int? cabinetLocationId, int? lockerTypeId, int? selectedMonth, int? selectedYear,
                                                                         DateTime? startDate, DateTime? endDate,
                                                                         bool isOrderByLockerNumber, int? cabinetId,
                                                                         int? currentPage, int? pageSize, int? positionId = null)
        {
            var p = new DynamicParameters();

            p.Add(GlobalDatabaseConstants.QueryParameters.CabinetLocationId, cabinetLocationId);
            p.Add(GlobalDatabaseConstants.QueryParameters.LockerTypeId, lockerTypeId);
            p.Add(GlobalDatabaseConstants.QueryParameters.SelectedMonth, selectedMonth);
            p.Add(GlobalDatabaseConstants.QueryParameters.SelectedYear, selectedYear);
            p.Add(GlobalDatabaseConstants.QueryParameters.StartDate, startDate);
            p.Add(GlobalDatabaseConstants.QueryParameters.EndDate, endDate);
            p.Add(GlobalDatabaseConstants.QueryParameters.IsOrderByLockerNumber, isOrderByLockerNumber);
            p.Add(GlobalDatabaseConstants.QueryParameters.CabinetId, cabinetId);
            p.Add(GlobalDatabaseConstants.QueryParameters.PageSize, pageSize);
            p.Add(GlobalDatabaseConstants.QueryParameters.CurrentPage, currentPage);
            p.Add(GlobalDatabaseConstants.QueryParameters.PositionId, positionId);

            var procedure = GlobalDatabaseConstants.StoredProcedures.GetAvailableLockers;

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    var dbModel = await conn.QueryAsync<AvailableLockerEntity>(procedure, p, commandType: CommandType.StoredProcedure);

                    return dbModel.ToList();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return new List<AvailableLockerEntity>();
                }
            }
        }

        public async Task<List<UpdatedAvailableLockerEntity>> GetUpdatedAvailableLocker(int? cabinetLocationId, int? lockerTypeId,
                                                                         DateTime? startDate, DateTime? endDate,
                                                                         bool isOrderByLockerNumber, int? cabinetId,
                                                                         int? currentPage, int? pageSize,
                                                                         int? positionId = null)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.CabinetLocationId, cabinetLocationId);
            p.Add(GlobalDatabaseConstants.QueryParameters.LockerTypeId, lockerTypeId);
            p.Add(GlobalDatabaseConstants.QueryParameters.StartDate, startDate);
            p.Add(GlobalDatabaseConstants.QueryParameters.EndDate, endDate);
            p.Add(GlobalDatabaseConstants.QueryParameters.IsOrderByLockerNumber, isOrderByLockerNumber);
            p.Add(GlobalDatabaseConstants.QueryParameters.CabinetId, cabinetId);
            p.Add(GlobalDatabaseConstants.QueryParameters.PageSize, pageSize);
            p.Add(GlobalDatabaseConstants.QueryParameters.CurrentPage, currentPage);
            p.Add(GlobalDatabaseConstants.QueryParameters.PositionId, positionId);

            var procedure = GlobalDatabaseConstants.StoredProcedures.GetUpdatedAvailableLockers;

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    var dbModel = await conn.QueryAsync<UpdatedAvailableLockerEntity>(procedure, p, commandType: CommandType.StoredProcedure);

                    return dbModel.ToList();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return new List<UpdatedAvailableLockerEntity>();
                }
            }
        }
        public async Task<UpdatedAvailableLockerEntity> GetBookingUpdatedPrice(int lockerTransactionId, int lockerDetailId, DateTime endDate)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.LockerTransactionsId, lockerTransactionId);
            p.Add(GlobalDatabaseConstants.QueryParameters.LockerDetailId, lockerDetailId);
            p.Add(GlobalDatabaseConstants.QueryParameters.EndDate, endDate);

            var procedure = GlobalDatabaseConstants.StoredProcedures.GetBookingUpdatedPrice;

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    var dbModel = await conn.QueryAsync<UpdatedAvailableLockerEntity>(procedure, p, commandType: CommandType.StoredProcedure);

                    return dbModel.FirstOrDefault();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return new UpdatedAvailableLockerEntity();
                }
            }
        }
        public async Task<List<BookingTransactionsViewModel>> GetBookingTransactions(DateTime? startDate, DateTime? endDate, int? companyId = null,
                                                                                int? currentPage = null, int? pageSize = null, BookingTransactionStatus? bookingStatus = null)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.CompanyId, companyId);
            p.Add(GlobalDatabaseConstants.QueryParameters.StartDate, startDate);
            p.Add(GlobalDatabaseConstants.QueryParameters.EndDate, endDate);
            p.Add(GlobalDatabaseConstants.QueryParameters.PageSize, pageSize);
            p.Add(GlobalDatabaseConstants.QueryParameters.CurrentPage, currentPage);
            p.Add(GlobalDatabaseConstants.QueryParameters.BookingStatus, bookingStatus.HasValue ? (int)bookingStatus.Value : null);
            p.Add(GlobalDatabaseConstants.QueryParameters.UserKeyId, null);
            var procedure = GlobalDatabaseConstants.StoredProcedures.GetBookingTransactions;
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    var dbModel = await conn.QueryAsync<BookingTransactionsViewModel>(procedure, p, commandType: CommandType.StoredProcedure);

                    return dbModel.ToList();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return new List<BookingTransactionsViewModel>();
                }
            }
        }

        public async Task<List<BookingTransactionsViewModel>> GetUserBookingTransactions(string userKeyId, DateTime? startDate, DateTime? endDate, int? companyId = null,
                                                                        int? currentPage = null, int? pageSize = null, BookingTransactionStatus? bookingStatus = null)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.CompanyId, companyId);
            p.Add(GlobalDatabaseConstants.QueryParameters.StartDate, startDate);
            p.Add(GlobalDatabaseConstants.QueryParameters.EndDate, endDate);
            p.Add(GlobalDatabaseConstants.QueryParameters.PageSize, pageSize);
            p.Add(GlobalDatabaseConstants.QueryParameters.CurrentPage, currentPage);
            p.Add(GlobalDatabaseConstants.QueryParameters.BookingStatus, bookingStatus.HasValue ? (int)bookingStatus.Value : null);
            p.Add(GlobalDatabaseConstants.QueryParameters.UserKeyId, userKeyId);

            var procedure = GlobalDatabaseConstants.StoredProcedures.GetBookingTransactions;
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    var dbModel = await conn.QueryAsync<BookingTransactionsViewModel>(procedure, p, commandType: CommandType.StoredProcedure);

                    return dbModel.ToList();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return new List<BookingTransactionsViewModel>();
                }
            }
        }

        public async Task<int> SaveLockerBooking(LockerBookingEntity lockerBookingEntity)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(LockerBookingEntity.LockerDetailId)), lockerBookingEntity.LockerDetailId);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.UserKeyId)), lockerBookingEntity.UserKeyId);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.StoragePeriodStart)), lockerBookingEntity.StoragePeriodStart);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.StoragePeriodEnd)), lockerBookingEntity.StoragePeriodEnd);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.SenderName)), lockerBookingEntity.SenderName);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.SenderMobile)), lockerBookingEntity.SenderMobile);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.SenderEmailAddress)), lockerBookingEntity.SenderEmailAddress);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.ReceiverName)), lockerBookingEntity.ReceiverName);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.ReceiverEmailAddress)), lockerBookingEntity.ReceiverEmailAddress);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.ReceiverPhoneNumber)), lockerBookingEntity.ReceiverPhoneNumber);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.TotalPrice)), lockerBookingEntity.TotalPrice);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.PaymentMethodId)), lockerBookingEntity.PaymentMethodId);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.PaymentReference)), lockerBookingEntity.PaymentReference);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.BookingStatus)), lockerBookingEntity.BookingStatus);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.IsSubscriptionBooking)), lockerBookingEntity.IsSubscriptionBooking);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.AccessPlan)), lockerBookingEntity.AccessPlan);

            //For DropOff
            if (lockerBookingEntity.BookingStatus == 1)
            {
                p.Add(string.Concat("@", nameof(LockerBookingEntity.DropOffCode)), lockerBookingEntity.DropOffCode);
                p.Add(string.Concat("@", nameof(LockerBookingEntity.DropOffQRCode)), lockerBookingEntity.DropOffQRCode);
            }
            //For Pickup
            else if (lockerBookingEntity.BookingStatus == 2)
            {
                p.Add(string.Concat("@", nameof(LockerBookingEntity.PickUpCode)), lockerBookingEntity.PickUpCode);
                p.Add(string.Concat("@", nameof(LockerBookingEntity.PickUpQRCode)), lockerBookingEntity.PickUpQRCode);
            }
            var sql = BuildInsertBookingScript(!string.IsNullOrEmpty(lockerBookingEntity.DropOffCode), !string.IsNullOrEmpty(lockerBookingEntity.DropOffQRCode), !string.IsNullOrEmpty(lockerBookingEntity.PickUpCode), !string.IsNullOrEmpty(lockerBookingEntity.PickUpQRCode));

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteScalarAsync<int>(sql, p, transaction);

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

        public async Task<int> UpdatedOTP(BookingTransactionStatus bookingStatus, string OTPCode, string qrCode, int lockerTransactionId, bool isSubscriptionBooking = false)
        {
            var sql = string.Empty;
            var p = new DynamicParameters();
            p.Add("@otp", OTPCode);
            p.Add("@qrCode", qrCode);
            p.Add("@lockerTransactionId", lockerTransactionId);
            p.Add("@dateToday", DateTime.Now);
            p.Add("@isSubscriptionBooking", isSubscriptionBooking);
            p.Add("@bookingStatus", bookingStatus);


            if (bookingStatus == BookingTransactionStatus.ForPickUp)
            {
                sql = string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerBooking, " SET ", "PickupDate = @dateToday,", "BookingStatus=@bookingStatus, ", "IsSubscriptionBooking=@isSubscriptionBooking, ",
                                    "PickUpCode=@otp,", " PickUpQRCode=@qrCode ",
                                    " WHERE LockerTransactionsId = @lockerTransactionId ");
            }
            else if (bookingStatus == BookingTransactionStatus.Completed)
            {
                sql = string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerBooking, " SET ", "PickupDate = @dateToday,", " BookingStatus =@bookingStatus",
                                   " WHERE LockerTransactionsId = @lockerTransactionId ");
            }
            else if (bookingStatus == BookingTransactionStatus.ForDropOff)
            {
                sql = string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerBooking, " SET ", "DropOffDate = @dateToday,", " BookingStatus =@bookingStatus,  ",
                                    " DropOffCode = @otp, DropOffQRCode = @qrCode",
                                    " WHERE LockerTransactionsId = @lockerTransactionId");
            }

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);

                    ret = ret > 0
                        ? GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated
                        : GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;

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

        public async Task<LockerBookingEntity> GetDropOffOTP(string OTPCode)
        {
            var p = new DynamicParameters();
            p.Add("@otp", OTPCode);


            var sql = string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.BookingLockerDetail,
                                    " WHERE (DropOffCode = @otp OR DropOffQRCode = @otp)");

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<LockerBookingEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }

        }

        public async Task<LockerBookingEntity> GetOTP(string OTPCode)
        {
            var p = new DynamicParameters();
            p.Add("@otp", OTPCode);
            p.Add("@dropStatus", 1);
            p.Add("@pickUpStatus", 2);

            var sql = string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.BookingLockerDetail,
                                    " WHERE (DropOffCode = @otp OR PickUpCode=@otp OR DropOffQRCode = @otp OR PickupQRCode = @otp) AND (BookingStatus = @dropStatus OR BookingStatus = @pickUpStatus)");

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<LockerBookingEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }

        }

        public async Task<LockerBookingEntity> GetOTPStatus(string OTPCode)
        {
            var p = new DynamicParameters();
            p.Add("@otp", OTPCode);


            var sql = string.Concat("SELECT * FROM ", GlobalDatabaseConstants.DatabaseTables.LockerBooking,
                                    " WHERE (DropOffCode = @otp OR PickUpCode=@otp OR DropOffQRCode = @otp OR PickupQRCode = @otp)");

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<LockerBookingEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }

        }

        public async Task<int> SetLockerDetailActivation(int id, bool isAvailable)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(LockerDetailEntity.LockerDetailId)), id);

            if (isAvailable)
                p.Add(string.Concat("@", nameof(LockerDetailEntity.IsAvailable)), 1);
            else
                p.Add(string.Concat("@", nameof(LockerDetailEntity.IsAvailable)), 0);

            sql = BuildActivateScriptLockerDetail();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);

                    if (ret > 0)
                    {
                        if (isAvailable)
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordActivated;
                        else
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordDeactivated;
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


        public async Task<List<LockerBookingHistoryModel>> GetDropOffHistory(string userKeyId)
        {
            var p = new DynamicParameters();
            p.Add("@userKeyId", userKeyId);

            List<LockerBookingHistoryModel> lockerBookingEntities = new List<LockerBookingHistoryModel>();

            var sql = string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.BookingHistory,
                                    " WHERE UserkeyId = @userKeyId AND DropOffDate IS NOT NULL ");

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {

                var dbModel = await conn.QueryAsync<LockerBookingHistoryModel>(sql, p);
                lockerBookingEntities = dbModel.ToList();


                return lockerBookingEntities;

            }
        }

        public async Task<List<LockerBookingHistoryModel>> GetPickupHistory(string userKeyId)
        {
            var p = new DynamicParameters();
            p.Add("@userKeyId", userKeyId);

            List<LockerBookingHistoryModel> lockerBookingEntities = new List<LockerBookingHistoryModel>();

            var sql = string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.BookingHistory,
                                    " WHERE UserkeyId = @userKeyId AND PickupDate IS NOT NULL ");

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {

                var dbModel = await conn.QueryAsync<LockerBookingHistoryModel>(sql, p);
                lockerBookingEntities = dbModel.ToList();


                return lockerBookingEntities;

            }
        }
        public async Task<int> SaveLockerZone(LockerZoneEntity lockerZoneEntity)
        {
            var p = new DynamicParameters();
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(lockerZoneEntity.PositionId)), lockerZoneEntity.PositionId);
            p.Add(string.Concat("@", nameof(lockerZoneEntity.Description)), lockerZoneEntity.Description);
            p.Add(string.Concat("@", nameof(lockerZoneEntity.Name)), lockerZoneEntity.Name);


            string sql;
            if (lockerZoneEntity.PositionId == 0)
                sql = BuildInsertLockerZoneScript();
            else
            {
                sql = BuildUpdateLockerZoneScript(lockerZoneEntity);
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

        string BuildInsertLockerZoneScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.LockerZones));

            sql.Append(" (");
            sql.Append(nameof(LockerZoneEntity.Name));
            sql.Append($", {nameof(LockerZoneEntity.Description)}");

            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(LockerZoneEntity.Name)}");
            sql.Append($", @{nameof(LockerZoneEntity.Description)}");

            sql.Append(")");

            return sql.ToString();
        }
        string BuildUpdateLockerZoneScript(LockerZoneEntity lockerZoneEntity)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerZones, " SET "));


            sql.Append(string.Concat(" ", nameof(LockerZoneEntity.Description), " = ",
                    "@", nameof(lockerZoneEntity.Description)));


            sql.Append(string.Concat(", ", nameof(LockerZoneEntity.Name), " = ",
                     "@", nameof(LockerZoneEntity.Name)));

            sql.Append(string.Concat(" WHERE ", nameof(LockerZoneEntity.PositionId), " = ", "@", nameof(LockerZoneEntity.PositionId)));

            return sql.ToString();

        }
        public async Task<List<LockerZoneEntity>> GetLockerZones(string name = null, int? currentId = null)
        {

            var sql = new StringBuilder(string.Concat("SELECT * FROM ", GlobalDatabaseConstants.DatabaseTables.LockerZones));
            if (name.HasText() && currentId.HasValue)
            {
                sql.Append(string.Concat(" WHERE ", nameof(LockerZoneEntity.Name), " = ",
                       "@", nameof(LockerZoneEntity.Name)));
                sql.Append(string.Concat(" AND ", nameof(LockerZoneEntity.PositionId), " <> ",
                      "@", nameof(LockerZoneEntity.PositionId)));
            }
            else if (currentId.HasValue)
            {
                sql.Append(string.Concat("WHERE ", nameof(LockerZoneEntity.PositionId), " <> ",
                       "@", nameof(LockerZoneEntity.PositionId)));
            }
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(LockerZoneEntity.Name)), name);
            p.Add(string.Concat("@", nameof(LockerZoneEntity.PositionId)), currentId);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<LockerZoneEntity>(sql.ToString(), p);

                return dbModel.ToList();
            }
        }

        public async Task<int> ReassignBooking(LockerBookingEntity existing, BookingLockerDetailModel model, int newLockerDetailId, string otpCode, string otpQRCode, int? adminUserId = null, int? companyUserId = null)
        {
            var insertNewBookingParams = new DynamicParameters();
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.LockerDetailId)), newLockerDetailId);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.UserKeyId)), existing.UserKeyId);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.StoragePeriodStart)), existing.StoragePeriodStart);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.StoragePeriodEnd)), existing.StoragePeriodEnd);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.SenderName)), existing.SenderName);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.SenderMobile)), existing.SenderMobile);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.SenderEmailAddress)), existing.SenderEmailAddress);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.ReceiverName)), existing.ReceiverName);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.ReceiverPhoneNumber)), existing.ReceiverPhoneNumber);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.ReceiverEmailAddress)), existing.ReceiverEmailAddress);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.TotalPrice)), existing.TotalPrice);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.PaymentMethodId)), existing.PaymentMethodId);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.PaymentReference)), existing.PaymentReference);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.BookingStatus)), existing.BookingStatus);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.PackageImage)), existing.PackageImage);
            insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.DateCreated)), DateTime.Now);
            string insertNewLockerBookingSql = "";
            //For DropOff
            if (existing.BookingStatus == 1)
            {
                insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.DropOffCode)), otpCode);
                insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.DropOffQRCode)), otpQRCode);
                insertNewLockerBookingSql = BuildInsertBookingScript(dropOffCodeRequired: true, dropOffQRCodeRequired: true);
            }
            //For Pickup
            else if (existing.BookingStatus == 2)
            {
                insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.PickUpCode)), otpCode);
                insertNewBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.PickUpQRCode)), otpQRCode);
                insertNewLockerBookingSql = BuildInsertBookingScript(pickUpCodeRequired: true, pickUpQRCodeRequired: true);
            }



            var updateExistingBookingParams = new DynamicParameters();
            updateExistingBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.ReassignedByAdminUser)), adminUserId);
            updateExistingBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.ReassignedByCompanyUser)), companyUserId);
            updateExistingBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.BookingStatus)), (int)BookingTransactionStatus.Reassigned);
            updateExistingBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.LockerTransactionsId)), model.lockerTransactionId);
            updateExistingBookingParams.Add(string.Concat("@", nameof(LockerBookingEntity.DateModified)), DateTime.Now);

            string updateExistingBookingLockerStatusSql = "";
            //For DropOff
            if (existing.BookingStatus == 1)
            {

                updateExistingBookingLockerStatusSql = @"update locker_bookings set DateModified= now(), ReassignedByAdminUser=" + adminUserId + ", " +
                    "lockerDetailId=" + newLockerDetailId + ", DropOffCode='" + otpCode + "', DropOffQRCode = '" + otpQRCode + "'," +
                    "PickupCode=NULL, PickupQRCode=NULL where LockerTransactionsId= " + model.lockerTransactionId;

            }
            //For Pickup
            else if (existing.BookingStatus == 2)
            {
                updateExistingBookingLockerStatusSql = @"update locker_bookings set DateModified=now(), ReassignedByAdminUser=" + adminUserId + ", " +
                    "lockerDetailId=" + newLockerDetailId + ", PickupCode='" + otpCode + "', PickupQRCode = '" + otpQRCode + "'" +
                    " where LockerTransactionsId= " + model.lockerTransactionId;

            }



            //updateExistingBookingLockerStatusSql = BuildUpdateExistingBookingLockerStatus(adminUserId: adminUserId, companyUserId: companyUserId);


            var insertLockerBookingHistoryParams = new DynamicParameters();
            insertLockerBookingHistoryParams.Add(string.Concat("@", nameof(ReassignedBookingLockerEntity.ReassignedByAdminUser)), adminUserId);
            insertLockerBookingHistoryParams.Add(string.Concat("@", nameof(ReassignedBookingLockerEntity.lockerTransactionsId)), model.lockerTransactionId);
            insertLockerBookingHistoryParams.Add(string.Concat("@", nameof(ReassignedBookingLockerEntity.OldLockerDetailId)), existing.LockerDetailId);
            insertLockerBookingHistoryParams.Add(string.Concat("@", nameof(ReassignedBookingLockerEntity.NewLockerDetailId)), newLockerDetailId);
            insertLockerBookingHistoryParams.Add(string.Concat("@", nameof(ReassignedBookingLockerEntity.ReassignmentDate)), DateTime.Now);
            insertLockerBookingHistoryParams.Add(string.Concat("@", nameof(ReassignedBookingLockerEntity.ReassignedByCompanyUser)), companyUserId);



            string inserLockerHistorySql = BuildInsertBookingLockerHistoryScript();

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    //var ret = await conn.Exe(insertNewLockerBookingSql, insertNewBookingParams, transaction);
                    // if (ret > 0)
                    //{
                    var ret = await conn.ExecuteAsync(updateExistingBookingLockerStatusSql, updateExistingBookingParams, transaction);
                    if (ret > 0)
                    {
                        ret = await conn.ExecuteAsync(inserLockerHistorySql, insertLockerBookingHistoryParams, transaction);
                        if (ret > 0)
                        {
                            ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.LockerReassignedSuccess;

                        }
                        else
                        {
                            transaction.Rollback();
                            ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;
                        }
                    }
                    else
                    {
                        transaction.Rollback();
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;
                    }

                    //}

                    //else
                    //  ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;

                    transaction.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message);

                    if (e.Message.Contains("duplicate"))
                    {
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.LockerAssignmentError;
                    }

                    if (e.Message.Contains("constraint"))
                    {
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.InvalidInput;
                    }


                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }

        }
        string BuildUpdateBookingLockerDetail(string pickUpCode, string dropOffCode, int? adminUserId = null, int? companyUserId = null)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerBooking, " SET "));


            sql.Append(string.Concat(" ", nameof(LockerBookingEntity.LockerDetailId), " = ",
                    "@", nameof(LockerBookingEntity.LockerDetailId)));

            if (!string.IsNullOrEmpty(pickUpCode))
                sql.Append(string.Concat(", ", nameof(LockerBookingEntity.PickUpCode), " = ",
                       "@", nameof(LockerBookingEntity.PickUpCode)));

            if (!string.IsNullOrEmpty(dropOffCode))
                sql.Append(string.Concat(", ", nameof(LockerBookingEntity.DropOffCode), " = ",
                       "@", nameof(LockerBookingEntity.DropOffCode)));

            if (adminUserId.HasValue && adminUserId > 0)
                sql.Append(string.Concat(", ", nameof(LockerBookingEntity.ReassignedByAdminUser), " = ",
                       "@", nameof(LockerBookingEntity.ReassignedByAdminUser)));

            if (companyUserId.HasValue && companyUserId > 0)
                sql.Append(string.Concat(", ", nameof(LockerBookingEntity.ReassignedByCompanyUser), " = ",
                       "@", nameof(LockerBookingEntity.ReassignedByCompanyUser)));

            sql.Append(string.Concat(", ", nameof(LockerBookingEntity.DateModified), " = ",
                      "@", nameof(LockerBookingEntity.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(LockerBookingEntity.LockerTransactionsId), " = ", "@", nameof(LockerBookingEntity.LockerTransactionsId)));

            return sql.ToString();

        }
        string BuildUpdateExistingBookingLockerStatus(int? adminUserId = null, int? companyUserId = null)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerBooking, " SET "));

            sql.Append(string.Concat(nameof(LockerBookingEntity.BookingStatus), " = ",
                "@", nameof(LockerBookingEntity.BookingStatus)));

            if (adminUserId.HasValue && adminUserId > 0)
                sql.Append(string.Concat(", ", nameof(LockerBookingEntity.ReassignedByAdminUser), " = ",
                       "@", nameof(LockerBookingEntity.ReassignedByAdminUser)));

            if (companyUserId.HasValue && companyUserId > 0)
                sql.Append(string.Concat(", ", nameof(LockerBookingEntity.ReassignedByCompanyUser), " = ",
                       "@", nameof(LockerBookingEntity.ReassignedByCompanyUser)));

            sql.Append(string.Concat(", ", nameof(LockerBookingEntity.DateModified), " = ",
                      "@", nameof(LockerBookingEntity.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(LockerBookingEntity.LockerTransactionsId), " = ", "@", nameof(LockerBookingEntity.LockerTransactionsId)));

            return sql.ToString();

        }
        public async Task<bool> IsValidTransaction(int lockerTransactionsId, int companyId)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.LockerTransactionId, lockerTransactionsId);
            p.Add(GlobalDatabaseConstants.QueryParameters.CompanyId, companyId);

            var procedure = GlobalDatabaseConstants.StoredProcedures.ValidateBookingTransaction;
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    var count = (await conn.QueryAsync<string>(procedure, p, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    if (int.TryParse(count, out var intCount))
                    {
                        if (intCount > 0)
                            return true;
                    }
                    return false;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return false;
                }
            }
        }
        public async Task<UserLockerBookingReportModel> GetBookingTransactionsReport(string userKeyId)
        {
            return new UserLockerBookingReportModel
            {
                CurrentMonthDetail = new CurrentMonthDetail
                {
                    DropOffCount = await CurrentMonthDetailReport(userKeyId, 1),
                    PickUpCount = await CurrentMonthDetailReport(userKeyId, 2),
                    ConfiscatedCount = await CurrentMonthDetailReport(userKeyId, 3),
                    CompletedCount = await CurrentMonthDetailReport(userKeyId, 4)
                },
                MonthlyDetail = await MonthlyReport(userKeyId),
                TotalBookings = await TotalBookings(userKeyId)
            };
        }
        public async Task<int> CurrentMonthDetailReport(string userKeyId, int bookingStatus)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.userKey, userKeyId);
            p.Add(GlobalDatabaseConstants.QueryParameters.SelectedMonth, DateTime.Now.Month);
            p.Add(GlobalDatabaseConstants.QueryParameters.Status, bookingStatus);

            var procedure = GlobalDatabaseConstants.StoredProcedures.UserMontlyReport;
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    return int.Parse((await conn.QueryAsync<string>(procedure, p, commandType: CommandType.StoredProcedure)).FirstOrDefault());
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return 0;
                }
            }
        }

        public async Task<List<MonthlyDetail>> MonthlyReport(string userKeyId)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.userKey, userKeyId);
            p.Add(GlobalDatabaseConstants.QueryParameters.SelectedYear, DateTime.Now.Year);

            var procedure = GlobalDatabaseConstants.StoredProcedures.UserAnuallyReport;
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    var dbModel = await conn.QueryAsync<MonthlyDetail>(procedure, p, commandType: CommandType.StoredProcedure);
                    return dbModel.ToList();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return new List<MonthlyDetail> { };
                }
            }
        }
        public async Task<int> TotalBookings(string userKeyId)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add(GlobalDatabaseConstants.QueryParameters.UserKeyId, userKeyId);

                var sql = BuildSelectTotalBookings();
                using (IDbConnection conn = this._databaseHelper.GetConnection())
                {
                    return int.Parse((await conn.QueryAsync<string>(sql, p)).FirstOrDefault());
                }
            }
            catch { return 0; }
        }
        string BuildSelectTotalBookings()
        {
            var query = new StringBuilder($"SELECT count({nameof(LockerBookingEntity.LockerTransactionsId)}) FROM {GlobalDatabaseConstants.Views.BookingLockerDetail}");

            query.Append($" WHERE {nameof(LockerBookingEntity.UserKeyId)}={GlobalDatabaseConstants.QueryParameters.UserKeyId} and year({nameof(LockerBookingEntity.DateCreated)})={DateTime.Now.Year}");

            return query.ToString();
        }
        public async Task<int> CancelLockerBooking(CancelBookingModel cancelBookingModel, LockerBookingEntity existingBooking, string userKeyId)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(LockerBookingEntity.LockerTransactionsId)), cancelBookingModel.lockerTransactionsId);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.BookingStatus)), BookingTransactionStatus.Cancelled);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.DateModified)), DateTime.Now);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.CancelledDate)), DateTime.Now);

            var sql = BuildCancelLockerBooking();

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
                        var description = new
                        {
                            Action = "CancelLockerBooking",
                            UserKeyId = userKeyId,
                            OldBookingStatus = existingBooking.BookingStatus,
                            NewBookingStatus = BookingTransactionStatus.Cancelled
                        };
                        p.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.LockerTransactionsId)), existingBooking.LockerTransactionsId);
                        p.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.CreatedDate)), DateTime.Now);
                        p.Add(string.Concat("@", nameof(LockerBookingHistoryEntity.Description)), JsonSerializer.Serialize(description));

                        sql = BuildLockerBookingHistory();
                        await conn.ExecuteAsync(sql, p, transaction);
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
        string BuildExtendLockerBooking()
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.LockerBooking, " SET "));

            sql.Append(string.Concat(" ", nameof(LockerBookingEntity.StoragePeriodEnd), " =",
                "@", nameof(LockerBookingEntity.StoragePeriodEnd)));
            sql.Append(string.Concat(" , ", nameof(LockerBookingEntity.TotalPrice), " =",
               "@", nameof(LockerBookingEntity.TotalPrice)));
            sql.Append(string.Concat(" , ", nameof(LockerBookingEntity.DateModified), " =",
                "@", nameof(LockerBookingEntity.DateModified)));

            sql.Append(string.Concat(" WHERE ", nameof(LockerBookingEntity.LockerTransactionsId), "=", "@", nameof(LockerBookingEntity.LockerTransactionsId)));

            return sql.ToString();
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
        public async Task<List<ActiveLockerBookingEntity>> GetAtiveLockerBookingDetail(int LockerTransactionsId)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.LockerTransactionId, LockerTransactionsId);

            var procedure = GlobalDatabaseConstants.StoredProcedures.ActiveLockerBooking;
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    var dbModel = await conn.QueryAsync<ActiveLockerBookingEntity>(procedure, p, commandType: CommandType.StoredProcedure);
                    return dbModel.ToList();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return new List<ActiveLockerBookingEntity>();
                }
            }
        }
        public async Task<LockerBookingEntity> GetLockerBookingDetail(int LockerTransactionsId)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.LockerTransactionId, LockerTransactionsId);

            var procedure = GlobalDatabaseConstants.StoredProcedures.LockerBookingDetail;
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    var dbModel = await conn.QueryAsync<LockerBookingEntity>(procedure, p, commandType: CommandType.StoredProcedure);
                    return dbModel.FirstOrDefault();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return new LockerBookingEntity();
                }
            }
        }
        public async Task<int> ActiveBookingsCount(int lockerDetailId, DateTime fromDate, DateTime toDate, int? excludeLockerTransactionId = null)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.LockerTransactionId, excludeLockerTransactionId);
            p.Add(GlobalDatabaseConstants.QueryParameters.LockerDetailId, lockerDetailId);
            p.Add(GlobalDatabaseConstants.QueryParameters.StartDate, fromDate);
            p.Add(GlobalDatabaseConstants.QueryParameters.EndDate, toDate);

            var procedure = GlobalDatabaseConstants.StoredProcedures.ActiveBookingCount;
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    var countStr = (await conn.QueryAsync<string>(procedure, p, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    if (int.TryParse(countStr, out var count))
                        return count;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
                return 0;
            }
        }

        public async Task<LockerBookingEntity> GetLockerBookingByTransactionId(int lockerTransactionId)
        {
            var builder = new SqlBuilder();
            DynamicParameters parameters = new DynamicParameters();
            builder.Select("*");

            parameters.Add(GlobalDatabaseConstants.QueryParameters.LockerTransactionsId, lockerTransactionId, DbType.Int32, ParameterDirection.Input);
            builder.Where(nameof(LockerBookingEntity.LockerTransactionsId) + " = " + GlobalDatabaseConstants.QueryParameters.LockerTransactionsId, parameters);

            var builderTemplate = builder.AddTemplate($"Select /**select**/ from {GlobalDatabaseConstants.DatabaseTables.LockerBooking} /**where**/ ");

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<LockerBookingEntity>(builderTemplate.RawSql, builderTemplate.Parameters);
                return dbModel.AsEnumerable().FirstOrDefault();
            }
        }

        public async Task<int> UpdateBookingStatus(int lockerTransactionId, int bookingStatus)
        {
            var p = new DynamicParameters();
            string queryValues;
            string sql;

            List<string> columns = new List<string> {
             nameof(LockerBookingEntity.LockerTransactionsId),
             nameof(LockerBookingEntity.BookingStatus),
            };

            p.Add(string.Concat("@", nameof(LockerBookingEntity.LockerTransactionsId)), lockerTransactionId);
            p.Add(string.Concat("@", nameof(LockerBookingEntity.BookingStatus)), bookingStatus);

            queryValues = SharedServices.UpdateQueryBuilder(columns);
            sql = $"UPDATE {GlobalDatabaseConstants.DatabaseTables.LockerBooking} SET {queryValues} " +
                  $"WHERE {nameof(LockerBookingEntity.LockerTransactionsId)}=@{nameof(LockerBookingEntity.LockerTransactionsId)}";

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
        public async Task<List<LockerBookingPaymentDetail>> GetUserBookings(string userkeyId,
            BookingTransactionStatus? bookingStatus = null, DateTime? fromDate = null,
            DateTime? toDate = null, int? currentPage = null, int? pageSize = null, bool activeOnly = false)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.UserKeyId, userkeyId);
            p.Add(GlobalDatabaseConstants.QueryParameters.StartDate, fromDate);
            p.Add(GlobalDatabaseConstants.QueryParameters.EndDate, toDate);
            p.Add(GlobalDatabaseConstants.QueryParameters.BookingStatus, bookingStatus);
            p.Add(GlobalDatabaseConstants.QueryParameters.CurrentPage, currentPage);
            p.Add(GlobalDatabaseConstants.QueryParameters.PageSize, pageSize);
            p.Add(GlobalDatabaseConstants.QueryParameters.IsActive, activeOnly);

            var procedure = GlobalDatabaseConstants.StoredProcedures.BookingDetailWithPayments;
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    var dbModel = await conn.QueryAsync<LockerBookingPaymentDetail>(procedure, p, commandType: CommandType.StoredProcedure);

                    return dbModel.ToList();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return new List<LockerBookingPaymentDetail>();
                }
            }
        }

        public async Task<List<LockerDetailEntity>> GetUnavailableLockers(DateTime? startDate = null, DateTime? endDate = null)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.StartDate, startDate);
            p.Add(GlobalDatabaseConstants.QueryParameters.EndDate, endDate);

            var procedure = GlobalDatabaseConstants.StoredProcedures.GetUnavailableLockers;
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    var dbModel = await conn.QueryAsync<LockerDetailEntity>(procedure, p, commandType: CommandType.StoredProcedure);

                    return dbModel.ToList();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return new List<LockerDetailEntity>();
                }
            }
        }

        public async Task<int> ValidateAvailableLocker(int cabinetLocationId, int lockerTypeId, int positionId,
               int lockerDetailId)
        {
             var p = new DynamicParameters();

            var sql = "SELECT count(*) FROM selected_lockers_for_booking WHERE " +
                      "CabinetLocationId=@CabinetLocationId AND LockerTypeId=@LockerTypeId AND PositionId=@PositionId " +
                      "AND LockerDetailId=@LockerTypeId AND StoragePeriodStart IS NOT NULL AND StoragePeriodEnd IS NOT NULL;";

            p.Add("@CabinetLocationId", cabinetLocationId);
            p.Add("@PositionId", positionId);
            p.Add("@LockerTypeId", lockerTypeId);

            try
            {

                using (IDbConnection conn = this._databaseHelper.GetConnection())
                {
                    return int.Parse((await conn.QueryAsync<string>(sql,p)).FirstOrDefault());
                }
            }
            catch { return 0; }
        }
        public async Task<int> SelectLockerForBooking(int cabinetLocationId, int lockerTypeId, int positionId,
             DateTime storageStartDate, DateTime storageEndDate, string userKeyId, int lockerDetailId)
        {

            var p = new DynamicParameters();
            var sql = $"INSERT INTO selected_lockers_for_booking (LockerTypeId, CabinetLocationId, PositionId, " +
                      $"UserKeyId, LockerDetailId, StoragePeriodStart, StoragePeriodEnd) VALUES ({lockerTypeId}, @CabinetLocationId, " +
                      $"@PositionId, @UserKeyId, @LockerDetailId @StorageStartDate, @StorageEndDate);";

            p.Add("@CabinetLocationId", cabinetLocationId);
            p.Add("@PositionId", positionId);
            p.Add("@LockerTypeId", lockerTypeId);   
            p.Add("@UserKeyId", userKeyId);
            p.Add("@LockerDetailId", lockerDetailId);
            p.Add("@StorageStartDate", storageStartDate);  
             p.Add("@StorageEndDate", storageEndDate);  

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
            
                try
                {
                    var ret = await conn.ExecuteAsync(sql,p);
                    if (ret > 0)
                    {
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
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

        public async Task<int> ClearLockerForBooking(int cabinetLocationId, int lockerTypeId, int positionId,
                 string userKeyId, int lockerDetailId)
        {
             var p = new DynamicParameters();
            var sql = $"DELETE FROM selected_lockers_for_booking WHERE UserKeyId=@UserKeyId AND " +
                      $"CabinetLocationId=@CabinetLocationId AND LockerTypeId=@LockerTypeId AND PositionId=@PositionId " +
                      $"AND LockerDetailId=@LockerDetailId AND StoragePeriodStart IS NOT NULL AND StoragePeriodEnd IS NOT NULL;";

            p.Add("@CabinetLocationId", cabinetLocationId);
            p.Add("@PositionId", positionId);
            p.Add("@LockerTypeId", lockerTypeId);   
            p.Add("@UserKeyId", userKeyId);
            p.Add("@LockerDetailId", lockerDetailId);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
            
                try
                {
                    var ret = await conn.ExecuteAsync(sql,p);
                    if (ret > 0)
                    {
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordDeleted;
                    }
                    else
                    {
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.NoRecords;
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
    
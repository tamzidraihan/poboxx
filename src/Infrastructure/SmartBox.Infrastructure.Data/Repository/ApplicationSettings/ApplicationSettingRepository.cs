using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.ApplicationSetting;
using SmartBox.Business.Core.Entities.User;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;

namespace SmartBox.Infrastructure.Data.Repository.ApplicationSettings
{
    public class ApplicationSettingRepository : GenericRepositoryBase<ApplicationSettingEntity, ApplicationSettingRepository>, IApplicationSettingRepository
    {
        public ApplicationSettingRepository(IDatabaseHelper databaseHelper, ILogger<ApplicationSettingRepository> logger) : base(databaseHelper, logger)
        {
        }

        string BuildSelectQuery(short applicationSettingId)
        {
            var query = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.ApplicationSettings}");

            if (applicationSettingId > 0)
                query.Append($" WHERE {nameof(ApplicationSettingEntity.ApplicationSettingsId)} = @{nameof(ApplicationSettingEntity.ApplicationSettingsId)}");

            return query.ToString();
        }


        public async Task<ApplicationSettingEntity> GetApplicationSetting(short applicationSettingId = 1)
        {
            var p = new DynamicParameters();
            p.Add($"@{nameof(ApplicationSettingEntity.ApplicationSettingsId)}", applicationSettingId);

            var sql = BuildSelectQuery(applicationSettingId);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<ApplicationSettingEntity>(sql, p);



                return dbModel.FirstOrDefault();
            }
        }
        public async Task<int> Save(ApplicationSettingEntity model)
        {
            var p = new DynamicParameters();
            bool isInsert = true;

            p.Add(string.Concat("@", nameof(model.ApplicationSettingsId)), model.ApplicationSettingsId);
            p.Add(string.Concat("@", nameof(model.OTPMinutes)), model.OTPMinutes);
            p.Add(string.Concat("@", nameof(model.IsOTPMixCharacters)), model.IsOTPMixCharacters);
            p.Add(string.Concat("@", nameof(model.OTPLength)), model.OTPLength);
            p.Add(string.Concat("@", nameof(model.AvailableLockerTypeNumberOfDays)), model.AvailableLockerTypeNumberOfDays);
            p.Add(string.Concat("@", nameof(model.DefaultCompanyPassword)), model.DefaultCompanyPassword);
            p.Add(string.Concat("@", nameof(model.MaintainanceReportReminderDay)), model.MaintainanceReportReminderDay);
            p.Add(string.Concat("@", nameof(model.MaintainanceReportReminderHour)), model.MaintainanceReportReminderHour);
            p.Add(string.Concat("@", nameof(model.MaintainanceOverdueReportReminderDay)), model.MaintainanceOverdueReportReminderDay);
            p.Add(string.Concat("@", nameof(model.MaintainanceOverdueReportReminderHour)), model.MaintainanceOverdueReportReminderHour);
            p.Add(string.Concat("@", nameof(model.MaxRadius)), model.MaxRadius);

            string sql;
            if (model.ApplicationSettingsId == 0)
            {
                p.Add(string.Concat("@", nameof(model.DateCreated)), DateTime.Now);
                sql = BuildInsertCommand();
            }
            else
            {
                p.Add(string.Concat("@", nameof(model.DateModified)), DateTime.Now);
                sql = BuildUpdateCommand(model);
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
        string BuildInsertCommand()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.ApplicationSettings));

            sql.Append(" (");
            sql.Append(nameof(ApplicationSettingEntity.OTPMinutes));
            sql.Append($", {nameof(ApplicationSettingEntity.IsOTPMixCharacters)}");
            sql.Append($", {nameof(ApplicationSettingEntity.OTPLength)}");
            sql.Append($", {nameof(ApplicationSettingEntity.AvailableLockerTypeNumberOfDays)}");
            sql.Append($", {nameof(ApplicationSettingEntity.DefaultCompanyPassword)}");
            sql.Append($", {nameof(ApplicationSettingEntity.MaintainanceReportReminderDay)}");
            sql.Append($", {nameof(ApplicationSettingEntity.MaintainanceReportReminderHour)}");
            sql.Append($", {nameof(ApplicationSettingEntity.MaintainanceOverdueReportReminderDay)}");
            sql.Append($", {nameof(ApplicationSettingEntity.MaintainanceOverdueReportReminderHour)}");
            sql.Append($", {nameof(ApplicationSettingEntity.MaxRadius)}");
            sql.Append($", {nameof(ApplicationSettingEntity.DateCreated)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(ApplicationSettingEntity.OTPMinutes)}");
            sql.Append($", @{nameof(ApplicationSettingEntity.IsOTPMixCharacters)}");
            sql.Append($", @{nameof(ApplicationSettingEntity.OTPLength)}");
            sql.Append($", @{nameof(ApplicationSettingEntity.AvailableLockerTypeNumberOfDays)}");
            sql.Append($", @{nameof(ApplicationSettingEntity.DefaultCompanyPassword)}");
            sql.Append($", @{nameof(ApplicationSettingEntity.MaintainanceReportReminderDay)}");
            sql.Append($", @{nameof(ApplicationSettingEntity.MaintainanceReportReminderHour)}");
            sql.Append($", @{nameof(ApplicationSettingEntity.MaintainanceOverdueReportReminderDay)}");
            sql.Append($", @{nameof(ApplicationSettingEntity.MaintainanceOverdueReportReminderHour)}");
            sql.Append($", @{nameof(ApplicationSettingEntity.MaxRadius)}");
            sql.Append($", @{nameof(ApplicationSettingEntity.DateCreated)}");
            sql.Append(")");

            return sql.ToString();
        }
        string BuildUpdateCommand(ApplicationSettingEntity model)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.ApplicationSettings, " SET "));


            sql.Append(string.Concat(" ", nameof(model.OTPMinutes), " = ",
                    "@", nameof(model.OTPMinutes)));

            sql.Append(string.Concat(", ", nameof(model.IsOTPMixCharacters), " = ",
                     "@", nameof(model.IsOTPMixCharacters)));

            sql.Append(string.Concat(", ", nameof(model.OTPLength), " = ",

                     "@", nameof(model.OTPLength)));
            sql.Append(string.Concat(", ", nameof(model.AvailableLockerTypeNumberOfDays), " = ",
                    "@", nameof(model.AvailableLockerTypeNumberOfDays)));

            sql.Append(string.Concat(", ", nameof(model.DefaultCompanyPassword), " = ",
                   "@", nameof(model.DefaultCompanyPassword)));

            sql.Append(string.Concat(", ", nameof(model.MaintainanceReportReminderDay), " = ",
                    "@", nameof(model.MaintainanceReportReminderDay)));
            sql.Append(string.Concat(", ", nameof(model.MaintainanceReportReminderHour), " = ",
                    "@", nameof(model.MaintainanceReportReminderHour)));
            sql.Append(string.Concat(", ", nameof(model.MaintainanceOverdueReportReminderDay), " = ",
                    "@", nameof(model.MaintainanceOverdueReportReminderDay)));
            sql.Append(string.Concat(", ", nameof(model.MaintainanceOverdueReportReminderHour), " = ",
                    "@", nameof(model.MaintainanceOverdueReportReminderHour)));
            sql.Append(string.Concat(", ", nameof(model.MaxRadius), " = ",
                    "@", nameof(model.MaxRadius)));
            sql.Append(string.Concat(", ", nameof(model.DateModified), " = ",
                   "@", nameof(model.DateModified)));


            sql.Append(string.Concat(" WHERE ", nameof(ApplicationSettingEntity.ApplicationSettingsId), " = ", "@", nameof(ApplicationSettingEntity.ApplicationSettingsId)));

            return sql.ToString();

        }
    }
}

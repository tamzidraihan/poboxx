using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Announcement;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Announcement
{
    public class PromoAnnouncementRepository : GenericRepositoryBase<PromoAnnouncementEntity, PromoAnnouncementRepository>, IPromoAnnouncementRepository
    {
        public PromoAnnouncementRepository(IDatabaseHelper databaseHelper, ILogger<PromoAnnouncementRepository> logger) : base(databaseHelper, logger)
        {
        }
        public async Task<List<PromoAnnouncementEntity>> Get()
        {
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<PromoAnnouncementEntity>(BuildGetCommand())).ToList();
            }
        }

        public async Task<int> Save(PromoAnnouncementEntity model)
        {
            var p = new DynamicParameters();
            bool isInsert = true;
            p.Add(string.Concat("@", nameof(model.Id)), model.Id);
            p.Add(string.Concat("@", nameof(model.Description)), model.Description);
            p.Add(string.Concat("@", nameof(model.Name)), model.Name);
            p.Add(string.Concat("@", nameof(model.StartDate)), model.StartDate);
            p.Add(string.Concat("@", nameof(model.ExternalUrl)), model.ExternalUrl);
            p.Add(string.Concat("@", nameof(model.AnnouncementTypeId)), model.AnnouncementTypeId);
            p.Add(string.Concat("@", nameof(model.Message)), model.Message);
            p.Add(string.Concat("@", nameof(model.Description)), model.Description);
            p.Add(string.Concat("@", nameof(model.EndDate)), model.EndDate);
            p.Add(string.Concat("@", nameof(model.Image)), model.Image);
           

            string sql;
            if (model.Id == 0)
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
        string BuildGetCommand()
        {
            return string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.PromoAnnouncement);
        }
        string BuildInsertCommand()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.PromoAnnouncement));
            sql.Append(" (");
            sql.Append(nameof(PromoAnnouncementEntity.Name));
            sql.Append($", {nameof(PromoAnnouncementEntity.Description)}");
            sql.Append($", {nameof(PromoAnnouncementEntity.StartDate)}");
            sql.Append($", {nameof(PromoAnnouncementEntity.EndDate)}");
            sql.Append($", {nameof(PromoAnnouncementEntity.ExternalUrl)}");
            sql.Append($", {nameof(PromoAnnouncementEntity.AnnouncementTypeId)}");
            sql.Append($", {nameof(PromoAnnouncementEntity.Image)}");
            sql.Append($", {nameof(PromoAnnouncementEntity.Message)}");
            sql.Append($", {nameof(PromoAnnouncementEntity.DateCreated)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(PromoAnnouncementEntity.Name)}");
            sql.Append($", @{nameof(PromoAnnouncementEntity.Description)}");
            sql.Append($", @{nameof(PromoAnnouncementEntity.StartDate)}");
            sql.Append($", @{nameof(PromoAnnouncementEntity.EndDate)}");
            sql.Append($", @{nameof(PromoAnnouncementEntity.ExternalUrl)}");
            sql.Append($", @{nameof(PromoAnnouncementEntity.AnnouncementTypeId)}");
            sql.Append($", @{nameof(PromoAnnouncementEntity.Image)}");
            sql.Append($", @{nameof(PromoAnnouncementEntity.Message)}");
            sql.Append($", @{nameof(PromoAnnouncementEntity.DateCreated)}");
            sql.Append(")");
            return sql.ToString();
        }
        string BuildUpdateCommand(PromoAnnouncementEntity model)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.PromoAnnouncement, " SET "));
            sql.Append(string.Concat(" ", nameof(model.Description), " = ",
                    "@", nameof(model.Description)));
            sql.Append(string.Concat(", ", nameof(model.Name), " = ",
                     "@", nameof(model.Name)));
            sql.Append(string.Concat(", ", nameof(model.ExternalUrl), " = ",
                    "@", nameof(model.ExternalUrl)));
            sql.Append(string.Concat(", ", nameof(model.StartDate), " = ",
                    "@", nameof(model.StartDate)));
            sql.Append(string.Concat(", ", nameof(model.EndDate), " = ",
                    "@", nameof(model.EndDate)));
            sql.Append(string.Concat(", ", nameof(model.AnnouncementTypeId), " = ",
                    "@", nameof(model.AnnouncementTypeId)));
            sql.Append(string.Concat(", ", nameof(model.Image), " = ",
                    "@", nameof(model.Image))); 
            sql.Append(string.Concat(", ", nameof(model.Message), " = ",
                    "@", nameof(model.Message)));
            sql.Append(string.Concat(", ", nameof(model.DateModified), " = ",
                    "@", nameof(model.DateModified)));
            sql.Append(string.Concat(" WHERE ", nameof(model.Id), " = ", "@", nameof(model.Id)));
            return sql.ToString();
        }
        public async Task<int> Delete(int id)
        {
            PromoAnnouncementEntity entity = new PromoAnnouncementEntity { Id = id };
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(entity.Id)), id);
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
        string BuildDeleteCommand(PromoAnnouncementEntity model)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.PromoAnnouncement));
            sql.Append(string.Concat(" WHERE ", nameof(model.Id), " = ", "@", nameof(model.Id)));
            return sql.ToString();
        }
    }
}

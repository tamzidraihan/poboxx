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
    public class AnnouncementTypeRepository: GenericRepositoryBase<AnnouncementTypeEntity, AnnouncementTypeRepository>, IAnnouncementTypeRepository
    {
        public AnnouncementTypeRepository(IDatabaseHelper databaseHelper, ILogger<AnnouncementTypeRepository> logger) : base(databaseHelper, logger)
        {
        }
        public async Task<List<AnnouncementTypeEntity>> Get()
        {
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                return (await conn.QueryAsync<AnnouncementTypeEntity>(BuildGetCommand())).ToList();
            }
        }

        public async Task<int> Save(AnnouncementTypeEntity model)
        {
            var p = new DynamicParameters();
            bool isInsert = true;
            p.Add(string.Concat("@", nameof(model.Id)), model.Id);
            p.Add(string.Concat("@", nameof(model.Description)), model.Description);
            p.Add(string.Concat("@", nameof(model.Name)), model.Name);
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
            return string.Concat("SELECT * FROM ", GlobalDatabaseConstants.Views.AnnouncementType);
        }
        string BuildInsertCommand()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.AnnouncementType));
            sql.Append(" (");
            sql.Append(nameof(AnnouncementTypeEntity.Name));
            sql.Append($", {nameof(AnnouncementTypeEntity.Description)}");
            sql.Append($", {nameof(AnnouncementTypeEntity.DateCreated)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(AnnouncementTypeEntity.Name)}");
            sql.Append($", @{nameof(AnnouncementTypeEntity.Description)}");
            sql.Append($", @{nameof(AnnouncementTypeEntity.DateCreated)}");
            sql.Append(")");
            return sql.ToString();
        }
        string BuildUpdateCommand(AnnouncementTypeEntity model)
        {
            var sql = new StringBuilder(string.Concat("UPDATE ", GlobalDatabaseConstants.DatabaseTables.AnnouncementType, " SET "));
            sql.Append(string.Concat(" ", nameof(model.Description), " = ",
                    "@", nameof(model.Description)));
            sql.Append(string.Concat(", ", nameof(model.Name), " = ",
                     "@", nameof(model.Name)));
            sql.Append(string.Concat(", ", nameof(model.DateModified), " = ",
                    "@", nameof(model.DateModified)));
            sql.Append(string.Concat(" WHERE ", nameof(model.Id), " = ", "@", nameof(model.Id)));
            return sql.ToString();
        }
        public async Task<int> Delete(int id)
        {
            AnnouncementTypeEntity entity = new AnnouncementTypeEntity { Id = id };
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
        string BuildDeleteCommand(AnnouncementTypeEntity model)
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.AnnouncementType));
            sql.Append(string.Concat(" WHERE ", nameof(model.Id), " = ", "@", nameof(model.Id)));
            return sql.ToString();
        }
    }
}

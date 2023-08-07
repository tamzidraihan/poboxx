using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Logs;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Logs
{
    public class MessageLogRepository : GenericRepositoryBase<MessageLogEntity, MessageLogRepository>, IMessageLogRepository
    {
        public MessageLogRepository(IDatabaseHelper databaseHelper, ILogger<MessageLogRepository> logger) : base(databaseHelper, logger)
        {
        }
        public async Task<List<MessageLogEntity>> Get(int? companyId = null, int? currentPage = null, int? pageSize = null)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.CompanyId, companyId);
            p.Add(GlobalDatabaseConstants.QueryParameters.CurrentPage, currentPage);
            p.Add(GlobalDatabaseConstants.QueryParameters.PageSize, pageSize);

            var procedure = GlobalDatabaseConstants.StoredProcedures.MessageLog;
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                try
                {
                    var dbModel = await conn.QueryAsync<MessageLogEntity>(procedure, p, commandType: CommandType.StoredProcedure);

                    return dbModel.ToList();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    throw;
                }
            }
        }

        public async Task<int> Save(MessageLogEntity model)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(model.Type)), model.Type);
            p.Add(string.Concat("@", nameof(model.Message)), model.Message);
            p.Add(string.Concat("@", nameof(model.Subject)), model.Subject);
            p.Add(string.Concat("@", nameof(model.CompanyId)), model.CompanyId);
            p.Add(string.Concat("@", nameof(model.DateCreated)), DateTime.Now);
            p.Add(string.Concat("@", nameof(model.isSent)), model.isSent);
            p.Add(string.Concat("@", nameof(model.Sender)), model.Sender);
            p.Add(string.Concat("@", nameof(model.Receipent)), model.Receipent);

            string sql = BuildInsertCommand();
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p, transaction);
                    if (ret > 0)
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;
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
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.MessageLog));

            sql.Append(" (");
            sql.Append(nameof(MessageLogEntity.Type));
            sql.Append($", {nameof(MessageLogEntity.Receipent)}");
            sql.Append($", {nameof(MessageLogEntity.Subject)}");
            sql.Append($", {nameof(MessageLogEntity.Message)}");
            sql.Append($", {nameof(MessageLogEntity.CompanyId)}");
            sql.Append($", {nameof(MessageLogEntity.isSent)}");
            sql.Append($", {nameof(MessageLogEntity.Sender)}");
            sql.Append($", {nameof(MessageLogEntity.DateCreated)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(MessageLogEntity.Type)}");
            sql.Append($", @{nameof(MessageLogEntity.Receipent)}");
            sql.Append($", @{nameof(MessageLogEntity.Subject)}");
            sql.Append($", @{nameof(MessageLogEntity.Message)}");
            sql.Append($", @{nameof(MessageLogEntity.CompanyId)}");
            sql.Append($", @{nameof(MessageLogEntity.isSent)}");
            sql.Append($", @{nameof(MessageLogEntity.Sender)}");
            sql.Append($", @{nameof(MessageLogEntity.DateCreated)}");

            sql.Append(")");

            return sql.ToString();
        }

    }
}

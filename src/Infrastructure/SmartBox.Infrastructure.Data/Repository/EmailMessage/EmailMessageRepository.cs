using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.AppMessage;
using SmartBox.Business.Core.Entities.Email;
using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Shared;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.EmailMessage
{
    public class EmailMessageRepository : GenericRepositoryBase<MessageEntity, EmailMessageRepository>, IEmailMessageRepository
    {
        public EmailMessageRepository(IDatabaseHelper databaseHelper, ILogger<EmailMessageRepository> logger) : base(databaseHelper, logger)
        {
        }


        string BuildInsertScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.EmailMessage));



            sql.Append(" (");
            sql.Append(nameof(EmailEntity.EmailMessageId));
            sql.Append($", {nameof(EmailEntity.Subject)}");
            sql.Append($", {nameof(EmailEntity.Message)}");
            sql.Append($", {nameof(EmailEntity.DateCreated)}");
            sql.Append($", {nameof(EmailEntity.DateModified)}");
            sql.Append($", {nameof(EmailEntity.IsDeleted)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append(nameof(EmailEntity.EmailMessageId));
            sql.Append($", @{nameof(EmailEntity.Subject)}");
            sql.Append($", @{nameof(EmailEntity.Message)}");
            
            sql.Append($", @{nameof(EmailEntity.DateCreated)}");
            sql.Append($", @{nameof(EmailEntity.DateModified)}");
            sql.Append($", @{nameof(EmailEntity.IsDeleted)}");


            sql.Append(")");

            return sql.ToString();


        }

        string BuildUpdateScript(EmailEntity emailEntity)
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.EmailMessage} SET ");

            sql.Append(string.Concat(" ", nameof(emailEntity.Subject), " = ",
                   "@", nameof(emailEntity.Subject)));

            sql.Append(string.Concat(", ", nameof(emailEntity.Message), " = ",
                   "@", nameof(emailEntity.Message)));

            

            sql.Append(string.Concat(", ", nameof(emailEntity.DateModified), " = ",
                    "@", nameof(emailEntity.DateModified)));


            sql.Append(string.Concat(" WHERE ", nameof(emailEntity.EmailMessageId), " = ", "@", nameof(emailEntity.EmailMessageId)));

            return sql.ToString();


        }



        string QueryBuilder()
        {
            var query = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.EmailMessage}");
            query.Append($" WHERE {nameof(MessageEntity.EmailMessageId)} = {GlobalDatabaseConstants.QueryParameters.EmailMessageId}");

            return query.ToString();
        }

        public async Task<MessageEntity> GetEmailMessage(int emailMessageId)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.EmailMessageId, emailMessageId);

            var sql = QueryBuilder();
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<MessageEntity>(sql, p);

 

                return dbModel.FirstOrDefault();
            }
        }

        public async Task<int> Save(EmailEntity email)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(EmailEntity.EmailMessageId)), email.EmailMessageId);
            p.Add(string.Concat("@", nameof(EmailEntity.Subject)), email.Subject);
            p.Add(string.Concat("@", nameof(EmailEntity.Message)), email.Message);
      


            p.Add(string.Concat("@", nameof(EmailEntity.IsDeleted)), email.IsDeleted);

            if (email.EmailMessageId <= 0)
            {
                p.Add(string.Concat("@", nameof(email.DateCreated)), DateTime.Now);
                sql = BuildInsertScript();

            }
            else
            {
                p.Add(string.Concat("@", nameof(email.DateModified)), DateTime.Now);
                sql = BuildUpdateScript(email);

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
                        if (email.EmailMessageId <= 0)
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
    }
}

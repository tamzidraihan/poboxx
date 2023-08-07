using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Entities.User;
using SmartBox.Business.Core.Models.User;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using static Slapper.AutoMapper;

namespace SmartBox.Infrastructure.Data.Repository.User
{
    public class UserRepository : GenericRepositoryBase<UserEntity, UserRepository>, IUserRepository
    {

        public UserRepository(IDatabaseHelper databaseHelper, ILogger<UserRepository> logger) : base(databaseHelper,
            logger)
        {

        }

        string BuildUpdateScript()
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.Users} SET ");

            sql.Append($"{nameof(UserEntity.Email)} = @{nameof(UserEntity.Email)}");
            sql.Append(", ");
            sql.Append($"{nameof(UserEntity.PhoneNumber)} = @{nameof(UserEntity.PhoneNumber)}");
            sql.Append(", ");
            sql.Append($"{nameof(UserEntity.FirstName)} = @{nameof(UserEntity.FirstName)}");
            sql.Append(", ");
            sql.Append($"{nameof(UserEntity.LastName)} = @{nameof(UserEntity.LastName)}");
            sql.Append(", ");
            sql.Append($"{nameof(UserEntity.Password)} = @{nameof(UserEntity.Password)}");
            sql.Append(", ");
            sql.Append($"{nameof(UserEntity.Photo)} = @{nameof(UserEntity.Photo)}");
            sql.Append(", ");
            sql.Append($"{nameof(UserEntity.IsLowerLockerPrefered)} = @{nameof(UserEntity.IsLowerLockerPrefered)}");
            sql.Append($" WHERE {nameof(UserEntity.UserKeyId)} = @{nameof(UserEntity.UserKeyId)}");

            return sql.ToString();
        }

        string BuildUpdateAdminScript(AdminUserEntity entity)
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.AdminUser} SET ");

            sql.Append($"{nameof(AdminUserEntity.Username)} = '{entity.Username}'");
            sql.Append(", ");
            sql.Append($"{nameof(AdminUserEntity.Email)} = '{entity.Email}'");
            sql.Append(", ");
            sql.Append($"{nameof(AdminUserEntity.FirstName)} = '{entity.FirstName}'");
            sql.Append(", ");
            sql.Append($"{nameof(AdminUserEntity.LastName)} = '{entity.LastName}'");
            sql.Append(", ");
            sql.Append($"{nameof(AdminUserEntity.Password)} = '{entity.Password}'");
            sql.Append($" WHERE {nameof(AdminUserEntity.AdminUserId)} = {entity.AdminUserId}");

            return sql.ToString();
        }

        string BuildUpdateOTPScript()
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.Users} SET ");

            sql.Append($"{nameof(UserEntity.OTP)} = @{nameof(UserEntity.OTP)}");
            sql.Append(", ");
            sql.Append($"{nameof(UserEntity.OTPExpirationDate)} = @{nameof(UserEntity.OTPExpirationDate)}");

            sql.Append($" WHERE {nameof(UserEntity.UserKeyId)} = @{nameof(UserEntity.UserKeyId)}");

            return sql.ToString();
        }

        string BuildUpdateMPINScript()
        {

            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.Users} SET ");

            sql.Append($"{nameof(UserEntity.MPIN)} = @{nameof(UserEntity.MPIN)}");

            sql.Append($" WHERE {nameof(UserEntity.UserKeyId)} = @{nameof(UserEntity.UserKeyId)}");

            return sql.ToString();

        }

        string BuildUpdateActivateScript()
        {
            var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.Users} SET ");

            sql.Append($"{nameof(UserEntity.IsActivated)} = @{nameof(UserEntity.IsActivated)}");
            sql.Append($" WHERE {nameof(UserEntity.PhoneNumber)} = @{nameof(UserEntity.PhoneNumber)}");

            return sql.ToString();
        }

        string BuildInsertScript()
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.Users));

            sql.Append(" (");
            sql.Append(nameof(UserEntity.UserKeyId));
            sql.Append($", {nameof(UserEntity.Email)}");
            sql.Append($", {nameof(UserEntity.PhoneNumber)}");
            sql.Append($", {nameof(UserEntity.FirstName)}");
            sql.Append($", {nameof(UserEntity.LastName)}");
            sql.Append($", {nameof(UserEntity.Password)}");
            sql.Append($", {nameof(UserEntity.OTP)}");
            sql.Append($", {nameof(UserEntity.OTPExpirationDate)}");
            //sql.Append($", {nameof(UserEntity.IsActivated)}");
            //sql.Append($", {nameof(UserEntity.IsDeleted)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  @{nameof(UserEntity.UserKeyId)}");
            sql.Append($", @{nameof(UserEntity.Email)}");
            sql.Append($", @{nameof(UserEntity.PhoneNumber)}");
            sql.Append($", @{nameof(UserEntity.FirstName)}");
            sql.Append($", @{nameof(UserEntity.LastName)}");
            sql.Append($", @{nameof(UserEntity.Password)}");
            sql.Append($", @{nameof(UserEntity.OTP)}");
            sql.Append($", @{nameof(UserEntity.OTPExpirationDate)}");
            //sql.Append($", @{nameof(UserEntity.IsActivated)}");
            //sql.Append($", @{nameof(UserEntity.IsDeleted)}");
            sql.Append(")");

            return sql.ToString();
        }

        string BuildInsertAdminScript(AdminUserEntity adminUserEntity)
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.AdminUser));

            sql.Append(" (");
            sql.Append(nameof(AdminUserEntity.Username));
            sql.Append($", {nameof(AdminUserEntity.Email)}");
            sql.Append($", {nameof(AdminUserEntity.FirstName)}");
            sql.Append($", {nameof(AdminUserEntity.LastName)}");
            sql.Append($", {nameof(AdminUserEntity.Password)}");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  '{adminUserEntity.Username}'");
            sql.Append($", '{adminUserEntity.Email}'");
            sql.Append($", '{adminUserEntity.FirstName}'");
            sql.Append($", '{adminUserEntity.LastName}'");
            sql.Append($", '{adminUserEntity.Password}'");
            sql.Append(");");
            sql.Append(" SELECT LAST_INSERT_ID();");

            return sql.ToString();
        }

        string BuildInsertUserFavouriteLocation(string userKeyId, int cabinetLocationId)
        {
            var sql = new StringBuilder(string.Concat("INSERT INTO ", GlobalDatabaseConstants.DatabaseTables.UserFavouritesLocations));

            sql.Append(" (");
            sql.Append("UserKeyId");
            sql.Append(", CabinetLocationId");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"  '{userKeyId}'");
            sql.Append($", {cabinetLocationId}");
            sql.Append(");");
            sql.Append(" SELECT LAST_INSERT_ID();");

            return sql.ToString();
        }

        string GetAppUsersScript()
        {
            var query = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Users}");
            return query.ToString();
        }
        string GetUserFromPhoneNumber()
        {
            var query = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Users}");
            query.Append($" WHERE {nameof(UserEntity.PhoneNumber)} = {GlobalDatabaseConstants.QueryParameters.PhoneNumber}");

            return query.ToString();
        }
        string GetUserFromEmail()
        {
            var query = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Users}");
            query.Append($" WHERE {nameof(UserEntity.Email)} = {GlobalDatabaseConstants.QueryParameters.Email}");

            return query.ToString();
        }

        string GetAdminUserScript(bool userName = true, bool emailFilter = true)
        {
            var query = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.AdminUser}");
            if (userName && emailFilter)
            {
                query.Append($" WHERE ( {nameof(AdminUserEntity.Username)} = @{nameof(AdminUserEntity.Username)}");
                query.Append($" AND {nameof(AdminUserEntity.Email)} = @{nameof(AdminUserEntity.Email)})");
            }
            else if (userName || emailFilter)
            {
                query.Append($" WHERE ( {nameof(AdminUserEntity.Username)} = @{nameof(AdminUserEntity.Username)}");
                query.Append($" OR {nameof(AdminUserEntity.Email)} = @{nameof(AdminUserEntity.Email)})");
            }
            return query.ToString();
        }


        string BuildSelectScriptUserByUserKeyId()
        {
            var query = new StringBuilder($"SELECT * FROM {GlobalDatabaseConstants.DatabaseTables.Users}");
            query.Append($" WHERE {nameof(UserEntity.UserKeyId)} = {GlobalDatabaseConstants.QueryParameters.UserKeyId}");

            return query.ToString();
        }

        public async Task<int> UpdateUserEntity(UserEntity userEntity)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(UserEntity.Email)), userEntity.Email);
            p.Add(string.Concat("@", nameof(UserEntity.PhoneNumber)), userEntity.PhoneNumber);
            p.Add(string.Concat("@", nameof(UserEntity.FirstName)), userEntity.FirstName);
            p.Add(string.Concat("@", nameof(UserEntity.LastName)), userEntity.LastName);
            p.Add(string.Concat("@", nameof(UserEntity.Password)), userEntity.Password);

            var sql = BuildUpdateScript();
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p);

                    ret = ret > 0
                        ? GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordUpdated
                        : GlobalConstants.ApplicationMessageNumber.ErrorMessage.NoItemSave;

                    transaction.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError($"Error on updating user {userEntity.Email}. Error: {e.Message}");
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }
        public async Task<int> UpdateUserOTP(UserEntity userEntity)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(UserEntity.UserKeyId)), userEntity.UserKeyId);
            p.Add(string.Concat("@", nameof(UserEntity.OTP)), userEntity.OTP);
            p.Add(string.Concat("@", nameof(UserEntity.OTPExpirationDate)), userEntity.OTPExpirationDate);

            var sql = BuildUpdateOTPScript();
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
                    _logger.LogError($"Error on updating user OTP {userEntity.UserKeyId}. Error: {e.Message}");
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

        public async Task<int> UpdateUserMPIN(UserEntity userEntity)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(UserEntity.UserKeyId)), userEntity.UserKeyId);
            p.Add(string.Concat("@", nameof(UserEntity.MPIN)), userEntity.MPIN);


            var sql = BuildUpdateMPINScript();
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
                    _logger.LogError($"Error on updating user MPIN {userEntity.UserKeyId}. Error: {e.Message}");
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }


        public async Task<int> SetUserEntity(UserEntity userEntity)
        {
            var p = new DynamicParameters();
            string sql;

            p.Add(string.Concat("@", nameof(UserEntity.Email)), userEntity.Email);
            p.Add(string.Concat("@", nameof(UserEntity.PhoneNumber)), userEntity.PhoneNumber);
            p.Add(string.Concat("@", nameof(UserEntity.FirstName)), userEntity.FirstName);
            p.Add(string.Concat("@", nameof(UserEntity.LastName)), userEntity.LastName);

            if (userEntity.UserId <= 0)
            {
                sql = BuildInsertScript();
                p.Add(string.Concat("@", nameof(UserEntity.UserKeyId)), userEntity.UserKeyId);
                p.Add(string.Concat("@", nameof(UserEntity.Password)), userEntity.Password);
                p.Add(string.Concat("@", nameof(UserEntity.OTP)), userEntity.OTP);
                p.Add(string.Concat("@", nameof(UserEntity.OTPExpirationDate)), userEntity.OTPExpirationDate);
            }
            else
            {
                sql = BuildUpdateScript();
                p.Add(string.Concat("@", nameof(UserEntity.UserKeyId)), userEntity.UserKeyId);
                p.Add(string.Concat("@", nameof(UserEntity.Password)), userEntity.Password);
                p.Add(string.Concat("@", nameof(UserEntity.Photo)), userEntity.Photo);
                p.Add(string.Concat("@", nameof(UserEntity.IsLowerLockerPrefered)), userEntity.IsLowerLockerPrefered);

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
                        if (userEntity.UserId <= 0)
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

        public async Task<UserEntity> GetUserByUserKeyId(string userKeyId)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.UserKeyId, userKeyId);

            var sql = BuildSelectScriptUserByUserKeyId();
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<UserEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }

        public async Task<UserEntity> GetUserFromPhoneNo(string phoneNumber)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.PhoneNumber, phoneNumber);

            var sql = GetUserFromPhoneNumber();
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<UserEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }

        public async Task<List<UserEntity>> GetAppUsers()
        {
            var sql = GetAppUsersScript();
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<UserEntity>(sql);

                return dbModel.ToList();
            }
        }

        public async Task<UserEntity> GetUserFromEmail(string email)
        {
            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.Email, email);

            var sql = GetUserFromEmail();
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<UserEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }

        public async Task<int> GetLastIdentity()
        {
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();

                try
                {
                    var ret = await conn.ExecuteScalarAsync<int>($"SELECT {nameof(UserEntity.UserId)} FROM {GlobalDatabaseConstants.DatabaseTables.Users} ORDER BY {nameof(UserEntity.UserId)} DESC LIMIT 1");

                    return ret;
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error on fetching the last user inserted id. Error: {e.Message}");
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

        public async Task<int> ActivateUser(string phoneNumber)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(UserEntity.PhoneNumber)), phoneNumber);
            p.Add(string.Concat("@", nameof(UserEntity.IsActivated)), 1);

            // {nameof(UserEntity.PhoneNumber)} = {Constants.QueryParameters.PhoneNumber}

            var sql = BuildUpdateActivateScript();

            //var sql = $"SELECT * FROM {Constants.DatabaseTables.Users} WHERE {nameof(UserEntity.UserKeyId)} = @{nameof(UserEntity.UserKeyId)} " +
            //            $" AND  {nameof(UserEntity.IsActivated)} = @{nameof(UserEntity.IsActivated)} ";

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

        public async Task<AdminUserEntity> GetAdminUser(string username, string email)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(AdminUserEntity.Username)), username);
            p.Add(string.Concat("@", nameof(AdminUserEntity.Email)), email);

            var sql = GetAdminUserScript(!string.IsNullOrEmpty(username), !string.IsNullOrEmpty(email));
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<AdminUserEntity>(sql, p);

                return dbModel.FirstOrDefault();
            }
        }
        public async Task<List<AdminUserEntity>> GetAdminUsers(string userName = null, string email = null)
        {
            var p = new DynamicParameters();

            p.Add(string.Concat("@", nameof(AdminUserEntity.Username)), userName);
            p.Add(string.Concat("@", nameof(AdminUserEntity.Email)), email);

            var sql = GetAdminUserScript(!string.IsNullOrEmpty(userName), !string.IsNullOrEmpty(email));
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var dbModel = await conn.QueryAsync<AdminUserEntity>(sql, p);

                return dbModel.ToList();
            }
        }

        public async Task<int> SetAdminUserEntity(AdminUserEntity adminUserEntity)
        {
            string sql = string.Empty;
            if (adminUserEntity.AdminUserId > 0)
                sql = BuildUpdateAdminScript(adminUserEntity);
            else
                sql = BuildInsertAdminScript(adminUserEntity);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = 0;
                    if (adminUserEntity.AdminUserId > 0)
                    {
                        ret = await conn.ExecuteAsync(sql, transaction: transaction);
                        if (ret > 0)
                            ret = adminUserEntity.AdminUserId;
                    }
                    else
                        ret = await conn.ExecuteScalarAsync<int>(sql, transaction: transaction);
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
        public async Task<int> DeleteAppUserEntity(string userKeyId)
        {
            var p = new DynamicParameters();


            string sql = $"delete from user_token where userid in (select userid from users where userKeyid = @UserKeyId;";
            sql = sql + $"delete from user_subscription where userkeyid in (select userkeyid from users where userKeyid = @UserKeyId);";
            sql = sql + $"delete from locker_booking_history where lockertransactionsId in  (select lockertransactionsid from locker_bookings lb, users u where lb.userkeyid = u.userkeyid and u.userkeyid =@UserKeyId);";
            sql = sql + $"delete from reassigned_booking_locker_history where lockertransactionsId in  (select lockertransactionsid from locker_bookings lb, users u where lb.userkeyid = u.userkeyid and u.userkeyid =@UserKeyId);";
            sql = sql + $"delete from payment_transaction where lockertransactionsId in  (select lockertransactionsid from locker_bookings lb, users u where lb.userkeyid = u.userkeyid and u.userkeyid =@UserKeyId);";
            sql = sql + $"delete from locker_bookings where  userkeyid in (select userkeyid from users where userKeyid = @UserKeyId;";
            sql = sql + $"delete from users where userkeyid = @UserKeyId;";



            p.Add("@UserKeyId", userKeyId);


            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();
                //  IDbTransaction transaction = conn.BeginTransaction();

                try
                {
                    var ret = await conn.ExecuteAsync(sql, p);
                    if (ret > 0)
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordDeleted;
                    else
                        ret = GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;


                    // transaction.Commit();
                    return ret;
                }
                catch (Exception e)
                {
                    // transaction.Rollback();
                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }
        public async Task<int> DeleteAdminUserEntity(int id)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(AdminUserEntity.AdminUserId)), id);
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

                    if (e.Message.Contains("foreign key constraint"))
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.RoleStillExists;


                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }
        string BuildDeleteCommand()
        {
            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.AdminUser));

            sql.Append(string.Concat(" WHERE ", nameof(AdminUserEntity.AdminUserId), " = ", "@", nameof(AdminUserEntity.AdminUserId)));

            return sql.ToString();

        }
        public async Task<int> UpdateUserPassword(int userId, string newHashPassword)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", nameof(UserEntity.UserId)), userId);
            p.Add(string.Concat("@", nameof(UserEntity.Password)), newHashPassword);

            var sql = BuildUpdateUserPasswordScript();
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
                    _logger.LogError($"Error on updating user new hash Password with User Id: {userId}. Error: {e.Message}");
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
            string BuildUpdateUserPasswordScript()
            {
                var sql = new StringBuilder($"UPDATE {GlobalDatabaseConstants.DatabaseTables.Users} SET ");

                sql.Append($"{nameof(UserEntity.Password)} = @{nameof(UserEntity.Password)}");

                sql.Append($" WHERE {nameof(UserEntity.UserId)} = @{nameof(UserEntity.UserId)}");

                return sql.ToString();
            }

        }

        public async Task<int> CreateUserFavouriteCabinetLocations(string userKeyId, int cabinetLocationId)
        {

            string sql = string.Empty;


            sql = BuildInsertUserFavouriteLocation(userKeyId, cabinetLocationId);

            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();


                try
                {
                    var ret = 0;

                    ret = await conn.ExecuteAsync(sql);

                    ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordAdded;

                    return ret;
                }
                catch (Exception e)
                {

                    _logger.LogError(e.Message);
                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

        public async Task<int> DeleteUserFavouriteCabinetLocations(int id)
        {
            var p = new DynamicParameters();
            p.Add(string.Concat("@", "Id"), id);

            var sql = new StringBuilder(string.Concat("DELETE from ", GlobalDatabaseConstants.DatabaseTables.UserFavouritesLocations));

            sql.Append(" WHERE id=@Id;");



            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                conn.Open();


                try
                {
                    var ret = await conn.ExecuteAsync(sql.ToString(), p);
                    if (ret > 0)
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.RecordDeleted;
                    else
                        ret = GlobalConstants.ApplicationMessageNumber.InformationMessage.NoRecords;



                    return ret;
                }
                catch (Exception e)
                {

                    _logger.LogError("Error deleting record in DeleteUserFavouritesLocations " + e.Message);

                    if (e.Message.Contains("foreign key constraint"))
                        return GlobalConstants.ApplicationMessageNumber.ErrorMessage.DeleteConstraintsError;


                    return GlobalConstants.ApplicationMessageNumber.ErrorMessage.UnexpectedError;
                }
            }
        }

        public async Task<List<CabinetLocationEntity>> GetUserFavoritesCabinetLocations(string userKeyId)
        {
            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                var p = new DynamicParameters();
                p.Add(string.Concat("@", "userKeyId"), userKeyId);
                var sql = $"SELECT  fav.Id, cl.* " +
                           "FROM vw_active_cabinet_location AS cl " +
                           "JOIN user_favourites_cabinet_locations AS fav " +
                           "ON cl.CabinetLocationId = fav.cabinetLocationId " +
                           "WHERE cl.CabinetLocationId IN (SELECT cabinetLocationId FROM user_favourites_cabinet_locations WHERE userKeyId = @userKeyId);";
                var dbModel = await conn.QueryAsync<CabinetLocationEntity>(sql, p);

                return dbModel.ToList();

            }

        }

        public async Task<List<UserFavouriteLocationModel>> GetUserFavoritesCabinetLocationsList(string userKeyId)
        {
            
            
                using (IDbConnection conn = this._databaseHelper.GetConnection())
                {
                try
                {
                    var p = new DynamicParameters();
                    p.Add(string.Concat("@", "userKeyId"), userKeyId);
                    var sql = $"SELECT CabinetLocationId, UserKeyId  FROM user_favourites_cabinet_locations where UserKeyId=@userKeyId;";


                   var  dbModel = await conn.QueryAsync<UserFavouriteLocationModel>(sql, p);

                    return dbModel.ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error on GetUserFavoritesCabinetLocationsList: " + ex.Message);

                }
                return null;

            }
           
        }

    }
}
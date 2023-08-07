using Dapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core.Entities.Cabinet;
using SmartBox.Business.Core.Models.Dashboard;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Data;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Dashboard
{
    public class DashboardRepository : GenericRepositoryBase<DashboardViewModel, DashboardRepository>, IDashboardRepository
    {
        public DashboardRepository(IDatabaseHelper databaseHelper, ILogger<DashboardRepository> logger) : base(databaseHelper, logger)
        {

        }

        public DashboardViewModel GetDashboardData(int? companyid = null)
        {
            var builder = new SqlBuilder();
            builder.Select("*");

            var p = new DynamicParameters();
            p.Add(GlobalDatabaseConstants.QueryParameters.CompanyId, companyid);

            var sp_GetBookingsCount = GlobalDatabaseConstants.StoredProcedures.GetBookingsCount;
            var sp_GetDropOffCount = GlobalDatabaseConstants.StoredProcedures.GetDropOffCount;
            var sp_GetLocationsCount = GlobalDatabaseConstants.StoredProcedures.GetLocationsCount;
            var sp_GetDeliveriesCount = GlobalDatabaseConstants.StoredProcedures.GetDeliveriesCount;
            var sp_GetRecentBookings = GlobalDatabaseConstants.StoredProcedures.GetRecentBookings;
            var sp_GetMostBookedLockers = GlobalDatabaseConstants.StoredProcedures.GetMostBookedLockers;
            var sp_GetRevenue = GlobalDatabaseConstants.StoredProcedures.GetRevenue;
            var sp_GetTodayNotification = GlobalDatabaseConstants.StoredProcedures.GetTodayNotifications;
            var sp_GetYesterdayNotification = GlobalDatabaseConstants.StoredProcedures.GetYesterdayNotifications;


            using (IDbConnection conn = this._databaseHelper.GetConnection())
            {
                  
                var dbBookings = conn.Query<string>(sp_GetBookingsCount, p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                var dbDropOffs = conn.Query<string>(sp_GetDropOffCount, p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                var dbLocations = conn.Query<string>(sp_GetLocationsCount, p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                var dbDeliveries = conn.Query<string>(sp_GetDeliveriesCount, p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                var dbRecentBookings = conn.Query<RecentBookingsViewModel>(sp_GetRecentBookings, p, commandType: CommandType.StoredProcedure).ToList();
                var dbMostBookedLockers = conn.Query<MostBookedLockerViewModel>(sp_GetMostBookedLockers, p, commandType: CommandType.StoredProcedure).ToList();
                var dbRevenues = conn.Query<RevenueViewModel>(sp_GetRevenue, p, commandType: CommandType.StoredProcedure).ToList();
                var dbTodayNotification = conn.Query<TodayNotifications>(sp_GetTodayNotification, p, commandType: CommandType.StoredProcedure).ToList();
                var dbYesterdayNotification = conn.Query<YesterdayNotifications>(sp_GetYesterdayNotification, p, commandType: CommandType.StoredProcedure).ToList();
                var recentBookings = new List<RecentBookingsViewModel>();
                foreach (var recentBooking in dbRecentBookings)
                {
                    var data = new RecentBookingsViewModel()
                    {
                        Description = recentBooking.Description,
                        LockerTransactionsId = recentBooking.LockerTransactionsId.ToString(),
                        Size = recentBooking.Size,
                        DateCreated = recentBooking.DateCreated
                    };
                    recentBookings.Add(data);
                }

                var mostBookings = new List<MostBookedLockerViewModel>();
                foreach(var mostBooking in dbMostBookedLockers)
                {
                    var data = new MostBookedLockerViewModel()
                    {
                        LockerDetailId = mostBooking.LockerDetailId,
                        Description = mostBooking.Description,
                        Percentage = mostBooking.Percentage.ToString(),
                        Size = mostBooking.Size
                    };
                    mostBookings.Add(data);
                }
                var revenues = new List<RevenueViewModel>();
                foreach (var revenue in dbRevenues)
                {
                    var data = new RevenueViewModel()
                    {
                        Month = revenue.Month,
                        Revenue = revenue.Revenue
                    };
                    revenues.Add(data);
                }
                var todayNotifications = new List<TodayNotifications>();
                foreach(var notification in dbTodayNotification)
                {
                    var data = new TodayNotifications()
                    {
                        DateCreated = notification.DateCreated,
                        Description = notification.Description,
                    };

                    todayNotifications.Add(data);
                }
                var yesterdayNotifications = new List<YesterdayNotifications>();
                foreach (var notification in dbYesterdayNotification)
                {
                    var data = new YesterdayNotifications()
                    {
                        DateCreated = notification.DateCreated,
                        Description = notification.Description,
                    };

                    yesterdayNotifications.Add(data);
                }
                var notifications = new NotificationsViewModel();
                notifications.TodayNotifications = todayNotifications;
                notifications.YesterdayNotifications = yesterdayNotifications;
                var DashboardViewModel = new DashboardViewModel();
                DashboardViewModel.Bookings = dbBookings.ToString();
                DashboardViewModel.Drops = dbDropOffs.ToString();
                DashboardViewModel.Locations = dbLocations.ToString();
                DashboardViewModel.Deliveries = dbDeliveries.ToString();
                DashboardViewModel.RecentBookings = recentBookings;
                DashboardViewModel.MostBookedLocker = mostBookings;
                DashboardViewModel.Revenue = revenues;
                DashboardViewModel.Notifications = notifications;
                return DashboardViewModel;
            }
        }
    }
}

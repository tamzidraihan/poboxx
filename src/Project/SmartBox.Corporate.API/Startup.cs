using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SmartBox.Infrastructure.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.User;
using SmartBox.Infrastructure.Data.Repository.AppMessage;
using SmartBox.Infrastructure.Data.Repository.User;
using SmartBox.Business.Services.Service.LogIn;
using SmartBox.Business.Core;
using System.Reflection;
using SmartBox.Infrastructure.Data.Repository.ApplicationSettings;
using SmartBox.Corporate.API.Application;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using SmartBox.Corporate.API.Services;
using SmartBox.Business.Core.Models.Notification;
using Microsoft.Extensions.Options;
using SmartBox.Business.Core.Models.Paymaya;
using SmartBox.Business.Services.Service.Notification;
using SmartBox.Business.Services.Service.OTP;
using SmartBox.Business.Core.Models.TextValue;
using SmartBox.Business.Services.Service.Location;
using SmartBox.Infrastructure.Data.Repository.Cabinet;
using SmartBox.Business.Services.Service.Cabinet;
using SmartBox.Business.Services.Service.Locker;
using SmartBox.Infrastructure.Data.Repository.Locker;
using SmartBox.Business.Services.Service.Payment;
using SmartBox.Infrastructure.Data.Repository.Payment;
using SmartBox.Business.Core.Models.PayMongo;
using SmartBox.Business.Services.Service.ApplicationSetting;
using SmartBox.Business.Core.Models.Email;
using SmartBox.Infrastructure.Data.Repository.EmailMessage;
using SmartBox.Infrastructure.Data.Repository.Company;
using SmartBox.Business.Services.Service.Company;
using SmartBox.Business.Services.Service.ParentCompany;
using SmartBox.Infrastructure.Data.Repository.ParentCompany;
using SmartBox.Infrastructure.Data.Repository.Pricing;
using SmartBox.Business.Services.Service.Pricing;
using SmartBox.Business.Services.Service.Dashboard;
using SmartBox.Infrastructure.Data.Repository.Dashboard;
using SmartBox.Business.Services.Service.Announcement;
using SmartBox.Infrastructure.Data.Repository.Announcement;
using SmartBox.Business.Services.Service.Maintenance;
using SmartBox.Infrastructure.Data.Repository.Maintenance;
using SmartBox.Infrastructure.Data.Repository.CompanyUser;
using SmartBox.Business.Services.Service.CompanyUser;
using SmartBox.Infrastructure.Data.Repository.Report;
using SmartBox.Business.Services.Service.Report;
using SmartBox.Business.Services.Service.Role;
using SmartBox.Infrastructure.Data.Repository.Role;
using SmartBox.Business.Services.Service.UserRole;
using SmartBox.Infrastructure.Data.Repository.UserRole;
using SmartBox.Business.Services.Service.RolePermission;
using SmartBox.Infrastructure.Data.Repository.Permission;
using SmartBox.Business.Services.Service.Permission;
using SmartBox.Infrastructure.Data.Repository.RolePermission;
using Hangfire;
using Hangfire.MySql;
using SmartBox.Business.Shared;
using SmartBox.Business.Services.Service.Notification.PushNotification;
using SmartBox.Infrastructure.Data.Repository.Logs;
using SmartBox.Business.Services.Service.Feedback;
using SmartBox.Infrastructure.Data.Repository.Feedback;
using SmartBox.Infrastructure.Data.Repository.FranchiseFeedbackQuestion;
using SmartBox.Business.Services.Service.FranchiseFeedbackQuestion;
using HangfireBasicAuthenticationFilter;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Globalization;
using SmartBox.Business.Services.Service.EmailMessage;
using SmartBox.Business.Services.Service.HTTPService;
using SmartBox.Business.Services.Service.Logs;

namespace SmartBox.Corporate.API
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = System.Environment.GetEnvironmentVariable("DBConnection");//this.Configuration.GetConnectionString("DBConnection");

            Action<TextValueModel> location = (opt =>
            {
                opt.Name = this.Configuration.GetSection("PHLocation:Url").Value;
            });

            Action<SMSOptionModel> smsOption = (opt =>
            {
                opt.Host = this.Configuration.GetSection("SMS:Host").Value;
                opt.AppKey = System.Environment.GetEnvironmentVariable("SMSAppKey");//this.Configuration.GetSection("SMS:AppKey").Value;
                opt.Secret =  System.Environment.GetEnvironmentVariable("SMSSecret");//this.Configuration.GetSection("SMS:Secret").Value;    
                opt.Username = this.Configuration.GetSection("SMS:Username").Value;
                opt.Password = System.Environment.GetEnvironmentVariable("SMSPassword");//this.Configuration.GetSection("SMS:Password").Value;
                opt.ShortcodeMask = this.Configuration.GetSection("SMS:Shortcode").Value;
            });

            Action<PaymongoSettingsModel> paymongo = (opt =>
            {
                opt.AuthorizationKey = this.Configuration.GetSection("PaymongoSettings:AuthorizationKey").Value;
                opt.PaymentUrl = this.Configuration.GetSection("PaymongoSettings:PaymentUrl").Value;
                opt.PaymentSourcesUrl = this.Configuration.GetSection("PaymongoSettings:PaymentSourcesUrl").Value;
                opt.SuccessAuthorizeUrl = this.Configuration.GetSection("PaymongoSettings:SuccessAuthorizeUrl").Value;
                opt.FailedAuthorizeUrl = this.Configuration.GetSection("PaymongoSettings:FailedAuthorizeUrl").Value;
                opt.SuccessPaymentUrl = this.Configuration.GetSection("PaymongoSettings:SuccessPaymentUrl").Value;
                opt.FailedPaymentUrl = this.Configuration.GetSection("PaymongoSettings:FailedPaymentUrl").Value;

            });

            Action<PaymayaSettingsModel> paymaya = (opt =>
            {
                opt.AuthorizationKey = System.Environment.GetEnvironmentVariable("MAYA_PUBLIC_KEY");// this.Configuration.GetSection("PaymayaSettings:PublicKey").Value;
                opt.SuccessAuthorizeUrl = this.Configuration.GetSection("PaymayaSettings:SuccessAuthorizeUrl").Value;
                opt.FailedAuthorizeUrl = this.Configuration.GetSection("PaymayaSettings:FailedAuthorizeUrl").Value;
                opt.CancelPaymentUrl = this.Configuration.GetSection("PaymayaSettings:CancelUrl").Value;
                opt.SuccessPaymentUrl = this.Configuration.GetSection("PaymayaSettings:SuccessPaymentUrl").Value;
                opt.FailedPaymentUrl = this.Configuration.GetSection("PaymayaSettings:FailedPaymentUrl").Value;
                opt.PaymentUrl = this.Configuration.GetSection("PaymayaSettings:PaymentUrl").Value;
                opt.CheckOutUrl = this.Configuration.GetSection("PaymayaSettings:CheckOutUrl").Value;
            });


            Action<EmailSettingsModel> email = (opt =>
            {
                opt.Key = this.Configuration.GetSection("SendgridSettings:Key").Value;
                opt.SenderName = this.Configuration.GetSection("SendgridSettings:SenderName").Value;
                opt.From = this.Configuration.GetSection("SendgridSettings:From").Value;
            });

            services.Configure<FirebaseConfiguration>(Configuration.GetSection("FirebaseConfiguration"));


            services.Configure<GlobalConfigurations>(Configuration.GetSection("GlobalConfigurations"));
            //services.Configure<HangfireConfig>(Configuration.GetSection("Hangfire"));
            services.Configure<SMTPConfiguration>(Configuration.GetSection("SMTPConfiguration"));
            

            services.Configure(email);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<EmailSettingsModel>>().Value);

            services.Configure(location);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<TextValueModel>>().Value);

            services.Configure(smsOption);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<SMSOptionModel>>().Value);

            services.Configure(paymongo);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<PaymongoSettingsModel>>().Value);

            services.Configure(paymaya);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<PaymayaSettingsModel>>().Value);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartBox.Corporate.API", Version = "v1" });
                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Description = "Please Enter Authentication Token",
                    Name = "Authorization"

                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearer"
                            }
                        },
                        new List<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.ConfigureJwt(Configuration);
            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 443;
            });



            services.AddMiniProfiler(options => options.RouteBasePath = "/profiler");
            services.AddScoped<IDatabaseHelper>(x => new DatabaseHelper(connection, Environment));

            services.AddScoped<IAppMessageService, AppMessageService>();
            services.AddScoped<IApplicationMessageService, ApplicationMessageService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILogInService, LogInService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ICabinetService, CabinetService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ILockerService, LockerService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IApplicationSettingService, ApplicationSettingService>(); 
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IParentCompanyService, ParentCompanyService>();
            services.AddScoped<IPricingService, PricingService>();
            services.AddScoped<IAnnouncementService, AnnouncementService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<ICompanyUserService, CompanyUserService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRolePermissionService, RolePermissionService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IEmailMessageService, EmailMessageService>();
            services.AddScoped<IFranchiseFeedbackService, FranchiseFeedbackService>();
            services.AddScoped<ILogService, LogService>();

            services.AddScoped<IFCMProvider, FCMProvider>();
            services.AddScoped<IFCMSender, FCMSender>();
            services.AddScoped<IApnSender, ApnSender>();

            services.AddScoped<IAppMessageRepository, AppMessageRepository>(); 
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IApplicationSettingRepository, ApplicationSettingRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<ICabinetRepository, CabinetRepository>();
            services.AddScoped<ILockerRepository, LockerRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IEmailMessageRepository, EmailMessageRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IParentCompanyRepository, ParentCompanyRepository>();
            services.AddScoped<IPricingTypeRepository, PricingTypeRepository>();
            services.AddScoped<IPriceAndChargingRepository, PriceAndChargingRepository>();
            services.AddScoped<IPricingMatrixConfigRepository, PricingMatrixConfigRepository>();
            services.AddScoped<IAnnouncementTypeRepository, AnnouncementTypeRepository>();
            services.AddScoped<IPromoAnnouncementRepository, PromoAnnouncementRepository>();
            services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
            services.AddScoped<IMaintenanceInspectionTestingRepository, MaintenanceInspectionTestingRepository>();
            services.AddScoped<IChargesRepository, ChargesRepository>();
            services.AddScoped<IMaintenanceReasonTypeRepository, MaintenanceReasonTypeRepository>();
            services.AddScoped<ICompanyUserRepository, CompanyUserRepository>();
            services.AddScoped<ICleanlinessReportRepository, CleanlinessReportRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRolePermissionsRepository, RolePermissionsRepository>();
            services.AddScoped<IUserSubscriptionBillingRepository, UserSubscriptionBillingRepository>();
            services.AddScoped<IUserTokenRepository, UserTokenRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IFranchiseFeedbackRepository, FranchiseFeedbackRepository>();
            services.AddHttpClient<IHttpService, HttpService>();

            // Add Hangfire services.
            //string hangfireConnectionString = System.Environment.GetEnvironmentVariable("HangfireDBConnection");//Configuration.GetConnectionString("HangfireDBConnection");
            //services.AddHangfire(configuration => configuration
            //    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //    .UseSimpleAssemblyNameTypeSerializer()
            //    .UseRecommendedSerializerSettings()
            //    .UseStorage(
            //        new MySqlStorage(
            //            hangfireConnectionString,
            //            new MySqlStorageOptions
            //            {
            //                QueuePollInterval = TimeSpan.FromSeconds(10),
            //                JobExpirationCheckInterval = TimeSpan.FromHours(1),
            //                CountersAggregateInterval = TimeSpan.FromMinutes(5),
            //                PrepareSchemaIfNecessary = true,
            //                DashboardJobListLimit = 25000,
            //                TransactionTimeout = TimeSpan.FromMinutes(1),
            //                TablesPrefix = "Hangfire",
            //            }
            //        )
            //    ));

            //// Add the processing server as IHostedService
            //services.AddHangfireServer(options => options.WorkerCount = 1);

            services.AddScoped<ICompanyCabinetRepository, CompanyCabinetRepository>();
            services.AddScoped<IMessageLogRepository, MessageLogRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartBox.Corporate.API v1"));
           

            // app.UseHttpsRedirection();
            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

           // app.UseHangfireDashboard("/hangfire", new DashboardOptions
           // {
           //     Authorization = new[]
           //   {
           //     new HangfireCustomBasicAuthenticationFilter
           //     {
           //         User = Configuration.GetSection("Hangfire:UserName").Value,
           //         Pass = System.Environment.GetEnvironmentVariable("HangfirePassword")//Configuration.GetSection("Hangfire:Password").Value
           //     }
           //   }
           // });

           ///* JobScheduler.AddOrUpdateRecurringJob<IUserService>("ExpiredLockerBookingsNotifications",
           //     s => s.ExpiredLockerBookingsNotifications(), Configuration.GetSection("Hangfire:ExpiredLockerBookingsNotifications_CronsExpression").Value);
           //*/
           // JobScheduler.AddOrUpdateRecurringJob<IReportService>("MaintenanceReportReminderNotification",
           //     s => s.MaintenanceReportReminderNotification(), Configuration.GetSection("Hangfire:MaintenanceReportReminderNotification_CronsExpression").Value);

           // JobScheduler.AddOrUpdateRecurringJob<IReportService>("MaintenanceReportOverdueNotification",
           //    s => s.MaintenanceReportOverdueNotification(), Configuration.GetSection("Hangfire:MaintenanceReportOverdueNotification_CronsExpression").Value);

            app.UseMiniProfiler();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseHttpsRedirection();
            app.UseJwtTokenMiddleware();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // Cache static files for 30 days
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=2592000");
                    ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddDays(30).ToString("R", CultureInfo.InvariantCulture));
                },
                ServeUnknownFileTypes = false,
                FileProvider = new PhysicalFileProvider(
                        Path.Combine(env.WebRootPath, "Images")),
                RequestPath = "/img",

            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using AutoMapper;
using Microsoft.Extensions.Options;
using NPOI.SS.Formula.Functions;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Report;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.Locker;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Core.Models.Report;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Services.Service.Notification;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.Report;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Business.Services.Service.Report
{
    public class ReportService : BaseMessageService<ReportService>, IReportService
    {
        private readonly ICleanlinessReportRepository cleanlinessReportRepository;
        private readonly INotificationService notificationService;
        private readonly HangfireConfig hangfireConfig;
        public ReportService(IAppMessageService appMessageService,
            IMapper mapper,
            IOptions<HangfireConfig> hangfireConfig,
            INotificationService notificationService,
            ICleanlinessReportRepository cleanlinessReportRepository
            ) : base(appMessageService, mapper)
        {
            this.cleanlinessReportRepository = cleanlinessReportRepository;
            this.notificationService = notificationService;
            this.hangfireConfig = hangfireConfig.Value;
        }
        public async Task<ResponseValidityModel> SaveCleanlinessReport(CleanlinessReportModel param)
        {
            var model = ValidateCleanlinessReport(param);

            if (model.MessageReturnNumber == 0)
            {

                var entity = Mapper.Map<CleanlinessReportModel, CleanlinessReportEntity>(param);
                var ret = await cleanlinessReportRepository.Save(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            return model;
        }
        ResponseValidityModel ValidateCleanlinessReport(CleanlinessReportModel param)
        {
            var model = new ResponseValidityModel();
           
            if (param.CompanyId < 1 || param.CabinetId < 1 ||
               (param.LockerPictures.Count > 0 && param.LockerPictures.Any(s => s.LockerNumber == null || s.LockerNumber == ""))
                || string.IsNullOrWhiteSpace(param.FrontPhoto) || string.IsNullOrWhiteSpace(param.LeftPhoto) || string.IsNullOrWhiteSpace(param.RightPhoto)
                )
            {
                if (param.CompanyId < 1)
                    model.MessagesList.Add(nameof(CleanlinessReportModel.CompanyId));

                if (param.CabinetId < 1)
                    model.MessagesList.Add(nameof(CleanlinessReportModel.CabinetId));

                if (param.LockerPictures.Count > 0 && param.LockerPictures.Any(s => s.LockerNumber == null || s.LockerNumber == ""))
                    model.MessagesList.Add(nameof(LockerPictureEntity.LockerNumber));

                if (!param.FrontPhoto.HasText())
                    model.MessagesList.Add(nameof(CleanlinessReportModel.FrontPhoto));

                if (!param.LeftPhoto.HasText())
                    model.MessagesList.Add(nameof(CleanlinessReportModel.LeftPhoto));

                if (!param.RightPhoto.HasText())
                    model.MessagesList.Add(nameof(CleanlinessReportModel.RightPhoto));

                if (model.MessagesList.Count > 0)
                {
                    model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldRequired).MappedResponseValidityModel(messageList: model.MessagesList);
                    model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, model.MessagesList.Count > 1 ? Shared.SharedServices.SetVerbMessage(true) : Shared.SharedServices.SetVerbMessage(false));
                    return model;
                }
            }
          
            return model;
        }
        public async Task<CleanlinessReportViewModel> CheckIsExist(int CompanyId, int Month, int Year)
        {
            var dbModel = await cleanlinessReportRepository.CheckIsExist(CompanyId,Month,Year);
            if (dbModel == null)
                return new CleanlinessReportViewModel();

            var model = Mapper.Map<CleanlinessReportViewModel>(dbModel);

            return model;
        }
        public async Task<CleanlinessReportParentViewModel> GetCleanlinessReport(int Month, int Year, int PageNumber, int PageSize, int? CompanyId, int? Status)
        {
            var dbModel = await cleanlinessReportRepository.Get(Month,Year,PageNumber,PageSize,CompanyId,Status);
            var model = new List<CleanlinessReportViewModel>();
            var viewModel = new CleanlinessReportParentViewModel();
            if (dbModel.Count > 0)
            {
               
                var grp = dbModel.GroupBy(s => s.CompanyName).Select(s => new { s.Key, s }).ToList();
               

                foreach (var item in grp)
                {
                    var firstItem = item.s.Count() > 0 ? item.s.First() : new CleanlinessReportEntity();
                    if (firstItem != null)
                    {
                        var pictures = new List<LockerPictureModel>();
                        foreach (var picture in item.s)
                        {
                            pictures.Add(new LockerPictureModel
                            {
                                InsidePhoto = picture.InsidePhoto,
                                LockerNumber = picture.LockerNumber,
                                OutsidePhoto = picture.OutsidePhoto
                            });
                        }

                        model.Add(new CleanlinessReportViewModel
                        {
                            Id = firstItem.Id,
                            CabinetId = firstItem.CabinetId,
                            CompanyBusinessName = firstItem.CompanyBusinessName,
                            LockerPictures = pictures,
                            CompanyId = firstItem.CompanyId,
                            CompanyName = firstItem.CompanyName,
                            CompanyOwnerName = firstItem.CompanyOwnerName,
                            DateSubmitted = firstItem.DateSubmitted,
                            FrontPhoto = firstItem.FrontPhoto,
                            IsSubmitted = firstItem.IsSubmitted,
                            LeftPhoto = firstItem.LeftPhoto,
                            Message = firstItem.Message,
                            Status = firstItem.Status,
                            Month = firstItem.Month,
                            RightPhoto = firstItem.RightPhoto
                           
                        });
                    }
                }

              
                
                 viewModel.Count = model.Count;
                 viewModel.CleanlinessReportViewModel = model.Skip((PageNumber - 1) * PageSize)
                                    .Take(PageSize).ToList();
           

            }

            return viewModel;
        }
        public async Task<ResponseValidityModel> DeleteCleanlinessReport(int id)
        {
            var model = new ResponseValidityModel();

            if (id > 0)
            {
                var ret = await cleanlinessReportRepository.Delete(id);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model.MessageReturnNumber = 1;
            return model;
        }
        public async Task MaintenanceReportReminderNotification()
        {
            await MissingMaintenanceReport_Notification(true);
        }
        public async Task MaintenanceReportOverdueNotification()
        {
            await MissingMaintenanceReport_Notification();
        }
        private async Task MissingMaintenanceReport_Notification(bool isForReminder = false)
        {
            var currentDate = DateTime.Now;
            var currentMonth = currentDate.Month;
            if (hangfireConfig.EmailNotificationEnable)
            {
                var allUnsubmittedMaintenanceReports = await cleanlinessReportRepository.GetUnsubmittedMaintenanceReport(currentMonth);
                string subject = "";
                string message = "";
                if (allUnsubmittedMaintenanceReports.Count > 0)
                {
                    if (isForReminder)
                    {
                        var remainingDays = currentDate.AddDays(1 - currentDate.Day).AddMonths(1).AddDays(-1).Day - currentDate.Day;
                        subject = (AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.MaintenanceReportReminderNotificationSubject).MappedResponseValidityModel()).Message;
                        message = (AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.MaintenanceReportReminderNotificationMessage).MappedResponseValidityModel()).Message.Replace("{Month}", $"'{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(currentMonth)}'").Replace("{Days}", $"'{remainingDays.ToString()}'");
                    }
                    else
                    {
                        subject = (AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.MaintenanceReportNotificationSubject).MappedResponseValidityModel()).Message;
                        message = (AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.MaintenanceReportNotificationMessage).MappedResponseValidityModel()).Message.Replace("{Month}", $"'{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(currentMonth)}'");
                    }
                }
                foreach (var item in allUnsubmittedMaintenanceReports)
                {
                    if (string.IsNullOrEmpty(item.Email)) continue;
                    await notificationService.SendSmtpEmailAsync(new EmailModel
                    {
                        Type = MessageType.PlainTextContent,
                        To = item.Email,
                        Subject = subject,
                        Message = message
                    });

                }
            }
        }
    }
}

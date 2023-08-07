using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartBox.Business.Core;
using SmartBox.Business.Core.Entities.Company;
using SmartBox.Business.Core.Entities.CompanyUser;
using SmartBox.Business.Core.Models.Company;
using SmartBox.Business.Core.Models.CompanyUser;
using SmartBox.Business.Core.Models.Email;
using SmartBox.Business.Core.Models.Notification;
using SmartBox.Business.Core.Models.ResponseValidity;
using SmartBox.Business.Services.Service.AppMessage;
using SmartBox.Business.Services.Service.Base;
using SmartBox.Business.Services.Service.Notification;
using SmartBox.Business.Shared;
using SmartBox.Business.Shared.Extensions;
using SmartBox.Infrastructure.Data.Repository.ApplicationSettings;
using SmartBox.Infrastructure.Data.Repository.Cabinet;
using SmartBox.Infrastructure.Data.Repository.Company;
using SmartBox.Infrastructure.Data.Repository.CompanyUser;
using SmartBox.Infrastructure.Data.Repository.ParentCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartBox.Business.Shared.GlobalConstants;

namespace SmartBox.Business.Services.Service.Company
{
    public class CompanyService : BaseMessageService<CompanyService>, ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ICabinetRepository _cabinetRepository;
        private readonly IParentCompanyRepository _parenCompanyRepository;
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<CompanyService> _logger;
        private readonly IApplicationSettingRepository _applicationSettingRepository;
        private string _defaultPwd;
        private readonly ICompanyCabinetRepository companyCabinetRepository;
        public string CompanyName { get; set; }
        public CompanyService(IAppMessageService appMessageService, IMapper mapper, ICompanyRepository companyRepository,
                              ICabinetRepository cabinetRepository, ICompanyCabinetRepository companyCabinetRepository, IParentCompanyRepository parenCompanyRepository,
                              ICompanyUserRepository companyUserRepository, ILogger<CompanyService> logger, INotificationService notificationService,
                              IApplicationSettingRepository applicationSettingRepository)
                              : base(appMessageService, mapper)
        {
            _companyRepository = companyRepository;
            _cabinetRepository = cabinetRepository;
            _parenCompanyRepository = parenCompanyRepository;
            _notificationService = notificationService;
            _companyUserRepository = companyUserRepository;
            _logger = logger;
            _applicationSettingRepository = applicationSettingRepository;
            this.companyCabinetRepository = companyCabinetRepository;
        }

        async Task<ResponseValidityModel> ValidateCompany(CompanyModel companyModel)
        {
            var model = new ResponseValidityModel();
            bool isExisting = await _parenCompanyRepository.CheckParentCompanyId(companyModel.ParentCompanyId);

            if (!isExisting)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingField).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.ParentCompanyId);
                return model;
            }

            if (!companyModel.CompanyName.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.CompanyModel.CompanyName);
            }

            if (!companyModel.ContactNumber.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.CompanyModel.ContactNumber);
            }

            if (!companyModel.ContactPerson.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.CompanyModel.ContactPerson);
            }

            if (!companyModel.Address.HasText())
            {
                model.MessagesList.Add(GlobalMessageView.CompanyModel.Address);
            }

            if (model.MessagesList.Count > 0)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.FieldRequired)
                                              .MappedResponseValidityModel(model.MessagesList);
            }

            return model;
        }
        async Task<ResponseValidityModel> ValidateCompanyKeyId(string companyKeyId, bool isDeleted)
        {
            var model = new ResponseValidityModel();

            var ret = await _companyRepository.GetCompany(companyKeyId: companyKeyId, isDeleted: isDeleted);

            if (ret.Count == 0)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingField).MappedResponseValidityModel();
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.CompanyKeyId);
            }

            return model;
        }
        async Task<ResponseValidityModel> ValidateCompanyLocation(List<int> cabinetLocationIds)
        {
            var model = new ResponseValidityModel();

            foreach (int x in cabinetLocationIds)
            {
                var ret = await _cabinetRepository.CheckIfExistCabinetLocationId(x);
                if (ret == null)
                    model.MessagesList.Add(x.ToString());
                ret = await _cabinetRepository.CheckCabinetLocationId(x);
                if (ret == null)
                    model.MessagesList.Add(x.ToString());

            }

            if (model.MessagesList.Count > 0)
            {
                model = this.AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.NotExistingMultipleField).MappedResponseValidityModel(model.MessagesList);
                model.Message = model.Message.Replace(GlobalConstants.MessageParameters.Field, GlobalConstants.MessageReplacement.CabinetLocationId);
            }

            return model;
        }

        CompanyUserEntity AutogenerateCompanyUserAdmin(int companyId, CompanyEntity company)
        {
            var id = _companyUserRepository.GetLastIdentity();
            var defaultPwd = _applicationSettingRepository.GetApplicationSetting().Result;
            _defaultPwd = defaultPwd.DefaultCompanyPassword;
            CompanyUserEntity companyUserEntity = new CompanyUserEntity();
            companyUserEntity.CompanyId = companyId;
            companyUserEntity.UserKeyId = Shared.SharedServices.GenerateCompanyUserKeyId(id + 1, company.CompanyName);
            companyUserEntity.Password = Shared.SharedServices.HashPassword(defaultPwd.DefaultCompanyPassword);
            companyUserEntity.FirstName = company.FirstName;
            companyUserEntity.LastName = company.Surname;
            companyUserEntity.Username = company.FirstName + company.Surname + companyId;
            companyUserEntity.IsAdmin = true;
            companyUserEntity.IsActivated = false;
            return companyUserEntity;
        }

        void MappedCompanyValidityModel(ResponseValidityCompanyModel model, ResponseValidityModel responseValidityModel)
        {
            model.Message = responseValidityModel.Message;
            model.MessageReturnNumber = responseValidityModel.MessageReturnNumber;
            model.MessagesList = responseValidityModel.MessagesList;
            model.ReturnStatusType = responseValidityModel.ReturnStatusType;
        }

        public async Task<List<CompanyViewModel>> GetCompanyList(string companyKeyId = null)
        {
            var dbModel = await _companyRepository.GetCompany(companyKeyId);
            var model = new List<CompanyViewModel>();

            if (dbModel.Count > 0)
            {
                var groupBy = dbModel.GroupBy(x => x.CompanyId).Select(s => new { key = s.Key, lst = s.ToList() }).ToList();
                foreach (var item in groupBy)
                {
                    var companyDetail = item.lst.First();
                    model.Add(new CompanyViewModel
                    {
                        CompanyId = companyDetail.CompanyId,
                        CompanyName = companyDetail.CompanyName,
                        CompanyKeyId = companyDetail.CompanyKeyId,
                        ContactNumber = companyDetail.ContactNumber,
                        CompanyLogo = companyDetail.CompanyLogo,
                        IsDeleted = companyDetail.IsDeleted,
                        ContactPerson = companyDetail.ContactPerson,
                        Address = companyDetail.Address,
                        ParentCompanyId = companyDetail.ParentCompanyId,
                        Photo = companyDetail.Photo,
                        Surname = companyDetail.Surname,
                        FirstName = companyDetail.FirstName,
                        MiddleName = companyDetail.MiddleName,
                        Age = companyDetail.Age,
                        ResidentialAddress = companyDetail.ResidentialAddress,
                        Email = companyDetail.Email,
                        MobileNumber = companyDetail.MobileNumber,
                        FaxNumber = companyDetail.FaxNumber,
                        MaritalStatus = companyDetail.MaritalStatus,
                        Citizenship = companyDetail.Citizenship,
                        TIN = companyDetail.TIN,
                        SSSNo = companyDetail.SSSNo,
                        DoB = companyDetail.DoB,
                        PlaceOfBirth = companyDetail.PlaceOfBirth,
                        BusinessName = companyDetail.BusinessName,
                        NatureOfBusinessId = companyDetail.NatureOfBusinessId,
                        BankName = companyDetail.BankName,
                        Branch = companyDetail.Branch,
                        AccountName = companyDetail.AccountName,
                        AccountNumber = companyDetail.AccountNumber,
                        AccountType = companyDetail.AccountType,
                        CompanyBranches = Mapper.Map<List<CompanyLocationModel>>(item.lst)
                    });
                }
            }
            return model;
        }

        public async Task<ResponseValidityCompanyModel> SaveCompany(CompanyModel companyModel)
        {
            var model = new ResponseValidityCompanyModel();
            var responseValidityModel = await ValidateCompany(companyModel);
            var isInsert = false;
            if (responseValidityModel.MessageReturnNumber == 0)
            {
                if (companyModel.CompanyKeyId.HasText())
                {
                    var dbModel = await _companyRepository.GetCompany(companyModel.CompanyKeyId);
                    var company = dbModel.FirstOrDefault();
                    if (company != null)
                        await _cabinetRepository.DeleteCompanyCabinets(company.CompanyId);
                }
                responseValidityModel = await ValidateCompanyLocation(companyModel.CabinetLocationIds);

                if (responseValidityModel.MessageReturnNumber == 0)
                {
                    var entity = Mapper.Map<CompanyEntity>(companyModel);

                    if (!companyModel.CompanyKeyId.HasText())
                    {
                        var id = await _companyRepository.GetLastIdentity() + 1;
                        entity.CompanyKeyId = Shared.SharedServices.GenerateCompanyKeyId(id, companyModel.CompanyName, false);
                        isInsert = true;
                        companyModel.CompanyKeyId = entity.CompanyKeyId;
                    }
                    else
                    {
                        responseValidityModel = await ValidateCompanyKeyId(companyModel.CompanyKeyId, false);
                        if (responseValidityModel.MessageReturnNumber < 0)
                        {
                            MappedCompanyValidityModel(model, responseValidityModel);
                            return model;
                        }
                    }

                    var ret = await _companyRepository.SaveCompany(entity, isInsert);
                    if (ret > 0)
                    {
                        var dbModel = await _companyRepository.GetCompany(companyModel.CompanyKeyId);
                        var companyId = dbModel.Select(o => o.CompanyId).FirstOrDefault();
                        if (entity.CabinetLocationIds.Count > 0)
                            ret = await _cabinetRepository.SetCompanyCabinetLocation(companyId, entity.CabinetLocationIds);
                        responseValidityModel = AppMessageService.SetMessage(ret).MappedResponseValidityModel();

                        if (isInsert)
                        {
                            var companyUserEntity = AutogenerateCompanyUserAdmin(companyId, entity);
                            ret = await _companyUserRepository.SaveCompanyUser(companyUserEntity, isInsert);
                            if (ret <= 0)
                            {
                                _logger.LogError(string.Format("Unable to save company user for company {0}", companyModel.CompanyName));
                                responseValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.CompanyUserNotCreated).MappedResponseValidityModel();
                            }
                            else
                            {
                                model.UserkeyId = companyUserEntity.UserKeyId;
                                model.Password = "Comp@ny@1234";
                                model.UserName = companyUserEntity.Username;
                                var CompanyLoginCredentialsMsg = AppMessageService.SetMessage(ApplicationMessageNumber.InformationMessage.CompanyLoginCredentials).Message;
                                CompanyLoginCredentialsMsg = CompanyLoginCredentialsMsg.Replace("{UserName}", model.UserName);
                                CompanyLoginCredentialsMsg = CompanyLoginCredentialsMsg.Replace("{Password}", model.Password);
                                CompanyLoginCredentialsMsg = CompanyLoginCredentialsMsg.Replace("{Recipient}", companyUserEntity.FirstName);
                                var msgList = new List<string>();
                                msgList.Add(CompanyLoginCredentialsMsg);
                                responseValidityModel.MessagesList = msgList;
                                await _notificationService.SendSmtpEmailAsync(new EmailModel
                                {
                                    Message = CompanyLoginCredentialsMsg,
                                    Subject = "POBoxX Login Credentials",
                                    To = entity.Email,
                                    Type = MessageType.HtmlContent
                                }, companyId);

                            }
                        }
                    }
                    else
                    {
                        responseValidityModel = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.UnexpectedError).MappedResponseValidityModel();
                    }
                }
            }
            MappedCompanyValidityModel(model, responseValidityModel);
            return model;
        }
        public Task<ResponseValidityModel> SetCompanyActivation(int id, bool isDeleted)
        {
            throw new NotImplementedException();
        }
        public async Task<List<CompanyCabinetViewModel>> GetCompanyCabinets(int? companyId = null, int? cabinetId = null, bool? unAssignedOnly = null)
        {
            var dbModel = await companyCabinetRepository.Get(companyId, cabinetId, unAssignedOnly);
            var model = new List<CompanyCabinetViewModel>();

            if (dbModel != null)
                model = Mapper.Map<List<CompanyCabinetViewModel>>(dbModel);
            return model;
        }
        public async Task<ResponseValidityModel> SaveCompanyCabinet(CompanyCabinetModel param)
        {
            var model = ValidateCompanyCabinet(param);

            if (model.MessageReturnNumber == 0)
            {
                var entity = Mapper.Map<CompanyCabinetModel, CompanyCabinetEntity>(param);
                var ret = await companyCabinetRepository.Save(entity);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            return model;
        }
        ResponseValidityModel ValidateCompanyCabinet(CompanyCabinetModel param)
        {
            var model = new ResponseValidityModel();
            if (param.CompanyId < 1 || param.CabinetId < 1)
                model.MessageReturnNumber = 1;

            return model;
        }
        public async Task<ResponseValidityModel> AssignCompanyCabinetToLocation(AssignCompanyCabinetModel param)
        {
            var model = ValidateAssignCompanyCabinet(param);

            if (model.MessageReturnNumber == 0)
            {
                var ret = await companyCabinetRepository.AssignCompanyCabinets(param);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.InvalidInput).MappedResponseValidityModel();
            return model;
        }
        public async Task<ResponseValidityModel> UnAssignCompanyCabinetToLocation(AssignCompanyCabinetModel param)
        {
            var model = ValidateAssignCompanyCabinet(param);

            if (model.MessageReturnNumber == 0)
            {
                var ret = await companyCabinetRepository.UnassignCompanyCabinets(param);
                model = AppMessageService.SetMessage(ret).MappedResponseValidityModel();
            }
            else
                model = AppMessageService.SetMessage(ApplicationMessageNumber.ErrorMessage.InvalidInput).MappedResponseValidityModel();
            return model;
        }

        ResponseValidityModel ValidateAssignCompanyCabinet(AssignCompanyCabinetModel param)
        {
            var model = new ResponseValidityModel();
            if (param.CompanyId < 1 || param.CabinetId < 1)
                model.MessageReturnNumber = 1;

            return model;
        }
        public async Task<List<UnassignCompanyModel>> GetUnAssignedCompanies(int? companyId = null)
        {
            var dbModel = await companyCabinetRepository.Get(companyId);
            var model = new List<UnassignCompanyModel>();

            if (dbModel.Count > 0)
            {
                model = dbModel.GroupBy(s => s.CompanyId)
                    .Where(s => s.Any(v => !v.IsAssigned))
                    .Select(s => new UnassignCompanyModel
                    {
                        CompanyId = s.Key,
                        CompanyName = s.FirstOrDefault()?.CompanyName
                    }).ToList();
            }

            return model;
        }
        public async Task<List<UnassignCompanyModel>> GetCompanyCabinets(int? companyId = null)
        {
            var dbModel = await companyCabinetRepository.Get(companyId);
            var model = new List<UnassignCompanyModel>();

            if (dbModel.Count > 0)
            {
                model = dbModel.GroupBy(s => s.CompanyId)
                    .Where(s => s.Any(v => !v.IsAssigned))
                    .Select(s => new UnassignCompanyModel
                    {
                        CompanyId = s.Key,
                        CompanyName = s.FirstOrDefault()?.CompanyName
                    }).ToList();
            }

            return model;
        }
        public async Task<List<UnassignedCompanyCabinetModel>> GetUnAssignedCompanyCabinets(int? cabinetId = null)
        {
            var dbModel = await companyCabinetRepository.Get(cabinetId: cabinetId, unAssignedOnly: true);
            var model = new List<UnassignedCompanyCabinetModel>();

            if (dbModel.Count > 0)
            {
                model = dbModel.GroupBy(s => s.CabinetId)
                    .Select(s => new UnassignedCompanyCabinetModel
                    {
                        CabinetId = s.Key,
                        CabinetNumber = s.FirstOrDefault()?.CabinetNumber,
                        CabinetLocationDescription = s.FirstOrDefault()?.CabinetLocationDescription
                    }).ToList();
            }

            return model;
        }

    }
}

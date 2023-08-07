using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartBox.Business.Core.Models.AppMessage;
using SmartBox.Business.Core.Models.Base;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.Permission;
using SmartBox.Business.Core.Models.ResponseValidity;

namespace SmartBox.Business.Services.Service.AppMessage
{
    public interface IApplicationMessageService
    {
        Task<List<ApplicationMessageModel>> GetAll();
        Task<ApplicationMessageModel> GetApplicationMessageById(int Id); 
        Task<ResponseValidityModel> Delete(int Id);
        Task<ResponseValidityModel> Create(ApplicationMessageModel model);
        Task<ResponseValidityModel> Update(ApplicationMessageModel model);
        
    }
}

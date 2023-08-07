using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Feedback
{
    public interface IFeedbackService
    {
        Task<ResponseValidityModel> Save(FeedbackModel model);
        List<FeedbackViewModel> GetAll();
        Task<FeedbackViewModel> GetById(int Id);
        Task<ResponseValidityModel> Delete(int Id);

    }
}

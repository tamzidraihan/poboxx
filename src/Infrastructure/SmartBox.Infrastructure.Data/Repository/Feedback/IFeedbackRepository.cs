using SmartBox.Business.Core.Entities.Feedback;
using SmartBox.Business.Core.Models.Feedback;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Feedback
{
    public interface IFeedbackRepository:IGenericRepositoryBase<FeedbackEntity>
    {


        Task<int> Save(FeedbackEntity feedback);
        Task<FeedbackEntity> GetById(int Id);

        List<FeedbackViewModel> GetFeedbacks();

        Task<int> DeleteFeedback(int Id);

    }
}

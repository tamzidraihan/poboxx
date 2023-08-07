using SmartBox.Business.Core.Entities.Payment;
using SmartBox.Infrastructure.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Infrastructure.Data.Repository.Payment
{
    public interface IPaymentRepository: IGenericRepositoryBase<PaymentMethodEntity>
    {
        Task<List<PaymentMethodEntity>> GetPaymentMethod();
        Task<int> SavePaymentInfo(PaymentInfoEntity paymentInfoEntity);
        Task<PaymentInfoEntity> GetPaymentInfoModel(string referenceId);
        Task<int> SavePaymentTransaction(PaymentTransactionEntity paymentTransactionEntity);
        Task<PaymentTransactionEntity> GetPaymentTransaction(string referenceId);
    }
}

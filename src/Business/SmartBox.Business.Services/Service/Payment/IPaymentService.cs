using SmartBox.Business.Core.Models.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.Payment
{
    public interface IPaymentService
    {
        Task<List<PaymentMethodModel>> GetPaymentMethod();
        Task<int> SetPaymentInfo(PaymentInfoModel paymentInfoModel);
        Task<PaymentInfoModel> GetPaymentInfoModel(string referenceId);
        Task<int> SetPaymentTransaction(PaymentTransactionModel paymentTransactionModel, bool newRecord = false);
        Task<PaymentTransactionModel> GetPaymentTransaction(string referenceId);
        Task<int> SavePaymentTransactionInfo(PaymentTransactionModel paymentTransactionModel);
    }
}

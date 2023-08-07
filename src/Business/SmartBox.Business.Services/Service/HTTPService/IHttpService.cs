using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.HTTPService
{
    public interface IHttpService
    {
        Task<bool> SendSMS(string phoneNumber, string message, string refCode);
    }
}

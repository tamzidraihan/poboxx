using SmartBox.Business.Core.Models.User;
using SmartBox.Business.Services.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Services.Service.LogIn
{
    public interface ILogInService : IBaseMessageService<ILogInService>
    {
        Task<AuthenticationModel> GetUser(LogInModel logInModel);
        Task<AuthenticationModel> GetAdminUser(string username, string password);
        string CreateToken(UserModel user, bool isAdmin);
    }
}

using System;
using SmartBox.Business.Core.Models.Base;

namespace SmartBox.Business.Core.Models.User
{
    public class UserFormModel : BaseModel, IUserBaseFormModel
    {
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsLowerLockerPrefered { get; set; }
        public string Photo { get; set; }
        public DateTime DateCreated { get; set; }
    }
}

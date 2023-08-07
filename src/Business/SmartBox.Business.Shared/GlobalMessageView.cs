using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Shared
{
    public struct GlobalMessageView
    {
        public struct CompanyModel
        {
            public const string CompanyName = "Company Name";
            public const string Address = "Address";
            public const string ContactNumber = "Contact Number";
            public const string ContactPerson = "Contact Person";
        }
        public struct RoleModel
        {
            public const string RoleName = "Role Name";
     
        }
        public struct PermissionModel
        {
            public const string PermissionName = "Permission Name";

        }

        public struct UserFormModel
        {
            public const string FirstName = "Firstname";
            public const string LastName = "Lastname";
            public const string Email = "Email";
            public const string PhoneNumber = "Phone number";
            public const string Username = "Username";
            public const string UserKeyId = "UserKeyId";
            public const string Password = "Password";

        }

        public struct FeedbackQuestionModel
        {
            public const string Question = "Question";
          

        }
        public struct FeedbackAnswerModel
        {
            public const string Answer = "Answer";


        }

        public struct ApplicationMessage
        {
            public const string Message = "Application Message";
        }

    }
}

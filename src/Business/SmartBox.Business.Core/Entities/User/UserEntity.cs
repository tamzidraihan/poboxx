using System;
using Org.BouncyCastle.Bcpg.OpenPgp;
using SmartBox.Business.Core.Entities.Base;

namespace SmartBox.Business.Core.Entities.User
{
    public class UserEntity : BaseEntity
    {
        public int UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public string UserKeyId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string OTP { get; set; }
        public string Photo { get; set; }
        public bool IsLowerLockerPrefered { get; set; }
        public string MPIN  { get; set; }
        public bool IsActivated { get; set; }
        public DateTime OTPExpirationDate { get; set; }
    }

}

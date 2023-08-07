using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Company
{
    public class CompanyBaseModel
    {
        public int ParentCompanyId { get; set; }
        public string CompanyKeyId { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string ContactPerson { get; set; }
        public byte[] CompanyLogo { get; set; }
        public bool IsDeleted { get; set; } = false;

        //New fields

        public byte[] Photo { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public int Age { get; set; }
        public string ResidentialAddress { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string FaxNumber { get; set; }
        public string MaritalStatus { get; set; }
        public string Citizenship { get; set; }
        public string TIN { get; set; }
        public string SSSNo { get; set; }
        public DateTime DoB { get; set; }
        public string PlaceOfBirth { get; set; }
        public string BusinessName { get; set; }
        public string NatureOfBusinessId { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
    }
}

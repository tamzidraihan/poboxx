using Dapper.Contrib.Extensions;
using SmartBox.Business.Shared;
using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;


namespace SmartBox.Business.Core.Entities.ParentCompany
{
    public class ParentCompanyEntity
    {

        [Key]
        public int ParentCompanyId { get; set; }
        public string ParentCompanyKeyId { get; set; }
        public string ParentCompanyName { get; set; }
        public byte[] ParentCompanyLogo { get; set; }
        public string ParentCompanyAddress { get; set; }
        public string ParentCompanyContactPerson { get; set; }
        public string ParentCompanyContactNumber { get; set; }

    }
}

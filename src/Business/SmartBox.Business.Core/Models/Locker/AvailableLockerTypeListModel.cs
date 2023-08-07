using SmartBox.Business.Core.Models.ResponseValidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Locker
{
    public class AvailableLockerTypeListModel
    {
        public DateTime Date { get; set; }
        //public  int Id { get; set; }
        public List<AvailableLockerTypeModel> Collection {get;set;}
    }
}

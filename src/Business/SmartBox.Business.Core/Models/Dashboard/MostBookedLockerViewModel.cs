using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Models.Dashboard
{
    public class MostBookedLockerViewModel
    {
        public int LockerDetailId { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string Percentage { get; set; }
   
    }
}

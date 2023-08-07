﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Business.Core.Entities.Cabinet
{
    public class CabinetEntity
    {
        public int CabinetId { get; set; } 
        public int CabinetLocationId { get;set;}
        public string CabinetNumber { get;set; }
        public int NumberOfLocker { get; set; }
        public string CabinetLocationDescription { get; set; }
        public string Address { get; set; }
        public string RegionId { get; set; }
        public string ProvinceId { get; set; }
        public string CityId { get; set; }
        public string BarangayId { get; set; }
        public string ZipCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

    }
}

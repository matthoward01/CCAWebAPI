using System;
using System.Collections.Generic;

#nullable disable

namespace CCAWebAPI.Models.CCASS
{
    public partial class Warranty
    {
        public string MerchandisedProductId { get; set; }
        public string SampleId { get; set; }
        public string Provider { get; set; }
        public string Duration { get; set; }
        public string WarrantyPeriod { get; set; }
        public string ProductWarrantyTypeCode { get; set; }
    }
}

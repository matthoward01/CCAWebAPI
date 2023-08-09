using System;
using System.Collections.Generic;

#nullable disable

namespace CCAWebAPI.Models
{
    public partial class Label
    {
        public string MerchandisedProductId { get; set; }
        public string SampleId { get; set; }
        public string DivisionLabelType { get; set; }
        public string DivisionLabelName { get; set; }
    }
}

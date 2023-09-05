using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCAWebAPI.ControllerModels
{
    public class Changes
    {
        public string Sample_ID { get; set; }
        public string Program { get; set; }
        public string Change { get; set; }
    }

    public class Status
    {
        public string Status_Type { get; set; }
        public string Sample_ID { get; set; }
        public string Program { get; set; }
        public string New_Status { get; set; }
    }

    public class Update
    {
        public string Program { get; set; }
        public string XlsFileName { get; set; }
        public bool isCanada { get; set; }
    }    
}

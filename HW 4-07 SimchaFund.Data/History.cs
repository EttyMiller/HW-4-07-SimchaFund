using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW_4_07_SimchaFund.Data
{

    public class History
    {
        public string EventName { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string RecordType { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW_4_07_SimchaFund.Data
{
    public class Contribution
    {
        public int ContributorId { get; set; }
        public int SimchaId { get;set; }
        public decimal Amount { get; set; }
        public bool Include { get; set; }
    }
}

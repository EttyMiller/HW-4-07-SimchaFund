using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW_4_07_SimchaFund.Data
{
    public class SimchaInfo
    {
        public int Id { get; set; }
        public string SimchaName { get; set; }
        public DateTime Date { get; set; }
        public int NumOfContributors { get; set; }
        public decimal TotalContributed { get; set; }
    }
}

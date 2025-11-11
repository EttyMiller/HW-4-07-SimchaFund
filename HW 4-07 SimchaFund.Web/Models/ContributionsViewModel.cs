using HW_4_07_SimchaFund.Data;

namespace HW_4_07_SimchaFund.Web.Models
{
    public class ContributionsViewModel
    {
        public List<Contributor> Contributors { get; set; }
        public string SimchaName { get; set; }
        public int SimchaId { get; set; }
    }
}

using HW_4_07_SimchaFund.Data;
namespace HW_4_07_SimchaFund.Web.Models

{
    public class IndexViewModel
    {
        public List<SimchaInfo> Simchas { get; set; }
        public int TotalContibutors { get; set; }
        public string Message { get; set; }
    }
}

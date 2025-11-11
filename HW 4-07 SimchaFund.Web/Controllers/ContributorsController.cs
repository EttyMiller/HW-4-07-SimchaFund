using HW_4_07_SimchaFund.Data;
using HW_4_07_SimchaFund.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HW_4_07_SimchaFund.Web.Controllers
{
    public class ContributorsController : Controller

    //when done proof check site
    {
        private readonly string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=True;TrustServerCertificate=true;";

        public IActionResult Index()
        {
            var db = new SimchaDb(_connectionString);
            var vm = new ContributorsViewModel();
            vm.Contributors = db.GetAllContributors();
            vm.TotalBalance = db.GetTotalBalance();
            return View(vm);
        }

        [HttpPost]
        public IActionResult New(Contributor contributor, decimal initialDeposit)
        {
            var db = new SimchaDb(_connectionString);
            if (contributor.FirstName != null && contributor.LastName != null && contributor.CellNumber != null)
            {
                if(contributor.Date == default(DateTime))
                {
                    contributor.Date = DateTime.Now;
                }
                db.AddContibutor(contributor, initialDeposit);
            }
            return Redirect("/contributors");
        }

        [HttpPost]
        public IActionResult Edit(Contributor contributor)
        {
            var db = new SimchaDb(_connectionString);
            if (contributor.FirstName != null && contributor.LastName != null && contributor.CellNumber != null)
            {
                db.EditContibutor(contributor);
            }

            return Redirect("/contributors");
        }

        public IActionResult History(int contribid)
        {
            var db = new SimchaDb(_connectionString);
            var vm = new HistoryViewModel();
            vm.History = db.GetPersonsHistory(contribid);
            vm.Name = db.GetPersonNameById(contribid);
            vm.Balance = db.GetPersonsBalance(contribid);

            return View(vm);
        }

        [HttpPost]
        public IActionResult Deposit(int contributorId, decimal amount, DateTime date)
        {
            var db = new SimchaDb(_connectionString);
            if (contributorId != 0 && amount != 0)
            {
                if (date == default(DateTime))
                {
                    date = DateTime.Now;
                }
                db.AddDeposit(contributorId, amount, date);
            }

            return Redirect("/contributors");
        }
    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HW_4_07_SimchaFund.Web.Models;
using HW_4_07_SimchaFund.Data;

namespace HW_4_07_SimchaFund.Web.Controllers;

public class HomeController : Controller


    //history, editcontributor - checked,date, deposit, doesn;t submit unless form completed
{
    private readonly string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=True;TrustServerCertificate=true;";

    public IActionResult Index()
    {
        var db = new SimchaDb(_connectionString);
        var vm = new IndexViewModel();
        vm.Simchas = db.GetAllSimchas();
        vm.TotalContibutors = db.GetTotalContributors();

        if (TempData["message"] != null)
        {
            vm.Message = (string)TempData["message"];
        }

        return View(vm);
    }

    [HttpPost]
    public IActionResult New(SimchaInfo simcha)
    {
        var db = new SimchaDb(_connectionString);
        db.AddSimcha(simcha);
        return Redirect("/");
    }

    public IActionResult Contributions(int simchaId)
    {
        var db = new SimchaDb(_connectionString);
        var vm = new ContributionsViewModel();
        vm.Contributors = db.GetContributorsForSimcha(simchaId);
        vm.SimchaName = db.GetSimchaName(simchaId);
        vm.SimchaId = simchaId;

        return View(vm);
    }

    [HttpPost]
    public IActionResult UpdateContributions(List<Contribution> contributions)
    {
        var db = new SimchaDb(_connectionString);

        contributions.RemoveAll(c => !c.Include);

        if (contributions.Count != 0)
        {
            db.UpdateContributions(contributions);
            TempData["message"] = $"Simcha updated succesfully";
        }

        return Redirect($"/");
    }
}

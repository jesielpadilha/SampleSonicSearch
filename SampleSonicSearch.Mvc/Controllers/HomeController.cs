using Microsoft.AspNetCore.Mvc;
using SampleSonicSearch.Mvc.Models.ViewModels;
using System.Diagnostics;

namespace SampleSonicSearch.Mvc.Controllers
{
  public class HomeController : Controller
  {
    public HomeController()
    {
    }

    public IActionResult Index()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
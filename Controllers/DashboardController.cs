
using Microsoft.AspNetCore.Mvc;

namespace HackathonMvcSqlite.Controllers
{
 public class DashboardController:Controller
 {
  public IActionResult Index()
  {
   return View();
  }
 }
}

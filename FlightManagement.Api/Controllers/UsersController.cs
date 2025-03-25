using Microsoft.AspNetCore.Mvc;

namespace FlightManagement.Api.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

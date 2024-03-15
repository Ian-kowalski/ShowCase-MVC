using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowCase_MVC.Models;
using System.Diagnostics;

namespace ShowCase_MVC.Controllers
{
    [Authorize]
    public class ZeeslagController : Controller
    {
        public IActionResult Zeeslag()
        {
            return View();
        }
        public IActionResult Start()
        {
            return View();
        }
        public IActionResult LobbySelect()
        {
            return View();
        }
        public IActionResult Lobby()
        {
            return View();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSIT158Site.Models;
using System.Security.Policy;

namespace MSIT158Site.Controllers
{
    public class AddressController : Controller
    {
        private readonly MyDBContext _context;
        public AddressController(MyDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetCities()
        {
            var cities = _context.Addresses.Select(a => a.City).Distinct().ToList();
            return Json(cities);
        }

        [HttpGet]
        public JsonResult GetSites(string city)
        {
            var sites = _context.Addresses.Where(a => a.City == city).Select(a => a.SiteId).Distinct().ToList();
            return Json(sites);
        }

        [HttpGet]
        public JsonResult GetRoads(string city, string siteId)
        {
            var roads = _context.Addresses.Where(a => a.City == city && a.SiteId == siteId).Select(a => a.Road).Distinct().ToList();
            return Json(roads);
        }
    }
}

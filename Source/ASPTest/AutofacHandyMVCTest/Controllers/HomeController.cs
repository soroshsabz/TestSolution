using Autofac.Features.AttributeFilters;
using AutofacHandyMVCTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AutofacHandyMVCTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDummyModel _dummyModel;

        public HomeController(ILogger<HomeController> logger, [KeyFilter(nameof(DummyA))]IDummyModel dummyModel)
        {
            _logger = logger;
            _dummyModel = dummyModel;
        }

        public IActionResult Index()
        {
            return View(_dummyModel);
        }

        public IActionResult Privacy()
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
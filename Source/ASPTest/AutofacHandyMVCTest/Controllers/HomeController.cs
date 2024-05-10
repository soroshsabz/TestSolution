using Autofac.Features.AttributeFilters;
using AutofacHandyMVCTest.Models;
using BSN.Commons.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System.Diagnostics;

namespace AutofacHandyMVCTest.Controllers
{
    public static class RequestExtensions
    {
        public static async Task<string> ReadAsStringAsync(this Stream requestBody, bool leaveOpen = false)
        {
            requestBody.Position = 0;
            using StreamReader reader = new(requestBody, leaveOpen: leaveOpen);
            var bodyAsString = await reader.ReadToEndAsync();

            return bodyAsString;
        }
    }

    public class RequireRouteValuesAttribute : ActionMethodSelectorAttribute
    {
        public RequireRouteValuesAttribute(string[] valueNames)
        {
            ValueNames = valueNames;
        }

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            bool contains = false;
            foreach (string name in ValueNames)
            {
                string routeData = routeContext.RouteData.Values.ToString();
                HttpRequest httpRequest = routeContext.HttpContext.Request;
                httpRequest.EnableBuffering();
                string body = routeContext.HttpContext.Request.Body.ReadAsStringAsync(true).Result;
                string body2 = routeContext.HttpContext.Request.Body.ReadAsStringAsync(true).Result;
                contains = body.Contains(name);
                if (!contains) break;
            }

            return contains;
        }

        public string[] ValueNames { get; }
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDummyModel _dummyModel;

        public HomeController(ILogger<HomeController> logger, [KeyFilter(nameof(DummyA))] IDummyModel dummyModel)
        {
            _logger = logger;
            _dummyModel = dummyModel;
        }

        [HttpGet]
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

        [HttpPost]
        [RequireRouteValues(new[] {nameof(OverloadModelA.AName)})]
        public IActionResult OverloadInsert([FromBody]OverloadModelA input)
        {
            return View();
        }

        [HttpPost]
        //[Route($"Home/{nameof(OverloadInsert)}")]
        [RequireRouteValues(new[] {nameof(OverloadModelB.BName)})]
        public IActionResult OverloadInsert([FromBody]OverloadModelB input)
        {
            return View(viewName: "Index", _dummyModel);
        }
    }
}
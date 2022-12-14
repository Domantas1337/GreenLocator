using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GreenLocator;

// to get the data add this string to url: "/api/WebService/GetAll"

[Route("api/[controller]")]
[ApiController]
public class WebServiceController : ApiController
{
    public IEnumerable<ElectricityPrice> Get()
    {
        var result = GetAll();
        return (IEnumerable<ElectricityPrice>)View(result);
    }

    public IActionResult Index()
    {
        var result = GetAll();
        return View(result);
    }

    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<ElectricityPrice>> GetAll()
    {
        return new[]
        {
            new ElectricityPrice { Country = "Austria", Price = 400 },
            new ElectricityPrice { Country = "Belgium", Price = 403.65 },
            new ElectricityPrice { Country = "Bulgaria", Price = 396.35},
            new ElectricityPrice { Country = "Lithuania", Price = 384.40}
        };
    }
}

public class ElectricityPrice
{
    public string? Country { get; set; }
    public double Price { get; set; }
}

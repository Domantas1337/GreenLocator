using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

// to get the data add this string to url: "/api/WebService"

namespace GreenLocator;

[Route("api/[controller]")]
[ApiController]
public class WebServiceController : Controller
{
    [HttpGet]
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

using System;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GreenLocator;

// to get the data add this string to url: "/api/WebService/GetAll"

//[System.Web.Http.Route("api/[controller]")]
public class WebServiceController : ApiController
{
    readonly ElPrice[] prices = new ElPrice[]
    {
        new ElPrice { Country = "Belgium", Price = 403.65 },
        new ElPrice { Country = "Bulgaria", Price = 396.35},
        new ElPrice { Country = "Austria", Price = 400 },
        new ElPrice { Country = "Lithuania", Price = 384.40}
    };

    public IEnumerable<ElPrice> GetAllPrices()
    {
        return prices;
    }

    public IHttpActionResult GetPrice(String country)
    {
        var price = prices.FirstOrDefault((p) => p.Country == country);
        if (price == null)
        {
            return NotFound();
        }
        return Ok(price);
    }
}

public class ElPrice
{
    public string? Country { get; set; }
    public double Price { get; set; }
}

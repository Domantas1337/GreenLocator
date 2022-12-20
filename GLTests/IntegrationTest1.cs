using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GLTests
{
    public class IntegrationTest1
    : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public IntegrationTest1(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Index")]
        [InlineData("/Info")]
        [InlineData("/ChatIndex")]
        [InlineData("/Error")]
        [InlineData("/Prices")]
        [InlineData("/Identity/Account/Login")]
        [InlineData("/Identity/Account/Register")]
        [InlineData("/Identity/Account/Manage")]
        public async Task Get_EndpointsReturnSuccess(string url)
        {          
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            Assert.True(response.IsSuccessStatusCode);
            
        }

    }

}

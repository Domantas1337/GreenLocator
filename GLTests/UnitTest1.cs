using GreenLocator.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using AutoFixture;

namespace GLTests
{
    public class UnitTest1
    {

        [Fact]
        public void ConnectionToDBValid()
        {
            using var context = new GreenLocatorDBContext();
            Assert.True(context.Database.CanConnect());
        }



    }
}
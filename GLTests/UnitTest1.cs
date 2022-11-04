using GreenLocator.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using AutoFixture;
using AutoFixture.Xunit2;
using AutoFixture.AutoMoq;

namespace GLTests
{
    public class UnitTest1
    {

        [Fact]
        public void ConnectionToDBValid()
        {
            var context = new GreenLocatorDBContext();
            Assert.True(context.Database.CanConnect());
        }

        [Theory, AutoData]
        public void ConnectionToDB_valid(IFixture fixture)
        {
            fixture.Customize(new AutoMoqCustomization());
            var context = fixture.Create<GreenLocatorDBContext>();

            Assert.True(context.Database.CanConnect());
        }

    }
}
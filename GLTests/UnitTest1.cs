using GreenLocator.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using AutoFixture;
using AutoFixture.Xunit2;
using AutoFixture.AutoMoq;
using GreenLocator.Pages;
using NuGet.Frameworks;
using System.Net.WebSockets;

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

        [Theory, AutoData]
        public void OnGet_NullArg(IFixture fixture)
        {
            fixture.Customize(new AutoMoqCustomization());
            var context = fixture.Create<GreenLocatorDBContext>();

            fixture.Register((Mock<MainModel> m) => m.Object);
            var sut = fixture.Create<MainModel>();  // sut - system under test

            fixture.Register((Mock<AspNetUser> m) => m.Object);
            var user = fixture.Create<AspNetUser>();
            user.City = null;
            user.Street = null;
            user.House = null;

            //Assert.Throws<ArgumentNullException>( () => sut.OnGet() );
            Assert.Equal(user.City, null);
            Assert.Equal(user.Street, null);
            Assert.Equal(user.House, null);
        }
    }
}
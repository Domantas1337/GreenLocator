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
        public void checkCheckIfCurrentUserArgsNull(IFixture fixture)
        {
            fixture.Customize(new AutoMoqCustomization());

            fixture.Register((Mock<AspNetUser> m) => m.Object);
            var user = fixture.Create<AspNetUser>();

            fixture.Register((Mock<MainModel> m) => m.Object);
            var sut = fixture.Create<MainModel>();

            user.City = null;
            user.Street = null;
            user.House = null;

            Assert.True(sut.checkIfCurrentUserArgsNull(user));
        }

        [Theory, AutoData]
        [InlineAutoData("Vilnius", "Jeruzales", 4, 1, 2)]
        [InlineAutoData("Vilnius", "Gelezinio Vilko", 15, 0, 2)]
        [InlineAutoData("Vilnius", "Visoriu sodu 1-oji", 78, 1, 0)]
        [InlineAutoData("Vilnius", "Dariaus ir Gireno", 56, 2, 1)]
        [InlineAutoData("Vilnius", "Didlaukio", 25, 0, 0)]
        [InlineAutoData("Ukmerge", "Vytauto", 69, 1, 1)]
        [InlineAutoData("Ukmerge", "Kauno", 55, 2, 2)]
        [InlineAutoData("Naujoji Akmene", "J. Dalinkeviciaus", 37, 2, 0)]
        public void Extensions_Test(string City, string Street, int House, 
            int ShareStatus, int ThingToShare, IFixture fixture)
        {
            fixture.Register((Mock<AspNetUser> m) => m.Object);
            var user = fixture.Create<AspNetUser>();

            user.City = null;
            user.Street = null;
            user.House = null;
            user.ShareStatus = null;
            user.ThingToShare = null;

            Assert.False(Extensions.CheckIfUsrNull(user));
            Assert.True(Extensions.CheckIfUsrFieldsNull(user));
            Assert.True(Extensions.CheckIfUsrStatusNull(user));

            user.City = City;
            user.Street = Street;
            user.House = House;
            user.ShareStatus = ShareStatus;
            user.ThingToShare = ThingToShare;

            Assert.False(Extensions.CheckIfUsrFieldsNull(user));
            Assert.False(Extensions.CheckIfUsrStatusNull(user));

            user.House = null;
            user.ThingToShare = null;

            Assert.True(Extensions.CheckIfUsrFieldsNull(user));
            Assert.True(Extensions.CheckIfUsrStatusNull(user));
        }

        [Theory]
        [InlineAutoData("Vilnius", "Jeruzales", 4)]
        [InlineAutoData("Vilnius", "Gelezinio Vilko", 15)]
        [InlineAutoData("Vilnius", "Visoriu sodu 1-oji", 78)]
        [InlineAutoData("Vilnius", "Dariaus ir Gireno", 56)]
        [InlineAutoData("Vilnius", "Didlaukio", 25)]
        [InlineAutoData("Ukmerge", "Vytauto", 69)]
        [InlineAutoData("Ukmerge", "Kauno", 55)]
        [InlineAutoData("Naujoji Akmene", "J. Dalinkeviciaus", 37)]
        public void checkInputValidation(string City, string Street, int House)
        {
            var user = new AspNetUser
            {
                City = City,
                Street = Street,
                House = House,
            };

            var enterInfoModel = new EnterInfoModel(null);

            Assert.True(enterInfoModel.InputValidation(user.City, user.Street, (int)user.House));
        }        
    }
}
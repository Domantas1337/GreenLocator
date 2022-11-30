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
using System.IO;

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

        [Theory]
        [InlineData(null, null, null)]
        public void checkCheckIfCurrentUserArgsNull(string? City, string? Street, int? House)
        {
            var user = new AspNetUser
            {
                City = City,
                Street = Street,
                House = House,
            };

            var MainModel = new MainModel(null!);

            Assert.True(MainModel.checkIfCurrentUserArgsNull(user));
        }

        [Theory]
        [InlineData("Vilnius", "Jeruzales", 4, 1, 2)]
        [InlineData("Vilnius", "Gelezinio Vilko", 15, 0, 2)]
        [InlineData("Vilnius", "Visoriu sodu 1-oji", 78, 1, 0)]
        [InlineData("Vilnius", "Dariaus ir Gireno", 56, 2, 1)]
        [InlineData("Vilnius", "Didlaukio", 25, 0, 0)]
        [InlineData("Ukmerge", "Vytauto", 69, 1, 1)]
        [InlineData("Ukmerge", "Kauno", 55, 2, 2)]
        [InlineData("Naujoji Akmene", "J. Dalinkeviciaus", 37, 2, 0)]
        public void Extensions_TestWhenAllValuesNotNull(string City, string Street, int House, 
            int ShareStatus, int ThingToShare)
        {
            var user = new AspNetUser
            {
                City = City,
                Street = Street,
                House = House,
                ShareStatus = ShareStatus,
                ThingToShare = ThingToShare,
            };

            user.City = City;
            user.Street = Street;
            user.House = House;
            user.ShareStatus = ShareStatus;
            user.ThingToShare = ThingToShare;

            Assert.False(Extensions.CheckIfUsrFieldsNull(user));
            Assert.False(Extensions.CheckIfUsrStatusNull(user));
            Assert.False(Extensions.CheckIfUsrNull(user));
        }

        [Theory]
        [InlineData(null, null, null, null, null)]
        [InlineData("Vilnius", null, null, null, null)]
        [InlineData("Vilnius", "Visoriu sodu 1-oji", null, null, 0)]
        [InlineData(null, "Dariaus ir Gireno", null, null, null)]
        [InlineData(null, "Didlaukio", 25, 0, null)]
        [InlineData("Ukmerge", "Vytauto", null, 0, null)]
        [InlineData("Ukmerge", null, 55, 2, null)]
        [InlineData("Naujoji Akmene", null, 37, null, null)]
        public void Extensions_TestWhenSomeValuesAreNull(string? City, string? Street, int? House,
           int? ShareStatus, int? ThingToShare)
        {
            var user = new AspNetUser
            {
                City = City,
                Street = Street,
                House = House,
                ShareStatus = ShareStatus,
                ThingToShare = ThingToShare,
            };

             Assert.False(Extensions.CheckIfUsrNull(user));
             Assert.True(Extensions.CheckIfUsrFieldsNull(user));
             Assert.True(Extensions.CheckIfUsrStatusNull(user));
        }

        [Theory]
        [InlineData("Vilnius", "Jeruzales", 4)]
        [InlineData("Vilnius", "Gelezinio Vilko", 15)]
        [InlineData("Vilnius", "Visoriu sodu 1-oji", 78)]
        [InlineData("Vilnius", "Dariaus ir Gireno", 56)]
        [InlineData("Vilnius", "Didlaukio", 25)]
        [InlineData("Ukmerge", "Vytauto", 69)]
        [InlineData("Ukmerge", "Kauno", 55)]
        [InlineData("Naujoji Akmene", "J. Dalinkeviciaus", 37)]
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

        [Theory]
        [InlineData("Vilnius", "didlaukio", 47, 2, 1)]
        [InlineData("Vilnius", "didlaukio", 47, 1, 1)]
        [InlineData("Vilnius", "didlaukio", 47, 1, 0)]
        [InlineData("Vilnius", "didlaukio", 47, 0, 1)]
        [InlineData("Vilnius", "didlaukio", 47, 0, 0)]
        [InlineData("Vilnius", "didlaukio", 48, 2, 2)]
        [InlineData("Vilnius", "didlaukijo", 47, 2, 2)]
        public void CheckNumOfMatchedPeople0(string? City, string? Street, int? House, int? shareStatus, int? thingToShare)
        {
            GreenLocatorDBContext context = new GreenLocatorDBContext();

            var user = new AspNetUser
            {
                City = City,
                Street = Street,
                House = House,
                ShareStatus = shareStatus,
                ThingToShare = thingToShare
            };

            object args = new object[2] { context, user };

            MainModel.NumOfMatchedPeople(args);

            Assert.Equal(0, MainModel.currentNumberOfMatches);
        }

        [Theory]
        [InlineData("Vilnius", "didlaukio", 47, 2, 2)]
        public void CheckNumOfMatchedPeople1(string? City, string? Street, int? House, int? shareStatus, int? thingToShare)
        {
            GreenLocatorDBContext context = new GreenLocatorDBContext();

            var user = new AspNetUser
            {
                City = City,
                Street = Street,
                House = House,
                ShareStatus = shareStatus,
                ThingToShare = thingToShare
            };

            object args = new object[2] { context, user };

            MainModel.NumOfMatchedPeople(args);

            Assert.Equal(1, MainModel.currentNumberOfMatches);
        }
    }
}
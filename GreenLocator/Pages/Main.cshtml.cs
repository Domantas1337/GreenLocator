using System.Runtime.CompilerServices;

using GreenLocator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GreenLocator.Pages;

public class MainModel : PageModel
{
    private readonly GreenLocatorDBContext _context;
    public MainModel(GreenLocatorDBContext context)
    {
        _context = context;
    }

    public UserInfo currentUser = new();
    public static int currentNumberOfMatches { get; set; } = 0;

    public string? ActionInput;
    public string? ApplianceInput;

    public IActionResult OnGet()
    {
        try
        {
            if (User.Identity == null)
            {
                return RedirectToPage("Error");
            }

            AspNetUser current = _context.AspNetUsers.First(x => x.UserName == User.Identity.Name);

            //Task numOfMatchedPeopleTask = NumOfMatchedPeople(_context, current);

            if (checkIfCurrentUserArgsNull(current) == true)
            {
                throw new ArgumentNullException();
            }

            currentUser.City = current.City!;
            currentUser.Street = current.Street!;
            currentUser.house = current.House ?? 0;

            if (Delegates.CheckUserInfo(Extensions.CheckIfUsrStatusNull, current))
            {
                current.ShareStatus = 0;
                current.ThingToShare = 0;

                _context.SaveChanges();
            }
            else
            {
                currentUser.ShareStatus = (Status)current.ShareStatus!;
                currentUser.ThingToShare = (Appliance)current.ThingToShare!;

            }

            //await numOfMatchedPeopleTask;

            /*ParameterizedThreadStart notifThreadStart = new(NumOfMatchedPeople!);
            Thread notifThread = new Thread(notifThreadStart);*/

            //object args = new object[2] { _context, current };
            //notifThread.Start(args);

            //NumOfMatchedPeople(_context, current);

            return Page();

        }
        catch (ArgumentNullException)
        {
            return RedirectToPage("EnterInfo");
        }
    }

    public async Task<IActionResult> OnPost()
    {
        try
        {
            if (User.Identity == null)
            {
                return RedirectToPage("Error");
            }

            AspNetUser current = _context.AspNetUsers.First(x => x.UserName == User.Identity.Name);

            var task = Task.Run(() => NumOfMatchedPeople(_context, current));

            ActionInput = Request.Form["ActionInput"];
            ApplianceInput = Request.Form["ApplianceInput"];

            SetCurrentUser(ActionInput, ApplianceInput);

            await task;

            current.ShareStatus = (int)currentUser.ShareStatus;
                current.ThingToShare = (int)currentUser.ThingToShare;

            

            _context.SaveChanges();

            return Page();
        }
        catch (InvalidOperationException ex)
        {
            ErrorLogging(ex);

            return RedirectToPage("EnterInfo");
        }
        catch (System.Data.SqlTypes.SqlNullValueException ex)
        {
            ErrorLogging(ex);

            return RedirectToPage("EnterInfo");
        }
        catch (Exception ex)
        {
            ErrorLogging(ex);

            return RedirectToPage("Error");
        }
    }

    public bool checkIfCurrentUserArgsNull(AspNetUser current)
    {
        if (current.City == null || current.Street == null || current.House == null)
            return true;

        return false;
    }

    private void SetCurrentUser<T>(T action, T appliance)
    {
        switch (action)
        {
            case "Borrow":
                currentUser.ShareStatus = (Status)1;
                break;

            case "Offer":
                currentUser.ShareStatus = (Status)2;
                break;

            default:
                break;
        }

        switch (appliance)
        {
            case "Washing machine":
                currentUser.ThingToShare = (Appliance)1;
                break;

            case "Oven":
                currentUser.ThingToShare = (Appliance)2;
                break;

            default:
                break;
        }
    }

    private static void ErrorLogging(Exception ex)
    {
        string filePath = @"C:\Error.txt";

        using (StreamWriter writer = new(filePath, true))
        {
            writer.WriteLine("-----------------------------------------------------------------------------");
            writer.WriteLine("Date : " + DateTimeOffset.UtcNow.ToString());
            writer.WriteLine();

            while (ex != null)
            {
                writer.WriteLine(ex.GetType().FullName);
                writer.WriteLine("Message : " + ex.Message);
                writer.WriteLine("StackTrace : " + ex.StackTrace);
            }
        }
    }

    static void NumOfMatchedPeople(GreenLocatorDBContext context, AspNetUser current)
    {
        //Array argArray = new Object[2];
        //argArray = (Array)args;

        try
        {
            //GreenLocatorDBContext? context = (GreenLocatorDBContext)argArray.GetValue(0)!;
            //AspNetUser current = (AspNetUser)argArray.GetValue(1)!;
            //int currentNumberOfMatches = (int)argArray.GetValue(2)!;

            //using GreenLocatorDBContext context = new GreenLocatorDBContext();


            int temp = context.AspNetUsers.Count(usr => usr.City == current.City && usr.Street == current.Street
                                   && usr.House == current.House && usr.ThingToShare == current.ThingToShare
                                   && usr.ShareStatus != current.ShareStatus);

            if (temp > currentNumberOfMatches)
            {
                currentNumberOfMatches = temp;
            }
            else if (temp < currentNumberOfMatches)
            {
                currentNumberOfMatches = temp;
            }
        }
        catch (ArgumentException)
        {
        }
        catch (IndexOutOfRangeException)
        {
        }
        catch (System.ObjectDisposedException)
        {
        }
    }
}

public enum Status
{
    NoValue, Borrow, Offer
}

public enum Appliance
{
    NoValue, WashingMachine, Oven
}

public struct UserInfo : IEquatable<UserInfo>
{
    public string City { get; set; }
    public string Street { get; set; }
    public int house { get; set; }
    public Status ShareStatus { get; set; }
    public Appliance ThingToShare { get; set; }

    public UserInfo(string City="", string Street="", int house=0, Status ShareStatus=0, Appliance ThingToShare=0)
    {
        this.City = City;
        this.Street = Street;
        this.house = house;
        this.ShareStatus= ShareStatus;
        this.ThingToShare = ThingToShare;
    }

    public bool Equals(UserInfo userInfo)
    {
        return (this.City, this.Street, this.house, this.ShareStatus, this.ThingToShare) ==
            (userInfo.City, userInfo.Street, userInfo.house, userInfo.ShareStatus, userInfo.ThingToShare);
    }
}
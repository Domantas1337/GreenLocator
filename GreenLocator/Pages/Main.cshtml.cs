
using GreenLocator.Data;
using GreenLocator.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GreenLocator.Pages;

public class MainModel : PageModel
{
    private readonly ApplicationDbContext _context;
    public MainModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public UserInfo currentUser = new();
    public static int currentNumberOfMatches { get; set; } = 0;

    public string? ActionInput;
    public string? ApplianceInput;

    [BindProperty]
    public MainViewModel MainViewModel { get; set; } = null!;

    public IActionResult OnGet()
    {
        if (User.Identity == null)
        {
            return RedirectToPage("Error");
        }
        
        IdentityUser userFromAuthenticationMiddleware = _context.Users.ToList().First(x => x.UserName == User.Identity.Name);

        AspNetUser currentUser = new AspNetUser()
        {
            UserName = userFromAuthenticationMiddleware.UserName,
            Email = userFromAuthenticationMiddleware.Email,
            NormalizedUserName = userFromAuthenticationMiddleware.NormalizedUserName,
            EmailConfirmed  = userFromAuthenticationMiddleware.EmailConfirmed,
            SecurityStamp = userFromAuthenticationMiddleware.SecurityStamp,
            PasswordHash = userFromAuthenticationMiddleware.PasswordHash,
            ConcurrencyStamp = userFromAuthenticationMiddleware.ConcurrencyStamp,
            PhoneNumber = userFromAuthenticationMiddleware.PhoneNumber,
            PhoneNumberConfirmed = userFromAuthenticationMiddleware.PhoneNumberConfirmed,
            TwoFactorEnabled = userFromAuthenticationMiddleware.TwoFactorEnabled,
            LockoutEnd = userFromAuthenticationMiddleware.LockoutEnd,
            LockoutEnabled = userFromAuthenticationMiddleware.LockoutEnabled,
            AccessFailedCount = userFromAuthenticationMiddleware.AccessFailedCount,
       
        };

        return InitializeStatus(currentUser);
    }

    public IActionResult InitializeStatus(AspNetUser current)
    {
        try
        {
            if (checkIfCurrentUserArgsNull(current) == true)
            {
                throw new ArgumentNullException();
            }

            currentUser.City = current.City!;
            currentUser.Street = current.Street!;
            currentUser.house = current.House??0;

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

            return Page();

        }
        catch (ArgumentNullException)
        {
            return RedirectToPage("EnterInfo");
        }
    }

    public IActionResult OnPost()
    {
        if (User.Identity == null)
        {
            return RedirectToPage("Error");
        }

        IdentityUser userFromAuthenticationMiddleware = _context.Users.ToList().First(x => x.UserName == User.Identity.Name);

        AspNetUser currentUser = new AspNetUser()
        {
            UserName = userFromAuthenticationMiddleware.UserName,
            Email = userFromAuthenticationMiddleware.Email,
            NormalizedUserName = userFromAuthenticationMiddleware.NormalizedUserName,
            EmailConfirmed = userFromAuthenticationMiddleware.EmailConfirmed,
            SecurityStamp = userFromAuthenticationMiddleware.SecurityStamp,
            PasswordHash = userFromAuthenticationMiddleware.PasswordHash,
            ConcurrencyStamp = userFromAuthenticationMiddleware.ConcurrencyStamp,
            PhoneNumber = userFromAuthenticationMiddleware.PhoneNumber,
            PhoneNumberConfirmed = userFromAuthenticationMiddleware.PhoneNumberConfirmed,
            TwoFactorEnabled = userFromAuthenticationMiddleware.TwoFactorEnabled,
            LockoutEnd = userFromAuthenticationMiddleware.LockoutEnd,
            LockoutEnabled = userFromAuthenticationMiddleware.LockoutEnabled,
            AccessFailedCount = userFromAuthenticationMiddleware.AccessFailedCount,

        };

        return GetInputAndChangeStatus(currentUser);
    }

    public IActionResult GetInputAndChangeStatus(AspNetUser current)
    {
        try
        {
            ActionInput = MainViewModel.ActionInput;
            ApplianceInput = MainViewModel.ApplianceInput;

            SetCurrentUser(ActionInput, ApplianceInput);
            _context.SaveChanges();

            ParameterizedThreadStart notifThreadStart = new(NumOfMatchedPeople!);
            Thread notifThread = new Thread(notifThreadStart);

            object args = new object[2] { _context, current };

            notifThread.Start(args);

            lock (current)
            {
                current.ShareStatus = (int)currentUser.ShareStatus;
                current.ThingToShare = (int)currentUser.ThingToShare;
            }

            notifThread.Join();

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

    public static void NumOfMatchedPeople(object args)
    {
            Array argArray = new Object[2];
            argArray = (Array)args;

            try
            {
                GreenLocatorDBContext? context = (GreenLocatorDBContext)argArray.GetValue(0)!;
                AspNetUser current = (AspNetUser)argArray.GetValue(1)!;

                int temp = context.AspNetUsers.Count(usr => usr.City == current.City && usr.Street == current.Street
                                   && usr.House == current.House && usr.ThingToShare == current.ThingToShare
                                   && usr.ShareStatus != current.ShareStatus && current.Id != usr.Id);
        
                if (temp > currentNumberOfMatches)
                {
                    currentNumberOfMatches = temp;
                }
                else if (temp < currentNumberOfMatches)
                {
                    currentNumberOfMatches = temp;
                }
            }

            catch (ArgumentException ex)
            {
                ErrorLogging(ex);
            }
            catch (IndexOutOfRangeException ex)
            {
                ErrorLogging(ex);
            }
            catch (ObjectDisposedException ex)
            {
                ErrorLogging(ex);
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
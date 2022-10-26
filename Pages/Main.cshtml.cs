using GreenLocator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GreenLocator.Pages;

public class MainModel : PageModel
{
    private readonly GreenLocatorDBContext _context;
    public MainModel(GreenLocatorDBContext context)
    {
        _context = context;
    }

    public UserInfo currentUser = new();

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

            currentUser.City = current.City ?? throw new ArgumentNullException();
            currentUser.Street = current.Street ?? throw new ArgumentNullException();
            currentUser.house = current.House ?? throw new ArgumentNullException();

            if (current.CheckIfUsrStatusNull())
            {
                current.ShareStatus = 0;
                current.ThingToShare = 0;

                _context.SaveChanges();
            }
            else
            {
                currentUser.ShareStatus = (Status)current.ShareStatus; // warnings after extension method implementation
                currentUser.ThingToShare = (Appliance)current.ThingToShare;

            }

            return Page();

        }
        catch (InvalidOperationException ex)
        {
            ErrorLogging(ex);

            return RedirectToPage("EnterInfo");
        }
        catch (ArgumentNullException)
        {
            return RedirectToPage("EnterInfo");
        }
        catch (Exception ex)
        {
            ErrorLogging(ex);

            return RedirectToPage("Error");
        }
    }

    public IActionResult OnPost()
    {
        try
        { 
            using (var context = new GreenLocatorDBContext())
            {
                if (User.Identity == null)
                {
                    return RedirectToPage("Error");
                }

                AspNetUser current = context.AspNetUsers.First(x => x.UserName == User.Identity.Name);

                ActionInput = Request.Form["ActionInput"];
                ApplianceInput = Request.Form["ApplianceInput"];

                SetCurrentUser(ActionInput, ApplianceInput);

                current.ShareStatus = (int)currentUser.ShareStatus;
                current.ThingToShare = (int)currentUser.ThingToShare;

                context.SaveChanges();
            }

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

    private void SetCurrentUser<T>(T action, T appliance)
    {
        switch (action)
        {
            case "Borrow":
                currentUser.ShareStatus = (Status) 1;
                break;

            case "Offer":
                currentUser.ShareStatus = (Status) 2;
                break;

            default:
                break;
        }

        switch (appliance)
        {
            case "Washing machine":
                currentUser.ThingToShare = (Appliance) 1;
                break;

            case "Oven":
                currentUser.ThingToShare = (Appliance) 2;
                break;

            default:
                break;
        }
    }

    private static void ErrorLogging(Exception ex)
    {
        string filePath = @"C:\Error.txt";

        using StreamWriter writer = new(filePath, true);
        writer.WriteLine("-----------------------------------------------------------------------------");
        writer.WriteLine("Date : " + DateTime.Now.ToString());
        writer.WriteLine();

        while (ex != null)
        {
            writer.WriteLine(ex.GetType().FullName);
            writer.WriteLine("Message : " + ex.Message);
            writer.WriteLine("StackTrace : " + ex.StackTrace);
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
using GreenLocator.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GreenLocator.Pages;

public class MainModel : PageModel
{
    public UserInfo currentUser = new();

    public string? ActionInput;
    public string? ApplianceInput;

    public IActionResult OnGet()
    {

        using (var context = new GreenLocatorDBContext())
        {
            try
            {
                if(User.Identity == null)
                {
                    return RedirectToPage("Error");
                }

                var userList = from usr in context.AspNetUsers
                            select usr;

                AspNetUser current = userList.First(x => x.UserName == User.Identity.Name);

                if (current.CheckIfUsrFieldsNull())
                {
                    throw new ArgumentNullException();
                }

                if(current.CheckIfUsrStatusNull())
                {
                    current.ShareStatus = 0;
                    current.ThingToShare = 0;

                    context.SaveChanges();
                }
                else
                {
                    currentUser.ShareStatus = (Status)current.ShareStatus;      // warnings appeared after implementing extension methods
                    currentUser.ThingToShare = (Appliance)current.ThingToShare;
                }

                    return Page();

            }
            catch (InvalidOperationException)
            {
                return RedirectToPage("EnterInfo");
            }
            catch (ArgumentNullException)
            {
                return RedirectToPage("EnterInfo");
            }
            catch (Exception)
            {
                return RedirectToPage("Error");
            }
            
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

                var userList = from usr in context.AspNetUsers
                               select usr;

                AspNetUser current = userList.First(x => x.UserName == User.Identity.Name);

                ActionInput = Request.Form["ActionInput"];
                ApplianceInput = Request.Form["ApplianceInput"];

                setCurrentUser(ActionInput, ApplianceInput);

                current.ShareStatus = (int)currentUser.ShareStatus;
                current.ThingToShare = (int)currentUser.ThingToShare;

                context.SaveChanges();
            }

            return Page();
        }
        catch (InvalidOperationException)
        {
            return RedirectToPage("EnterInfo");
        }
        catch (System.Data.SqlTypes.SqlNullValueException)
        {
            return RedirectToPage("EnterInfo");
        }
        catch (Exception)
        {
            return RedirectToPage("Error");
        }
    }

    private void setCurrentUser(string action, string appliance)
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
}

public enum Status
{
    NoValue, Borrow, Offer
}

public enum Appliance
{
    NoValue, WashingMachine, Oven
}

public class UserInfo{
    public string City = "";
    public string Street = "";
    public int house;
    public Status ShareStatus;
    public Appliance ThingToShare;
}
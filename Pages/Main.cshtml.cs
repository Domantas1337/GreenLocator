using GreenLocator.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Linq;


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
                AspNetUser? current = null;

                var curr1 = from usr in context.AspNetUsers
                           where usr.UserName == User.Identity.Name
                           select usr;
                curr1.Select(x => x.UserName);

                
                foreach (var stud in context.AspNetUsers)
                {
                    if (stud.UserName == User.Identity.Name)
                    {
                        current = stud;
                        break;
                    }
                }

                if(current != null)
                {
                    currentUser.City = current.City;
                    currentUser.Street = current.Street;
                    currentUser.house = current.House;

                    if (current.ShareStatus == null || current.ThingToShare == null)
                    {
                        current.ShareStatus = 0;
                        current.ThingToShare = 0;

                        context.SaveChanges();
                    }
                    else
                    {
                        currentUser.ShareStatus = (Status)current.ShareStatus;
                        currentUser.ThingToShare = (Appliance)current.ThingToShare;
                    }

                    return Page();
                }
                else
                {
                    return RedirectToPage("EnterInfo");
                }
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
                AspNetUser? current = null;
                foreach (var stud in context.AspNetUsers)
                {
                    if (stud.UserName == User.Identity.Name)
                    {
                        current = stud; // panaudot linq
                        break;
                    }
                }
                if(current != null)
                {
                    ActionInput = Request.Form["ActionInput"];
                    ApplianceInput = Request.Form["ApplianceInput"];

                    setCurrentUser(ActionInput, ApplianceInput);

                    current.ShareStatus = (int)currentUser.ShareStatus;
                    current.ThingToShare = (int)currentUser.ThingToShare;

                    context.SaveChanges();
                }
                else
                {
                    return RedirectToPage("Error");
                }

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
    public string? City;
    public string? Street;
    public int? house;
    public Status ShareStatus;
    public Appliance ThingToShare;
}
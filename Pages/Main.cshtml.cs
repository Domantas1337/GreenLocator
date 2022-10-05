using GreenLocator.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace GreenLocator.Pages;

public class MainModel : PageModel
{
    public UserInfo currentUser = new UserInfo();

    public readonly string[] StatusArr = { "Borrow", "Share" };

    public readonly string[] ThingArr = { "Washing machine", "Oven" };

    public IActionResult OnGet()
    {
        using (var context = new GreenLocatorDBContext())
        {
            try
            {
                if(User.Identity.Name == null)
                {
                    return RedirectToPage("Error");
                }
                AspNetUser? current = null;
                foreach(var stud in context.AspNetUsers)
                {
                    if (stud.UserName == User.Identity.Name)
                    {
                        current = stud;
                        break;
                    }
                }

                if(current == null)
                {
                    return RedirectToPage("EnterInfo");
                }
                else
                {
                    currentUser.City = current.City;
                    currentUser.Street = current.Street;
                    currentUser.house = (int)current.House;

                    currentUser.ShareStatus = (int)current.ShareStatus;
                    currentUser.ThingToShare = (int)current.ThingToShare;

                    return Page();
                }
            }
            catch (System.InvalidOperationException ex)
            {
                return RedirectToPage("EnterInfo");
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex)
            {
                return RedirectToPage("EnterInfo");
            }
            catch (Exception ex)
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
                if (User.Identity.Name == null)
                {
                    return RedirectToPage("Error");
                }
                AspNetUser? current = null;
                foreach (var stud in context.AspNetUsers)
                {
                    if (stud.UserName == User.Identity.Name)
                    {
                        current = stud;
                        break;
                    }
                }

                ActionInput = Request.Form["ActionInput"];
                ApplianceInput = Request.Form["ApplianceInput"];

                setCurrentUser(ActionInput, ApplianceInput);

                current.ShareStatus = currentUser.ShareStatus;
                current.ThingToShare = currentUser.ThingToShare;

                context.SaveChanges();
            }

            return Page();
        }
        catch (System.InvalidOperationException ex)
        {
            return RedirectToPage("EnterInfo");
        }
        catch (System.Data.SqlTypes.SqlNullValueException ex)
        {
            return RedirectToPage("EnterInfo");
        }
        catch (Exception ex)
        {
            return RedirectToPage("Error");
        }
    }

    private void setCurrentUser(string action, string appliance)
    {

        switch (action)
        {
            case "Borrow":
                currentUser.ShareStatus = 0;
                break;
            case "Share":
                currentUser.ShareStatus = 1;
                break;
            default:
                break;
        }

        switch (appliance)
        {
            case "Washing machine":
                currentUser.ThingToShare = 0;
                break;
            case "Oven":
                currentUser.ThingToShare = 1;
                break;
            default:
                break;
        }
    }
}

public class UserInfo{
    public string? City;
    public string? Street;
    public int? house;
    public int? ShareStatus;    // 0 - wants to receive ; 1 - offering
    public int? ThingToShare;   // 0 - washing machine ; 1 - oven
}

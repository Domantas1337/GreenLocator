using GreenLocator.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace GreenLocator.Pages;

public class MainModel : PageModel
{
    public UserInfo currentUser = new UserInfo();
    public string ActionInput;
    public string ApplianceInput;

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
            ActionInput = Request.Form["ActionInput"];
            ApplianceInput = Request.Form["ApplianceInput"];
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
}

public class UserInfo{
    public string? City;
    public string? Street;
    public int? house;
    public int? ShareStatus;    // 0 - not sharing ; 1 - offering ; 2 - wants to receive
    public int? ThingToShare;   // 0 - oven ; 1 - washing machine
}

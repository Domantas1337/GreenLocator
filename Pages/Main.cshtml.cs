using GreenLocator.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace GreenLocator.Pages;

public class MainModel : PageModel
{
    public UserInfo currentUser = new UserInfo();
    public string ActionInput, ApplianceInput;

    public IActionResult OnGet()
    {
        //ActionInput = Request.Form["ActionInput"];
        //ApplianceInput = Request.Form["ApplianceInput"];


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

                    /*if(current.BorrowOrShare)
                    {
                        
                    }*/

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
}

public class UserInfo{
    public string? City;
    public string? Street;
    public int? house;
}

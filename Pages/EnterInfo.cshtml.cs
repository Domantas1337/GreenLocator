using GreenLocator.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace GreenLocator.Pages;

public class EnterInfoModel : PageModel
{
    public string CityInput, StreetInput;
    public int HouseInput;

    public IActionResult OnPost()
    {
        CityInput = Request.Form["CityInput"];
        StreetInput = Request.Form["StreetInput"];
        HouseInput = int.Parse(Request.Form["HouseInput"]);

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

                if (current == null)
                {
                    return RedirectToPage("EnterInfo");
                }
                else
                {
                    current.City = CityInput;
                    current.Street = StreetInput;
                    current.House = HouseInput;

                    context.SaveChanges();

                    return RedirectToPage("Main");
                }
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
        catch (System.FormatException ex)
        {
            return RedirectToPage("EnterInfo");
        }
        catch (Exception ex)
        {
            return RedirectToPage("Error");
        }

    }
    public void OnGet()
    {
    }

}

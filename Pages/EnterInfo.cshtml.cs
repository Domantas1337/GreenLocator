using System.ComponentModel.DataAnnotations;

using GreenLocator.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GreenLocator.Pages;

public class EnterInfoModel : PageModel
{
    [BindProperty]
    public EnterInfoViewModel EnterInfoViewModel { get; set; }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
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

                if (current == null)
                {
                    return RedirectToPage("EnterInfo");
                }
                else
                {
                    current.City = EnterInfoViewModel.CityInput;
                    current.Street = EnterInfoViewModel.StreetInput;
                    current.House = EnterInfoViewModel.HouseInput;

                    context.SaveChanges();

                    return RedirectToPage("Main");
                }
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
        catch (FormatException)
        {
            return RedirectToPage("EnterInfo");
        }
        catch (Exception)
        {
            return RedirectToPage("Error");
        }

    }
    public void OnGet()
    {
    }

}
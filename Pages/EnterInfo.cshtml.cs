using GreenLocator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace GreenLocator.Pages;

public class EnterInfoModel : PageModel
{
    [BindProperty]
    public EnterInfoViewModel EnterInfoViewModel { get; set; } = null!;

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
                    throw new FormatException();
                }

                AspNetUser current = context.AspNetUsers.First(x => x.UserName == User.Identity.Name);

                if(!InputValidation(city:EnterInfoViewModel.CityInput,
                                    street:EnterInfoViewModel.StreetInput,
                                    house:EnterInfoViewModel.HouseInput))
                {
                    throw new FormatException();
                }

                current.City = EnterInfoViewModel.CityInput ?? throw new ArgumentNullException();
                current.Street = EnterInfoViewModel.StreetInput ?? throw new ArgumentNullException();
                current.House = EnterInfoViewModel.HouseInput;

                if (current.CheckIfUsrFieldsNull())
                {
                    throw new ArgumentNullException();
                }

                context.SaveChanges();

                return RedirectToPage("Main");
            }

        }

        catch (FormatException)
        {
            return RedirectToPage("EnterInfo");
        }
        catch (ArgumentNullException)
        {
            return RedirectToPage("EnterInfo");
        }
        catch (InvalidOperationException ex)
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

    private bool InputValidation(string city, string street, int house)
    {
        if (CheckString(city) && CheckString(street) && CheckHouse(house))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckString(string input)
    {
        string pattern = "^[a-zA-Z]{3,50}$";

        Regex rx = new Regex(pattern);

        return rx.IsMatch(input);
    }

    private bool CheckHouse(int input)
    {
        string pattern = "^[0-9]{1,4}$";

        Regex rx = new Regex(pattern);

        return rx.IsMatch(input.ToString());
    }

    private static void ErrorLogging(Exception ex)
    {
        string filePath = @"C:\Error.txt";

        using StreamWriter writer = new(filePath, true);
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

}
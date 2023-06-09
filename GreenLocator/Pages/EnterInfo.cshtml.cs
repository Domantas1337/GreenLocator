using GreenLocator.Data;
using GreenLocator.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace GreenLocator.Pages;

public class EnterInfoModel : PageModel
{
    protected readonly ApplicationDbContext _context;
    public EnterInfoModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public EnterInfoViewModel EnterInfoViewModel { get; set; } = null!;

    public IActionResult OnPost()
    {
        if (User.Identity == null)
        {
            throw new FormatException();
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


        return GetInputAndRedirect(currentUser);
    }

    public IActionResult GetInputAndRedirect(AspNetUser current)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        try
        {
            if (!InputValidation(city: EnterInfoViewModel.CityInput,
                                street: EnterInfoViewModel.StreetInput,
                                house: EnterInfoViewModel.HouseInput))
            {
                throw new FormatException();
            }

            current.City = EnterInfoViewModel.CityInput ?? throw new ArgumentNullException();
            current.Street = EnterInfoViewModel.StreetInput ?? throw new ArgumentNullException();
            current.House = EnterInfoViewModel.HouseInput;

            if (Delegates.CheckUserInfo(Extensions.CheckIfUsrFieldsNull, current))
            {
                throw new ArgumentNullException();
            }

            _context.SaveChanges();

            return RedirectToPage("Main");
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
    public bool InputValidation(string city, string street, int house)
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

    private static bool CheckString(string input)
    {
        string pattern = "^[a-zA-Z0-9 -.]{3,50}$";

        Regex rx = new Regex(pattern);

        return rx.IsMatch(input);
    }

    private static bool CheckHouse(int input)
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
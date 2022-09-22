using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace GreenLocator.Pages;

public class MainModel : PageModel
{
    public UserInfo currentUser = new UserInfo();

    public IActionResult OnGet()
    {
        try
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;" +
                "Initial Catalog=aspnet-GreenLocator-53bc9b9d-9d6a-45d4-8429-2a2761773502;Integrated Security=True;" +
                "Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;" +
                "MultiSubnetFailover=False";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT City, Street, house FROM AspNetUsers WHERE UserName = '" + User.Identity.Name + "'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            currentUser.City = reader.GetString(0);
                            currentUser.Street = reader.GetString(1);
                            currentUser.house = reader.GetInt32(2);
                        }
                    }
                }
            }
            return Page();
        }
        catch(System.InvalidOperationException ex)
        {
            return RedirectToPage("EnterInfo");
        }
        catch(System.Data.SqlTypes.SqlNullValueException ex)
        {
            return RedirectToPage("EnterInfo");
        }
        catch(Exception ex)
        {
            return RedirectToPage("Error");
        }
    }
}

public class UserInfo{
    public string City;
    public string Street;
    public int house;
}

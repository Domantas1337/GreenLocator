using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace GreenLocator.Pages;

public class EnterInfoModel : PageModel
{
    public UserInfo updateCurrentUser = new UserInfo();

    public string CityInput, StreetInput;
    public int HouseInput;

    public IActionResult OnPost()
    {
        CityInput = Request.Form["CityInput"];
        StreetInput = Request.Form["StreetInput"];
        HouseInput = int.Parse(Request.Form["HouseInput"]);

        try
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;" +
                "Initial Catalog=aspnet-GreenLocator-53bc9b9d-9d6a-45d4-8429-2a2761773502;Integrated Security=True;" +
                "Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;" +
                "MultiSubnetFailover=False";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE AspNetUsers SET City = @cit, Street = @strt, house = @hs WHERE UserName = '" + User.Identity.Name + "'";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    updateCurrentUser.City = CityInput;     // Is textfieldu gautus duomenis ivedat cia
                    updateCurrentUser.Street = StreetInput;
                    updateCurrentUser.house = HouseInput;

                    command.CommandText = sql;

                    command.Parameters.AddWithValue("@cit", updateCurrentUser.City);
                    command.Parameters.AddWithValue("@strt", updateCurrentUser.Street);
                    command.Parameters.AddWithValue("@hs", updateCurrentUser.house);


                    command.ExecuteNonQuery();
                }
            }
            return RedirectToPage("Main");
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
    public void OnGet()
    {
    }

}

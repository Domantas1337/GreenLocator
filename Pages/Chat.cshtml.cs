using GreenLocator.Models;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GreenLocator.Pages
{
    public class ChatModel : PageModel
    {

        public List<SelectListItem> Options { get; set; }
        public void OnGet()
        {
            using (var context = new GreenLocatorDBContext())
            {
                if (User.Identity == null)
                {
                    RedirectToPage("Error");
                }

                AspNetUser current = context.AspNetUsers.First(x => x.UserName == User.Identity.Name);

                Options = context.AspNetUsers.Where(usr => usr.City == current.City && usr.Street == current.Street
                                && usr.House == current.House && usr.ThingToShare == current.ThingToShare
                                && usr.ShareStatus != current.ShareStatus).
                           Select(x => new SelectListItem {
                               Value = x.UserName,
                               Text = x.UserName

                           }).ToList();

                

            }
        }
    }
}

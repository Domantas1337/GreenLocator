using GreenLocator.Models;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GreenLocator.Pages
{
    public class ChatModel : PageModel
    {
        private readonly GreenLocatorDBContext _context;
        public ChatModel(GreenLocatorDBContext context)
        {
            _context = context ?? throw new ArgumentNullException();
        }

    public List<SelectListItem>? Options { get; set; }

        public void OnGet()
        {
            if (User.Identity == null)
            {
                RedirectToPage("Error");
            }
            else
            {
                AspNetUser current = _context.AspNetUsers.First(x => x.UserName == User.Identity.Name);
                generateMatchList(current);
            }
        }

        public void generateMatchList(AspNetUser current)
        {
            Options = _context.AspNetUsers.Where(usr => usr.City == current.City && usr.Street == current.Street
                                && usr.House == current.House && usr.ThingToShare == current.ThingToShare
                                && usr.ShareStatus != current.ShareStatus && current.Id != usr.Id).
                           Select(x => new SelectListItem
                           {
                               Value = x.UserName,
                               Text = x.UserName
                           }).ToList();
        }
    }
}

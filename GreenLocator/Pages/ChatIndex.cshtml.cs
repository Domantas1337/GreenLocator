using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GreenLocator.Pages;

public class ChatIndexModel : PageModel
{
    private readonly ILogger<ChatIndexModel> _logger;

    public ChatIndexModel(ILogger<ChatIndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        if (HttpContext.User.Identity != null)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToPage("Chat");
            }
            return Page();
        }
        return RedirectToPage("Error");
    }
}


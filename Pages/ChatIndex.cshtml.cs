using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GreenLocator.Pages;

public class ChatModel : PageModel
{
    private readonly ILogger<ChatModel> _logger;

    public ChatModel(ILogger<ChatModel> logger)
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


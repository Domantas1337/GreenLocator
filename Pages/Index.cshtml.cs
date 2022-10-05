using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GreenLocator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        if (HttpContext.User.Identity != null)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToPage("Main");
            }
            return Page();
        }
        return RedirectToPage("Error");
    }
}
using System.ComponentModel.DataAnnotations;

namespace GreenLocator.Models;

public class MainViewModel
{
    [Required]
    public string? ActionInput { get; set; }

    [Required]
    public string? ApplianceInput { get; set; }

}
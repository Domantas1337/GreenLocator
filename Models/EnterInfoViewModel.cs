using System.ComponentModel.DataAnnotations;

namespace GreenLocator.Models;

public class EnterInfoViewModel
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string CityInput { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string StreetInput { get; set; }

    [Required]
    public int HouseInput { get; set; }

}
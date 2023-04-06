using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
namespace GreenLocator.Models
{
    [ExcludeFromCodeCoverage]
    public partial class AspNetUserClaim
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }

        public virtual AspNetUser User { get; set; } = null!;
    }
}

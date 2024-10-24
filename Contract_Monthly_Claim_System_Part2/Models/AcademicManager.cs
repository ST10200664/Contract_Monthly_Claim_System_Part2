using Microsoft.AspNetCore.Identity;

namespace Contract_Monthly_Claim_System_Part2.Models
{
    public class AcademicManager : IdentityUser
    {
        public int AcademicManagerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public ICollection<Claim> ApprovedClaims { get; set; }
    }
}

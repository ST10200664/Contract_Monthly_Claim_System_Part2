using Microsoft.AspNetCore.Identity;

namespace Contract_Monthly_Claim_System_Part2.Models
{
    public class ProgrammeCoordinator : IdentityUser
    {
        public int ProgrammeCoordinatorId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public ICollection<Claim> VerifiedClaims { get; set; }
    }
}

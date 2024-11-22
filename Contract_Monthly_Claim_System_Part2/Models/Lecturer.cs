using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Contract_Monthly_Claim_System_Part2.Models
{
    public class Lecturer : IdentityUser
    {
        public int LecturerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public ICollection<Claim> Claims { get; set; } = new List<Claim>();


    }
}

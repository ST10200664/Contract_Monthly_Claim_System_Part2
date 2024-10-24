using Microsoft.AspNetCore.Identity;

namespace Contract_Monthly_Claim_System_Part2.Models
{   public class Claim : IdentityUser
        {
            public int ClaimId { get; set; }
            public DateTime SubmissionDate { get; set; }
            public decimal HoursWorked { get; set; }
            public decimal HourlyRate { get; set; }
            public string Status { get; set; } // Submitted, Approved, Rejected
            public decimal  Amount { get; set; }
            public int LecturerId { get; set; }
            public Lecturer Lecturer { get; set; }
            public string Description { get; set; }

            public string SupportingDocument { get; set; } // Path to the uploaded document
        }
    
}

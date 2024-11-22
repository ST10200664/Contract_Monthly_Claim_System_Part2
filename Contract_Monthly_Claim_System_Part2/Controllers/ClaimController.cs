using Contract_Monthly_Claim_System_Part2.Data;
using Contract_Monthly_Claim_System_Part2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Contract_Monthly_Claim_System_Part2.Controllers
{
    public class ClaimController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ClaimsHub> _hubContext;

        public ClaimController(ApplicationDbContext context, IHubContext<ClaimsHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: Claim/Create
       
        public IActionResult Create()
        {
            return View();
        }

        // POST: Claim/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HoursWorked,HourlyRate,SupportingDocument")] Claim claim)
        {
            if (ModelState.IsValid)
            {
                claim.SubmissionDate = DateTime.Now;
                claim.Status = "Submitted";
                _context.Add(claim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(claim);
        }

        // GET: Claim/Index
       
        public async Task<IActionResult> Index()
        {
            return View(await _context.Claims.ToListAsync());
        }

        // Approval actions will follow
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HoursWorked,HourlyRate")] Claim claim, IFormFile supportingDocument)
        {
            if (supportingDocument != null)
            {
                var filePath = Path.Combine("Uploads", supportingDocument.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await supportingDocument.CopyToAsync(stream);
                }
                claim.SupportingDocument = filePath;
            }

            claim.SubmissionDate = DateTime.Now;
            claim.Status = "Submitted";
            _context.Add(claim);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult ApproveClaims()
        {
            var claims = _context.Claims.Where(c => c.Status == "Verified by Coordinator").ToList();
            return View(claims);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaimAsync(int claimId)
        {
            var claim = await _context.Claims.FirstOrDefaultAsync(c => c.ClaimId == claimId); // Use async version

            if (claim == null)
            {
                return NotFound(); // Return 404 if claim doesn't exist
            }

            claim.Status = "Approved";
            await _context.SaveChangesAsync();

            // Notify clients in real-time via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveClaimStatusUpdate", claim.ClaimId, claim.Status);

            return RedirectToAction("ApproveClaims");
        }

        [HttpPost]
        public IActionResult RejectClaim(int claimId)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimId == claimId);
            if (claim != null)
            {
                claim.Status = "Rejected";
                _context.SaveChanges();
            }

            return RedirectToAction("ApproveClaims");
        }

        public IActionResult GenerateReport()
        {
            var approvedClaims = _context.Claims.Where(c => c.Status == "Approved").ToList();
            return View(approvedClaims);
        }

        [Authorize(Policy = "AdminOnly")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        [Authorize(Policy = "LecturerOnly")]
        public IActionResult LecturerDashboard()
        {
            return View();
        }


    }


}

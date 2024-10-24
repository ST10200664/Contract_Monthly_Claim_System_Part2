using Contract_Monthly_Claim_System_Part2.Data;
using Contract_Monthly_Claim_System_Part2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Contract_Monthly_Claim_System_Part2.Controllers
{
    public class AcademicManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ClaimsHub> _hubContext;
        public AcademicManagerController(ApplicationDbContext context, IHubContext<ClaimsHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        //  AcademicManagers
        public async Task<IActionResult> Index()
        {
            return View(await _context.AcademicManagers.ToListAsync());
        }

        //  AcademicManager/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var manager = await _context.AcademicManagers
                .Include(am => am.ApprovedClaims)
                .FirstOrDefaultAsync(m => m.AcademicManagerId == id);

            if (manager == null) return NotFound();

            return View(manager);
        }

        // AcademicManager/Create
        public IActionResult Create()
        {
            return View();
        }

        // AcademicManager/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AcademicManagerId,Name,Email")] AcademicManager manager)
        {
            if (ModelState.IsValid)
            {
                _context.Add(manager);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(manager);
        }

        // AcademicManager/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var manager = await _context.AcademicManagers.FindAsync(id);
            if (manager == null) return NotFound();

            return View(manager);
        }

        // AcademicManager/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AcademicManagerId,Name,Email")] AcademicManager manager)
        {
            if (id != manager.AcademicManagerId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(manager);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.AcademicManagers.Any(e => e.AcademicManagerId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(manager);
        }

        // AcademicManager/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var manager = await _context.AcademicManagers
                .FirstOrDefaultAsync(m => m.AcademicManagerId == id);

            if (manager == null) return NotFound();

            return View(manager);
        }

        //  AcademicManager/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var manager = await _context.AcademicManagers.FindAsync(id);
            _context.AcademicManagers.Remove(manager);
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
    }
}

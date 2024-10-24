using Contract_Monthly_Claim_System_Part2.Data;
using Contract_Monthly_Claim_System_Part2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Contract_Monthly_Claim_System_Part2.Controllers
{
    public class ProgrammeCoordinatorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ClaimsHub> _hubContext;

        public ProgrammeCoordinatorController(ApplicationDbContext context, IHubContext<ClaimsHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: ProgrammeCoordinators
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProgrammeCoordinators.ToListAsync());
        }

        // GET: ProgrammeCoordinator/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var coordinator = await _context.ProgrammeCoordinators
                .Include(pc => pc.VerifiedClaims)
                .FirstOrDefaultAsync(m => m.ProgrammeCoordinatorId == id);

            if (coordinator == null) return NotFound();

            return View(coordinator);
        }

        // GET: ProgrammeCoordinator/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProgrammeCoordinator/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProgrammeCoordinatorId,Name,Email")] ProgrammeCoordinator coordinator)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coordinator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coordinator);
        }

        // GET: ProgrammeCoordinator/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var coordinator = await _context.ProgrammeCoordinators.FindAsync(id);
            if (coordinator == null) return NotFound();

            return View(coordinator);
        }

        // POST: ProgrammeCoordinator/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProgrammeCoordinatorId,Name,Email")] ProgrammeCoordinator coordinator)
        {
            if (id != coordinator.ProgrammeCoordinatorId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coordinator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ProgrammeCoordinators.Any(e => e.ProgrammeCoordinatorId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(coordinator);
        }

        // GET: ProgrammeCoordinator/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var coordinator = await _context.ProgrammeCoordinators
                .FirstOrDefaultAsync(m => m.ProgrammeCoordinatorId == id);

            if (coordinator == null) return NotFound();

            return View(coordinator);
        }

        // POST: ProgrammeCoordinator/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coordinator = await _context.ProgrammeCoordinators.FindAsync(id);
            _context.ProgrammeCoordinators.Remove(coordinator);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult VerifyClaims()
        {
            var claims = _context.Claims.Where(c => c.Status == "Pending").ToList();
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


            return RedirectToAction("VerifyClaims");
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

            return RedirectToAction("VerifyClaims");
        }


    }
}

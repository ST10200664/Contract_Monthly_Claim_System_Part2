using Contract_Monthly_Claim_System_Part2.Data;
using Contract_Monthly_Claim_System_Part2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Contract_Monthly_Claim_System_Part2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;  // Injecting DbContext to interact with the database

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Display the homepage
        public IActionResult Index()
        {
            return View();
        }

        // Display the Privacy policy page
        public IActionResult Privacy()
        {
            return View();
        }

        // Display a list of claims (for lecturers or managers to view)
        public async Task<IActionResult> ViewClaims()
        {
            var claims = await _context.Claims.Include(c => c.Lecturer).ToListAsync(); // Retrieve claims and related lecturer data
            return View(claims); // Pass claims to the view
        }

        // Display details for a specific claim
        public async Task<IActionResult> ClaimDetails(int? id, bool v)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.Claims
                .Include(c => c.Lecturer)  // Include related lecturer data
                .FirstOrDefaultAsync(m => v);

            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }

        // Create a new claim (for lecturers)
        public IActionResult CreateClaim()
        {

            // Retrieve a list of lecturers from the database
            ViewBag.Lecturers = new SelectList(_context.Lecturers, "Id", "FullName");
            return View();
        }

        // Handle the creation of a new claim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateClaim(Claim claim)
        {
            if (ModelState.IsValid)
            {
                _context.Add(claim);
                await _context.SaveChangesAsync();  // Save the claim to the database
                return RedirectToAction(nameof(ViewClaims));  // Redirect to view all claims
            }

            return View(claim);  // If invalid, redisplay the form with validation errors
        }

        // Handle errors
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DestinationPlanner.Models;

namespace DestinationPlanner.Controllers
{
    public class BookingsController : Controller
    {
        private readonly AppDBContext _context;

        public BookingsController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchkey)
        {
            var bookings = _context.Bookings
                .Include(b => b.Package)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchkey))
            {
                searchkey = searchkey.ToLower();
                bookings = bookings.Where(b =>
                    b.CustomerName.ToLower().Contains(searchkey) ||
                    b.Package.PackageName.ToLower().Contains(searchkey));
            }

            ViewData["searchkey"] = searchkey;
            return View(await bookings.ToListAsync());
        }

        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.Packages = new SelectList(await _context.Packages.ToListAsync(), "PackageId", "PackageName");

            if (id == 0)
                return View(new Booking());
            else
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null) return NotFound();
                return View(booking);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("BookingId,CustomerName,PackageId,TravelDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    _context.Add(booking);
                }
                else
                {
                    _context.Update(booking);
                }
                await _context.SaveChangesAsync();

                var bookings = await _context.Bookings.Include(b => b.Package).ToListAsync();
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", bookings) });
            }

            ViewBag.Packages = new SelectList(await _context.Packages.ToListAsync(), "PackageId", "PackageName");
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", booking) });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            var bookings = await _context.Bookings.Include(b => b.Package).ToListAsync();
            return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", bookings) });
        }

        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Package)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null) return NotFound();

            return PartialView("Details", booking);
        }

        // GET: Search Page
        public IActionResult Search()
        {
            return View();
        }

        // POST: Search Bookings by Travel Date
        [HttpPost]
        public async Task<IActionResult> Search(DateTime travelDate)
        {
            if (travelDate == DateTime.MinValue)
            {
                ViewBag.Message = "Please enter a valid travel date.";
                return View();
            }

            var bookings = await _context.Bookings
                .Include(b => b.Package)
                .Include(b => b.Package.Destination)
                .Where(b => b.TravelDate == travelDate)
                .ToListAsync();

            if (bookings.Count == 0)
            {
                ViewBag.Message = "No bookings found for the selected date.";
            }

            return View(bookings);
        }
    }
}

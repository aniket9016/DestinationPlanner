using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DestinationPlanner.Models;

namespace DestinationPlanner.Controllers
{
    public class PackagesController : Controller
    {
        private readonly AppDBContext _context;

        public PackagesController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchkey)
        {
            var packages = _context.Packages
                .Include(p => p.Destination)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchkey))
            {
                searchkey = searchkey.ToLower();
                packages = packages.Where(p =>
                    p.PackageName.ToLower().Contains(searchkey) ||
                    p.Destination.DestinationName.ToLower().Contains(searchkey));
            }

            ViewData["searchkey"] = searchkey;
            return View(await packages.ToListAsync());
        }

        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.Destinations = new SelectList(await _context.Destinations.ToListAsync(), "DestinationId", "DestinationName");

            if (id == 0)
                return View(new Package());
            else
            {
                var package = await _context.Packages.FindAsync(id);
                if (package == null) return NotFound();
                return View(package);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("PackageId,PackageName,Price,Duration,DestinationId")] Package package)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    _context.Add(package);
                }
                else
                {
                    _context.Update(package);
                }
                await _context.SaveChangesAsync();

                var packages = await _context.Packages.Include(p => p.Destination).ToListAsync();
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", packages) });
            }

            ViewBag.Destinations = new SelectList(await _context.Destinations.ToListAsync(), "DestinationId", "DestinationName");
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", package) });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package == null) return NotFound();

            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();

            var packages = await _context.Packages.Include(p => p.Destination).ToListAsync();
            return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", packages) });
        }

        public async Task<IActionResult> Details(int id)
        {
            var package = await _context.Packages
                .Include(p => p.Destination)
                .FirstOrDefaultAsync(p => p.PackageId == id);

            if (package == null) return NotFound();

            return PartialView("Details", package);
        }
    }
}
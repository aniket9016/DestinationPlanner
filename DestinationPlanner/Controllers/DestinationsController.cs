using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DestinationPlanner.Models;

namespace DestinationPlanner.Controllers
{
    public class DestinationsController : Controller
    {
        private readonly AppDBContext _context;

        public DestinationsController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchkey)
        {
            var destinations = _context.Destinations
                .Include(d => d.City)
                .Include(d => d.Country)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchkey))
            {
                searchkey = searchkey.ToLower();
                destinations = destinations.Where(d =>
                    d.DestinationName.ToLower().Contains(searchkey) ||
                    d.City.CityName.ToLower().Contains(searchkey) ||
                    d.Country.CountryName.ToLower().Contains(searchkey));
            }

            ViewData["searchkey"] = searchkey;

            return View(await destinations.ToListAsync());
        }

        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            ViewBag.Countries = new SelectList(await _context.Countries.ToListAsync(), "CountryId", "CountryName");
            ViewBag.Cities = new SelectList(await _context.Cities.ToListAsync(), "CityId", "CityName");

            if (id == 0)
                return View(new Destination()); 
            else
            {
                var destination = await _context.Destinations.FindAsync(id);
                if (destination == null) return NotFound();
                return View(destination);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("DestinationId,DestinationName,CountryId,CityId")] Destination destination)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    _context.Add(destination);
                }
                else
                {
                    _context.Update(destination);
                }
                await _context.SaveChangesAsync();

                var destinations = await _context.Destinations
                    .Include(d => d.City)
                    .Include(d => d.Country)
                    .ToListAsync();

                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", destinations) });
            }

            ViewBag.Countries = new SelectList(await _context.Countries.ToListAsync(), "CountryId", "CountryName");
            ViewBag.Cities = new SelectList(await _context.Cities.ToListAsync(), "CityId", "CityName");
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", destination) });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var destination = await _context.Destinations.FindAsync(id);
            if (destination == null) return NotFound();

            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();

            var destinations = await _context.Destinations
                .Include(d => d.City)
                .Include(d => d.Country)
                .ToListAsync();

            return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", destinations) });
        }

        public JsonResult GetCitiesByCountry(int countryId)
        {
                       var cities = _context.Cities
                .Where(c => c.CountryId == countryId)
                .Select(c => new { c.CityId, c.CityName })
                .ToList();

            return Json(cities);
        }
        public async Task<IActionResult> Details(int id)
        {
            var destination = await _context.Destinations
                .Include(d => d.City)
                .Include(d => d.Country)
                .FirstOrDefaultAsync(d => d.DestinationId == id);

            if (destination == null) return NotFound();

            return PartialView("Details", destination);
        }

    }
}

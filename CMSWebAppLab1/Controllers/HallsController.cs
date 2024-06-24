using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMSWebAppLab1.Data;
using CMSWebAppLab1.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;

namespace CMSWebAppLab1.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class HallsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HallsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Halls
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Halls.Include(h => h.Cinema);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Halls/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hall = await _context.Halls
                .Include(h => h.Cinema)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hall == null)
            {
                return NotFound();
            }

            return View(hall);
        }

        // GET: Halls/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var cinemas = _context.Cinemas.Select(c => new SelectListItem
            {
                Text = c.CinemaName,
                Value = c.Id.ToString()
            }).ToList();

            // Create a ViewBag property to hold the list
            ViewBag.CinemaID = new SelectList(cinemas, "Value", "Text");
            return View();
        }

        // POST: Halls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,CinemaId,HallName,MaxPlaces")] Hall hall)
        {
            hall.Cinema = await _context.Cinemas.FindAsync(hall.CinemaId);

            if (hall.Cinema != null)
            {
                hall.Cinema.Halls.Add(hall);
                _context.Add(hall);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var cinemas = _context.Cinemas.Select(c => new SelectListItem
            {
                Text = c.CinemaName,
                Value = c.Id.ToString()
            }).ToList();

            // Create a ViewBag property to hold the list
            ViewBag.CinemaID = new SelectList(cinemas, "Value", "Text", hall.CinemaId);
            return View(hall);
        }

        // GET: Halls/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hall = await _context.Halls.FindAsync(id);
            if (hall == null)
            {
                return NotFound();
            }

            var cinemas = _context.Cinemas.Select(c => new SelectListItem
            {
                Text = c.CinemaName,
                Value = c.Id.ToString()
            }).ToList();

            // Create a ViewBag property to hold the list
            ViewBag.CinemaID = new SelectList(cinemas, "Value", "Text", hall.CinemaId);
            return View(hall);
        }

        // POST: Halls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CinemaId,HallName,MaxPlaces")] Hall hall)
        {
            if (id != hall.Id)
            {
                return NotFound();
            }

            hall.Cinema = await _context.Cinemas.FindAsync(hall.CinemaId);

            if (hall.Cinema != null)
            {
                hall.Cinema.Halls.Add(hall);
                try
                {
                    _context.Update(hall);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HallExists(hall.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var cinemas = _context.Cinemas.Select(c => new SelectListItem
            {
                Text = c.CinemaName,
                Value = c.Id.ToString()
            }).ToList();

            // Create a ViewBag property to hold the list
            ViewBag.CinemaID = new SelectList(cinemas, "Value", "Text", hall.CinemaId);
            return View(hall);
        }

        // GET: Halls/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hall = await _context.Halls
                .Include(h => h.Cinema)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hall == null)
            {
                return NotFound();
            }

            return View(hall);
        }

        // POST: Halls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hall = await _context.Halls.FindAsync(id);
            if (hall != null)
            {
                _context.Halls.Remove(hall);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HallExists(int id)
        {
            return _context.Halls.Any(e => e.Id == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMSWebAppLab1.Data;
using CMSWebAppLab1.Models;
using Microsoft.AspNetCore.Authorization;

namespace CMSWebAppLab1.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class PersonsToMoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PersonsToMoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PersonsToMovies
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PersonsToMovies.Include(p => p.Movie).Include(p => p.Person);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PersonsToMovies/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personsToMovie = await _context.PersonsToMovies
                .Include(p => p.Movie)
                .Include(p => p.Person)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (personsToMovie == null)
            {
                return NotFound();
            }

            return View(personsToMovie);
        }

        // GET: PersonsToMovies/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var movies = _context.Movies.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToList();
            var persons = _context.Persons.Select(c => new SelectListItem
            {
                Text = c.PersonName,
                Value = c.Id.ToString()
            }).ToList();
            ViewBag.MovieId = new SelectList(movies, "Value", "Text");
            ViewBag.PersonId = new SelectList(persons, "Value", "Text");
            return View();
        }

        // POST: PersonsToMovies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,MovieId,PersonId")] PersonsToMovie personsToMovie)
        {
            personsToMovie.Person = await _context.Persons.FindAsync(personsToMovie.PersonId);
            personsToMovie.Movie = await _context.Movies.FindAsync(personsToMovie.MovieId);

            if (personsToMovie.Person != null && personsToMovie.Movie != null)
            {
                _context.Add(personsToMovie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var movies = _context.Movies.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToList();
            var persons = _context.Persons.Select(c => new SelectListItem
            {
                Text = c.PersonName,
                Value = c.Id.ToString()
            }).ToList();
            ViewBag.MovieId = new SelectList(movies, "Value", "Text");
            ViewBag.PersonId = new SelectList(persons, "Value", "Text"); 
            return View(personsToMovie);
        }

        // GET: PersonsToMovies/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personsToMovie = await _context.PersonsToMovies.FindAsync(id);
            if (personsToMovie == null)
            {
                return NotFound();
            }
            var movies = _context.Movies.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToList();
            var persons = _context.Persons.Select(c => new SelectListItem
            {
                Text = c.PersonName,
                Value = c.Id.ToString()
            }).ToList();
            ViewBag.MovieId = new SelectList(movies, "Value", "Text");
            ViewBag.PersonId = new SelectList(persons, "Value", "Text"); 
            return View(personsToMovie);
        }

        // POST: PersonsToMovies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId,PersonId")] PersonsToMovie personsToMovie)
        {
            if (id != personsToMovie.Id)
            {
                return NotFound();
            }

            personsToMovie.Person = await _context.Persons.FindAsync(personsToMovie.PersonId);
            personsToMovie.Movie = await _context.Movies.FindAsync(personsToMovie.MovieId);

            if (personsToMovie.Person != null && personsToMovie.Movie != null)
            {
                try
                {
                    _context.Update(personsToMovie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonsToMovieExists(personsToMovie.Id))
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
            var movies = _context.Movies.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToList();
            var persons = _context.Persons.Select(c => new SelectListItem
            {
                Text = c.PersonName,
                Value = c.Id.ToString()
            }).ToList();
            ViewBag.MovieId = new SelectList(movies, "Value", "Text");
            ViewBag.PersonId = new SelectList(persons, "Value", "Text"); 
            return View(personsToMovie);
        }

        // GET: PersonsToMovies/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personsToMovie = await _context.PersonsToMovies
                .Include(p => p.Movie)
                .Include(p => p.Person)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (personsToMovie == null)
            {
                return NotFound();
            }

            return View(personsToMovie);
        }

        // POST: PersonsToMovies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personsToMovie = await _context.PersonsToMovies.FindAsync(id);
            if (personsToMovie != null)
            {
                _context.PersonsToMovies.Remove(personsToMovie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonsToMovieExists(int id)
        {
            return _context.PersonsToMovies.Any(e => e.Id == id);
        }
    }
}

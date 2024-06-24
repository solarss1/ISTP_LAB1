using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMSWebAppLab1.Data;
using CMSWebAppLab1.Models;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using Microsoft.AspNetCore.Authorization;

namespace CMSWebAppLab1.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sessions
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sessions.Include(s => s.Hall).Include(s => s.Movie);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Sessions/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Hall)
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // GET: Sessions/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var halls = _context.Halls.Select(c => new SelectListItem
            {
                Text = c.HallName,
                Value = c.Id.ToString()
            }).ToList();
            var movies = _context.Movies.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToList();
            ViewBag.HallId = new SelectList(halls, "Value", "Text");
            ViewBag.MovieId = new SelectList(movies, "Value", "Text");
            return View();
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,HallId,MovieId,StartTime,Price")] Session session)
        {
            session.Hall = await _context.Halls.FindAsync(session.HallId);
            session.Movie = await _context.Movies.FindAsync(session.MovieId);

            if (session.Hall != null && session.Movie != null)
            {
                session.Hall.Sessions.Add(session);
                session.Movie.Sessions.Add(session);
                _context.Add(session);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var halls = _context.Halls.Select(c => new SelectListItem
            {
                Text = c.HallName,
                Value = c.Id.ToString()
            }).ToList();
            var movies = _context.Movies.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToList();
            ViewBag.HallId = new SelectList(halls, "Value", "Text");
            ViewBag.MovieId = new SelectList(movies, "Value", "Text");
            return View(session);
        }

        // GET: Sessions/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            var halls = _context.Halls.Select(c => new SelectListItem
            {
                Text = c.HallName,
                Value = c.Id.ToString()
            }).ToList();
            var movies = _context.Movies.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToList();
            ViewBag.HallId = new SelectList(halls, "Value", "Text");
            ViewBag.MovieId = new SelectList(movies, "Value", "Text");
            return View(session);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HallId,MovieId,StartTime,Price")] Session session)
        {
            if (id != session.Id)
            {
                return NotFound();
            }

            session.Hall = await _context.Halls.FindAsync(session.HallId);
            session.Movie = await _context.Movies.FindAsync(session.MovieId);

            if (session.Hall != null && session.Movie != null)
            {
                try
                {
                    session.Hall.Sessions.Add(session);
                    session.Movie.Sessions.Add(session);
                    _context.Update(session);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessionExists(session.Id))
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
            var halls = _context.Halls.Select(c => new SelectListItem
            {
                Text = c.HallName,
                Value = c.Id.ToString()
            }).ToList();
            var movies = _context.Movies.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToList();
            ViewBag.HallId = new SelectList(halls, "Value", "Text");
            ViewBag.MovieId = new SelectList(movies, "Value", "Text");
            return View(session);
        }

        // GET: Sessions/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var session = await _context.Sessions
                .Include(s => s.Hall)
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session != null)
            {
                _context.Sessions.Remove(session);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> SearchSessions(SessionSearchViewModel searchModel)
        {
            var query = _context.Sessions
                .Include(s => s.Movie)
                .ThenInclude(m => m.PersonsToMovies)
                .ThenInclude(ptm => ptm.Person)
                .Include(s => s.Hall)
                .ThenInclude(h => h.Cinema)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.Title))
            {
                query = query.Where(s => s.Movie.Title.Contains(searchModel.Title));
            }

            if (!string.IsNullOrEmpty(searchModel.ActorName))
            {
                query = query.Where(s => s.Movie.PersonsToMovies.Any(ptm => ptm.Person.TookPartAs == "Actor" && ptm.Person.PersonName.Contains(searchModel.ActorName)));
            }

            if (!string.IsNullOrEmpty(searchModel.DirectorName))
            {
                query = query.Where(s => s.Movie.PersonsToMovies.Any(ptm => ptm.Person.TookPartAs == "Director" && ptm.Person.PersonName.Contains(searchModel.DirectorName)));
            }

            var result = await query.Select(s => new SessionResultViewModel
            {
                Title = s.Movie.Title,
                ActorName = s.Movie.PersonsToMovies.Where(ptm => ptm.Person.TookPartAs == "Actor").Select(ptm => ptm.Person.PersonName).FirstOrDefault(),
                DirectorName = s.Movie.PersonsToMovies.Where(ptm => ptm.Person.TookPartAs == "Director").Select(ptm => ptm.Person.PersonName).FirstOrDefault(),
                StartTime = s.StartTime,
                Price = s.Price,
                CinemaName = s.Hall.Cinema.CinemaName,
                HallName = s.Hall.HallName
            }).ToListAsync();

            searchModel.Sessions = result;
            return View(searchModel);
        }

    }
}

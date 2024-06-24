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
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tickets
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Tickets.Include(t => t.Session);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tickets/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Session)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var sessions = _context.Sessions.Select(c => new SelectListItem
            {
                Text = c.Id + ": " + c.Movie.Title + ", " + c.StartTime,
                Value = c.Id.ToString()
            }).ToList();

            // Create a ViewBag property to hold the list
            ViewBag.SessionId = new SelectList(sessions, "Value", "Text");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,SessionId,SeatNumber,SoldTime")] Ticket ticket)
        {
            ticket.Session = await _context.Sessions.FindAsync(ticket.SessionId);

            if (ticket.Session != null)
            {
                ticket.Session.Tickets.Add(ticket);
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var sessions = _context.Sessions.Select(c => new SelectListItem
            {
                Text = c.Id + ": " + c.Movie.Title + ", " + c.StartTime,
                Value = c.Id.ToString()
            }).ToList();

            // Create a ViewBag property to hold the list
            ViewBag.SessionId = new SelectList(sessions, "Value", "Text");
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            var sessions = _context.Sessions.Select(c => new SelectListItem
            {
                Text = c.Id + ": " + c.Movie.Title + ", " + c.StartTime,
                Value = c.Id.ToString()
            }).ToList();

            // Create a ViewBag property to hold the list
            ViewBag.SessionId = new SelectList(sessions, "Value", "Text");
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SessionId,SeatNumber,SoldTime")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            ticket.Session = await _context.Sessions.FindAsync(ticket.SessionId);

            if (ticket.Session != null)
            {
                try
                {
                    ticket.Session.Tickets.Add(ticket);
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
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
            var sessions = _context.Sessions.Select(c => new SelectListItem
            {
                Text = c.Id + ": " + c.Movie.Title + ", " + c.StartTime,
                Value = c.Id.ToString()
            }).ToList();

            // Create a ViewBag property to hold the list
            ViewBag.SessionId = new SelectList(sessions, "Value", "Text");
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Session)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Buy(int sessionId)
        {
            var session = await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .ThenInclude(h => h.Cinema)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null)
            {
                return NotFound();
            }

            var availableSeats = Enumerable.Range(1, session.Hall.MaxPlaces)
                .Except(_context.Tickets.Where(t => t.SessionId == session.Id).Select(t => t.SeatNumber))
                .ToList();

            var model = new BuyTicketViewModel
            {
                SessionId = session.Id,
                MovieTitle = session.Movie.Title,
                HallName = session.Hall.HallName,
                CinemaName = session.Hall.Cinema.CinemaName,
                StartTime = session.StartTime,
                Price = session.Price,
                AvailableSeats = availableSeats
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Buy(BuyTicketViewModel model)
        {
            if (model.SessionId > 0 && model.SelectedSeat > 0)
            {
                var seatIsTaken = _context.Tickets.Any(t => t.SessionId == model.SessionId && t.SeatNumber == model.SelectedSeat);

                if (seatIsTaken)
                {
                    ModelState.AddModelError("", "The selected seat is already taken.");
                    return View(model);
                }

                var ticket = new Ticket
                {
                    SessionId = model.SessionId,
                    SeatNumber = model.SelectedSeat,
                    SoldTime = DateTime.Now
                };

                _context.Add(ticket);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Confirmation), new { id = ticket.Id });
            }

            return View(model);
        }

        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Confirmation(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Session)
                .ThenInclude(s => s.Movie)
                .Include(t => t.Session.Hall)
                .ThenInclude(h => h.Cinema)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            var model = new TicketConfirmationViewModel
            {
                TicketId = ticket.Id,
                MovieTitle = ticket.Session.Movie.Title,
                CinemaName = ticket.Session.Hall.Cinema.CinemaName,
                HallName = ticket.Session.Hall.HallName,
                StartTime = ticket.Session.StartTime,
                TicketPlaceNumber = ticket.SeatNumber,
                TicketSoldDateTime = ticket.SoldTime,
                Price = ticket.Session.Price
            };

            return View(model);
        }
    }
}

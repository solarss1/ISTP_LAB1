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
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using S = DocumentFormat.OpenXml.Spreadsheet;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace CMSWebAppLab1.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class CinemasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CinemasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cinemas
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cinemas.ToListAsync());
        }

        // GET: Cinemas/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cinema = await _context.Cinemas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }

        // GET: Cinemas/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cinemas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,CinemaName,Address")] Cinema cinema)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cinema);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cinema);
        }

        // GET: Cinemas/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null)
            {
                return NotFound();
            }
            return View(cinema);
        }

        // POST: Cinemas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CinemaName,Address")] Cinema cinema)
        {
            if (id != cinema.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cinema);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CinemaExists(cinema.Id))
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
            return View(cinema);
        }

        // GET: Cinemas/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cinema = await _context.Cinemas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }

        // POST: Cinemas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema != null)
            {
                _context.Cinemas.Remove(cinema);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CinemaExists(int id)
        {
            return _context.Cinemas.Any(e => e.Id == id);
        }

        // Action to load cinema selection page
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> SelectCinema()
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

        // Action to handle cinema selection and show halls
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> CinemaHalls(int id)
        {
            var cinema = await _context.Cinemas
                .Include(c => c.Halls)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cinema == null)
            {
                return NotFound("Halls not found");
            }

            return View("CinemaHalls", cinema);
        }

        // Action to handle cinema selection and show sessions
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> CinemaSessions(int id)
        {
            var cinema = await _context.Cinemas
                .Include(c => c.Halls)
                    .ThenInclude(h => h.Sessions)
                        .ThenInclude(s => s.Movie)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cinema == null)
            {
                return NotFound("Cinema not found");
            }

            return View("CinemaSessions", cinema);
        }
        
        private FileContentResult ExportToExcel(List<Cinema> cinemas)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                    workbookPart.Workbook = new S.Workbook();
                    S.Sheets sheets = workbookPart.Workbook.AppendChild(new S.Sheets());

                    uint sheetId = 1;
                    foreach (var cinema in cinemas)
                    {
                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new S.Worksheet(new S.SheetData());

                        S.Sheet sheet = new S.Sheet()
                        {
                            Id = workbookPart.GetIdOfPart(worksheetPart),
                            SheetId = sheetId++,
                            Name = cinema.CinemaName
                        };
                        sheets.Append(sheet);

                        S.SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<S.SheetData>();

                        // Add header row
                        var headerRow = new S.Row();
                        headerRow.Append(
                            new S.Cell { CellValue = new S.CellValue("Назва зали"), DataType = S.CellValues.String },
                            new S.Cell { CellValue = new S.CellValue("Назва фільму"), DataType = S.CellValues.String },
                            new S.Cell { CellValue = new S.CellValue("Початок сеансу"), DataType = S.CellValues.String },
                            new S.Cell { CellValue = new S.CellValue("Ціна"), DataType = S.CellValues.String }
                        );
                        sheetData.AppendChild(headerRow);

                        // Add rows for each session
                        foreach (var hall in cinema.Halls)
                        {
                            foreach (var session in hall.Sessions)
                            {
                                var row = new S.Row();
                                row.Append(
                                    new S.Cell { CellValue = new S.CellValue(hall.HallName), DataType = S.CellValues.String },
                                    new S.Cell { CellValue = new S.CellValue(session.Movie.Title), DataType = S.CellValues.String },
                                    new S.Cell { CellValue = new S.CellValue(session.StartTime.ToString("yyyy-MM-dd HH:mm")), DataType = S.CellValues.String },
                                    new S.Cell { CellValue = new S.CellValue(session.Price.ToString("C")), DataType = S.CellValues.String }
                                );
                                sheetData.AppendChild(row);
                            }
                        }
                    }

                    workbookPart.Workbook.Save();
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CinemaSessions.xlsx");
            }
        }


        private FileContentResult ExportToWord(List<Cinema> cinemas)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new W.Document();
                    W.Body body = mainPart.Document.AppendChild(new W.Body());

                    foreach (var cinema in cinemas)
                    {
                        // Add cinema name as a heading
                        W.Paragraph heading = body.AppendChild(new W.Paragraph());
                        W.Run runHeading = heading.AppendChild(new W.Run());
                        runHeading.AppendChild(new W.Text(cinema.CinemaName));
                        runHeading.AppendChild(new W.Break());

                        // Create a table for each cinema's sessions
                        W.Table table = new W.Table();
                        W.TableProperties props = new W.TableProperties(
                            new W.TableBorders(
                                new W.TopBorder { Val = new EnumValue<W.BorderValues>(W.BorderValues.Single), Size = 12 },
                                new W.BottomBorder { Val = new EnumValue<W.BorderValues>(W.BorderValues.Single), Size = 12 },
                                new W.LeftBorder { Val = new EnumValue<W.BorderValues>(W.BorderValues.Single), Size = 12 },
                                new W.RightBorder { Val = new EnumValue<W.BorderValues>(W.BorderValues.Single), Size = 12 },
                                new W.InsideHorizontalBorder { Val = new EnumValue<W.BorderValues>(W.BorderValues.Single), Size = 12 },
                                new W.InsideVerticalBorder { Val = new EnumValue<W.BorderValues>(W.BorderValues.Single), Size = 12 }
                            ));
                        table.AppendChild(props);

                        // Add a row for the header
                        W.TableRow headerRow = new W.TableRow();
                        headerRow.Append(
                            new W.TableCell(new W.Paragraph(new W.Run(new W.Text("Назва зали")))),
                            new W.TableCell(new W.Paragraph(new W.Run(new W.Text("Назва фільму")))),
                            new W.TableCell(new W.Paragraph(new W.Run(new W.Text("Початок сесії")))),
                            new W.TableCell(new W.Paragraph(new W.Run(new W.Text("Ціна"))))
                        );
                        table.Append(headerRow);

                        // Add rows for each session
                        foreach (var hall in cinema.Halls)
                        {
                            foreach (var session in hall.Sessions)
                            {
                                W.TableRow row = new W.TableRow();
                                row.Append(
                                    new W.TableCell(new W.Paragraph(new W.Run(new W.Text(hall.HallName)))),
                                    new W.TableCell(new W.Paragraph(new W.Run(new W.Text(session.Movie.Title)))),
                                    new W.TableCell(new W.Paragraph(new W.Run(new W.Text(session.StartTime.ToString("g"))))),
                                    new W.TableCell(new W.Paragraph(new W.Run(new W.Text(session.Price.ToString("C")))))
                                );
                                table.Append(row);
                            }
                        }

                        body.AppendChild(table);
                        // Add a page break after each cinema
                        body.AppendChild(new W.Paragraph(new W.Run(new W.Break() { Type = W.BreakValues.Page })));
                    }

                    mainPart.Document.Save();
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "CinemaSessionsReport.docx");
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportCinemaSessions(string format)
        {
            var cinemas = await _context.Cinemas
                        .Include(c => c.Halls)
                        .ThenInclude(h => h.Sessions)
                        .ThenInclude(s => s.Movie)
                        .ToListAsync();

            if (format.Equals("xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return ExportToExcel(cinemas);
            }
            else if (format.Equals("docx", StringComparison.OrdinalIgnoreCase))
            {
                return ExportToWord(cinemas);
            }

            return View("Error");
        }
    }
}

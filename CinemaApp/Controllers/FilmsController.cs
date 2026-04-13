using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaApp.Models;
using ClosedXML.Excel;

namespace CinemaApp.Controllers
{
    public class FilmsController : Controller
    {
        private readonly CinemaContext _context;

        public FilmsController(CinemaContext context)
        {
            _context = context;
        }

        // GET: Films
        public async Task<IActionResult> Index()
        {
            return View(await _context.Films.ToListAsync());
        }

        // Експорт у Excel
        public async Task<IActionResult> ExportToExcel()
        {
            var films = await _context.Films.ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Фільми");

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Назва";
            worksheet.Cell(1, 3).Value = "Жанр";
            worksheet.Cell(1, 4).Value = "Тривалість";
            worksheet.Cell(1, 5).Value = "Опис";

            for (int i = 0; i < films.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = films[i].Id;
                worksheet.Cell(i + 2, 2).Value = films[i].Title;
                worksheet.Cell(i + 2, 3).Value = films[i].Genre;
                worksheet.Cell(i + 2, 4).Value = films[i].Duration;
                worksheet.Cell(i + 2, 5).Value = films[i].Description;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "films.xlsx");
        }

        // Імпорт з Excel
        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return RedirectToAction(nameof(Index));

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

            foreach (var row in rows)
            {
                var film = new Film
                {
                    Title = row.Cell(2).GetString(),
                    Genre = row.Cell(3).GetString(),
                    Duration = row.Cell(4).GetValue<int>(),
                    Description = row.Cell(5).GetString()
                };
                _context.Films.Add(film);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Films/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var film = await _context.Films.FirstOrDefaultAsync(m => m.Id == id);
            if (film == null) return NotFound();

            return View(film);
        }

        // GET: Films/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Films/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,Duration,Description")] Film film)
        {
            if (ModelState.IsValid)
            {
                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }

        // GET: Films/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var film = await _context.Films.FindAsync(id);
            if (film == null) return NotFound();

            return View(film);
        }

        // POST: Films/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,Duration,Description")] Film film)
        {
            if (id != film.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(film);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }

        // GET: Films/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var film = await _context.Films.FirstOrDefaultAsync(m => m.Id == id);
            if (film == null) return NotFound();

            return View(film);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film != null) _context.Films.Remove(film);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.Id == id);
        }
    }
}
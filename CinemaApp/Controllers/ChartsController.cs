using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaApp.Models;

namespace CinemaApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChartsController : ControllerBase
{
    private readonly CinemaContext _context;

    public ChartsController(CinemaContext context)
    {
        _context = context;
    }

    //Кількість фільмів за жанром
    [HttpGet("filmsByGenre")]
    public async Task<JsonResult> GetFilmsByGenre()
    {
        var data = await _context.Films
            .GroupBy(f => f.Genre)
            .Select(g => new { Genre = g.Key, Count = g.Count() })
            .ToListAsync();

        return new JsonResult(data);
    }

    //Кількість сеансів по залах
    [HttpGet("sessionsByHall")]
    public async Task<JsonResult> GetSessionsByHall()
    {
        var data = await _context.Sessions
            .Include(s => s.Hall)
            .GroupBy(s => s.Hall.Name)
            .Select(g => new { Hall = g.Key, Count = g.Count() })
            .ToListAsync();

        return new JsonResult(data);
    }
}
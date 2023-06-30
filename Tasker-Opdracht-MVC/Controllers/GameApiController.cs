using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tasker_Opdracht_MVC.Data;
using Tasker_Opdracht_MVC.Data.Entities;

namespace Tasker_Opdracht_MVC.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GameApiController : ControllerBase
{
    private readonly GameDBContext _context;

    public GameApiController(GameDBContext context)
    {
        _context = context;
    }

    // GET: api/GameApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> GetGames()
    {
        if (_context.Games == null)
        {
            return NotFound();
        }
        return await _context.Games.ToListAsync();
    }

    // GET: api/GameApi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Game>> GetGame(int id)
    {
        if (_context.Games == null)
        {
            return NotFound();
        }
        var game = await _context.Games.FindAsync(id);

        if (game == null)
        {
            return NotFound();
        }

        return game;
    }

    // PUT: api/GameApi/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGame(int id, Game game)
    {
        if (id != game.Id)
        {
            return BadRequest();
        }

        _context.Entry(game).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!GameExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/GameApi
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Game>> PostGame(Game game)
    {
        if (_context.Games == null)
        {
            return Problem("Entity set 'GameDBContext.Games'  is null.");
        }
        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetGame", new { id = game.Id }, game);
    }

    // DELETE: api/GameApi/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        if (_context.Games == null)
        {
            return NotFound();
        }
        var game = await _context.Games.FindAsync(id);
        if (game == null)
        {
            return NotFound();
        }

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool GameExists(int id)
    {
        return (_context.Games?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}

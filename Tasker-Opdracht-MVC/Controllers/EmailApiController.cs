using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tasker_Opdracht_MVC.Data;
using Tasker_Opdracht_MVC.Data.Entities;

namespace Tasker_Opdracht_MVC.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailApiController : ControllerBase
{
    private readonly EmailDBContext _context;

    public EmailApiController(EmailDBContext context)
    {
        _context = context;
    }

    // GET: api/EmailModels
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Email>>> GetForms()
    {
        if (_context.Forms == null)
        {
            return NotFound();
        }
        return await _context.Forms.ToListAsync();
    }

    // GET: api/EmailModels/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Email>> GetEmailModel(int id)
    {
        if (_context.Forms == null)
        {
            return NotFound();
        }
        var emailModel = await _context.Forms.FindAsync(id);

        if (emailModel == null)
        {
            return NotFound();
        }

        return emailModel;
    }

    // PUT: api/EmailModels/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmailModel(int id, Email emailModel)
    {
        if (id != emailModel.Id)
        {
            return BadRequest();
        }

        _context.Entry(emailModel).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EmailModelExists(id))
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

    // POST: api/EmailModels
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Email>> PostEmailModel(Email emailModel)
    {
        if (_context.Forms == null)
        {
            return Problem("Entity set 'EmailDBContext.Forms'  is null.");
        }
        _context.Forms.Add(emailModel);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetEmailModel", new { id = emailModel.Id }, emailModel);
    }

    // DELETE: api/EmailModels/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmailModel(int id)
    {
        if (_context.Forms == null)
        {
            return NotFound();
        }
        var emailModel = await _context.Forms.FindAsync(id);
        if (emailModel == null)
        {
            return NotFound();
        }

        _context.Forms.Remove(emailModel);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EmailModelExists(int id)
    {
        return (_context.Forms?.Any(e => e.Id == id)).GetValueOrDefault();
    }


}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReactionLab.Data;

namespace ReactionLab.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReactionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReactionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateReaction(int id, [FromBody] ReactionRequest request)
    {
        var reaction = await _context.Reactions.FindAsync(id);
        if (reaction == null) return NotFound();

        reaction.Title = request.Title;
        reaction.Equation = request.Equation;

        if (request.MoleculeData != null) reaction.MoleculeData = request.MoleculeData;
        if (request.StepsData != null) reaction.StepsData = request.StepsData;

        await _context.SaveChangesAsync();

        return Ok(new { reaction.Id, reaction.Title, reaction.Equation });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteReaction(int id)
    {
        var reaction = await _context.Reactions.FindAsync(id);
        if (reaction == null) return NotFound();

        _context.Reactions.Remove(reaction);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Reaction '{reaction.Title}' deleted" });
    }
}

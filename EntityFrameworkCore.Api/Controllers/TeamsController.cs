using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.Data;
using EntityFrameworkCore.Domain;
using EntityFrameworkCore.Api.Models;

namespace EntityFrameworkCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly FootballLeagueDbContext _context;

        public TeamsController(FootballLeagueDbContext context)
        {
            _context = context;
        }

        // GET: api/Teams
        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        public async Task<ActionResult<IEnumerable<TeamDTO>>> GetTeams()
        {
            //return await _context.Teams.ToListAsync(); cip...104. Return DTO instead of entity.
            //cip...104. Use Select to project the entity into a DTO.
            var teams = await _context.Teams
                .Select(t => new TeamDTO //cip...104. Projecting to DTO.
                {
                    ID = t.Id,
                    Name = t.Name
                })
                .ToListAsync();
            return teams;
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            //var team = await _context.Teams.FindAsync(id);
            var team = await _context.Teams
                .Include(t => t.Coach)
                .Include(t => t.League)
                .FirstOrDefaultAsync(t => t.Id == id); //cip...104. ths can cause a cyclic reference exception if the entity has navigation properties that reference each other. Use DTOs to avoid this issue.

            if (team == null)
            {
                return NotFound();
            }

            return team;
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeam(int id, Team team)
        {
            if (id != team.Id)
            {
                return BadRequest();
            }

            _context.Entry(team).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
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

        // POST: api/Teams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeam", new { id = team.Id }, team);
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            //cip...103 refactoring to stop 2 x db trips.
            //var team = await _context.Teams.FindAsync(id);
            //if (team == null)
            //{
            //    return NotFound();
            //}

            //_context.Teams.Remove(team);
            //await _context.SaveChangesAsync();
            await _context.Teams.Where(t => t.Id == id).ExecuteDeleteAsync(); //cip...103. Use ExecuteDeleteAsync to delete without loading the entity into memory.
            return NoContent();
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}

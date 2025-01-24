using BaseballCalcASP.Data;
using BaseballCalcASP.Models;
using BaseballModelsLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Numerics;
using System.Text.Json;

namespace BaseballCalcASP.APIControllers
{
    [ApiController]
    [Route("api/players")]
//    [Authorize(Roles = "admin, user")]
    public class PlayersControllerAPI : ControllerBase
    {
        private readonly BaseballCalcASPContext _context;

        public PlayersControllerAPI(BaseballCalcASPContext context)
        {
            _context = context;
        }

        // GET: Players
        [HttpGet]
        [Route("listplayers")]
        public async Task<ActionResult<List<Player>>> GetPlayers()
        {
            if (_context.Players == null)
            {
                return null;
            }

            //            var jsonString = JsonSerializer.Serialize(_context.Players);
            //          return await _context.Players.ToListAsync();//.ToJson();
            var jsonString = JsonSerializer.Serialize(_context.Players);
            return Content(jsonString, "application/json");
        }
        [HttpGet]
        [Route("findplayer")]
        // POST: api/players/findplayer
        public async Task<ActionResult<Player>> Findplayer(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .FirstOrDefaultAsync(m => m.Id == id);
            if (player == null)
            {
                return NotFound();
            }

            if (player.TeamId != null)
            {
                player.Team = _context.Teams.Find(player.TeamId);
            }

            // Ok function automatically serializes player object to JSON
            //return Ok(player);
            var jsonString = JsonSerializer.Serialize(player);
            return Content(jsonString, "application/json");
        }

        // POST: Players - Accept JSON to add a list of players
        [HttpPost]
        [Route("addplayers")]
        public async Task<IActionResult> AddPlayers([FromBody] List<Player> players)
        {
            // The[FromBody] List<Team> parameter tells ASP.NET Core to automatically deserialize the JSON payload into a List < Player > object.
            // the json is automatically converted on receipt to List<Player>, no need to deserialize here
            if (players == null || players.Count == 0)
            {
                return BadRequest("No players found in the payload.");
            }

            try
            {
                int? teamid = players[0].TeamId;
                var playersOfSameTeamToDelete = _context.Players.Where(s => s.TeamId == teamid).ToList();
                if (playersOfSameTeamToDelete.Count > 0)
                {
                    _context.Players.RemoveRange(playersOfSameTeamToDelete);
                }
                // Add the deserialized teams to the database context
                foreach (var player in players)
                {
                    _context.Players.Add(player);
                }

                // Save changes asynchronously
                //_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Teams ON");//allow setting id field temporarily
                await _context.SaveChangesAsync();
                //_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Teams OFF");

                return Ok($"{players.Count} Players added successfully.");
            }
            catch (JsonException ex)
            {
                return BadRequest($"Invalid JSON format: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}

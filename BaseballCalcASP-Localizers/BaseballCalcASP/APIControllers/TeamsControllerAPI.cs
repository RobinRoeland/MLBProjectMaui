using BaseballCalcASP.Data;
using BaseballModelsLib.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities.IO;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;

namespace BaseballCalcASP.APIControllers
{
    [ApiController]
    [Route("api/teams")]
//    [Authorize(Roles = "admin, user")]
    public class TeamsControllerAPI : ControllerBase
    {
        private readonly BaseballCalcASPContext _context;

        public TeamsControllerAPI(BaseballCalcASPContext context)
        {
            _context = context;
        }

        // GET: Teams
        [HttpGet]
        [Route("listteams")]
        public async Task<ActionResult<List<Team>>> ListTeams()
        {
            if (_context.Teams == null)
            {
                return null;
            }

            //            var jsonString = JsonSerializer.Serialize(_context.Players);
            //          return await _context.Players.ToListAsync();//.ToJson();
            string jsonString = JsonConvert.SerializeObject(_context.Teams, Formatting.Indented);
            return Content(jsonString, "application/json");
        }

        // GET: Teams
        [HttpGet]
        [Route("getteamcount")]
        public async Task<ActionResult<int>> countTeamsInTable()
        {
            int count = 0;
            if (_context.Teams != null)
            {
                count = _context.Teams.Count();
            }
            string jsonString = JsonConvert.SerializeObject(count, Formatting.Indented);
            return Ok(jsonString);
        }


        // POST: Teams - Accept JSON to add teams
        [HttpPost]
        [Route("addteams")]
        public async Task<IActionResult> AddTeams([FromBody] List<Team> teams) 
        {
            // The[FromBody] List<Team> parameter tells ASP.NET Core to automatically deserialize the JSON payload into a List < Team > object.
            // the json is automatically converted on receipt to List<Team>, no need to deserialize here
            if (teams == null || teams.Count == 0)
            {
                return BadRequest("No teams found in the payload.");
            }

            try
            {
                // delete all statistics
                var statisticsToDelete = _context.Statistics.ToList();
                if (statisticsToDelete.Count > 0)
                {
                    _context.Statistics.RemoveRange(statisticsToDelete);
                }
                // delete all games
                var gamesToDelete = _context.Games.ToList();
                if (gamesToDelete.Count > 0)
                {
                    _context.Games.RemoveRange(gamesToDelete);
                }
                // delete all players
                var playersToDelete = _context.Players.ToList();
                if (playersToDelete.Count > 0)
                {
                    _context.Players.RemoveRange(playersToDelete);
                }
                // delete all teams
                var teamsToDelete = _context.Teams.ToList();
                if (teamsToDelete.Count > 0)
                {
                    _context.Teams.RemoveRange(teamsToDelete);
                }

                // Add the deserialized teams to the database context
                foreach (var team in teams)
                {
                    _context.Teams.Add(team);
                }

                // Save changes asynchronously
                //_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Teams ON");//allow setting id field temporarily
                await _context.SaveChangesAsync();
                //_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Teams OFF");

                return Ok($"{teams.Count} Teams added successfully.");
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

using BaseballCalcASP.Data;
using BaseballModelsLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace BaseballCalcASP.APIControllers
{
    [ApiController]
    [Route("api/statistics")]
//    [Authorize(Roles = "admin, user")]
    public class GameStatisticsControllerAPI : ControllerBase
    {
        private readonly BaseballCalcASPContext _context;

        public GameStatisticsControllerAPI(BaseballCalcASPContext context)
        {
            _context = context;
        }
        
        // POST: Statistics - Accept JSON to add a list of ScoreStatistics
        [HttpPost]
        [Route("bookStatistics")]
        public async Task<IActionResult> AddStatistics([FromBody] List<ScoreStatistic> statisticsScores)
        {
            // The[FromBody] List<Team> parameter tells ASP.NET Core to automatically deserialize the JSON payload into a List < Team > object.
            // the json is automatically converted on receipt to List<Team>, no need to deserialize here
            if (statisticsScores == null || statisticsScores.Count == 0)
            {
                return BadRequest("No statisticsScores found in the payload.");
            }

            try
            {
                // Add the deserialized teams to the database context
                foreach (var scorecomponent in statisticsScores)
                {
                    _context.Statistics.Add(scorecomponent);
                }

                // Save changes asynchronously
                //_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Teams ON");//allow setting id field temporarily
                await _context.SaveChangesAsync();
                //_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Teams OFF");

                return Ok($"{statisticsScores.Count} scores added successfully.");
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
        
        // POST: Statistics - Accept JSON to replace a list of ScoreStatistics
        /*[HttpPost]
        [Route("replaceStatistics")]
        public async Task<IActionResult> ReplaceStatistics([FromBody] List<ScoreStatistic> statisticsScores)
        {
            if (statisticsScores == null || statisticsScores.Count == 0)
            {
                return BadRequest("No statisticsScores found in the payload.");
            }

            try
            {
                // Extract unique primary key combinations
                var keysToDelete = statisticsScores.Select(s => new { s.GameId, s.CurrentInning, s.PlayerId }).Distinct();

                // Delete existing records with the same primary key combinations
                foreach (var key in keysToDelete)
                {
                    var existingRecords = await _context.Statistics
                        .Where(s => s.GameId == key.GameId && s.CurrentInning == key.CurrentInning && s.PlayerId == key.PlayerId)
                        .ToListAsync();

                    _context.Statistics.RemoveRange(existingRecords);
                }

                // Add the new statistics
                _context.Statistics.AddRange(statisticsScores);

                // Save changes asynchronously
                await _context.SaveChangesAsync();

                return Ok($"{statisticsScores.Count} scores replaced successfully.");
            }
            catch (JsonException ex)
            {
                return BadRequest($"Invalid JSON format: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }*/
        
    }
}

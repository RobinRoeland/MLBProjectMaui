using BaseballCalcASP.Data;
using BaseballModelsLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Numerics;

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
        
        [HttpPost]
        [Route("deletescoresforgame")]
        public async Task<IActionResult> DeleteGame([FromBody] Game game)
        {
            if (game == null)
            {
                return BadRequest("No game found in the payload.");
            }

            try
            {
                // delete scores linked to game                
                    var scoresToDelete = _context.Statistics.Where(s => s.GameId == game.Id).ToList();
                    if (scoresToDelete.Count > 0)
                    {
                        _context.Statistics.RemoveRange(scoresToDelete);
                    }
                // Save changes asynchronously
                await _context.SaveChangesAsync();

                // Return the updated or added game
                var jsonString = JsonSerializer.Serialize(game);
                return Ok("Statistics deleted succesfully.");
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

        [HttpGet]
        [Route("distinctScoreNames/{personMLBId}")]
        public async Task<IActionResult> GetDistinctScoreNames(string personMLBId)
        {
            if (string.IsNullOrEmpty(personMLBId))
            {
                return BadRequest("PersonMLBId cannot be null or empty.");
            }

            try
            {
                int personid = Convert.ToInt32(personMLBId);
                var distinctScoreNames = await _context.Statistics
                    .Where(s => s.PersonMLBId == personid)
                    .Select(s => s.ScoreName)
                    .Distinct()
                    .ToListAsync();

                return Ok(distinctScoreNames);
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

        [HttpPost]
        [Route("ScoreNames")]
        public async Task<ActionResult<List<StatisticsKPI>>> GetStatistics([FromBody] Player player)
        {
            KPIFactory myFactory = new KPIFactory();
            List<string> KPIStringLijst = myFactory.mKPIs.Keys.ToList();

            List<StatisticsKPI> statistics = new List<StatisticsKPI>();
            List<ScoreStatistic> allScoresForPlayer = _context.Statistics.Where(score => score.PersonMLBId == player.Id).ToList();

            if (allScoresForPlayer.Count > 0)
            {
                foreach (string KPInaam in KPIStringLijst)
                {
                    if (myFactory.mKPIs.ContainsKey(KPInaam))
                        statistics.Add(myFactory.mKPIs[KPInaam].Calculate(allScoresForPlayer));
                }
            }

            return statistics;
        }

        [HttpPost]
        [Route("ScoreNamesForKPI")]
        public async Task<ActionResult<List<StatisticsKPI>>> GetDetailStatisticsForKPI([FromBody] StatisticsKPI statistic)
        {
            KPIFactory myFactory = new KPIFactory();

            List<string> usedScoreNames = myFactory.mKPIs[statistic.StatisticsName].UsedScores;
            List<ScoreStatistic> allScoresForPlayer = _context.Statistics.Where(score => score.PersonMLBId == statistic.SpelerId).ToList();

            List<StatisticsKPI> statistics = new List<StatisticsKPI>();
            foreach (string scoreName in usedScoreNames)
            {
                if (myFactory.mKPIs.ContainsKey(scoreName))
                    statistics.Add(myFactory.mKPIs[scoreName].Calculate(allScoresForPlayer));
                else
                {
                    statistics.Add(new StatisticsKPI(
                            scoreName,
                            allScoresForPlayer.Where(score => score.ScoreName == scoreName).Select(score => score.ScoreValue).Sum(),
                            statistic.SpelerId
                        )
                    );
                }
            }

            return statistics;
        }
    }
}

using BaseballCalcASP.Data;
using BaseballModelsLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace BaseballCalcASP.APIControllers
{
    [ApiController]
    [Route("api/games")]
//    [Authorize(Roles = "admin, user")]
    public class GamesControllerAPI : ControllerBase
    {
        private readonly BaseballCalcASPContext _context;

        public GamesControllerAPI(BaseballCalcASPContext context)
        {
            _context = context;
        }

        // GET: Players
        [HttpGet]
        [Route("listgames")]
        public async Task<ActionResult<List<Game>>> GetGames()
        {
            if (_context.Games == null)
            {
                return null;
            }

            //            var jsonString = JsonSerializer.Serialize(_context.Players);
            //          return await _context.Players.ToListAsync();//.ToJson();
            var jsonString = JsonSerializer.Serialize(_context.Games);
            return Content(jsonString, "application/json");
        }
        [HttpPost]
        [Route("listgamesforuser")]
        public async Task<ActionResult<List<Game>>> GetGames([FromBody]string forUserParam)
        {
            if (_context.Games == null)
            {
                return NotFound();
            }
            var gamesWithState = _context.Games.Where(s => s.Finished == false && s.User == forUserParam).ToList();

            if (gamesWithState.Count == 0)
            {
                return NotFound();
            }
            var jsonString = JsonSerializer.Serialize(gamesWithState);
            return Content(jsonString, "application/json");
        }
        /*[HttpGet]
        [Route("findgame")]
        // POST: api/players/findplayer
        public async Task<ActionResult<Game>> Findplayer(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            //            player.Team = _context.Teams.Find(player.TeamId);

            // Ok function automatically serializes player object to JSON
            //return Ok(player);
            var jsonString = JsonSerializer.Serialize(game);
            return Content(jsonString, "application/json");
        }*/

        // POST: Games - Accept JSON to add a list of players
        [HttpPost]
        [Route("addgame")]
        public async Task<IActionResult> AddGame([FromBody] Game game)
        {
            // The[FromBody] List<Team> parameter tells ASP.NET Core to automatically deserialize the JSON payload into a List < Team > object.
            // the json is automatically converted on receipt to List<Team>, no need to deserialize here
            if (game == null)
            {
                return BadRequest("No game found in the payload.");
            }

            try
            {
                // Add the deserialized teams to the database context
                game.Id = _context.Games.Count() + 1;
                _context.Games.Add(game);
                
                // Save changes asynchronously
                await _context.SaveChangesAsync();

                //return Ok($"Game added successfully.");
                //return the game (contains now id)
                var jsonString = JsonSerializer.Serialize(game);
                return Content(jsonString, "application/json");
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
        // POST: Update Game - Add a new game or update the param already is an existing one
        [HttpPost]
        [Route("updategame")]
        public async Task<IActionResult> UpdateGame([FromBody] Game game)
        {
            if (game == null)
            {
                return BadRequest("No game found in the payload.");
            }

            try
            {
                // Check if the exist in the database
                var existingGame = await _context.Games.FindAsync(game.Id);
                if (existingGame != null)
                {
                    // Update the existing game class properties
                    // existingGame.Name = game.Name; // could do one by one 
                    _context.Entry(existingGame).CurrentValues.SetValues(game);//copy all except id
                    _context.Games.Update(existingGame);
                }
                else
                {
                    // Assign a new ID if it's a new game and insert it in the table
                    game.Id = _context.Games.Count() + 1;
                    await _context.Games.AddAsync(game);
                }

                // Save changes asynchronously
                await _context.SaveChangesAsync();

                // Return the updated or added game
                var jsonString = JsonSerializer.Serialize(game);
                return Content(jsonString, "application/json");
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
        
        // POST: Delete Game - delete the given game and remove all scores linked to game
        [HttpPost]
        [Route("deletegame")]
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

                // Check if the game exist in the database
                var existingGame = await _context.Games.FindAsync(game.Id);
                if (existingGame != null)
                {
                    // Update the existing game class properties
                    // existingGame.Name = game.Name; // could do one by one 
                    _context.Games.Remove(existingGame);
                }
                // Save changes asynchronously
                await _context.SaveChangesAsync();

                // Return the updated or added game
                var jsonString = JsonSerializer.Serialize(game);
                return Content(jsonString, "application/json");
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

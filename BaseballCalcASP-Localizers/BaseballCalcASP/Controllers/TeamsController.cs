using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaseballCalcASP.Data;
using BaseballCalcASP.Models;
using BaseballModelsLib.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BaseballCalcASP.Controllers
{
    public class TeamsController : Controller
    {
        private readonly BaseballCalcASPContext _context;

        public TeamsController(BaseballCalcASPContext context)
        {
            _context = context;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
              return _context.Teams != null ? 
                          View(await _context.Teams
                          .Where(t => t.Deleted == false)
                          .ToListAsync()) :
                          Problem("Entity set 'BaseballCalcASPContext.Team'  is null.");
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([Bind("Id,Name")] Team team)
        {
            if (ModelState.IsValid)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: Teams/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Team team)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: Teams/AddPlayer/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddPlayer(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            var plist = _context.Players.Where(p => p.Deleted == false && p.TeamId == null).ToList();
            ViewBag.Players = new SelectList(plist, "Id", "Name");
            if (plist.Count() < 1)
            {
                return RedirectToAction("Create", "Players", new { id = team.Id });
            }
            return View(team);
        }

        // POST: Teams/AddPlayer/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddPlayer(int id, int playerid, [Bind("Id")] Team team)
        {
            if (id != team.Id)
            {
                return NotFound();
            }
            var player = _context.Players.Find(playerid);
            if (player != null)
            {
                player.TeamId = team.Id;
                player.Team = await _context.Teams.FindAsync(id);
                _context.Update(player);
            }
            await _context.SaveChangesAsync();
            foreach (Team t in _context.Teams.Where(t => t.Deleted == false))
            {
                t.TotalPlayers = _context.Players.Where(p => p.TeamId == t.Id && p.Deleted == false).Count();
                _context.Update(t);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Teams/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Teams == null)
            {
                return Problem("Entity set 'BaseballCalcASPContext.Team'  is null.");
            }
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                team.Deleted = true;
                _context.Update(team);
                //_context.Teams.Remove(team);

                foreach (Player speler in _context.Players.Where(p => p.Team.Id == team.Id && p.Deleted == false))
                {
                    speler.TeamId = null;
                    speler.Team = null;
                    _context.Update(speler);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
          return (_context.Teams?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

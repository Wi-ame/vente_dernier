using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using vente_en_ligne.Data;
using vente_en_ligne.Models;

namespace vente_en_ligne.Controllers
{
    public class BansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bans
        public async Task<IActionResult> Index()
        {
              return _context.Bans != null ? 
                          View(await _context.Bans.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Bans'  is null.");
        }

        // GET: Bans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bans == null)
            {
                return NotFound();
            }

            var ban = await _context.Bans
                .FirstOrDefaultAsync(m => m.Id_Ban == id);
            if (ban == null)
            {
                return NotFound();
            }

            return View(ban);
        }
        public async Task<IActionResult> Bans()
        {
            List<Ban> favoriteList = await _context.Bans.ToListAsync();
            List<Proprietaire> proprietaires = new List<Proprietaire>();
            foreach (var favori in favoriteList)
            {
                Proprietaire proprio = new Proprietaire();
                proprio = (from obj in _context.Proprietaires where obj.INterID == favori.IDP select obj).FirstOrDefault();
                proprietaires.Add(proprio);
            }
            return PartialView(proprietaires);
        }
        public async Task<IActionResult> AjouterBan()
        {
            return PartialView("_AjouterBan");
        }

        // GET: Bans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IDP")] Ban ban)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ban);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ban);
        }

        // GET: Bans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bans == null)
            {
                return NotFound();
            }

            var ban = await _context.Bans.FindAsync(id);
            if (ban == null)
            {
                return NotFound();
            }
            return View(ban);
        }

        // POST: Bans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Ban,IDP")] Ban ban)
        {
            if (id != ban.Id_Ban)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ban);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BanExists(ban.Id_Ban))
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
            return View(ban);
        }

        // GET: Bans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bans == null)
            {
                return NotFound();
            }

            var ban = await _context.Bans
                .FirstOrDefaultAsync(m => m.Id_Ban == id);
            if (ban == null)
            {
                return NotFound();
            }

            return View(ban);
        }

        // POST: Bans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bans == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Bans'  is null.");
            }
            var ban = await _context.Bans.FindAsync(id);
            if (ban != null)
            {
                _context.Bans.Remove(ban);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BanExists(int id)
        {
          return (_context.Bans?.Any(e => e.Id_Ban == id)).GetValueOrDefault();
        }
    }
}

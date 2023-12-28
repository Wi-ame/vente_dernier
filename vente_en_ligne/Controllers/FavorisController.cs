using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using vente_en_ligne.Data;
using vente_en_ligne.Models;

namespace vente_en_ligne.Controllers
{
    public class FavorisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FavorisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Favoris
        public async Task<IActionResult> Index()
        {
              return _context.Favorites != null ? 
                          View(await _context.Favorites.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Favorites'  is null.");
        }

        // GET: Favoris/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Favorites == null)
            {
                return NotFound();
            }

            var favori = await _context.Favorites
                .FirstOrDefaultAsync(m => m.Id_fav == id);
            if (favori == null)
            {
                return NotFound();
            }

            return View(favori);
        }
        public async Task<IActionResult> AjouterFavori()
        {
            return PartialView("_AjouterFavori");
        }

        // GET: Favoris/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Favoris/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IDP")] Favori favori)
        {
            if (ModelState.IsValid)
            {
                _context.Add(favori);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(favori);
        }
       
		public async Task<IActionResult> Favorites()
		{
			List<Favori> favoriteList = await _context.Favorites.ToListAsync();
            List<Proprietaire> proprietaires = new List<Proprietaire>();
            foreach(var favori in favoriteList)
            {
                Proprietaire proprio = new Proprietaire();
                proprio = (from obj in _context.Proprietaires where obj.INterID == favori.IDP select obj).FirstOrDefault();
                proprietaires.Add(proprio);
            }
			return PartialView(proprietaires);
		}

		// GET: Favoris/Edit/5
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Favorites == null)
            {
                return NotFound();
            }

            var favori = await _context.Favorites.FindAsync(id);
            if (favori == null)
            {
                return NotFound();
            }
            return View(favori);
        }

        // POST: Favoris/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_fav,IDP")] Favori favori)
        {
            if (id != favori.Id_fav)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(favori);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavoriExists(favori.Id_fav))
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
            return View(favori);
        }

        // GET: Favoris/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Favorites == null)
            {
                return NotFound();
            }

            var favori = await _context.Favorites
                .FirstOrDefaultAsync(m => m.Id_fav == id);
            if (favori == null)
            {
                return NotFound();
            }

            return View(favori);
        }

        // POST: Favoris/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Favorites == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Favorites'  is null.");
            }
            var favori = await _context.Favorites.FindAsync(id);
            if (favori != null)
            {
                _context.Favorites.Remove(favori);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavoriExists(int id)
        {
          return (_context.Favorites?.Any(e => e.Id_fav == id)).GetValueOrDefault();
        }
    }
}

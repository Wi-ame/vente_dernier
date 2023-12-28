using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using vente_en_ligne.Data;
using vente_en_ligne.Models;

namespace vente_en_ligne.Controllers
{
    public class AdminsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AdminDashboard()
        {
            return View();
        }
        // GET: Admins
        public async Task<IActionResult> Index(string cin, string password)
        {
            bool authentificationReussie = AuthentificationReussie(cin, password);
            // Si l'authentification réussit, redirigez vers la vue "Create" du contrôleur "Produit"
            if (authentificationReussie)
            {
                TempData["Username"] = cin;
                // Remplacez "NomDuControleur" par le vrai nom de votre contrôleur "Produit"
                return RedirectToAction("AdminDashboard", "Admins");
            }
            return _context.Admin != null ?
                          View(await _context.Admin.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Proprietaires'  is null.");
        }
        private bool AuthentificationReussie(string cin, string password)
        {
            // Votre logique de recherche dans la base de données
            // Assurez-vous que le mot de passe est stocké de manière sécurisée, par exemple en utilisant le hachage

            // Exemple fictif :
            var proprietaire = _context.Admin
                .FirstOrDefault(p => p.CIN == cin && p.password == password);

            // Retournez true si un propriétaire correspondant est trouvé, sinon retournez false
            return proprietaire != null;
        }
        // GET: Admins/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Admin == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.CIN == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public async Task<IActionResult> Create()
        {
            return PartialView("_Create");
        }
        public async Task<IActionResult> CreateProp()
        {
            return PartialView("_Create");
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CIN,password")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Admin == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CIN,password")] Admin admin)
        {
            if (id != admin.CIN)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.CIN))
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
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Admin == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.CIN == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }
       

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Admin == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Admins'  is null.");
            }
            var admin = await _context.Admin.FindAsync(id);
            if (admin != null)
            {
                _context.Admin.Remove(admin);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(string id)
        {
          return (_context.Admin?.Any(e => e.CIN == id)).GetValueOrDefault();
        }
    }
}

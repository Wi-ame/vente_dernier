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
    public class PaniersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaniersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Paniers
        public async Task<IActionResult> Index()
        {
            return _context.Paniers != null ?
                        View(await _context.Paniers.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Paniers'  is null.");
        }

        // GET: Paniers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Paniers == null)
            {
                return NotFound();
            }

            var panier = await _context.Paniers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (panier == null)
            {
                return NotFound();
            }

            return View(panier);
        }

        [HttpPost]
        public IActionResult AjouterAuPanier(int idProduit)
        {
            var produit = _context.Produits.Find(idProduit);

            if (produit == null)
            {
                return NotFound();
            }

            // Vérifiez si le produit est déjà dans le panier
            var panierItem = _context.Paniers.FirstOrDefault(item => item.IDPro == idProduit);

            if (panierItem != null)
            {
                // Le produit est déjà dans le panier, mettez à jour la quantité
                if (produit.stock >= panierItem.Quantité + 1)
                {
                    panierItem.Quantité++;
                    panierItem.Total = panierItem.Quantité * produit.prix;
                }
                else
                {
                    // Affichez un message d'erreur si la quantité dépasse le stock
                    TempData["ErrorMessage"] = "La quantité demandée dépasse le stock disponible.";
                }
            }
            else
            {
                // Le produit n'est pas dans le panier, ajoutez-le
                var nouveauPanierItem = new Panier
                {
                    IDPro = idProduit,
                    Quantité = 1,
                    Total = produit.prix
                };

                if (produit.stock >= nouveauPanierItem.Quantité)
                {
                    _context.Add(nouveauPanierItem);
                }
                else
                {
                    // Affichez un message d'erreur si la quantité dépasse le stock
                    TempData["ErrorMessage"] = "La quantité demandée dépasse le stock disponible.";
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }



        // GET: Paniers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Paniers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IDPro,Quantité,Total")] Panier panier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(panier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(panier);
        }

        // GET: Paniers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Paniers == null)
            {
                return NotFound();
            }

            var panier = await _context.Paniers.FindAsync(id);
            if (panier == null)
            {
                return NotFound();
            }
            return View(panier);
        }

        // POST: Paniers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IDPro,Quantité,Total")] Panier panier)
        {
            if (id != panier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(panier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PanierExists(panier.Id))
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
            return View(panier);
        }

        // GET: Paniers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Paniers == null)
            {
                return NotFound();
            }

            var panier = await _context.Paniers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (panier == null)
            {
                return NotFound();
            }

            return View(panier);
        }

        // POST: Paniers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Paniers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Paniers'  is null.");
            }
            var panier = await _context.Paniers.FindAsync(id);
            if (panier != null)
            {
                _context.Paniers.Remove(panier);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PanierExists(int id)
        {
            return (_context.Paniers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using Stripe;
using vente_en_ligne.Data;
using vente_en_ligne.Models;

namespace vente_en_ligne.Controllers
{
    public class UtilisateursController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly StripeSettings _stripesettings;

        public UtilisateursController(ApplicationDbContext context, IOptions<StripeSettings> stripesettings)
        {
            _context = context;
            _stripesettings = stripesettings.Value;
            StripeConfiguration.ApiKey = _stripesettings.SecretKey;
        }
        // GET: Utilisateurs
        public async Task<IActionResult> Index()
        {
            return _context.Utilisateurs != null ?
                        View(await _context.Utilisateurs.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Utilisateurs'  is null.");
        }
       
        // GET: Utilisateurs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Utilisateurs == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(m => m.ID == id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            return View(utilisateur);
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Authent()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Authent(string email)
        {
            // Vérifier si l'e-mail existe dans la table Utilisateurs
            var utilisateur = _context.Utilisateurs.SingleOrDefault(u => u.Email == email);
            var items = _context.Paniers.ToList();

            // Calculez la somme totale
            double totalAmount = CalculateTotalAmount(items);
            ViewBag.TotalAmount = totalAmount;
            long total = Convert.ToInt64(totalAmount);
            if (utilisateur != null)
            {
                // L'e-mail existe, créer une session de paiement avec Stripe
                StripeConfiguration.ApiKey = "sk_test_51OQiReEh5A0vvIjDBfwtWg77g2JxSXcGyXel8lT9H9HY60vDAQBgx11ReTvb7S7zkFih4BJbRHJSCGv1XaCTjsPX00gW2KOxYw";

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Total",
                        },
                        UnitAmount = total*100, // Replace with your fixed amount in cents (e.g., $50.00)
                    },
                    Quantity = 1,
                },
            },
                    Mode = "payment",
                    SuccessUrl = Url.Action("Index", "Home", null, Request.Scheme),
                    CancelUrl = Url.Action("Error", "Home", null, Request.Scheme),
                };

                var service = new SessionService();
                var session = service.Create(options);

                // Rediriger vers la page de paiement Stripe
                return Redirect(session.Url);
            }
            else
            {
                // L'e-mail n'existe pas, afficher un message d'erreur
                ViewData["ErrorMessage"] = "This Email doesn't exist.";
                return View();
            }
        }

        public double CalculateTotalAmount(List<Panier> items)
        {
            double totalAmount = 0;

            foreach (var item in items)
            {
                totalAmount += item.Total;
            }

            return totalAmount;
        }


        // POST: Utilisateurs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID", "Nom, Prenom, Email, Tel")] Utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(utilisateur);
                    await _context.SaveChangesAsync();

                    // Récupérer l'ID généré après l'ajout de l'utilisateur
                    int userId = utilisateur.ID;

                    // Rediriger vers l'action CreatePanierPrinc du contrôleur PanierPrincs
                    return RedirectToAction("Index", "Paniers");

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Une erreur s'est produite lors de l'ajout de l'utilisateur : " + ex.Message);
                }
            }
            return View(utilisateur);
        }

        // GET: Utilisateurs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Utilisateurs == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }
            return View(utilisateur);
        }

        // POST: Utilisateurs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Nom,Prenom,Email,Tel")] Utilisateur utilisateur)
        {
            if (id != utilisateur.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utilisateur);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilisateurExists(utilisateur.ID))
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
            return View(utilisateur);
        }

        // GET: Utilisateurs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Utilisateurs == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(m => m.ID == id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            return View(utilisateur);
        }

        // POST: Utilisateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Utilisateurs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Utilisateurs'  is null.");
            }
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur != null)
            {
                _context.Utilisateurs.Remove(utilisateur);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilisateurExists(int id)
        {
            return (_context.Utilisateurs?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
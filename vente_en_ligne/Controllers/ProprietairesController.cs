using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using vente_en_ligne.Data;
using vente_en_ligne.Models;

namespace vente_en_ligne.Controllers
{
    public class ProprietairesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProprietairesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> DeleteProprio()
        {
            return PartialView("_DeleteProprio");
        }

        public async Task<IActionResult> Rechercher()
        {
            return PartialView("_SearchProprio");
        }
        [HttpGet]
        public async Task<IActionResult> ListProduct(String id)
        {
            List<Produit> list_pr=(from obj in _context.Produits where obj.IDP== id select obj).ToList();
            return PartialView(list_pr);
        }
        public async Task<IActionResult> PropriHome()
        {
            return View();
        }
        public async Task<IActionResult> HistoryProprio()
        {
            return PartialView("_HistoryProprio");
        }
        // GET: Proprietaires
        public async Task<IActionResult> Index(string cin, string password)
        {
            bool authentificationReussie = AuthentificationReussie(cin, password);

            // Si l'authentification réussit, redirigez vers la vue "Create" du contrôleur "Produit"
            if (authentificationReussie)
            {
                TempData["Username"] = cin;
                // Remplacez "NomDuControleur" par le vrai nom de votre contrôleur "Produit"
                return RedirectToAction("PropriHome", "Proprietaires");
            }
            return _context.Proprietaires != null ?
                          View(await _context.Proprietaires.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Proprietaires'  is null.");
        }
      
        private bool AuthentificationReussie(string cin, string password)
        {
            // Votre logique de recherche dans la base de données
            // Assurez-vous que le mot de passe est stocké de manière sécurisée, par exemple en utilisant le hachage

            // Exemple fictif :
            var proprietaire = _context.Proprietaires
                .FirstOrDefault(p => p.INterID == cin && p.password == password);

            // Retournez true si un propriétaire correspondant est trouvé, sinon retournez false
            return proprietaire != null;
        }
        // GET: Proprietaires/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Proprietaires == null)
            {
                return NotFound();
            }

            var proprietaire = await _context.Proprietaires
                .FirstOrDefaultAsync(m => m.INterID == id);
            if (proprietaire == null)
            {
                return NotFound();
            }

            return View(proprietaire);
        }

        // GET: Proprietaires/Create
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> CreateProp()
        {
            return PartialView("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProp([Bind("INterID,NomEntreprise,AdresseEntreprise,Nom,Prenom,Tele,Email,password")] Proprietaire proprietaire)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );

                return BadRequest(new { errors });

            }

            if (ModelState.IsValid)
            {
                _context.Add(proprietaire);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(CreateProp)); // Rediriger vers la même action pour afficher le message
            }

            // Si le modèle n'est pas valide, retourner la vue sans stocker de message
            return View(proprietaire);
        }

        // POST: Proprietaires/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("INterID,NomEntreprise,AdresseEntreprise,Nom,Prenom,Tele,Email,password")] Proprietaire proprietaire)
        {
            if (ModelState.IsValid)
            {
                _context.Add(proprietaire);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(proprietaire);
        }

        // GET: Proprietaires/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proprietaire = await _context.Proprietaires.FindAsync(id);
            if (proprietaire == null)
            {
                return NotFound();
            }

            return View(proprietaire);
        }

        // POST: Proprietaires/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("INterID,NomEntreprise,AdresseEntreprise,Nom,Prenom,Tele,Email,password")] Proprietaire proprietaire)
        {
            if (id != proprietaire.INterID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proprietaire);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ModifyProprio)); // Redirection vers une vue de confirmation
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Gérer les erreurs de concurrence si nécessaire
                    throw;
                }
            }
            // Si le modèle n'est pas valide, retourner la vue de modification avec les erreurs
            return View(proprietaire);
        }
        public async Task<IActionResult> ModifyProprio()
        {
            return PartialView("_ModifyProprio");
        }
        public async Task<IActionResult> ModifyForm()
        {
            return PartialView("_ModifyForm");
        }
        // HomeController.cs
        public IActionResult CheckIfIdExists(string searchId)
        {
            // Logique pour vérifier si l'ID existe dans la base de données
            bool idExists = ProprietaireExists(searchId);

            // Retourner un résultat JSON
            return Json(new { exists = idExists });
        }
        public IActionResult GetOwnerData(string id)
        {
            // Récupérer les données du propriétaire en fonction de l'ID depuis la base de données
            Proprietaire ownerData = GetOwnerDataFromDatabase(id);

            // Retourner les données au format JSON
            return Json(ownerData);
        }

        [HttpPost]
        public IActionResult InsertOwnerData([FromBody] Proprietaire updatedOwnerData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Recherchez l'enregistrement existant dans la base de données
                    var existingOwner = _context.Proprietaires.FirstOrDefault(o => o.INterID == updatedOwnerData.INterID);

                    if (existingOwner != null)
                    {
                        // Mettez à jour les propriétés de l'enregistrement existant avec les nouvelles valeurs
                        existingOwner.Nom = updatedOwnerData.Nom;
                        existingOwner.Prenom = updatedOwnerData.Prenom;
                        existingOwner.Tele = updatedOwnerData.Tele;
                        existingOwner.Email = updatedOwnerData.Email;
                        existingOwner.NomEntreprise = updatedOwnerData.NomEntreprise;
                        existingOwner.AdresseEntreprise = updatedOwnerData.AdresseEntreprise;
                        existingOwner.password = updatedOwnerData.password;

                        // Continuez pour chaque propriété que vous souhaitez mettre à jour

                        // Sauvegardez les modifications dans la base de données
                        _context.SaveChanges();

                        // Retournez un indicateur de réussite
                        return Json(new { success = true, message = "Les modifications ont été enregistrées avec succès." });
                    }
                    else
                    {
                        // L'enregistrement n'a pas été trouvé, retournez une erreur
                        return NotFound(new { success = false, message = "Enregistrement non trouvé dans la base de données." });
                    }
                }
                else
                {
                    // Le modèle n'est pas valide, retournez les erreurs de validation
                    var errors = ModelState.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                    return BadRequest(new { success = false, errors });
                }
            }
            catch (Exception ex)
            {
                // Loguez l'exception ou traitez-la selon vos besoins
                return StatusCode(500, new { success = false, message = "Une erreur s'est produite lors de la sauvegarde des modifications." });
            }
        }

        // Méthode fictive pour récupérer les données du propriétaire à partir de la base de données
        private Proprietaire GetOwnerDataFromDatabase(string id)
        {
            Proprietaire ownerData = new Proprietaire();
            ownerData = (from obj in _context.Proprietaires where obj.INterID == id select obj).FirstOrDefault();

            return ownerData;
        }
        [HttpPost]

        private Proprietaire InsertOwnerDataFromDatabase(Proprietaire newOwnerData)
        {
            _context.Proprietaires.Add(newOwnerData);
            _context.SaveChanges();
            return newOwnerData;
        }

        // GET: Proprietaires/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Proprietaires == null)
            {
                return NotFound();
            }

            var proprietaire = await _context.Proprietaires
                .FirstOrDefaultAsync(m => m.INterID == id);
            if (proprietaire == null)
            {
                return NotFound();
            }

            return View(proprietaire);
        }

        // POST: Proprietaires/Delete/5
        // POST: Proprietaires/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("INterID")] Proprietaire proprietaire)
        {
            try
            {
                if (_context.Proprietaires == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Proprietaires' is null.");
                }

                // Retrieve the proprietaire by ID
                var existingProprietaire = await _context.Proprietaires.FindAsync(proprietaire.INterID);

                if (existingProprietaire == null)
                {
                    return NotFound(); // Return 404 if the proprietaire is not found
                }

                _context.Proprietaires.Remove(existingProprietaire);
                await _context.SaveChangesAsync();

                return View("DeleteProprio");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return Problem($"An error occurred while deleting: {ex.Message}");
            }
        }
        private bool ProprietaireExists(string id)
        {
            return (_context.Proprietaires?.Any(e => e.INterID == id)).GetValueOrDefault();
        }
    }
}
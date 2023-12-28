using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using vente_en_ligne.Data;
using vente_en_ligne.Models;
namespace vente_en_ligne.Controllers
{
    public class ProduitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProduitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Produits
        public async Task<IActionResult> Index()
        {
              return _context.Produits != null ? 
                          View(await _context.Produits.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Produits'  is null.");
        }
        public async Task<IActionResult> ModifyPr()
        {
            return View();
        }
        public async Task<IActionResult> ModifyProduct()
        {
            return PartialView("_ModifyProduct");
        }
        public async Task<IActionResult> ModifyProductForm()
        {
            var categoriesList = _context.Categories
               .Select(c => new SelectListItem { Value = c.CategorieID.ToString(), Text = c.CategorieName })
               .ToList();
            ViewData["CategoriesList"] = categoriesList;

            return PartialView("_ModifyFormProduct");
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Produits == null)
            {
                return NotFound();
            }

            var productDetails = await _context.Produits
                .Where(p => p.IdPr == id)
                .Select(p => new
                {
                    ProductId = p.IdPr,
                    ProductName = p.Name,
                    ProductDescription = p.Description,
                    ProductPrice = p.prix,
                    CategoryName = _context.Categories.FirstOrDefault(c => c.CategorieID == p.IDC).CategorieName,
                    OwnerName = _context.Proprietaires.FirstOrDefault(o => o.INterID == p.IDP).Nom,
                    OwnerFirstName = _context.Proprietaires.FirstOrDefault(o => o.INterID == p.IDP).Prenom,
                    OwnerTelephone = _context.Proprietaires.FirstOrDefault(o => o.INterID == p.IDP).Tele,
                    ProductDateDepot = p.DateDepot,
                    ProductImageData = p.ImageData,
                    ProductStock = p.stock
                    // Ajoutez d'autres colonnes au besoin
                })
                .FirstOrDefaultAsync();

            // Vérifiez si le produit est null après la requête
            if (productDetails == null)
            {
                return NotFound();
            }

            ViewBag.OwnerName = productDetails.OwnerName;
            ViewBag.OwnerTelephone = productDetails.OwnerTelephone;
            ViewBag.CategoryName = productDetails.CategoryName;
            ViewBag.OwnerFirstname = productDetails.OwnerFirstName;

            var produit = new Produit
            {
                IdPr = productDetails.ProductId,
                Name = productDetails.ProductName,
                Description = productDetails.ProductDescription,
                prix = productDetails.ProductPrice,
                DateDepot = productDetails.ProductDateDepot,
                ImageData = productDetails.ProductImageData,
                stock = productDetails.ProductStock
                // Ajoutez d'autres propriétés au besoin
            };

            return View(produit);
        }
        public async Task<IActionResult> History()
        {
            List<Produit> historique = await _context.Produits.ToListAsync();
            return View(historique);
        }
        // GET: Produits/Create
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult DeleteProduct()
        {
            return View();
        }
        public async Task<IActionResult> CreateProduct()

        {
            var categoriesList = _context.Categories
                .Select(c => new SelectListItem { Value = c.CategorieID.ToString(), Text = c.CategorieName })
                .ToList();


            ViewData["CategoriesList"] = categoriesList;

            return PartialView("_CreateProduct");
        }
        public async Task<IActionResult> CreatePr()
        {
            var categoriesList = _context.Categories
                .Select(c => new SelectListItem { Value = c.CategorieID.ToString(), Text = c.CategorieName })
                .ToList();


            ViewData["CategoriesList"] = categoriesList;

            return PartialView("_CreatePr");
        }
        public async Task<IActionResult> RemoveProduct()
        {
            return PartialView("_RemoveProduct");
        }
        public async Task<IActionResult> RemovePr()
        {
            return PartialView("_RemovePr");
        }
        // POST: Produits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Produits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPr,Name,Description,prix,IDC,IDP,DateDepot,ImageFile,stock")] Produit produit)
        {
            try
            {
                Console.WriteLine($"IDP: {produit.IDP}");
                Console.WriteLine($"IDC: {produit.IDC}");

                // Assurez-vous que le champ "Category" a une valeur sélectionnée
                if (produit.IDC > 0)
                {
                    // Gérer l'envoi de l'image
                    if (produit.ImageFile != null && produit.ImageFile.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await produit.ImageFile.CopyToAsync(stream);
                            produit.ImageData = stream.ToArray();
                        }
                        if (produit.ImageData != null)
                        {
                            Console.WriteLine($"ImageData Length: {produit.ImageData.Length} bytes");
                        }
                        else
                        {
                            Console.WriteLine("No image data");
                        }
                    }

                    _context.Add(produit);
                    await _context.SaveChangesAsync();
                    return View("CreateProduct");
                }
                else
                {
                    ModelState.AddModelError("IDC", "Please select a category.");
                }
            }
            catch (Exception ex)
            {
                // Gérer l'exception, par exemple, afficher dans la console de débogage
                Console.WriteLine(ex.Message);
                // Rediriger vers une page d'erreur ou une autre action en cas d'erreur
                return RedirectToAction("Error");
            }

            // Si la validation échoue, assurez-vous de récupérer à nouveau la liste des catégories
            var categoriesList = await _context.Categories
                .Select(c => new SelectListItem { Value = c.CategorieID.ToString(), Text = c.CategorieName })
                .ToListAsync();

            ViewData["CategoriesList"] = categoriesList;

            // Vous pouvez également réinitialiser la liste dans le modèle Produit
            produit.CategoriesList = categoriesList;

            return View(produit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePr([Bind("IdPr,Name,Description,prix,IDC,IDP,DateDepot,ImageFile,stock")] Produit produit)
        {
            try
            {
                Console.WriteLine($"IDP: {produit.IDP}");
                Console.WriteLine($"IDC: {produit.IDC}");

                // Assurez-vous que le champ "Category" a une valeur sélectionnée
                if (produit.IDC > 0)
                {
                    // Gérer l'envoi de l'image
                    if (produit.ImageFile != null && produit.ImageFile.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await produit.ImageFile.CopyToAsync(stream);
                            produit.ImageData = stream.ToArray();
                        }
                        if (produit.ImageData != null)
                        {
                            Console.WriteLine($"ImageData Length: {produit.ImageData.Length} bytes");
                        }
                        else
                        {
                            Console.WriteLine("No image data");
                        }
                    }

                    _context.Add(produit);
                    await _context.SaveChangesAsync();
                    return View("CreatePr");
                }
                else
                {
                    ModelState.AddModelError("IDC", "Please select a category.");
                }
            }
            catch (Exception ex)
            {
                // Gérer l'exception, par exemple, afficher dans la console de débogage
                Console.WriteLine(ex.Message);
                // Rediriger vers une page d'erreur ou une autre action en cas d'erreur
                return RedirectToAction("Error");
            }

            // Si la validation échoue, assurez-vous de récupérer à nouveau la liste des catégories
            var categoriesList = await _context.Categories
                .Select(c => new SelectListItem { Value = c.CategorieID.ToString(), Text = c.CategorieName })
                .ToListAsync();

            ViewData["CategoriesList"] = categoriesList;

            // Vous pouvez également réinitialiser la liste dans le modèle Produit
            produit.CategoriesList = categoriesList;

            return View(produit);
        }
        // GET: Produits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Produits == null)
            {
                return NotFound();
            }

            var produit = await _context.Produits.FindAsync(id);
            if (produit == null)
            {
                return NotFound();
            }

            var categoriesList = _context.Categories
                .Select(c => new SelectListItem { Value = c.CategorieID.ToString(), Text = c.CategorieName })
                .ToList();

            ViewData["CategoriesList"] = categoriesList;

            return View(produit);
        }

        // POST: Produits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,[Bind("IdPr,Name,Description,prix,IDC,ImageFile,stock")] Produit produit)
        {
            id = produit.IdPr;
            Console.WriteLine(id);
            var categoriesList = _context.Categories
                   .Select(c => new SelectListItem { Value = c.CategorieID.ToString(), Text = c.CategorieName })
                   .ToList();
            ViewData["CategoriesList"] = categoriesList;
            Produit product = new Produit();
            product = (from obj in _context.Produits
                      where obj.IdPr == id
                      select obj).FirstOrDefault();
            product.Name = produit.Name;
            product.prix = produit.prix;
            if (produit.IDC>0) {
                product.IDC = produit.IDC;
            }
            product.ImageFile = produit.ImageFile;
            product.stock = produit.stock;
            product.Description=produit.Description;
            await _context.SaveChangesAsync();
            ViewData["IDC"] = new SelectList(_context.Categories, "CategorieID", "CategorieID", produit.IDC);
            ViewData["IDP"] = new SelectList(_context.Proprietaires, "INterID", "INterID", produit.IDP);
            return View("ModifyProduct");
        }
      
        // GET: Produits/Delete/5
        public IActionResult CheckIfIdExists(int searchId)
        {
            // Logique pour vérifier si l'ID existe dans la base de données
            bool idExists = ProduitExists(searchId);

            // Retourner un résultat JSON
            return Json(new { exists = idExists });
        }
        public IActionResult GetProductData(int id)
        {
            // Récupérer les catégories depuis la base de données
            // Récupérer les données du produit en fonction de l'ID depuis la base de données
            Produit productData = GetProductDataFromDatabase(id);

            if (productData == null)
            {
                return NotFound();
            }

            // Convertir les données binaires de l'image en une chaîne Base64
            var imageBase64 = Convert.ToBase64String(productData.ImageData);
            var imageDataUrl = $"data:image/png;base64,{imageBase64}";

            // Récupérer les données liées à des clés étrangères
            var ownerData = _context.Proprietaires.Find(productData.IDP);
            var categoryData = _context.Categories.Find(productData.IDC);
            // Créer un objet anonyme pour renvoyer toutes les données nécessaires
            var result = new
            {
                idPr = productData.IdPr,
                name = productData.Name,
                prix = productData.prix,
                dateDepot = productData.DateDepot,
                stock = productData.stock,
                description = productData.Description,
                iDC = categoryData.CategorieName,
                iDP = ownerData.INterID,
                imageFile = imageDataUrl
            };
            // Retourner toutes les données au format JSON
            return Json(result);
        }
        // Méthode fictive pour récupérer les données du propriétaire à partir de la base de données
        private Produit GetProductDataFromDatabase(int id)
        {
            Produit ProduiData = new Produit();
            ProduiData = (from obj in _context.Produits where obj.IdPr == id select obj).FirstOrDefault();
            return ProduiData;
        }
        [HttpPost]
        public async Task<IActionResult> InsertProductData([FromForm] Produit updatedProduitData)
        {
            Console.WriteLine("Salam");
            try
            {

                if (ModelState.IsValid)
                {
                    Console.WriteLine($"IDP: {updatedProduitData.IDP}");

                    // Recherchez l'enregistrement existant dans la base de données
                    var existingProduct = _context.Produits
                        .FirstOrDefault(o => o.IdPr == updatedProduitData.IdPr);

                    if (updatedProduitData.ImageFile != null && updatedProduitData.ImageFile.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await updatedProduitData.ImageFile.CopyToAsync(stream);
                            existingProduct.ImageData = stream.ToArray();
                        }
                    }

                    if (existingProduct != null)
                    {
                        // Mettez à jour les propriétés de l'enregistrement existant avec les nouvelles valeurs
                        existingProduct.Name = updatedProduitData.Name;
                        existingProduct.prix = updatedProduitData.prix;
                        existingProduct.DateDepot = updatedProduitData.DateDepot;
                        existingProduct.stock = updatedProduitData.stock;
                        existingProduct.IDC = updatedProduitData.IDC;
                        existingProduct.IdPr = updatedProduitData.IdPr;
                        existingProduct.IDP = updatedProduitData.IDP;
                        existingProduct.ImageFile = updatedProduitData.ImageFile;
                        existingProduct.Description = updatedProduitData.Description;

                        return View(existingProduct);

                    }
                    else
                    {
                        // L'enregistrement n'a pas été trouvé, retournez une erreur
                        return NotFound(new { success = false, message = "Enregistrement non trouvé dans la base de données." });
                    }
                }
                else
                {
                    Console.WriteLine("ModelState is not valid. Errors:");

                    foreach (var entry in ModelState.Values)
                    {
                        foreach (var error in entry.Errors)
                        {
                            Console.WriteLine(error.ErrorMessage);
                        }
                    }

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

                Console.WriteLine($"An error occurred: {ex.Message}");

                // Loguez l'exception ou traitez-la selon vos besoins
                return StatusCode(500, new { success = false, message = "Une erreur s'est produite lors de la sauvegarde des modifications." });
            }
        }

        [HttpPost]
        private Produit InsertProductDataFromDatabase(Produit newProduitData)
        {

            _context.Produits.Add(newProduitData);
            _context.SaveChanges();
            return newProduitData;
        }
        // POST: Produits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        /*public async Task<IActionResult> Edit(int id, [Bind("IdPr,Name,Description,prix,IDC,IDP,DateDepot,ImageFile,stock")] Produit produit)
        {
            if (id != produit.IdPr)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (produit.ImageFile != null && produit.ImageFile.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await produit.ImageFile.CopyToAsync(stream);
                            produit.ImageData = stream.ToArray();
                        }
                        if (produit.ImageData != null)
                        {
                            Console.WriteLine($"ImageData Length: {produit.ImageData.Length} bytes");
                        }
                        else
                        {
                            Console.WriteLine("No image data");
                        }
                    }
                    _context.Update(produit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProduitExists(produit.IdPr))
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
            return View(produit);
        }
        // GET: Produits/Delete/5
        public IActionResult CheckIfIdExists(int searchId)
        {
            // Logique pour vérifier si l'ID existe dans la base de données
            bool idExists = ProduitExists(searchId);

            // Retourner un résultat JSON
            return Json(new { exists = idExists });
        }
        public IActionResult GetProductData(int id)
        {
            // Récupérer les catégories depuis la base de données
            // Récupérer les données du produit en fonction de l'ID depuis la base de données
            Produit productData = GetProductDataFromDatabase(id);

            if (productData == null)
            {
                return NotFound();
            }

            // Convertir les données binaires de l'image en une chaîne Base64
            var imageBase64 = Convert.ToBase64String(productData.ImageData);
            var imageDataUrl = $"data:image/png;base64,{imageBase64}";

            // Récupérer les données liées à des clés étrangères
            var ownerData = _context.Proprietaires.Find(productData.IDP);
            var categoryData = _context.Categories.Find(productData.IDC);
            Console.WriteLine(productData.IDP);
            if (ownerData == null || categoryData == null)
            {
                return NotFound(); // Ou retournez une réponse appropriée selon vos besoins
            }
            // Créer un objet anonyme pour renvoyer toutes les données nécessaires
            var result = new
            {
                IDP = productData.IDP,
                IdPr = productData.IdPr,
                Name = productData.Name,
                prix = productData.prix,
                DateDepot = productData.DateDepot,
                stock = productData.stock,
                Description = productData.Description,
                IDC = categoryData.CategorieName,
                ImageFile = imageDataUrl,
            };
            Console.WriteLine(result.IDP);
            // Retourner toutes les données au format JSON
            return Json(result);
        }
        // Méthode fictive pour récupérer les données du propriétaire à partir de la base de données
        private Produit GetProductDataFromDatabase(int id)
        {
            Produit ProduiData = new Produit();
            ProduiData = (from obj in _context.Produits where obj.IdPr == id select obj).FirstOrDefault();
            return ProduiData;
        }*/

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Produits == null)
            {
                return NotFound();
            }

            var produit = await _context.Produits
                .FirstOrDefaultAsync(m => m.IdPr == id);
            if (produit == null)
            {
                return NotFound();
            }

            return View(produit);
        }

        // POST: Produits/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("IdPr")] Produit produit)
        {
            try
            {
                if (_context.Produits == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Produit' is null.");
                }

                // Retrieve the proprietaire by ID
                var existingProduct = await _context.Produits.FindAsync(produit.IdPr);

                if (existingProduct == null)
                {
                    return NotFound(); // Return 404 if the proprietaire is not found
                }

                _context.Produits.Remove(existingProduct);
                await _context.SaveChangesAsync();

                return View("RemoveProduct");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return Problem($"An error occurred while deleting: {ex.Message}");
            }
        }
        [HttpPost, ActionName("DeleteProd")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProd([Bind("IdPr")] Produit produit)
        {
            try
            {
                if (_context.Produits == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Produit' is null.");
                }

                // Retrieve the proprietaire by ID
                var existingProduct = await _context.Produits.FindAsync(produit.IdPr);

                if (existingProduct == null)
                {
                    return NotFound(); // Return 404 if the proprietaire is not found
                }

                _context.Produits.Remove(existingProduct);
                await _context.SaveChangesAsync();

                return View("DeleteProd");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return Problem($"An error occurred while deleting: {ex.Message}");
            }
        }

        private bool ProduitExists(int id)
        {
          return (_context.Produits?.Any(e => e.IdPr == id)).GetValueOrDefault();
        }
    }

}

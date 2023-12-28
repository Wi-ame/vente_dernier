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
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return _context.Categories != null ?
                        View(await _context.Categories.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategorieID == id);
            if (categories == null)
            {
                return NotFound();
            }

            return View(categories);
        }
        public IActionResult GetCategoryIdByName(string categoryName)
        {
            var categoryId = _context.Categories
                .Where(c => c.CategorieName == categoryName)
                .Select(c => c.CategorieID)
                .FirstOrDefault();

            return Json(new { categoryId });
        }
        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> CreateCat()
        {
            return PartialView("_CreateCat");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCat([Bind("CategorieID,CategorieName")] Categories categorie)
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
                _context.Add(categorie);
                await _context.SaveChangesAsync();


                return View(); // Rediriger vers la même action pour afficher le message
            }

            // Si le modèle n'est pas valide, retourner la vue sans stocker de message
            return View(categorie);
        }
        public async Task<IActionResult> RemoveCat()
        {
            return PartialView("_RemoveCat");
        }
        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategorieID,CategorieName")] Categories categories)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categories);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categories);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories.FindAsync(id);
            if (categories == null)
            {
                return NotFound();
            }
            return View(categories);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategorieID,CategorieName")] Categories categories)
        {
            if (id != categories.CategorieID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categories);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriesExists(categories.CategorieID))
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
            return View(categories);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategorieID == id);
            if (categories == null)
            {
                return NotFound();
            }

            return View(categories);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("CategorieName")] Categories categories)
        {
            try
            {
                if (_context.Proprietaires == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.Proprietaires' is null.");
                }

                // Retrieve the proprietaire by ID
                Categories existingCategory = new Categories();
                existingCategory= (from obj in _context.Categories
                                   where obj.CategorieName == categories.CategorieName
                                   select obj).FirstOrDefault();

                _context.Categories.Remove(existingCategory);
                await _context.SaveChangesAsync();
                return View("RemoveCat");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return Problem($"An error occurred while deleting: {ex.Message}");
            }
        }

        private bool CategoriesExists(int id)
        {
            return (_context.Categories?.Any(e => e.CategorieID == id)).GetValueOrDefault();
        }
    }
}
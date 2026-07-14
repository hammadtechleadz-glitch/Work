using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project02.Data;
using project02.Models;

namespace project02.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _manager;

        public CategoryController(ApplicationDbContext context, UserManager<IdentityUser> manager)
        {
            _context = context;
            _manager = manager;
        }

        public IActionResult Index()
        {
            var currentUserId = _manager.GetUserId(User);
            var allCategories = _context.Categories.Where(x => x.UserId == currentUserId).ToList();
            return View(allCategories);
        }

        //Get /Create]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category categorymodel)
        {
            Console.WriteLine("POST CREATE HIT");
            var currentUserId = _manager.GetUserId(User);
            categorymodel.UserId = currentUserId;
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("Notes");
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                return View(categorymodel);
            }

            _context.Categories.Add(categorymodel);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        //Get /Edit
        [Authorize]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) { return NotFound(); }
            var currentUserId = _manager.GetUserId(User);
            if (category.UserId != currentUserId)
            {
                return Unauthorized();

            }

            return View(category);
        }

        //Post /Edit
        [HttpPost]
        public IActionResult Edit(Category categorymodel)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("Notes");
            if (!ModelState.IsValid)
            {
                return View(categorymodel);
            }
            var currentUserId = _manager.GetUserId(User);
            var categoryFromDb = _context.Categories.Find(categorymodel.id);
            if (categoryFromDb == null) return NotFound();
            if (categoryFromDb.UserId != currentUserId) return Unauthorized();

            categoryFromDb.category = categorymodel.category;
            _context.Categories.Update(categoryFromDb);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        //Get /Delete
        [Authorize]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var Category = _context.Categories.Find(id);
            if (Category == null)
            {
                return NotFound();
            }
            var currentUserId = _manager.GetUserId(User);
            if (Category.UserId != currentUserId)
            {
                return Unauthorized();

            }
            return View(Category);
        }

        //Post /Delete
        [HttpPost]
        public IActionResult Delete(Category categorymodel)
        {
            var currentUserId = _manager.GetUserId(User);
            var categoryFromDb = _context.Categories
        .Include(c => c.Notes)
        .FirstOrDefault(c => c.id == categorymodel.id);//we have used it beause we nned to find not only the categoires but aslo all their rleadted notes so that ther cateoes id we can do it null using llop
            if (categoryFromDb == null) return NotFound();
            if (categoryFromDb.UserId != currentUserId)
            {
                return Unauthorized();

            }
            foreach (var note in categoryFromDb.Notes)
            {
                note.categoryId = null;
            }

            _context.Categories.Remove(categoryFromDb);
            _context.SaveChanges();

            return RedirectToAction("Index");


        }
    }
}

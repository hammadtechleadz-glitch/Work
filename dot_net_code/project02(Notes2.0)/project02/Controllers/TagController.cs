using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project02.Data;
using project02.Models;

namespace project02.Controllers
{
    public class TagController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _manager;

        public TagController(ApplicationDbContext context, UserManager<IdentityUser> manager)
        {
            _context = context;
            _manager = manager;
        }
        public IActionResult Index()
        {
            var currentUserId = _manager.GetUserId(User);
            var allTags = _context.Tag.Where(x => x.UserId == currentUserId).ToList();
            return View(allTags);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Tag categorymodel)
        {
            Console.WriteLine("POST CREATE HIT");
            var currentUserId = _manager.GetUserId(User);
            categorymodel.UserId = currentUserId;// we are setting cutrent user id here
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                return View(categorymodel);
            }

            _context.Tag.Add(categorymodel);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        //Get /Edit
        [Authorize]
        public IActionResult Edit(int id)
        {
            var category = _context.Tag.Find(id);
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
        public IActionResult Edit(Tag categorymodel)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            if (!ModelState.IsValid)
            {
                return View(categorymodel);
            }
            var currentUserId = _manager.GetUserId(User);
            var categoryFromDb = _context.Tag.Find(categorymodel.id);
            if (categoryFromDb == null) return NotFound();
            if (categoryFromDb.UserId != currentUserId) return Unauthorized();

            categoryFromDb.name = categorymodel.name;
            _context.Tag.Update(categoryFromDb);
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

            var Category = _context.Tag.Find(id);
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
        public IActionResult Delete(Tag categorymodel)
        {
            var currentUserId = _manager.GetUserId(User);
            var categoryFromDb = _context.Tag
        .FirstOrDefault(c => c.id == categorymodel.id);//find te tag whose id mathces the id send by the dlete form
            if (categoryFromDb == null) return NotFound();
            if (categoryFromDb.UserId != currentUserId)
            {
                return Unauthorized();

            }
            var noteTagRows = _context.NoteTag
        .Where(nt => nt.TagId == categoryFromDb.id)
        .ToList();

            _context.NoteTag.RemoveRange(noteTagRows);
            _context.Tag.Remove(categoryFromDb);
            _context.SaveChanges();

            return RedirectToAction("Index");


        }
    }
}

    
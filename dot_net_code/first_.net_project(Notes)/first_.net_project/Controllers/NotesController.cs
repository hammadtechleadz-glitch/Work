using first_.net_project.Data;
using first_.net_project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace first_.net_project.Controllers
{
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }// this is dependency injection

        // GET: /Notes
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var notes = await _context.Notes.ToListAsync();
            return View(notes);
        }

        // GET: /Notes/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }
        // GET: /Notes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note == null)
                return NotFound();

            return View(note);
        }

        // POST: /Notes/Edit/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(Note note)
        {
            if (!ModelState.IsValid)
            {
                return View(note);
            }
            _context.Notes.Update(note);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Notes/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note == null)
                return NotFound();

            return View(note);
        }
        // POST: /Notes/Delete/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Notes/Create, this is just endpoint, without and interface, so from calls this when we hit submit
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Note note)
        {
            if (!ModelState.IsValid)
            {
                return View(note);
            }
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project02.Data;
using project02.Models;

namespace project02.Controllers
{
    public class NotesController: Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _manager;

        public NotesController(ApplicationDbContext context, UserManager<IdentityUser> manager)
        {
            _context = context;
            _manager= manager;
        }

        
        [Authorize]//Read
        public IActionResult Index(int? categoryId ,string? SearchText, int[] selectedTagIds)
        {
            var currentUserId = _manager.GetUserId(User);
            

            var allCategories = _context.Categories.Where(x => x.UserId == currentUserId).ToList();// to get al the categories so that user sse his and selct one before saving the note
            ViewBag.Categories = allCategories;// for the dwoand down
            var allTags = _context.Tag.Where(x => x.UserId == currentUserId).ToList();
            ViewBag.Tags = allTags;

            // Pass the selected array back to pre-check the checkboxes on reload
            ViewBag.SelectedTagIds = selectedTagIds ?? new int[0];// if ht eleft side of the right side is null, use the right side, crate an array with zero slto beause arrya are fized size
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedSearchText = SearchText;

            var allNotes = _context.Notes
             .Include(x => x.category)
             .Include(x => x.NoteTag)         
                 .ThenInclude(nt => nt.tag)   
             .Where(x => x.UserId == currentUserId);//loadd all categoires with notes and all tha tag objects that are ascoited with tah notes ofnthe cutrrent user
            if (categoryId.HasValue){
                 allNotes = allNotes.Where(x => x.categoryId == categoryId);// thi overried the prev data so retunr [] if inalcid id given explicitly inhe get poararmter
            }

            if (selectedTagIds != null && selectedTagIds.Length > 0)
            {
                // This returns notes that have AT LEAST ONE of the selected tags (OR filter)
                allNotes = allNotes.Where(x => x.NoteTag.Any(nt => selectedTagIds.Contains(nt.TagId)));
            }

            if (!string.IsNullOrWhiteSpace(SearchText)) {
                allNotes = allNotes.Where(x =>
            x.title.Contains(SearchText) || x.content.Contains(SearchText));
            }
            return View(allNotes.ToList());
        }

        //Get /Create
        [Authorize]
        public IActionResult Create()
        {
            var currentUserId = _manager.GetUserId(User);
            var allCategories = _context.Categories.Where(x => x.UserId == currentUserId).ToList();// to get al the categories so that user sse his and selct one before saving the note
            ViewBag.Categories = allCategories;
            ViewBag.Tags = _context.Tag.Where(x => x.UserId == currentUserId).ToList();
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(Notes note, int[] selectedTagIds)
        {
            Console.WriteLine("POST CREATE HIT");
            var currentUserId = _manager.GetUserId(User);
            note.UserId = currentUserId;
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("category");
            ModelState.Remove("NoteTag");
            if (!ModelState.IsValid) {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                var allCategories = _context.Categories.Where(x => x.UserId == currentUserId).ToList();// to get al the categories so that user sse his and selct one before saving the note
                ViewBag.Categories = allCategories;
                ViewBag.Tags = _context.Tag.Where(x => x.UserId == currentUserId).ToList();

                return View(note);
            }
            
            _context.Notes.Add(note);
            _context.SaveChanges();

            if (selectedTagIds != null && selectedTagIds.Length > 0)
            {
                foreach (var tagId in selectedTagIds)//Lopping and adding by manually craete notetag obj and saving them
                {
                    var noteTag = new NoteTag
                    {
                        NoteId = note.id, // Linked to our newly generated note ID
                        TagId = tagId
                    };
                    _context.NoteTag.Add(noteTag);
                }
                _context.SaveChanges(); // Save the relations
            }


            return RedirectToAction("Index");
        }
        // GET: /Notes/Edit/5
        [Authorize]
        public IActionResult Edit(int id)
        {
            // 1. Eagerly load the NoteTag entries so we can find existing tags
            var Notes = _context.Notes
                .Include(n => n.NoteTag)
                .FirstOrDefault(n => n.id == id);

            if (Notes == null) { return NotFound(); }

            var currentUserId = _manager.GetUserId(User);
            if (Notes.UserId != currentUserId)
            {
                return Unauthorized();
            }

            // 2. Load the dropdown lists
            var allCategories = _context.Categories.Where(x => x.UserId == currentUserId).ToList();
            ViewBag.Categories = allCategories;
            ViewBag.Tags = _context.Tag.Where(x => x.UserId == currentUserId).ToList();

            // 3. CRITICAL: Extract the linked tag IDs into an array for your JavaScript to read
            ViewBag.CurrentTagIds = Notes.NoteTag.Select(nt => nt.TagId).ToArray();

            return View(Notes);
        }

        // POST: /Notes/Edit
        [HttpPost]
        [Authorize]
        public IActionResult Edit(Notes note, int[] selectedTagIds) // <-- Accept the tag IDs from the hidden inputs
        {
            var currentUserId = _manager.GetUserId(User);

            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("category");
            ModelState.Remove("NoteTag");

            if (!ModelState.IsValid)
            {
                // Reload your view bags if something fails validation
                ViewBag.Categories = _context.Categories.Where(x => x.UserId == currentUserId).ToList();
                ViewBag.Tags = _context.Tag.Where(x => x.UserId == currentUserId).ToList();
                ViewBag.CurrentTagIds = selectedTagIds; // Keep current selections visible
                return View(note);
            }

            var noteFromDb = _context.Notes.Find(note.id);
            if (noteFromDb == null) return NotFound();
            if (noteFromDb.UserId != currentUserId) return Unauthorized();

            // 1. Update the core note text properties
            noteFromDb.title = note.title;
            noteFromDb.content = note.content;
            noteFromDb.categoryId = note.categoryId;
            _context.Notes.Update(noteFromDb);

            // 2. Clear out the previous relationship entries to prevent duplicates
            var oldTags = _context.NoteTag.Where(nt => nt.NoteId == note.id).ToList();
            _context.NoteTag.RemoveRange(oldTags);
            _context.SaveChanges();

            // 3. Rewrite only the tags that are currently active on the user's screen
            if (selectedTagIds != null && selectedTagIds.Length > 0)
            {
                foreach (var tagId in selectedTagIds)
                {
                    var noteTag = new NoteTag
                    {
                        NoteId = note.id,
                        TagId = tagId
                    };
                    _context.NoteTag.Add(noteTag);
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        //Get /Delete
        [Authorize]
        public IActionResult Delete(int id)
        {
            if (id <= 0) {
                return BadRequest();
            }

            var Notes = _context.Notes.Find(id);
            if (Notes == null) {
                return NotFound();
            }
            var currentUserId = _manager.GetUserId(User);
            if (Notes.UserId != currentUserId)
            {
                return Unauthorized();

            }
            return View(Notes);
        }
            
        //Post /Delete
        [HttpPost]
        [Authorize]
        public IActionResult Delete(Notes note)
        {
            var currentUserId = _manager.GetUserId(User);
            var noteFromDb = _context.Notes.Find(note.id);//fetch it from db because from will not send it, so we have noteid, we use it to get the note object from db, eentogu we have note obj here but it is mdoel binded that has form data ndsue rid null basue ther isnno user id in from so fethc it from db to get actaul note obj tha will has userid
            if (noteFromDb == null) return NotFound();
            if (noteFromDb.UserId != currentUserId)
            {
                return Unauthorized();

            }
            var associatedTags = _context.NoteTag.Where(nt => nt.NoteId == note.id).ToList();// it is a lsit on tags, we nne taht in the crationa s well, but we have only ids so we do taht mually caretioan of noretag
            if (associatedTags.Any())
            {
                _context.NoteTag.RemoveRange(associatedTags);//remove rnage will not nned a llop toremove all 
            }

            _context.Notes.Remove(noteFromDb);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

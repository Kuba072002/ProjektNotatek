using Ganss.Xss;
using ProjektNotatek.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektNotatek.Models;
using System.Security.Claims;
using ProjektNotatek.Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations;
//using ProjektNotatek.Models.Notes;

namespace ProjektNotatek.Controllers {
    [Authorize]
    public class NotesController : Controller {

        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotesController(DataContext dataContext, UserManager<ApplicationUser> userManager) {
            _dataContext = dataContext;
            _userManager = userManager;
        }

        public async Task<ActionResult> IndexAsync() {
            var username = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var user = await _userManager.FindByNameAsync(username);
            NoteHomeModel model = new NoteHomeModel();
            if (user != null) {
                model.PublicNotes = await GetPublicNotes(username);
                model.YourNotes = await _dataContext.Notes.Where(n => n.Username.Equals(username)).ToListAsync();
                model.SharedNotes = await GetSharedNotes(username);
            }
            return View(model);
        }

        public async Task<ActionResult> Get(int id) {
            var note = await _dataContext.Notes.FirstOrDefaultAsync(x => x.Id == id);
            var username = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var user = await _userManager.FindByNameAsync(username);
            if (note != null && user != null) {
                if (CheckIfUserHaveAccessToNoteAsync(note, username)) {
                    var model = new GetNoteModel {
                        Note = note
                    };
                    return View("Get", model);
                }
                return Redirect("~/Identity/AccessDenied");
            }
            return Redirect("~/Identity/AccessDenied");
        }

        [HttpGet]
        public ActionResult Create() {
            var model = new CreateNoteModel();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateNoteModel model) {
            if (ModelState.IsValid) {
                if (model.Option == null)
                    model.Option = "Normal";
                var username = HttpContext.User.FindFirstValue(ClaimTypes.Name);
                var ifExist = await _dataContext.Notes
                    .Where(n => n.Username.Equals(username) && n.Title.Equals(model.Title))
                    .FirstOrDefaultAsync() == null ? false :true;
                if (!ifExist) {
                    var sanitizer = new HtmlSanitizer();
                    bool isPublic = model.Option.Equals("Public") ? true : false;
                    Note note = new Note {
                        //UserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
                        Username = username,
                        Content = sanitizer.Sanitize(model.NoteContent),
                        Title = model.Title,
                        IsPublic = isPublic                       
                    };
                    if (model.Option != "Encrypted") {
                        _dataContext.Notes.Add(note);
                        await _dataContext.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    //if(!model.Password.Equals("")) {
                        var s = CheckPasswordQuality(model.Password);
                        if (!s.Equals("ok")) {
                            ModelState.AddModelError("Createnote", s);
                            return View(model);
                        }
                        note.Password= model.Password;
                    //}
                    var encryptedNote = CreateEncryptedNote(note);
                    _dataContext.Notes.Add(encryptedNote);
                    await _dataContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("Createnote", "You already have note with that title");
                return View(model);
            }
            return View(model);
        }

        public async Task<ActionResult> Share(int id) {
            var note = await _dataContext.Notes.FirstOrDefaultAsync(x => x.Id == id);
            var username = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            if (note != null && username != null) {
                if (username.Equals(note.Username)) {
                    var model = new ShareNoteModel();
                    model.Usernames = await _dataContext.NotesShared.Where(ns=>ns.NoteId == note.Id).Select(ns => ns.Username).ToListAsync();

                    return View("Share", model);
                }
            }
            return Redirect("~/Identity/AccessDenied");
        }
        [HttpPost]
        public async Task<ActionResult> Share(ShareNoteModel model, int id) {
            if (ModelState.IsValid) {
                var note = await _dataContext.Notes.FirstOrDefaultAsync(x => x.Id == id);
                var authorName = HttpContext.User.FindFirstValue(ClaimTypes.Name);
                if (model.Username.Equals(authorName)) {
                    ModelState.AddModelError("Share", "Cannot share yourself");
                    return View(model);
                }
                if (note != null && authorName != null) {
                    if (authorName.Equals(note.Username)) {
                        var user = await _userManager.FindByNameAsync(model.Username);
                        var userNames = await _dataContext.NotesShared
                                .Where(ns => ns.NoteId == note.Id).Select(ns => ns.Username).ToListAsync();
                        if (user != null || userNames.Contains(authorName)) {
                           _dataContext.NotesShared.Add(new NoteShared {
                                Username = model.Username,
                                NoteId = note.Id,
                                Note = note
                            });
                            await _dataContext.SaveChangesAsync();
                            return View("Share", new ShareNoteModel {
                                Username = string.Empty,
                                Usernames = userNames
                            });//await _dataContext.NotesShared.Where(ns => ns.NoteId == note.Id).Select(ns => ns.Username).ToListAsync();
                        }
                        ModelState.AddModelError("Share", "Problem user not exist or already shared");
                        return View(model);
                    }
                }
            }
            return Redirect("~/Identity/AccessDenied");
        }

        private async Task<ActionResult> SetPublicAsync(int id) {
            var note = await _dataContext.Notes.FirstOrDefaultAsync(x => x.Id == id);
            if (HttpContext.User.FindFirstValue(ClaimTypes.Name).Equals(note.Username)) {
                note.IsPublic = true;
                await _dataContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return Redirect("~/Identity/AccessDenied");
        }

        private async Task<List<Note>> GetPublicNotes(string username) {
            var result = await _dataContext.Notes.Where(n => n.IsPublic == true && !n.Username.Equals(username)).ToListAsync();
            return result;
        }

        private async Task<List<Note>> GetSharedNotes(string username) {
            var notes = await _dataContext.NotesShared
                    .Where(s => s.Username.Equals(username))
                    .Select(ns => ns.Note).ToListAsync();
            return notes;//await _dataContext.Notes.Where(n => notesId.Contains(n.Id)).ToListAsync();
        }

        private bool CheckIfUserHaveAccessToNoteAsync(Note note,string username) {
            if (note.IsPublic || username.Equals(note.Username)) {
                return true;
            }
            var result = _dataContext.NotesShared
                    .FirstOrDefault(s => (s.Username.Equals(username)) && s.NoteId.Equals(note.Id));
            if(result != null) {
                return true;
            }
            return false;
        }

        private Note CreateEncryptedNote(Note note) {
            note.IsPublic = false;
            note.IsEncrypted = true;
            CustomPasswordHasher passwordHasher = new();
            note.Password = Convert.ToBase64String(passwordHasher.HashMethod(note.Password,10000));
            note.Content = NoteEncrypter.Encrypt(note.Content);
            return note;
        }

        public async Task<ActionResult> GetEncrypt(int id) {
            var note = await _dataContext.Notes.FirstOrDefaultAsync(x => x.Id == id);
            var username = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            if (note != null && username != null) {
                if (username.Equals(note.Username)) {
                    var model = new GetEncryptedModel();
                    
                    return View("GetEncrypt", model);
                }
            }
            return Redirect("~/Identity/AccessDenied");
        }
        [HttpPost]
        public async Task<ActionResult> GetEncrypt(GetEncryptedModel model,int id) {
            var note = await _dataContext.Notes.FirstOrDefaultAsync(x => x.Id == id);
            var username = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            //var user = await _userManager.FindByNameAsync(username);
            if (note != null && username != null) {
                if (username.Equals(note.Username)) {
                    CustomPasswordHasher passwordHasher = new();
                    int embeddedIterCount;
                    if(!passwordHasher.VerifyPassword(Convert.FromBase64String(note.Password), model.Password,out embeddedIterCount))
                        return Redirect("~/Identity/AccessDenied");
                    if (embeddedIterCount == 10000) {
                        note.Content = NoteEncrypter.Decrypt(note.Content);
                        var model1 = new GetNoteModel {
                            Note = note
                        };
                        return View("Get", model1);
                    }
                    return Redirect("~/Identity/AccessDenied");

                }
                return Redirect("~/Identity/AccessDenied");
            }
            return Redirect("~/Identity/AccessDenied");
        }


        private string CheckPasswordQuality(string password) {
            bool hasLowerLetter = false;
            bool hasUpperLetter = false;
            bool hasDigit = false;
            bool hasSpecialChar = false;
            int poolSize = 0;
            if (password == null ||password.Length==0)
                return "Haslo nie moze byc puste";
            foreach (char c in password) {
                if (char.IsLower(c)) {
                    hasLowerLetter = true;
                }
                else if (char.IsUpper(c)) {
                    hasUpperLetter = true;
                }
                else if (char.IsDigit(c)) {
                    hasDigit = true;
                }
                else {
                    hasSpecialChar = true;
                }
            }

            if (hasLowerLetter) {
                poolSize += 26;
            }
            if (hasUpperLetter) {
                poolSize += 26;
            }
            if (hasDigit) {
                poolSize += 10;
            }
            if (hasSpecialChar) {
                poolSize += 32;
            }

            double entropy = password.Length * Math.Log2(poolSize);
            if (entropy < 30) {
                return 
                    ("Bardzo słabe hasło" + entropy.ToString("0.##"));
            }
            if (entropy < 50) {
                return 
                    ("Hasło nie jest wystarczajaco mocne " + entropy.ToString("0.##"));
            }
            else {
                return "ok";
            }
        }
        

    }
}

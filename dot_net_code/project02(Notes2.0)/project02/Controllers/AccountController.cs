using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project02.Models;


namespace project02.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _manager;
        private readonly SignInManager<IdentityUser> _signinmanager;


        public AccountController(UserManager<IdentityUser> manager, SignInManager<IdentityUser> signinmanager)
        {
            _manager = manager;
            _signinmanager = signinmanager;
        }
        //Get /Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.email,
                Email = model.email
            };
            var result = await _manager.CreateAsync(user, model.password);
            if (result.Succeeded)
            {
                // Bake authentication cookie instantly
                await _signinmanager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Notes");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        public IActionResult Login()
        {
            return View();
        }
        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model) {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _signinmanager.PasswordSignInAsync(model.email, model.password, isPersistent: false, lockoutOnFailure: false);// this auto create cookie
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Notes");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            
          await _signinmanager.SignOutAsync();// this auto delete the cookie cookie and does not retun a value, so no iof else this time
           
            
          return RedirectToAction("Login", "Account");
          
        }
    }
    }

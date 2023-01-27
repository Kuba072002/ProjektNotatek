using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjektNotatek.Data;
using ProjektNotatek.Models;
using System.Security.Claims;

namespace ProjektNotatek.Controllers {
    public class IdentityController : Controller {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentityController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public ActionResult Signup() {
            var model = new SignupViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Signup(SignupViewModel model) {
            if (ModelState.IsValid) {
                if( (await _userManager.FindByEmailAsync(model.Email)) == null && (await _userManager.FindByNameAsync(model.Username)) == null) {
                    var user = new ApplicationUser {
                        UserName = model.Username,
                        Email = model.Email
                    };

                    var result = await _userManager.CreateAsync(user,model.Password);
                    if (result.Succeeded) {
                        user = await _userManager.FindByEmailAsync(model.Email);

                        var claims = new List<Claim> {
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Role, "User")
                        };
                        await _userManager.AddClaimsAsync(user,claims);

                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confiramtionLink = Url
                            .ActionLink("ConfirmEmail", "Identity", new { userId = user.Id, @token = token });
                        //send here email
                        return RedirectToAction("EmailConfirmation", new { link = confiramtionLink });
                    }
                    ModelState.AddModelError("Signup", string.Join("\n", result.Errors.Select(x => x.Description)));
                    return View(model);
                }
                ModelState.AddModelError("Signup", "That email or username is taken.");
                return View(model);

            }
            return View(model);
        }

        //[HttpGet]
        public async Task<ActionResult> ConfirmEmail(string userId,string token) {
            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if(result.Succeeded) {
                return RedirectToAction("Signin");
            }

            return new NotFoundResult();
        }

        //[HttpGet]
        public ActionResult EmailConfirmation(string link) {
            ViewData["Message"] = link;
            return View();
        }

        [HttpGet]
        public ActionResult Signin() {
            return View(new SigninViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Signin(SigninViewModel model) {
            if (ModelState.IsValid) {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null) {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, true);
                    if (result.Succeeded) {
                        return Redirect("~/");
                    }
                    else if (result.IsLockedOut) {
                        ModelState.AddModelError("Login", "Account temporary locked");
                    }
                    else {
                        ModelState.AddModelError("Login", "Cannot login");
                    }
                }
                else {
                    ModelState.AddModelError("Login", "Invalid data");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }

        [HttpGet]
        public ActionResult ForgotPassword() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model) {
            if (!ModelState.IsValid)
                return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ForgotPasswordConfirmation");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var confiramtionLink = Url.Action("ResetPassword", "Identity", new { token, email = user.Email }, Request.Scheme);
            
            return RedirectToAction("ForgotPasswordConfirmation",new {link = confiramtionLink });
        }

        public ActionResult ForgotPasswordConfirmation(string link) {
            ViewData["Message"] = link;
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email) {
            var model = new ResetPasswordModel { Token = token, Email = email };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel) {
            if (!ModelState.IsValid)
                return View(resetPasswordModel);
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
                RedirectToAction(nameof(ResetPasswordConfirmation));
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            if (!resetPassResult.Succeeded) {
                foreach (var error in resetPassResult.Errors) {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View();
            }
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }
        [HttpGet]
        public IActionResult ResetPasswordConfirmation() {
            return View();
        }

        public ActionResult AccessDenied() {
            return View();
        }

        private double CalculateEntropy(string password) {
            bool hasLowerLetter = false;
            bool hasUpperLetter = false;
            bool hasDigit = false;
            bool hasSpecialChar = false;
            int poolSize = 0;

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
            
            return entropy;
        }

    }
}

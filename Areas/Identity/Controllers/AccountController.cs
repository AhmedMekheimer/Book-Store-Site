using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Online_Book_Store.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(SignVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);

            ApplicationUser applicationUser = new()
            {
                UserName = registerVM.UserName,
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                Address = registerVM.Address,
                Email = registerVM.Email
            };

            var result = await _userManager.CreateAsync(applicationUser, registerVM.Password);

            // User Added Sucessfully 
            if (result.Succeeded)
            {
                // Success msg
                TempData["success-notification"] = "User added Successfully, Check your account for the Confirmation Email";

                //Send Confirmation Email (Authentication)

                //Generating Email Conf. Token 
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);

                //Link sent to Email, to be redirected to 'action' that verifies token 
                var link = Url.Action(nameof(ConfirmEmail), "Account", new { userId = applicationUser.Id, token = token, area = "Identity" }, Request.Scheme);

                //Sending Confirmation Email to New Account
                await _emailSender.SendEmailAsync(applicationUser.Email, "Account Confirmation", $"<h1>Confirm Your Account By Clicking <a href='{link}'>here</a></h1>");

                return RedirectToAction("Register", "Account", new { area = "Identity" });
            }

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }
            return View(registerVM);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is not null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                    TempData["success-notification"] = "Confirm Email Successfully";
                else
                    TempData["error-notification"] = $"{String.Join(",", result.Errors)}";

                return RedirectToAction("Register", "Account", new { area = "Identity" });
            }
            return NotFound();
        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInVM signInVM)
        {
            if (!ModelState.IsValid)
                return View(signInVM);

            var user = await _userManager.FindByNameAsync(signInVM.UserNameOrEmail) ??
                       await _userManager.FindByEmailAsync(signInVM.UserNameOrEmail);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(
                                                signInVM.UserNameOrEmail,
                                                signInVM.Password,
                                                signInVM.RememberMe,
                                                lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    if (!user.EmailConfirmed)
                    {
                        TempData["error-notification"] = "Confirm Your Email";
                        return View(signInVM);
                    }

                    TempData["success-notification"] = "Signed In Successfully";
                    return RedirectToAction("SignIn", "Account", new { area = "Identity" });
                }
                if (result.IsLockedOut)
                {
                    TempData["error-notification"] = $"You are locked out until {user.LockoutEnd}";
                    return View(signInVM);
                }

                ModelState.AddModelError("UserNameOrEmail", "Invalid UserName Or Email");
                ModelState.AddModelError("Password", "Invalid Password");
                return View(signInVM);

            }
            ModelState.AddModelError("UserNameOrEmail", "Invalid UserName Or Email");
            ModelState.AddModelError("Password", "Invalid Password");
            return View(signInVM);
        }

        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            TempData["success-notification"] = $"Signed Out Successfully";
            return RedirectToAction("SignIn", "Account", new { area = "Identity" });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Data;
using System.Threading.Tasks;

namespace Online_Book_Store.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Policy = $"{SD.Workers}")]
        public async Task<IActionResult> Index(string search)
        {
            List<ApplicationUser> users;
            if (search is not null)
            {
                users = await _userManager.Users.Where(e => (e.UserName.Contains(search) || e.FirstName.Contains(search) || e.LastName.Contains(search) || e.Email.Contains(search))).ToListAsync();
            }
            else
                users = await _userManager.Users.ToListAsync();

            Dictionary<ApplicationUser, string> keyValuePairs = new();
            foreach (var item in users)
            {
                keyValuePairs.Add(item, String.Join(",", await _userManager.GetRolesAsync(item)));
            }

            ViewBag.Search = search;
            return View(keyValuePairs);
        }

        [Authorize(Policy = $"{SD.Admins}")]
        public IActionResult Create()
        {
            return View(new UserCreateVM());
        }

        [Authorize(Policy = $"{SD.Admins}")]
        [HttpPost]
        public async Task<IActionResult> Create(UserCreateVM userCreateVM)
        {
            if (!ModelState.IsValid)
                return View(userCreateVM);

            // Check if username or email already exists
            var userExists = await _userManager.FindByNameAsync(userCreateVM.UserName);
            var emailExists = await _userManager.FindByEmailAsync(userCreateVM.Email);
            if (emailExists != null || userExists != null)
            {
                if (userExists != null)
                    ModelState.AddModelError("UserName", "Username is already taken.");
                if (emailExists != null)
                    ModelState.AddModelError("Email", "Email is already in use.");
                return View(userCreateVM);
            }

            ApplicationUser applicationUser = new()
            {
                UserName = userCreateVM.UserName,
                FirstName = userCreateVM.FirstName,
                LastName = userCreateVM.LastName,
                Address = userCreateVM.Address,
                Email = userCreateVM.Email,
                EmailConfirmed=userCreateVM.ConfirmEmail
            };

            var userCreateResult = await _userManager.CreateAsync(applicationUser, userCreateVM.Password);

            var userRoles = await _userManager.GetRolesAsync(applicationUser);

            var rolesRemoveResult=await _userManager.RemoveFromRolesAsync(applicationUser, userRoles);

            var rolesAddResult = await _userManager.AddToRolesAsync(applicationUser, userCreateVM.Roles);

            // User Created, Roles added Successfully
            if (userCreateResult.Succeeded && rolesAddResult.Succeeded && rolesRemoveResult.Succeeded)
            {
                // Success msg
                TempData["success-notification"] = "User created Successfully";
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }

            foreach (var item in userCreateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }
            foreach (var item in rolesAddResult.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }
            foreach (var item in rolesRemoveResult.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }

            return View(userCreateVM);
        }

    }
}

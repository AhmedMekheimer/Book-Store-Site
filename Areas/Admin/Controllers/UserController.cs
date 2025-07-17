using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Online_Book_Store.Models;
using Online_Book_Store.ViewModels.Identity;
using System.Data;
using System.Security.Claims;
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
                EmailConfirmed = userCreateVM.ConfirmEmail
            };

            var userCreateResult = await _userManager.CreateAsync(applicationUser, userCreateVM.Password);

            var userRoles = await _userManager.GetRolesAsync(applicationUser);

            var rolesRemoveResult = await _userManager.RemoveFromRolesAsync(applicationUser, userRoles);

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

        [Authorize(Policy = $"{SD.Admins}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (await _userManager.FindByIdAsync(id) is ApplicationUser user)
            {
                UserEditVM userEditVM = new UserEditVM()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    ConfirmEmail = user.EmailConfirmed,
                    Address = user.Address
                };

                var userRoles = (await _userManager.GetRolesAsync(user)).ToList();
                userEditVM.Roles = userRoles;

                return View(userEditVM);
            }
            return NotFound();

        }

        [Authorize(Policy = $"{SD.Admins}")]
        [HttpPost]
        public async Task<IActionResult> Edit(UserEditVM userEditVM)
        {
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
                return View(userEditVM);

            if (await _userManager.FindByIdAsync(userEditVM.UserId) is ApplicationUser applicationUser)
            {
                if (userEditVM.Email != applicationUser.Email)
                {
                    // Check if email is already in use
                    var emailExists = await _userManager.FindByEmailAsync(userEditVM.Email);
                    if (emailExists != null)
                    {
                        ModelState.AddModelError("Email", "Email is already in use.");
                        return View(userEditVM);
                    }
                }
                if (userEditVM.UserName != applicationUser.UserName)
                {
                    // Check if username is already in use
                    var userExists = await _userManager.FindByNameAsync(userEditVM.UserName);
                    if (userExists != null)
                    {
                        ModelState.AddModelError("UserName", "Username is already in use.");
                        return View(userEditVM);
                    }
                }
                applicationUser.UserName = userEditVM.UserName;
                applicationUser.FirstName = userEditVM.FirstName;
                applicationUser.LastName = userEditVM.LastName;
                applicationUser.Email = userEditVM.Email;
                applicationUser.Address = userEditVM.Address;
                applicationUser.EmailConfirmed = userEditVM.ConfirmEmail;

                var userEditResult = await _userManager.UpdateAsync(applicationUser);

                var userRoles = await _userManager.GetRolesAsync(applicationUser);

                var rolesRemoveResult = await _userManager.RemoveFromRolesAsync(applicationUser, userRoles);

                var rolesAddResult = await _userManager.AddToRolesAsync(applicationUser, userEditVM.Roles);

                // User and its Roles Updated Successfully
                if (userEditResult.Succeeded && rolesAddResult.Succeeded && rolesRemoveResult.Succeeded)
                {
                    // Success msg
                    TempData["success-notification"] = "User updated Successfully";
                    return RedirectToAction("Index", "User", new { area = "Admin" });
                }

                foreach (var item in userEditResult.Errors)
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

                return View(userEditVM);
            }
            return NotFound();
        }

        [Authorize(Policy = $"{SD.Admins}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (await _userManager.FindByIdAsync(id) is ApplicationUser applicationUser)
            {
                // Get current user ID
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Prevent self-deletion
                if (id == currentUserId)
                {
                    TempData["error-notification"] = "You cannot delete your own account";
                    return RedirectToAction("Index");
                }

                var result = await _userManager.DeleteAsync(applicationUser);
                if (result.Succeeded)
                { 
                    TempData["success-notification"] = "User Deleted Successfully";
                    return RedirectToAction("Index", "User", new { area = "Admin" });
                }

                TempData["error-notification"] = "User Deletion Error";
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }
            TempData["error-notification"] = "User Not Found";
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        [Authorize(Policy = $"{SD.Admins}")]
        public async Task<IActionResult> BlockUnBlock(string id)
        {
            if (await _userManager.FindByIdAsync(id) is ApplicationUser applicationUser)
            {
                // Get current user ID
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Prevent self-blocking
                if (id == currentUserId)
                {
                    TempData["error-notification"] = "You cannot block your own account";
                    return RedirectToAction("Index");
                }

                if (applicationUser.LockoutEnabled)
                {
                    applicationUser.LockoutEnabled = false;
                    applicationUser.LockoutEnd = DateTime.UtcNow.AddDays(30);
                    TempData["success-notification"] = $"User Blocked until {applicationUser.LockoutEnd}";
                }
                else
                {
                    applicationUser.LockoutEnabled = true;
                    applicationUser.LockoutEnd = null;
                    TempData["success-notification"] = "User is UnBlocked Successfully";
                }
                var result=await _userManager.UpdateAsync(applicationUser);
                if (result.Succeeded)
                    return RedirectToAction("Index", "User", new { area = "Admin" });
                else
                {
                    TempData["error-notification"] = "User data wasn't updated";
                    return RedirectToAction("Index", "User", new { area = "Admin" });
                }
            }
            TempData["error-notification"] = "User Not Found";
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }
    }
}

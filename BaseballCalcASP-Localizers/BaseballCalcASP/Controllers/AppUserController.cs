using BaseballCalcASP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace BaseballCalcASP.Controllers
{
    [Authorize(Roles = "admin")]
    public class AppUserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AppUserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        //GET: AppUser
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            //var appUsers = _userManager.Users.Where(c => c.deleted == false);
            var appUsers = _userManager.Users;
            return View(await appUsers.ToListAsync());
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
                return NotFound();

            return View(appUser);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles.Select(r => r.Name).ToList();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(CreateAppUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appUser = new AppUser { UserName = model.UserName, Email = model.EmailAddress };
                appUser.FirstName = model.FirstName;
                appUser.LastName = model.LastName;
                appUser.deleted = false;
                appUser.EmailConfirmed = true;
                var result = await _userManager.CreateAsync(appUser, model.Password);
                var resultAddRoles = await _userManager.AddToRolesAsync(appUser, model.Roles);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles.Select(r => r.Name).ToList();
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(appUser);
            var model = new EditAppUserViewModel
            {
                Id = appUser.Id,
                UserName = appUser.UserName,
                EmailAddress = appUser.Email,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                deleted = appUser.deleted,
                Roles = userRoles.ToList()
            };

            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles.Select(r => r.Name).ToList();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(EditAppUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appUser = await _userManager.FindByIdAsync(model.Id);
                if (appUser != null)
                {
                    appUser.UserName = model.UserName;
                    appUser.FirstName = model.FirstName;
                    appUser.LastName = model.LastName;
                    appUser.deleted = model.deleted;

                    //vervang oude roles met nieuwe
                    var oldRoles = await _userManager.GetRolesAsync(appUser);
                    var resultRemoveRoles = await _userManager.RemoveFromRolesAsync(appUser, oldRoles);
                    var resultAddRoles = await _userManager.AddToRolesAsync(appUser, model.Roles);

                    if (resultRemoveRoles.Succeeded && resultAddRoles.Succeeded)
                    {
                        var result = await _userManager.UpdateAsync(appUser);

                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index");
                        }

                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    else
                    {
                        foreach (var error in resultRemoveRoles.Errors.Concat(resultAddRoles.Errors))
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles.Select(r => r.Name).ToList();
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
                return NotFound();
            var model = new DeleteAppUserViewModel { Id=appUser.Id, UserName=appUser.UserName, EmailAddress=appUser.Email };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser != null)
            {
                //var result = await _userManager.DeleteAsync(appUser);
                appUser.deleted = true;
                var result = await _userManager.UpdateAsync(appUser);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
    }
}
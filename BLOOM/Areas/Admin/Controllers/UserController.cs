using BLOOM.Business.Services;
using BLOOM.Business.Services.IServices;
using BLOOM.Models;
using BLOOM.Models.ViewModels;
using BLOOM.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BLOOM.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.RoleAdmin+","+SD.RoleEmployee)]

    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole>_roleManager;
        private readonly IApplicationUserService _userService;


        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IApplicationUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult>RoleManagement(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            RoleManagementVM RoleVM = new()
            {
                ApplicationUser = user,
                RoleList = _roleManager.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                   Text=u.Name,
                   Value=u.Name
                })
            };
            RoleVM.ApplicationUser.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            return View(RoleVM);
        }

        [HttpPost]
        public async Task<IActionResult> RoleManagement(RoleManagementVM roleManagementVM)
        {
            var user = await _userService.GetUserByIdAsync(roleManagementVM.ApplicationUser.Id);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            var oldRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            if (!(roleManagementVM.ApplicationUser.Role == oldRole))
            { 
                //update role
                await _userManager.RemoveFromRoleAsync(user, oldRole);
                await _userManager.AddToRoleAsync(user, roleManagementVM.ApplicationUser.Role);

            }
            TempData["success"] = "Role has been Updated";
            return RedirectToAction(nameof(Index));
        }




        public async Task<IActionResult> ChangePassword(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            AdminChangePasswordVm adminChangePasswordVm = new()
            {
                UserEmail=user.Email,
                UserId=user.Id,
            };
            return View(adminChangePasswordVm);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(AdminChangePasswordVm adminChangePasswordVm)
        {
            if (!ModelState.IsValid)
            {
                return View(adminChangePasswordVm);
            }

            var user = await _userService.GetUserByIdAsync(adminChangePasswordVm.UserId);

            if (user == null)
            {
                return NotFound();
            }

          var token= await _userManager.GeneratePasswordResetTokenAsync(user);
           var result= await _userManager.ResetPasswordAsync(user, token, adminChangePasswordVm.NewPassword);

            if (result.Succeeded)
            {
                TempData["success"] = $"Password for {user.Email} has been changed successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            adminChangePasswordVm.UserEmail = user.Email;
            return View(adminChangePasswordVm);
            
        }




        #region API CALLS
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();

            foreach(var user in users)
            {
                user.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            }

            return Json(new { data = users });
        }


        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody] string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

           if(await _userManager.IsLockedOutAsync(user))
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                return Json(new { success = true, message = "User unlocked successfully" });
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(1000));
                return Json(new { success = true, message = "User locked successfully" });
            
             }
        }

        #endregion


    }
}


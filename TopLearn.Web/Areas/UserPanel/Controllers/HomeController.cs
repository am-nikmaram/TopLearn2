using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class HomeController : Controller
    {
        private IUserService _userServise;

        public HomeController(IUserService userService)
        {
            _userServise = userService;
        }
        public IActionResult Index()
        {
            return View(_userServise.GetUserInformation(User.Identity.Name));
        }
        [Route("UserPanel/EditProfile")]
        public IActionResult EditProfile()
        {
            return View(_userServise.GetDataForEditProfileUser(User.Identity.Name));
        }

        [HttpPost]
        [Route("UserPanel/EditProfile")]
        public IActionResult EditProfile(EditProfileViewModel profile)
        {
            if(!ModelState.IsValid)
            {
                return View(profile);
            }
            _userServise.EditProfile(User.Identity.Name, profile);
            ViewBag.IsSuccess = true;
            //  ViewBag.EditProfile = true;
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Login?EditProfile=true");

        }

        [Route("UserPanel/ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }


        [Route("UserPanel/ChangePassword")]
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel change)
        {
            string currentUserName = User.Identity.Name;
            if (!ModelState.IsValid)
                return View(change);
            if (!_userServise.CompareOldPassword(change.oldPassword, currentUserName))
            {
                ModelState.AddModelError("OldPassword", "کلمه عبور وارد شده معتبر نمی باشد");
                return View(change);
            }
            _userServise.ChangeUserPassword(currentUserName, change.Password);
            ViewBag.Issuccess = true;
            return View();
        }

    }
}
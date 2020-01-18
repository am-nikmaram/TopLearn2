using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Admin.Users
{
    [PermissionChecker(4)]
    public class EditUserModel : PageModel
    {

        IUserService _userService;
        IPermissionService _persissionService;
        public EditUserModel(IUserService userService,IPermissionService permissionService)
        {
            _userService = userService;
            _persissionService = permissionService;
        }
        [BindProperty]
        public EditUserViewModel EditUserViewModel { get; set; }
        public void OnGet(int id)
        {
            EditUserViewModel = _userService.GetUserForShowInEditMode(id);
            ViewData["Roles"] = _persissionService.GetRoles();
        }
        public IActionResult OnPost(List<int> SelectedRoles)
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }
            _userService.EditUserFromAdmin(EditUserViewModel);
            //Edit Roles User
            _persissionService.EditRolesUser(EditUserViewModel.UserId, SelectedRoles);

            return RedirectToPage("Index");
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Pages.Admin.Roles
{
    [PermissionChecker(7)]
    public class CreateRoleModel : PageModel
    {
        IPermissionService _permissionService;
        public CreateRoleModel(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        [BindProperty]
        public Role Role { get; set; }

        public void OnGet()
        {
            ViewData["Permissions"] = _permissionService.GetAllPermission();
        }
        public IActionResult OnPost(List<int> SelectedPermission)
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }
            Role.IsDelete = false;
            int roleId=_permissionService.AddRole(Role);
            //ADD Permission
            _permissionService.AddPermissionsToRole(roleId, SelectedPermission);

            return RedirectToPage("Index");   
        }
    }
}
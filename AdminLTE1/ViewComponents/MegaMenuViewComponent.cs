using AdminLTE1.Models;
using AdminLTE1.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
//@inject SignInManager<ApplicationUser> SignInManager

namespace AdminLTE1.ViewComponents
{
    public class MegaMenuViewComponent : ViewComponent
    {
        private readonly AddDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public SignInManager<ApplicationUser> SignInManager { get; }

        public MegaMenuViewComponent(AddDbContext context,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> SignInManager)
        {
            this._context = context;
            this.userManager = userManager;
            this.SignInManager = SignInManager;
        }

        public IViewComponentResult Invoke()
        {
            var userId = userManager.GetUserId(HttpContext.User);
            if (SignInManager.IsSignedIn(HttpContext.User) && HttpContext.User.IsInRole("SuperAdmin"))
            {
                var result = (from Menus in _context.ClassMenu                            
                              select new PermissionViewModelEx
                              {
                                  Id = Menus.Id,
                                  Path = Menus.Path,
                                  Name = Menus.Name,
                                  ParentId = Menus.ParentId ?? 0
                              }).ToList();
                return View(result);
            }
            else
            {
                var result = (from Menus in _context.ClassMenu
                              where Menus.Name == "Class" || Menus.Name == "Class8" || Menus.Name == "Class9" || Menus.Name == "Class10"
                              || Menus.Name == "Class11" || Menus.Name == "Class12"
                              select new PermissionViewModelEx
                              {
                                  Id = Menus.Id,
                                  Path = Menus.Path,
                                  Name = Menus.Name,
                                  ParentId = Menus.ParentId ?? 0
                              }).ToList();
                return View(result);
            }
            
        }
    }
    
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdminLTE1.Areas.Identity.Pages.Account;
using AdminLTE1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AdminLTE1.Controllers
{
    public class AddUserController : Controller
    {
        private readonly AddDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostEnvironment;
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        

        public AddUserController(AddDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostingEnvironment;
        }
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterModel.InputModel model)
        {
            //var user1 = await GetCurrentUserAsync();

           // var userId = user1?.Id;
            //var userId = _userManager.GetUserId();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    CId = model.CId,
                    StateId = model.StateId,
                    CityId = model.CityId,
                    ProfilePicture = model.ProfilePicture
                };


                if (model.ProfilePictureFile != null)
                {
                    //var paths = Path.Combine(_hostEnvironment.WebRootPath, "Upload",model.ProfilePicture);

                    //if (System.IO.File.Exists(paths))
                    //{
                    //    // If file found, delete it    
                    //    System.IO.File.Delete(Path.Combine(paths));
                    //}

                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(model.ProfilePictureFile.FileName);
                    string extension = Path.GetExtension(model.ProfilePictureFile.FileName);
                    user.ProfilePicture = DateTime.Now.ToString("yymmssfff") + extension;


                    string path = Path.Combine(wwwRootPath, "Upload", user.ProfilePicture);
                    var fileStream = new FileStream(path, FileMode.Create);
                    model.ProfilePictureFile.CopyTo(fileStream);
                    await _userManager.UpdateAsync(user);
                }
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUser", "User");
                }
                return View(model);
            }
            else
                return View(model);
        }
        //[HttpPost]
        public JsonResult UserExist()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            if (userId != null)
            {
                return Json(true, new JsonSerializerSettings());
            }
            else
            {
                return Json(false, new JsonSerializerSettings());
            }
            
        }
    }
}
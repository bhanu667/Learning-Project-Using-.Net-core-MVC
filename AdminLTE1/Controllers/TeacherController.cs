using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdminLTE1.Helper;
using AdminLTE1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AdminLTE1.Controllers
{
    //[AuthorizeActionFilter]
    [Authorize(Roles = "SuperAdmin")]
    public class TeacherController : Controller
    {
        private readonly AddDbContext _context;
        private readonly IHostingEnvironment _hostEnvironment;
        public TeacherController(AddDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostEnvironment = hostingEnvironment;
        }

        public IActionResult TeacherList()
        {
            ViewBag.result = _context.Teacher.ToList();
            return View();
        }
        [HttpGet]
        public IActionResult EditTeacher(int id)
        {
            var result = _context.Teacher.Where(x => x.TeacherId == id);
            if (result == null)
            {
                ViewBag.ErrorMessage = $"Teacher with Id = {id} cannot be found";
                return View("NotFound");
            }
            var model = new Teacher
            {
                TeacherId = result.FirstOrDefault().TeacherId,
                TeacherName = result.FirstOrDefault().TeacherName,
                TeacherClassId = result.FirstOrDefault().TeacherClassId,
                Subject = result.FirstOrDefault().Subject,
                TeacherImage = result.FirstOrDefault().TeacherImage,
                Degree = result.FirstOrDefault().Degree
            };
            return View(model);
        }
       
        [HttpPost]
        public IActionResult EditTeacher(Teacher model)
        {
            if (model != null)
            {
                var teacher = _context.Teacher.Where(x => x.TeacherId == model.TeacherId);
                if (teacher != null)
                {
                    teacher.FirstOrDefault().TeacherName = model.TeacherName;
                    teacher.FirstOrDefault().TeacherClassId = model.TeacherClassId;
                    teacher.FirstOrDefault().Subject = model.Subject;
                    teacher.FirstOrDefault().Degree = model.Degree;
                    var TeacherImage = teacher.FirstOrDefault().TeacherImage;


                    if (model.TeacherImageFile != null)
                    {
                        if (TeacherImage != model.TeacherImage)
                        {
                            string wwwRootPath = _hostEnvironment.WebRootPath;
                            string fileName = Path.GetFileNameWithoutExtension(model.TeacherImageFile.FileName);
                            string extension = Path.GetExtension(model.TeacherImageFile.FileName);
                            teacher.FirstOrDefault().TeacherImage = DateTime.Now.ToString("yymmssfff") + extension;


                            string path = Path.Combine(wwwRootPath, "Images", teacher.FirstOrDefault().TeacherImage);
                            var fileStream = new FileStream(path, FileMode.Create);
                            model.TeacherImageFile.CopyTo(fileStream);
                        }
                        else if (TeacherImage == null)
                        {
                            string wwwRootPath = _hostEnvironment.WebRootPath;
                            string fileName = Path.GetFileNameWithoutExtension(model.TeacherImageFile.FileName);
                            string extension = Path.GetExtension(model.TeacherImageFile.FileName);
                            teacher.FirstOrDefault().TeacherImage = DateTime.Now.ToString("yymmssfff") + extension;


                            string path = Path.Combine(wwwRootPath, "Images", teacher.FirstOrDefault().TeacherImage);
                            var fileStream = new FileStream(path, FileMode.Create);
                            model.TeacherImageFile.CopyTo(fileStream);
                        }
                    }
                    //_context.
                    //_context.Entry(teacher).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    
                    _context.SaveChanges();

                    return RedirectToAction("TeacherList");
                }
                return View();
            }
            return View();
        }

        public IActionResult DeleteTeacher(int Id)
        {
            var teacher = _context.Teacher.FirstOrDefault(m => m.TeacherId == Id);
            _context.Teacher.Remove(teacher);
            _context.SaveChanges();

            TempData["successMessage"] = "Teacher Deleted Successfully.";
            TempData.Keep();

            return RedirectToAction("TeacherList");

        }

        public IActionResult AddTeacher()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTeacher(Teacher model)
        {
            if (ModelState.IsValid)
            {
                var obj = new Teacher();
                obj.TeacherName = model.TeacherName;
                obj.Subject = model.Subject;
                obj.TeacherClassId = model.TeacherClassId;
                obj.Degree = model.Degree;
                if (model.TeacherImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(model.TeacherImageFile.FileName);
                    string extension = Path.GetExtension(model.TeacherImageFile.FileName);
                    obj.TeacherImage = DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath, "Images", obj.TeacherImage);
                    var fileStream = new FileStream(path, FileMode.Create);
                    model.TeacherImageFile.CopyTo(fileStream);
                }
                _context.Teacher.Add(obj);
                _context.SaveChanges();
            }
            return Json("Added Successfully");
        }
    }
}
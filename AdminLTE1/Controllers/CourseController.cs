using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminLTE1.Helper;
using AdminLTE1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminLTE1.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class CourseController : Controller
    {
        private readonly AddDbContext _context;

        public CourseController(AddDbContext context)
        {
            _context = context;
        }

        public IActionResult CourseList()
        {
            var result = _context.Course.ToList();
            return View(result);
        }

        public IActionResult AddCourse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCourse(Course model)
        {
            if (ModelState.IsValid)
            {
                var obj = new Course();
                obj.CourseName = model.CourseName;
                obj.CourseClassId = model.CourseClassId;               
                _context.Course.Add(obj);
                _context.SaveChanges();
                return Json("Added Successfully");
            }
            return View();
        }

        [HttpGet]
        public IActionResult EditCourse(int id)
        {
            var result = _context.Course.Where(x => x.CourseId == id);
            if (result == null)
            {
                ViewBag.ErrorMessage = $"Course with Id = {id} cannot be found";
                return View("NotFound");
            }
            var model = new Course
            {
                CourseId = result.FirstOrDefault().CourseId,
                CourseClassId = result.FirstOrDefault().CourseClassId,
                CourseName = result.FirstOrDefault().CourseName
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EditCourse(Course model)
        {
            if (model != null)
            {
                var course = _context.Course.Where(x => x.CourseId == model.CourseId);
                if (course != null)
                {
                    course.FirstOrDefault().CourseClassId = model.CourseClassId;
                    course.FirstOrDefault().CourseName = model.CourseName;
                    _context.SaveChanges();
                    return RedirectToAction("CourseList");
                }
                return View();
            }
            return View();
        }

        public IActionResult DeleteCourse(int Id)
        {
            var course = _context.Course.FirstOrDefault(m => m.CourseId == Id);
            _context.Course.Remove(course);
            _context.SaveChanges();

            TempData["successMessage"] = "Teacher Deleted Successfully.";
            TempData.Keep();

            return RedirectToAction("TeacherList");

        }

    }
}
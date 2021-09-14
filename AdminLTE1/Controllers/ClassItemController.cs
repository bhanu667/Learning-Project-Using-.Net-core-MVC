using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminLTE1.Models;
using AdminLTE1.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminLTE1.Controllers
{
    public class ClassItemController : Controller
    {
        private readonly AddDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClassItemController(AddDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }      

        public IActionResult Class(int id)
        {

            var userId = _userManager.GetUserId(HttpContext.User);
            @ViewBag.price = _context.ClassMenu.Where(x => x.Id == id).FirstOrDefault().Price;
            ViewBag.button = _context.Cart.Where(x => x.ClassId == id && x.UserId == userId).ToList();
            BookCourseTeacherViewModel obj = new BookCourseTeacherViewModel();
            obj.book = (from c in _context.ClassMenu
                        join b in _context.Book.Where(r=>r.BookClassId==id)
                        on c.Id equals b.BookClassId
                        select new Book
                        {
                            BookId = b.BookId,
                            BookName = b.BookName,
                            Author = b.Author,
                            Description = b.Description,
                            BookImage = b.BookImage
                        }).ToList();

            obj.course = (from c in _context.ClassMenu
                          join cour in _context.Course.Where(r => r.CourseClassId == id)
                          on c.Id equals cour.CourseClassId
                          select new Course
                          {
                              CourseName = cour.CourseName
                          }).ToList();
            obj.teacher = (from c in _context.ClassMenu
                           join t in _context.Teacher.Where(r => r.TeacherClassId == id)
                           on c.Id equals t.TeacherClassId
                           select new Teacher
                           {
                               TeacherName = t.TeacherName,
                               Subject = t.Subject,
                               Degree = t.Degree,
                               TeacherImage = t.TeacherImage
                           }).ToList();
            ViewBag.ClassId = id;
            return View(obj);
        }

        [HttpGet]
        public IActionResult CreateClass()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateClass(ClassMenu model)
        {
            if (model != null)
            {
                var menuModel = new ClassMenu();
                menuModel.Name = model.Name;
                menuModel.Path = model.Path;
                menuModel.ParentId = model.ParentId;
                menuModel.Price = model.Price;
                _context.ClassMenu.Add(menuModel);
                _context.SaveChanges();
            }
            return Json("Created Successfully");
        }

        public IActionResult GetClasses()
        {
            List<ClassMenu> lstmodel = new List<ClassMenu>();
            var models = _context.ClassMenu;
            foreach (var item in models)
            {
                ClassMenu mvm = new ClassMenu();
                mvm.Id = item.Id;
                mvm.Name = item.Name;
                mvm.Path = item.Path;
                mvm.ParentId = item.ParentId;
                mvm.Price = item.Price;
                lstmodel.Add(mvm);
            }

            ViewBag.successMessage = TempData["successMessage"];
            return View(lstmodel);
        }

        public IActionResult Description(int id)
        {
           
            ViewBag.desc = _context.Book.Where(x => x.BookId == id).ToList();
            return View();
        }
        [HttpGet]
        public IActionResult EditClass(int Id)
        {
            ClassMenu mvm = new ClassMenu();
            var menu = _context.ClassMenu.FirstOrDefault(m => m.Id == Id);

            if (menu != null)
            {
                mvm.Id = menu.Id;
                mvm.Name = menu.Name;
                mvm.Path = menu.Path;
                mvm.ParentId = menu.ParentId;
                mvm.Price = menu.Price;
            }
            return View(mvm);
        }      

        [HttpPost]
        public IActionResult EditClass(ClassMenu model)
        {
            if (model != null)
            {
                var MenuItem = _context.ClassMenu.FirstOrDefault(m => m.Id == model.Id);
                if (MenuItem != null)
                {
                    MenuItem.Name = model.Name;
                    MenuItem.Path = model.Path;
                    MenuItem.ParentId = model.ParentId;
                    MenuItem.Price = model.Price;
                    //_context.Entry(MenuItem).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();

                    return RedirectToAction("GetClasses");
                }
                return View();
            }
            return View();
        }
        [HttpGet]
        public IActionResult DeleteClass(int Id)
        {
            var menu = _context.ClassMenu.Where(m => m.Id == Id).FirstOrDefault();
            _context.ClassMenu.Remove(menu);
            _context.SaveChanges();

            TempData["successMessage"] = "Menu Deleted Successfully.";
            TempData.Keep();

            return RedirectToAction("GetClasses");

        }

        public IActionResult SendMail()
        {

            return View();
        }
    }
}
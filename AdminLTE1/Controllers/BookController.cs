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
    [Authorize(Roles = "SuperAdmin")]
    public class BookController : Controller
    {
        private readonly AddDbContext _context;
        private readonly IHostingEnvironment _hostEnvironment;

        public BookController(AddDbContext context, IHostingEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult BookList()
        {
            var result = _context.Book.ToList();                        
            return View(result);
        }

        [HttpGet]
        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBook(Book model)
        {
            if (ModelState.IsValid)
            {
                var book = new Book();

                book.BookName = model.BookName;
                book.Author = model.Author;
                book.Description = model.Description;
                book.BookClassId = model.BookClassId;
                book.IsActive = model.IsActive;
                if (model.BookImageFile != null)
                {                   
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(model.BookImageFile.FileName);
                    string extension = Path.GetExtension(model.BookImageFile.FileName);
                    book.BookImage = DateTime.Now.ToString("yymmssfff") + extension;


                    string path = Path.Combine(wwwRootPath, "Images", book.BookImage);
                    var fileStream = new FileStream(path, FileMode.Create);
                    model.BookImageFile.CopyTo(fileStream);
                }
                _context.Book.Add(book);
                _context.SaveChanges();
            }                        
                return Json("Book Added Successfully");
        }

        [HttpGet]
        public IActionResult EditBook(int id)
        {
            var result = _context.Book.Where(x => x.BookId == id);
            if (result == null)
            {
                ViewBag.ErrorMessage = $"Teacher with Id = {id} cannot be found";
                return View("NotFound");
            }
            var model = new Book
            {
                BookId = result.FirstOrDefault().BookId,
                BookName = result.FirstOrDefault().BookName,
                Author = result.FirstOrDefault().Author,
                Description = result.FirstOrDefault().Description,
                BookClassId = result.FirstOrDefault().BookClassId,
                IsActive = result.FirstOrDefault().IsActive,
                BookImage = result.FirstOrDefault().BookImage
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EditBook(Book model)
        {
            if (model != null)
            {
                var book = _context.Book.Where(x => x.BookId == model.BookId);
                if (book != null)
                {
                    book.FirstOrDefault().BookName = model.BookName;
                    book.FirstOrDefault().Author = model.Author;
                    book.FirstOrDefault().Description = model.Description;
                    book.FirstOrDefault().BookClassId = model.BookClassId;
                    book.FirstOrDefault().IsActive = model.IsActive;
                    var BookImage = book.FirstOrDefault().BookImage;

                    if (model.BookImageFile != null)
                    {
                        if (BookImage != model.BookImage)
                        {
                            string wwwRootPath = _hostEnvironment.WebRootPath;
                            string fileName = Path.GetFileNameWithoutExtension(model.BookImageFile.FileName);
                            string extension = Path.GetExtension(model.BookImageFile.FileName);
                            book.FirstOrDefault().BookImage = DateTime.Now.ToString("yymmssfff") + extension;


                            string path = Path.Combine(wwwRootPath, "Images", book.FirstOrDefault().BookImage);
                            var fileStream = new FileStream(path, FileMode.Create);
                            model.BookImageFile.CopyTo(fileStream);
                        }
                        else if (BookImage == null)
                        {
                            string wwwRootPath = _hostEnvironment.WebRootPath;
                            string fileName = Path.GetFileNameWithoutExtension(model.BookImageFile.FileName);
                            string extension = Path.GetExtension(model.BookImageFile.FileName);
                            book.FirstOrDefault().BookImage = DateTime.Now.ToString("yymmssfff") + extension;


                            string path = Path.Combine(wwwRootPath, "Images", book.FirstOrDefault().BookImage);
                            var fileStream = new FileStream(path, FileMode.Create);
                            model.BookImageFile.CopyTo(fileStream);
                        }
                    }
                    //_context.Entry(book).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();

                    return RedirectToAction("BookList");
                }
                return View();
            }
            return View();
        }

        public IActionResult DeleteBook(int Id)
        {
            var book = _context.Book.FirstOrDefault(m => m.BookId == Id);
            _context.Book.Remove(book);
            _context.SaveChanges();

            TempData["successMessage"] = "Teacher Deleted Successfully.";
            TempData.Keep();

            return RedirectToAction("BookList");

        }
    }
}
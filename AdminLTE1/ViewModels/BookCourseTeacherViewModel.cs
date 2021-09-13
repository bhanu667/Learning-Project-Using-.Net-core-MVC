using AdminLTE1.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLTE1.ViewModels
{
    public class BookCourseTeacherViewModel
    {
        public List<Book> book { get; set; }
        public List<Teacher> teacher { get; set; }
        public List<Course> course { get; set; }
    }
}

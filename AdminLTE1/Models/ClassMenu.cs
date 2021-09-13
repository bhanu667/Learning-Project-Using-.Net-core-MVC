using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLTE1.Models
{
    public class ClassMenu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public decimal? Price { get; set; }
        public int? ParentId { get; set; }
        
    }
}

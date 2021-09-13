using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLTE1.ViewModels
{
    public class CartClassMenuViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ClassId { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public int ParentId { get; set; }
    }
}

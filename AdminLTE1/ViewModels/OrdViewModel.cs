using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLTE1.ViewModels
{
    public class OrdViewModel
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }

        public int Id { get; set; }
        public int OrdId { get; set; }
        public int ClassId { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }

        public string Name { get; set; }
    }
}

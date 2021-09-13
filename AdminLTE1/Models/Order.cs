using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLTE1.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

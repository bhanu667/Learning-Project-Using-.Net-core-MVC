using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLTE1.Models
{
    public class OrderDtl
    {
        [Key]
        public int Id { get; set; }
        public int OrdId { get; set; }
        public int ClassId { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }

        //public virtual Order Order { get; set; }
    }
}

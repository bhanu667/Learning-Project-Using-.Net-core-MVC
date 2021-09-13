using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLTE1.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ClassId { get; set; }
        public int Quantity { get; set; }

        public virtual ClassMenu Class { get; set; }
    }
}

using AdminLTE1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLTE1.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public int ClassId { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SampleId { get; set; }
        
        public string ShippingAddress { get; set; }
        public string TransactionFee { get; set; }
        public string PaymentMethod { get; set; }
        public string UniqId { get; set; }

        public List<OrderDtlViewModel> orderDtl { get; set; }
    }
    public class OrderDtlViewModel
    {        
        public int Id { get; set; }
        public int OrdId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }        
        public decimal Amount { get; set; }
        public int? Quantity { get; set; }
    }
}

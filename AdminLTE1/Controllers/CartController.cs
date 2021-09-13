using AdminLTE1.Models;
using AdminLTE1.PayPalHelper;
using AdminLTE1.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace AdminLTE1.Controllers
{
    //[Route("cart")]
    public class CartController : Controller
    {
        public IConfiguration Configuration { get; }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AddDbContext _api;
        public CartController(IConfiguration _configuration, UserManager<ApplicationUser> userManager, AddDbContext api)
        {
            Configuration = _configuration;
            _userManager = userManager;
            this._api = api;
        }

        public IActionResult Invoice(string id)
        {
            var inc = _api.PaymentDetail.Where(x => x.OrderId == id).FirstOrDefault();
            return View(inc);
        }

        public IActionResult InvoicePdf(int id)
        {
            var inc = _api.PaymentDetail.Where(x => x.Id == id).FirstOrDefault();
            return new ViewAsPdf("InvoicePdf", inc);
        }


        public async Task<IActionResult> Refund(string Id)
        {
            string CaptureId = Id;
            PayPalRefund.CapturesRefund(CaptureId, true).Wait();
            return View();
        }

        public IActionResult Cancle()
        {
            return View();
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
                var result = (from c in _api.ClassMenu
                              join b in _api.Cart.Where(c => c.UserId == userId)
                              on c.Id equals b.ClassId
                              orderby c.Name
                              select new CartClassMenuViewModel
                              {
                                  Id = b.Id,
                                  ClassId = c.Id,
                                  Name = c.Name,
                                  Price = c.Price ?? 0,
                                  Quantity = b.Quantity >= 1 ? b.Quantity : 1
                              }).ToList();
            return View(result);
        }

        public IActionResult OrderDetail()
        {
           List<OrderViewModel> listOrder = new List<OrderViewModel>();
            var userId = _userManager.GetUserId(HttpContext.User);
            if (userId != null)
            {
                var orders = _api.Order.Where(x=>x.UserId== userId).ToList();
                foreach (var order in orders)
                {
                    OrderViewModel odr = new OrderViewModel();
                    odr.OrderId = order.OrderId;
                    odr.UserId = order.UserId;
                    odr.Status = order.Status;
                    odr.TotalAmount = order.TotalAmount;
                    odr.CreatedDate = order.CreatedDate;

                    //payment table
                    var payDtl = _api.PaymentDetail.Where(x => x.UniqId == userId).ToList();
                    odr.UniqId = payDtl.FirstOrDefault().UniqId;
                    odr.ShippingAddress = payDtl.FirstOrDefault().ShippingAddress;
                    odr.TransactionFee = payDtl.FirstOrDefault().TransactionFee;
                    odr.PaymentMethod = payDtl.FirstOrDefault().PaymentMethod;

                    //order detail table

                    var orderDetail = _api.OrderDtl.Where(x => x.OrdId == odr.OrderId).ToList();
                    List<OrderDtlViewModel> orderDl = new List<OrderDtlViewModel>();
                    foreach (var ordDtl in orderDetail)
                    {
                        OrderDtlViewModel orderDlItem = new OrderDtlViewModel();
                        orderDlItem.Id = ordDtl.Id;
                        orderDlItem.OrdId = ordDtl.OrdId;                        
                        orderDlItem.ClassId = ordDtl.ClassId;
                        orderDlItem.ClassName = _api.ClassMenu.Where(c => c.Id == ordDtl.ClassId).FirstOrDefault().Name;
                        orderDlItem.Quantity = ordDtl.Quantity;
                        orderDlItem.Amount = ordDtl.Amount;
                        orderDl.Add(orderDlItem);
                    }
                    odr.orderDtl = orderDl;                   
                    listOrder.Add(odr);  
                }
            }
            return View(listOrder);
        }
        public JsonResult AddToCart(int classId)
        {
            Cart cObj = new Cart();
            var userId = _userManager.GetUserId(HttpContext.User);
            var cls = _api.ClassMenu.Where(x => x.Id == classId).FirstOrDefault();
                cObj.ClassId = classId;
                cObj.UserId = userId;
                cObj.Quantity = 1;
                _api.Cart.Add(cObj);
                _api.SaveChanges();
            return Json("true",new JsonSerializerSettings());
        }

        public IActionResult AddCart(int classId, string operation, int? quantity)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var cls = _api.ClassMenu.Where(x => x.Id == classId).FirstOrDefault();
            if (operation == "add")
            {
                var result = _api.Cart.FirstOrDefault(c => c.ClassId == classId && c.UserId == userId);
                if (result != null)
                {
                    result.Quantity = result.Quantity + 1;
                    _api.Entry(result).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _api.SaveChanges();
                }              
            }
            else if (operation == "remove")
            {
                var result = _api.Cart.FirstOrDefault(c => c.ClassId == classId && c.UserId == userId);
                if (quantity == 1)
                    _api.Cart.Remove(result);
                else
                {
                    result.Quantity = result.Quantity - 1;
                    _api.Entry(result).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                _api.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult CourseExist(int classId)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var result = _api.Cart.FirstOrDefault(c => c.ClassId == classId && c.UserId == userId);
            if (result != null)
            {
                return Json(true, new JsonSerializerSettings());
            }
            else
            {
                return Json(false, new JsonSerializerSettings());
            }
        }

        [HttpGet]
        public JsonResult CartItem()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            if (userId != null)
            {
                var result = _api.Cart.Where(c => c.UserId == userId).ToList();
                if (result.Count != 0)
                {
                    return Json(result.Count, new JsonSerializerSettings());
                }
                else
                {
                    return Json(false, new JsonSerializerSettings());
                }
            }
            return Json(false, new JsonSerializerSettings());
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(double total)
        {
            var userid = _userManager.GetUserId(HttpContext.User);         
            var orderId = userid;
            var payPalAPI = new PayPalAPI(Configuration);
            string url = await payPalAPI.getRedirectURLToPayPal(total, "USD", orderId);
            return Redirect(url);
        }


        public async Task<IActionResult> Success([FromQuery(Name = "paymentId")] string paymentId,
            [FromQuery(Name = "PayerID")] string payerID)
        {
            var payPalAPI = new PayPalAPI(Configuration);
            PayPalPaymentExecutedResponse result = await payPalAPI.executedPayment(paymentId, payerID);
            if (result != null)
            {
                var userid = _userManager.GetUserId(HttpContext.User);
                var cartItems = _api.Cart.Where(x => x.UserId == userid).ToList();
                var productList = _api.ClassMenu.ToList();

                //Add Data in Order table
                Order order = new Order();
                order.UserId = userid;
                order.CreatedDate = DateTime.Now;
                order.TotalAmount = Convert.ToDecimal(result.transactions.FirstOrDefault().amount.total);
                order.Status = "Payment Successfully";
                var results = _api.Order.Add(order);
                _api.SaveChanges();

                // Add Data in Order Details
                foreach (var item in cartItems)
                {
                    OrderDtl dtl = new OrderDtl();
                    dtl.OrdId = order.OrderId;
                    dtl.ClassId = item.ClassId;
                    var ps = _api.ClassMenu.Where(x => x.Id == item.ClassId).FirstOrDefault();
                    dtl.Amount = Convert.ToDecimal(ps.Price);
                    dtl.Quantity = item.Quantity;
                    _api.OrderDtl.Add(dtl);
                    _api.SaveChanges();
                }


                //Delete cart
                var all = _api.Cart.ToList();
                _api.Cart.RemoveRange(all);
                _api.SaveChanges();

                //Update status of order
                var ord = _api.Order.FirstOrDefault();
                ord.Status = "Completed";
                _api.SaveChanges();
                try
                {
                    PaymentDetail lst = new PaymentDetail();

                    Debug.WriteLine("Transaction Details");
                    Debug.WriteLine("cart: " + result.cart);
                    Debug.WriteLine("create_time: " + result.create_time.ToLongDateString());
                    Debug.WriteLine("id: " + result.id);
                    Debug.WriteLine("intent: " + result.intent);
                    Debug.WriteLine("links 0 - href: " + result.links[0].href);
                    Debug.WriteLine("links 0 - method: " + result.links[0].method);
                    Debug.WriteLine("links 0 - rel: " + result.links[0].rel);
                    Debug.WriteLine("payer_info - first_name: " + result.payer.payer_info.first_name);
                    Debug.WriteLine("payer_info - last_name: " + result.payer.payer_info.last_name);
                    Debug.WriteLine("payer_info - email: " + result.payer.payer_info.email);
                    Debug.WriteLine("payer_info - country_code: " + result.payer.payer_info.country_code);
                    Debug.WriteLine("payer_info - shipping_address: " + result.payer.payer_info.shipping_address);
                    Debug.WriteLine("payer_info - payer_id: " + result.payer.payer_info.payer_id);
                    Debug.WriteLine("state: " + result.state);

                    var address = $"{result.payer.payer_info.shipping_address.recipient_name} {result.payer.payer_info.shipping_address.line1} {result.payer.payer_info.shipping_address.city} {result.payer.payer_info.shipping_address.country_code} {result.payer.payer_info.shipping_address.postal_code}";

                    lst.OrderId = result.id;
                    lst.PayerId = result.payer.payer_info.payer_id;
                    lst.Email = result.payer.payer_info.email;
                    lst.FirstName = result.payer.payer_info.first_name;
                    lst.LastName = result.payer.payer_info.last_name;
                    lst.Intent = result.intent;
                    lst.State = result.state;
                    lst.CountryCode = result.payer.payer_info.country_code;
                    lst.PaymentMethod = result.payer.payment_method;
                    lst.Amount = result.transactions.FirstOrDefault()?.amount.total;
                    lst.ShippingAddress = address;
                    lst.TransactionFee = result.transactions.FirstOrDefault().related_resources.FirstOrDefault().sale.transaction_fee.value;
                    lst.CreateDate = result.create_time;
                    lst.SaleId = result.transactions.FirstOrDefault().related_resources.FirstOrDefault().sale.id;
                    lst.UniqId = result.transactions.FirstOrDefault().custom;
                    ViewBag.SaleId = result.transactions.FirstOrDefault().related_resources.FirstOrDefault().sale.id;
                    _api.PaymentDetail.Add(lst);
                    _api.SaveChanges();
                }
                catch (System.Exception ex)
                {
                    var msg = ex.Message;
                }
                ViewBag.result = result;
                return View("Success");
            }
            return View();
        }

    }
}
